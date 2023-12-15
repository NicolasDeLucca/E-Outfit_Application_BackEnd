using static Blogs.Domain.Functions;
using Blogs.Domain;
using Blogs.Domain.BusinessEntities;
using Blogs.Domain.SearchCriteria;
using Blogs.Interfaces;
using Blogs.Models.In;
using Blogs.Models.Out;
using Blogs.WebApi.Filters.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;

namespace Blogs.WebApi.Controllers
{
    [EnableCors("AllowEverything")]
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IManager<User> _userManager;
        private readonly IManager<Offense> _offenseManager;
        private readonly IManager<Session> _sessionManager;

        public UserController(IManager<User> userManager, IManager<Offense> offenseManager,
            IManager<Session> sessionManager)
        {
            _userManager = userManager;
            _offenseManager = offenseManager;
            _sessionManager = sessionManager;
        }

        [HttpGet]
        [ServiceFilter(typeof(AuthenticationFilter))]
        [UserRoleFilter(UserRole = UserRole.Admin)]
        public IActionResult GetAll()
        {
            var retrievedUsers = _userManager.GetAll();
            var mappedUsers = retrievedUsers.Select(u => new UserBasicInfo(u));

            return Ok(mappedUsers);
        }

        [HttpGet("activity")]
        [ServiceFilter(typeof(AuthenticationFilter))]
        [UserRoleFilter(UserRole = UserRole.Admin)]
        public IActionResult GetAllByActivity([FromQuery] string bottomDate, [FromQuery] string topDate)
        {
            var isValidbottomDate = DateTime.TryParse(bottomDate, out DateTime minDate);
            var isValidtopDate = DateTime.TryParse(topDate, out DateTime maxDate);
            if (!isValidbottomDate || !isValidtopDate)
                return BadRequest("Invalid date format");

            SearchingUserByActivity activityCriteria = new() {MinDate = minDate, MaxDate = maxDate};
            var activityUsers = _userManager.GetAllBy(activityCriteria);
            var retrievedUsers = BlogersActivity(activityUsers, a => true, m => true);
            var mappedUsers = retrievedUsers.Select(u => new UserBasicInfo(u));

            return Ok(mappedUsers);
        }

        [HttpGet("offensiveActivity")]
        [ServiceFilter(typeof(AuthenticationFilter))]
        [UserRoleFilter(UserRole = UserRole.Admin)]
        public IActionResult GetAllByOffensiveActivity([FromQuery] string bottomDate, [FromQuery] string topDate)
        {
            var isValidbottomDate = DateTime.TryParse(bottomDate, out DateTime minDate);
            var isValidtopDate = DateTime.TryParse(topDate, out DateTime maxDate);
            if (!isValidbottomDate || !isValidtopDate)
                return BadRequest("Invalid date format");

            var offensiveWords = _offenseManager.GetAll().Select(offense => offense.Word).ToList();
            SearchingUserByOffensiveActivity activityCriteria = new() {MinDate = minDate, MaxDate = maxDate, OffensiveWords = offensiveWords};
            var activityUsers = _userManager.GetAllBy(activityCriteria);
            var retrievedUsers = BlogersActivity(activityUsers, a => a.IsOffensive(offensiveWords), m => m.IsOffensive(offensiveWords));
            var mappedUsers = retrievedUsers.Select(u => new UserBasicInfo(u));

            return Ok(mappedUsers);
        }

        [ServiceFilter(typeof(AuthenticationFilter))]
        [HttpGet("{id:int}")]
        public IActionResult GetById([FromRoute] int id)
        {
            var retrievedUser = _userManager.GetById(id);

            var userDetail = new UserDetail(retrievedUser);
            return Ok(userDetail);
        }

        [HttpPost]
        public IActionResult Post([FromBody] UserModel user)
        {
            var createdUserEntity = user.ToEntity();
            var createdUser = _userManager.Create(createdUserEntity);

            return Created($"api/users/{createdUser.Id}", new UserDetail(createdUser));
        }

        [HttpPut("{id:int}")]
        [ServiceFilter(typeof(AuthenticationFilter))]
        [UserRoleFilter(UserRole = UserRole.Admin)]
        public IActionResult PutAsAdmin([FromRoute] int id, [FromBody] UserModel user)
        {
            var createdUserEntity = user.ToEntity();
            createdUserEntity.Id = id;

            _userManager.Update(createdUserEntity);
            return NoContent();
        }

        [HttpPut]
        [ServiceFilter(typeof(AuthenticationFilter))]
        public IActionResult Put([FromHeader] string Authorization, [FromBody] UserModel user)
        {
            var createdUserEntity = user.ToEntity();
            Session currentSession = ControllerHelper.GetSession(_sessionManager, Authorization);
            createdUserEntity.Id = currentSession.AuthenticatedUser.Id;

            _userManager.Update(createdUserEntity);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [ServiceFilter(typeof(AuthenticationFilter))]
        [UserRoleFilter(UserRole = UserRole.Admin)]
        public IActionResult DeleteAsAdmin([FromRoute] int id)
        {
            _userManager.Delete(id);
            return NoContent();
        }

        [HttpDelete]
        [ServiceFilter(typeof(AuthenticationFilter))]
        public IActionResult Delete([FromHeader] string Authorization)
        {
            Session currentSession = ControllerHelper.GetSession(_sessionManager, Authorization);
            var id = currentSession.AuthenticatedUser.Id;

            _userManager.Delete(id);
            return NoContent();
        }
    }
}
