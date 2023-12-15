using Blogs.Domain;
using Blogs.Domain.BusinessEntities;
using Blogs.Interfaces;
using Blogs.Models.In;
using Blogs.Models.Out;
using Blogs.WebApi.Filters.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Blogs.WebApi.Controllers
{
    [EnableCors("AllowEverything")]
    [Route("api/sessions")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        private readonly IManager<Session> _sessionManager;

        public SessionController(IManager<Session> sessionManager)
        {
            _sessionManager = sessionManager;
        }

        [HttpGet]
        public IActionResult GetCurrentUser([FromHeader] string Authorization)
        {
            Session currentSession = ControllerHelper.GetSession(_sessionManager, Authorization);
            return Ok(new UserDetail(currentSession.AuthenticatedUser));
        }

        [HttpPost]
        public IActionResult Post([FromBody] LogInModel log)
        {
            Session createdSession = new() {AuthenticatedUser = log.ToEntity()};
            var retrievedSession = _sessionManager.Create(createdSession);

            return Ok(new LogInDetail(retrievedSession));
        }

        [HttpDelete("{id:int}")]
        [ServiceFilter(typeof(AuthenticationFilter))]
        [UserRoleFilter(UserRole = UserRole.Admin)]
        public IActionResult DeleteAsAdmin([FromRoute] int id)
        {
            _sessionManager.Delete(id);
            return NoContent();
        }

        [HttpDelete]
        [ServiceFilter(typeof(AuthenticationFilter))]
        public IActionResult Delete([FromHeader] string Authorization)
        {
            var currentSession = ControllerHelper.GetSession(_sessionManager, Authorization);
            _sessionManager.Delete(currentSession.Id);

            return NoContent();
        }
    }
}
