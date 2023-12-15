using Blogs.Domain.BusinessEntities;
using static Blogs.Instances.DomainInstances;
using Blogs.Importing.Parameters;
using Blogs.Interfaces;
using Blogs.Models.Out;
using Blogs.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Blogs.Models.In;

namespace Blogs.WebApi.Test.Controllers
{
    [TestClass]
    public class ArticleImporterControllerTest
    {
        private Mock<IImporterService<Article, ParameterType>> _articleImporterServiceMock;
        private Mock<IImporter<Article, ParameterType>> _articleImporterMock;

        [TestInitialize]
        public void SetUp()
        {
            _articleImporterServiceMock = new Mock<IImporterService<Article, ParameterType>>(MockBehavior.Strict);
            
            _articleImporterMock = new Mock<IImporter<Article, ParameterType>>(MockBehavior.Strict);
            _articleImporterMock.SetupAllProperties();
            _articleImporterMock.Setup(i => i.GetName()).Returns("ImporterName");
            _articleImporterMock.Setup(i => i.GetParameters()).Returns(new List<IParameter<ParameterType>>());
        }

        [TestMethod]
        public void GetAllArticlesImportersOkTest()
        {
            List<IImporter<Article, ParameterType>> importersList = new() {_articleImporterMock.Object};
            var expectedImporters = importersList.Select(i => new ArticleImporterDetail(i)).ToList();

            _articleImporterServiceMock.Setup(s => s.GetImporters()).Returns(importersList);

            ArticleImporterController importerController = new(_articleImporterServiceMock.Object);
            IActionResult result = importerController.GetArticleImporters();

            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            OkObjectResult okResult = (OkObjectResult)result;
            var responseModel = (List<ArticleImporterDetail>) okResult.Value;

            _articleImporterMock.VerifyAll();
            _articleImporterServiceMock.VerifyAll();
            Assert.IsTrue(okResult.StatusCode == StatusCodes.Status200OK);
            CollectionAssert.AreEquivalent(expectedImporters, responseModel);
        }

        [TestMethod]
        public void GetAllImportedArticlesOkTest()
        {
            var defaulParameter = new Tuple<string, object>("File to Parse", new object());

            ArticleImporterInputModel inputModel = new() {Name = "importerName", Parameters = new(){defaulParameter}};
            List<Article> importedArticles = new(){CreateSimpleArticle(new User(), 0)};
            List<ArticleDetail> articlesDetail = importedArticles.Select(a => new ArticleDetail(a)).ToList();

            _articleImporterServiceMock.Setup(s => s.Import(It.IsAny<string>(), It.IsAny<List<IParameter<ParameterType>>>())).Returns(importedArticles);

            ArticleImporterController importerController = new(_articleImporterServiceMock.Object);
            IActionResult result = importerController.Import(inputModel);

            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            OkObjectResult okResult = (OkObjectResult)result;
            var responseModel = (List<ArticleDetail>) okResult.Value;

            _articleImporterServiceMock.VerifyAll();
            Assert.IsTrue(okResult.StatusCode == StatusCodes.Status200OK);
            CollectionAssert.AreEquivalent(articlesDetail, responseModel);
        }
    }
}
