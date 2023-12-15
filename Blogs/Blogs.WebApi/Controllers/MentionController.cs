using Blogs.Domain;
using Blogs.Domain.BusinessEntities;
using Blogs.Domain.BusinessEntities.Mentions;
using Blogs.Domain.SearchCriteria;
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
    [Route("api/mentions")]
    [ApiController]
    public class MentionController : ControllerBase
    {
        private readonly IManager<Mention> _mentionManager;
        private readonly IManager<Session> _sessionService;

        public MentionController(IManager<Mention> mentionManager, IManager<Session> sessionService)
        {
            _mentionManager = mentionManager;
            _sessionService = sessionService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var retrievedMentions = _mentionManager.GetAll();
            var mappedMentions = retrievedMentions.Select(m => new MentionBasicInfo(m));

            return Ok(mappedMentions);
        }

        [HttpGet("type")]
        public IActionResult GetAllByType<T>()
        {
            ISearchCriteria<Mention> searchCriteria = new SearchingMentionByType<T>();
            var retrievedMentions = _mentionManager.GetAllBy(searchCriteria);
            var mappedMentions = retrievedMentions.Select(m => new MentionBasicInfo(m));

            return Ok(mappedMentions);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById([FromRoute] int id)
        {
            var mentionToRetrieve = _mentionManager.GetById(id);

            MentionDetail mentionDetail = new(mentionToRetrieve);
            return Ok(mentionDetail);
        }

        [HttpGet("authorInfo/{id:int}")]
        public IActionResult GetMentionAuthorInformation([FromRoute] int id)
        {
            var mention = _mentionManager.GetById(id);
            return Ok(new UserDetail(mention.Author));
        }

        [HttpPost]
        public IActionResult Post([FromHeader] string Authorization, [FromBody] MentionModel mention)
        {
            Session currentSession = ControllerHelper.GetSession(_sessionService, Authorization);

            var createdMentionEntity = mention.ToEntity();
            createdMentionEntity.Author = currentSession.AuthenticatedUser;
            var createdMention = _mentionManager.Create(createdMentionEntity);

            currentSession.AuthenticatedUser.PostedMentions.Add(createdMention);
            return Created($"api/mentions/{createdMention.Id}", new MentionDetail(createdMention));
        }

        [HttpPut("{id:int}")]
        [UserRoleFilter(UserRole = UserRole.Admin)]
        public IActionResult BehaviourFix([FromRoute] int id, [FromBody] MentionModel mention)
        {
            var updatedMentionEntity = mention.ToEntity();
            updatedMentionEntity.Id = id;
            _mentionManager.Update(updatedMentionEntity);

            return NoContent();
        }
    }
}
