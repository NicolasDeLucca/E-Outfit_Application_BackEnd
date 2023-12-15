using Blogs.Domain.BusinessEntities;
using Blogs.Importing.Parameters;
using Blogs.Interfaces;
using Blogs.Models.Out;
using Blogs.Models.In;
using Blogs.WebApi.Filters.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Blogs.WebApi.Controllers
{
    [EnableCors("AllowEverything")]
    [ServiceFilter(typeof(AuthenticationFilter))]
    [Route("api/importers")]
    [ApiController]
    public class ArticleImporterController : ControllerBase
    {
        private readonly IImporterService<Article, ParameterType> _articleImporterService;

        public ArticleImporterController(IImporterService<Article, ParameterType> articleImporterService)
        {
            _articleImporterService = articleImporterService;
        }

        [HttpGet]
        public IActionResult GetArticleImporters()
        {
            List<IImporter<Article, ParameterType>> articleImporters = _articleImporterService.GetImporters();
            List<ArticleImporterDetail> importersDetail = articleImporters.Select(i => new ArticleImporterDetail(i)).ToList();
            
            return Ok(importersDetail);
        }

        [HttpPost]
        [Route("resources")]
        public IActionResult Import([FromBody] ArticleImporterInputModel articleImporterModel)
        {
            var articleImporterEntity = articleImporterModel.ToEntity();
            List<Article> importedArticles = _articleImporterService.Import(articleImporterEntity.Item1, articleImporterEntity.Item2);
            List<ArticleDetail> articlesDetail = importedArticles.Select(a => new ArticleDetail(a)).ToList();   

            return Ok(articlesDetail);
        }

    }
}
