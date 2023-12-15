using Blogs.Domain;
using Blogs.Domain.BusinessEntities;
using Blogs.Domain.BusinessEntities.Mentions;
using Blogs.Interfaces;
using Blogs.Models.In;
using Blogs.Models.Out;
using Blogs.WebApi.Filters.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Blogs.WebApi.Controllers
{
    [EnableCors("AllowEverything")]
    [ServiceFilter(typeof(AuthenticationFilter))]
    [UserRoleFilter(UserRole = UserRole.Admin)]
    [Route("api/offenses")]
    [ApiController]
    public class OffenseController : ControllerBase
    {
        private readonly IManager<Offense> _offenseManager;
        private readonly IManager<Article> _articleManager;
        private readonly IManager<Mention> _mentionManager;

        public OffenseController(IManager<Offense> offenseManager, IManager<Article> articleManager,
            IManager<Mention> mentionManager)
        {
            _offenseManager = offenseManager;
            _articleManager = articleManager;
            _mentionManager = mentionManager;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var retrievedOffenses = _offenseManager.GetAll();
            IEnumerable<OffenseDetail> mappedOffenses = retrievedOffenses.Select(of => new OffenseDetail(of));

            return Ok(mappedOffenses);
        }

        [HttpPost]
        public IActionResult Post([FromBody] OffenseModel offenseModel)
        {
            var createdOffenseEntity = offenseModel.ToEntity();
            var createdOffense = _offenseManager.Create(createdOffenseEntity);

            foreach (var art in _articleManager.GetAll()) _articleManager.Update(art); //
            foreach (var men in _mentionManager.GetAll()) _mentionManager.Update(men); //

            return Created($"api/offenses/{createdOffense.Id}", new OffenseDetail(createdOffense));
        }

        [HttpPut("{id:int}")]
        public IActionResult Put([FromRoute] int id, [FromBody] OffenseModel offenseModel)
        {
            var createdOffenseEntity = offenseModel.ToEntity();
            createdOffenseEntity.Id = id;
            _offenseManager.Update(createdOffenseEntity);

            foreach (var art in _articleManager.GetAll()) _articleManager.Update(art); //
            foreach (var men in _mentionManager.GetAll()) _mentionManager.Update(men); //

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete([FromRoute] int id)
        {
            _offenseManager.Delete(id);
            return NoContent();
        }
    }
}
