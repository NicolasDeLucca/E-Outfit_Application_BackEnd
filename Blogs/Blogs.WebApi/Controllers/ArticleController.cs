using Blogs.Domain;
using Blogs.Domain.BusinessEntities;
using Blogs.Domain.SearchCriteria;
using Blogs.Interfaces;
using Blogs.Models.In;
using Blogs.Models.Out;
using Blogs.WebApi.Filters.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using static Blogs.Domain.Functions;

namespace Blogs.WebApi.Controllers
{
    [EnableCors("AllowEverything")]
    [Route("api/articles")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly IManager<Session> _sessionService;
        private readonly IManager<Article> _articleManager;
        private readonly IManager<User> _userManager;
        private const int maxRangeDays = 6000;

        public ArticleController(IManager<Article> articleManager, IManager<Session> sessionService,
            IManager<User> userManager)
        {
            _articleManager = articleManager;
            _sessionService = sessionService;
            _userManager = userManager;
        }

        [HttpGet]
        [ServiceFilter(typeof(AuthenticationFilter))]
        public IActionResult GetAll()
        {
            var retrievedArticles = _articleManager.GetAll();
            IEnumerable<ArticleBasicInfo> mappedArticles = retrievedArticles.Select(a => new ArticleBasicInfo(a));

            return Ok(mappedArticles);
        }

        [HttpGet("dateRange")]
        [ServiceFilter(typeof(AuthenticationFilter))]
        [UserRoleFilter(UserRole = UserRole.Admin)]
        public IActionResult GetAllByRangeUpdatingDate([FromQuery] string bottomDate, [FromQuery] string topDate)
        {
            var isValidbottomDate = DateTime.TryParse(bottomDate, out DateTime minDate);
            var isValidtopDate = DateTime.TryParse(topDate, out DateTime maxDate);
            if (!isValidbottomDate || !isValidtopDate)
                return BadRequest("Invalid date format");

            var searchCriteria = new SearchingByUpdatedDate<Article> {MinDate = minDate, MaxDate = maxDate};
            var retrievedArticles = _articleManager.GetAllBy(searchCriteria);
            var mappedArticles = retrievedArticles.Select(a => new ArticleBasicInfo(a));

            return Ok(mappedArticles);
        }

        [HttpGet("content")]
        [ServiceFilter(typeof(AuthenticationFilter))]
        public IActionResult GetAllByText([FromHeader] string Authorization, [FromQuery] string text)
        {
            ISearchCriteria<Article> searchCriteria = new SearchingArticleByText {Text = text};
            var retrievedArticles = _articleManager.GetAllBy(searchCriteria).Where(a => a.Visibility == Visibility.Public);

            Session currentSession = ControllerHelper.GetSession(_sessionService, Authorization);
            List<Article> retrievedUserArticles = new();
            List<Article> userArticles = new();
            userArticles = currentSession.AuthenticatedUser.PostedArticles.ToList();
            
            foreach (var art in userArticles) if (retrievedArticles.Contains(art)) retrievedUserArticles.Add(art);

            IEnumerable<ArticleBasicInfo> mappedArticles = retrievedUserArticles.Select(a => new ArticleBasicInfo(a));
            return Ok(mappedArticles);
        }

        [HttpGet("topModified")]   
        public IActionResult GetTopModified([FromQuery] int top)
        {
            DateTime now = DateTime.Now;
            SearchingByUpdatedDate<Article> updatedDateCriteria = new()
                {MaxDate = now, MinDate = now.AddDays(- maxRangeDays)};

            var updatedArticles = _articleManager.GetAllBy(updatedDateCriteria);
            var retrievedArticles = TopModifiedArticles(updatedArticles, top);
            var mappedArticles = retrievedArticles.Select(a => new ArticleBasicInfo(a));

            return Ok(mappedArticles);
        }

        [HttpGet("{id:int}")]
        [ServiceFilter(typeof(AuthenticationFilter))]
        public IActionResult GetById([FromHeader] string Authorization, [FromRoute] int id)
        {
            Session currentSession = ControllerHelper.GetSession(_sessionService, Authorization);
            var authorUser = currentSession.AuthenticatedUser;

            var articleToRetrieve = _articleManager.GetById(id);
            var isArticleMine = authorUser.PostedArticles.ToList().Exists(a => a.Id == id);
            if (!isArticleMine && articleToRetrieve.Visibility == Visibility.Private)
                return NoContent();

            ArticleDetail articleDetail = new(articleToRetrieve);
            return Ok(articleDetail);
        }

        [HttpPost]
        [ServiceFilter(typeof(AuthenticationFilter))]
        public IActionResult Post([FromHeader] string Authorization, [FromBody] ArticleModel articleModel)
        {
            Session currentSession = ControllerHelper.GetSession(_sessionService, Authorization);

            var createdArticleEntity = articleModel.ToEntity();
            createdArticleEntity.Author = currentSession.AuthenticatedUser;
            var createdArticle = _articleManager.Create(createdArticleEntity);

            currentSession.AuthenticatedUser.PostedArticles.Add(createdArticle);
            return Created($"api/articles/{createdArticle.Id}", new ArticleDetail(createdArticle));
        }

        [HttpPut("any/{id:int}")]
        [ServiceFilter(typeof(AuthenticationFilter))]
        [UserRoleFilter(UserRole = UserRole.Admin)]
        public IActionResult PutAsAdmin([FromRoute] int id, [FromBody] ArticleModel articleModel)
        {
            var allUsers = _userManager.GetAll();
            var authorUser = allUsers.FirstOrDefault(u => u.PostedArticles.Any(a => a.Id == id));
            if (authorUser == null)
                return NotFound("Could not find specified article");

            var articleToUpdate = articleModel.ToEntity();
            articleToUpdate.Id = id;
            articleToUpdate.Author = authorUser;

            _articleManager.Update(articleToUpdate);
            return NoContent();
        }

        [HttpPut("{id:int}")]
        [ServiceFilter(typeof(AuthenticationFilter))]
        public IActionResult Put([FromHeader] string Authorization, [FromRoute] int id, [FromBody] ArticleModel articleModel)
        {
            var updatedArticle = articleModel.ToEntity();
            Session currentSession = ControllerHelper.GetSession(_sessionService, Authorization);
            var authorUser = currentSession.AuthenticatedUser;
            
            var articleToUpdate = authorUser.PostedArticles.ToList().Find(a => a.Id == id);
            if (articleToUpdate == null) 
                return NotFound("Could not find specified article");
                    
            updatedArticle.Id = id;
            updatedArticle.Author = authorUser;

            _articleManager.Update(updatedArticle);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [ServiceFilter(typeof(AuthenticationFilter))]
        public IActionResult Delete([FromHeader] string Authorization, [FromRoute] int id)
        {
            Session currentSession = ControllerHelper.GetSession(_sessionService, Authorization);
            var authorUser = currentSession.AuthenticatedUser;

            var articleToDelete = authorUser.PostedArticles.ToList().Find(a => a.Id == id);
            if (articleToDelete == null)
                return NotFound("Could not find specified article");

            _articleManager.Delete(id);
            return NoContent();
        }
    }
}
