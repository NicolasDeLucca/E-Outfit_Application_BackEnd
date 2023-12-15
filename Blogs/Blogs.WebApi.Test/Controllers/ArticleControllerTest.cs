using Blogs.Domain.BusinessEntities;
using static Blogs.Instances.DomainInstances;
using static Blogs.Instances.ModelInstances;
using Blogs.Interfaces;
using Blogs.Models.Out;
using Blogs.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Blogs.WebApi.Test.Controllers
{
    [TestClass]
    public class ArticleControllerTest
    {
        private Mock<IManager<Article>> _articleManagerMock;
        private Mock<IManager<Session>> _sessionManagerMock;
        private Mock<IManager<User>> _userManagerMock;

        [TestInitialize]
        public void SetUp()
        {
            _articleManagerMock = new Mock<IManager<Article>>(MockBehavior.Strict);
            _sessionManagerMock = new Mock<IManager<Session>>(MockBehavior.Strict);
            _userManagerMock = new Mock<IManager<User>>(MockBehavior.Strict);
        }

        [TestMethod]
        public void GetAllArticlesOkTest()
        {
            List<Article> retrievedArticles = new() {CreateSimpleArticle(new User(), 0)};
            List<ArticleBasicInfo> articlesBasicInfo = new();
            foreach (Article a in retrievedArticles) articlesBasicInfo.Add(new ArticleBasicInfo(a));

            _articleManagerMock.Setup(m => m.GetAll()).Returns(retrievedArticles);

            ArticleController articleController = new(_articleManagerMock.Object, _sessionManagerMock.Object,
                _userManagerMock.Object);
            IActionResult result = articleController.GetAll();

            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            OkObjectResult okResult = (OkObjectResult)result;

            var responseModel = (IEnumerable<ArticleBasicInfo>) okResult.Value;

            _articleManagerMock.VerifyAll();

            Assert.IsTrue(okResult.StatusCode == StatusCodes.Status200OK);
            CollectionAssert.AreEquivalent(articlesBasicInfo, responseModel.ToList());
        }

        [TestMethod]
        public void GetAllArticlesByTextOkTest()
        {
            var authenticatedUser = new User();
            var newArticle = CreateSimpleArticle(authenticatedUser, 0);
            authenticatedUser.PostedArticles.Add(newArticle);

            var actualSession = new Session() {AuthenticatedUser = authenticatedUser};

            List<Article> retrievedArticles = new() {};
            List<ArticleBasicInfo> articlesBasicInfo = new();
            foreach (Article a in retrievedArticles) articlesBasicInfo.Add(new ArticleBasicInfo(a));

            _articleManagerMock.Setup(m => m.GetAllBy(It.IsAny<ISearchCriteria<Article>>())).Returns(retrievedArticles);
            _sessionManagerMock.Setup(m => m.GetAllBy(It.IsAny<ISearchCriteria<Session>>())).Returns(new List<Session> {actualSession});

            ArticleController articleController = new(_articleManagerMock.Object, _sessionManagerMock.Object,
                _userManagerMock.Object);
            IActionResult result = articleController.GetAllByText(actualSession.AuthToken.ToString(), "....");

            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            OkObjectResult okResult = (OkObjectResult)result;
            var responseModel = (IEnumerable<ArticleBasicInfo>) okResult.Value;

            _articleManagerMock.VerifyAll();
            _sessionManagerMock.VerifyAll();

            Assert.IsTrue(okResult.StatusCode == StatusCodes.Status200OK);
            CollectionAssert.AreEquivalent(articlesBasicInfo, responseModel.ToList());
        }

        [TestMethod]
        public void GetArticleByIdOkTest()
        {
            var authUser = new User {Id = 0};
            var retrievedArticle = CreateSimpleArticle(authUser, 0);
            retrievedArticle.Visibility = Domain.Visibility.Public;
            authUser.PostedArticles.Add(retrievedArticle);
            var session = new Session {AuthenticatedUser = authUser};

            _sessionManagerMock.Setup(m => m.GetAllBy(It.IsAny<ISearchCriteria<Session>>())).Returns(new List<Session> {session});
            _articleManagerMock.Setup(m => m.GetById(It.IsAny<int>())).Returns(retrievedArticle);
            
            ArticleController articleController = new(_articleManagerMock.Object, _sessionManagerMock.Object,
                _userManagerMock.Object);
            IActionResult result = articleController.GetById(session.AuthToken.ToString(), 0);

            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            OkObjectResult okResult = (OkObjectResult)result;
            var responseModel = (ArticleDetail) okResult.Value;

            _sessionManagerMock.VerifyAll();
            _articleManagerMock.VerifyAll();

            Assert.IsTrue(okResult.StatusCode == StatusCodes.Status200OK);
            Assert.IsTrue(new ArticleDetail(retrievedArticle).Equals(responseModel));
        }

        [TestMethod]
        public void PostArticleCreatedTest()
        {
            var user = new User();
            var activeSession = new Session {AuthenticatedUser = user};
            var articleModel = CreateArticleModel();
            var articleEntity = articleModel.ToEntity();
            articleEntity.Author = user;
            IEnumerable<Session> sessions = new List<Session>() {activeSession};
            ArticleDetail articleDetail = new(articleEntity);

            _articleManagerMock.Setup(m => m.Create(It.IsAny<Article>())).Returns(articleEntity);
            _sessionManagerMock.Setup(s => s.GetAllBy(It.IsAny<ISearchCriteria<Session>>())).Returns(sessions);
            
            ArticleController articleController = new(_articleManagerMock.Object, _sessionManagerMock.Object,
                _userManagerMock.Object);
            IActionResult result = articleController.Post(activeSession.AuthToken.ToString(), articleModel);

            Assert.IsInstanceOfType(result, typeof(CreatedResult));
            CreatedResult createdResult = (CreatedResult)result;
            var responseModel = (ArticleDetail) createdResult.Value;

            _articleManagerMock.VerifyAll();
            _sessionManagerMock.VerifyAll();

            Assert.IsTrue(createdResult.StatusCode == StatusCodes.Status201Created);
            Assert.IsTrue(articleDetail.Equals(responseModel));
        }

        [TestMethod]
        public void PutArticleAsAdminByIdNoContentTest()
        {
            var articleModel = CreateArticleModel();
            var articleEntity = articleModel.ToEntity();
            articleEntity.Id = 0;

            var authUser = new User();
            authUser.PostedArticles.Add(articleEntity);

            _userManagerMock.Setup(m => m.GetAll()).Returns(new List<User> {authUser});
            _articleManagerMock.Setup(m => m.Update(It.IsAny<Article>()));

            ArticleController articleController = new(_articleManagerMock.Object, _sessionManagerMock.Object,
                _userManagerMock.Object);
            IActionResult result = articleController.PutAsAdmin(0, articleModel);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            NoContentResult noContentResult = (NoContentResult)result;

            _userManagerMock.VerifyAll();
            _articleManagerMock.VerifyAll();

            Assert.IsTrue(noContentResult.StatusCode == StatusCodes.Status204NoContent);
        }

        [TestMethod]
        public void PutArticleByIdNoContentTest()
        {
            var articleModel = CreateArticleModel();

            var inputId = 0;
            var authUser = new User();
            authUser.PostedArticles.Add(new Article {Id = inputId});
            var activeSession = new Session() {AuthenticatedUser = authUser};

            _sessionManagerMock.Setup(m => m.GetAllBy(It.IsAny<ISearchCriteria<Session>>())).Returns(new List<Session> {activeSession});
            _articleManagerMock.Setup(m => m.Update(It.IsAny<Article>()));

            ArticleController articleController = new(_articleManagerMock.Object, _sessionManagerMock.Object,
                _userManagerMock.Object);
            IActionResult result = articleController.Put(activeSession.AuthToken.ToString(), inputId, articleModel);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            NoContentResult noContentResult = (NoContentResult)result;

            _sessionManagerMock.VerifyAll();
            _articleManagerMock.VerifyAll();

            Assert.IsTrue(noContentResult.StatusCode == StatusCodes.Status204NoContent);
        }

        [TestMethod]
        public void DeleteArticleByIdNoContentTest()
        {
            var authUser = new User {Id = 0};
            authUser.PostedArticles.Add(new Article {Id = 0});
            var session = new Session {AuthenticatedUser = authUser};

            _sessionManagerMock.Setup(m => m.GetAllBy(It.IsAny<ISearchCriteria<Session>>())).Returns(new List<Session> {session});
            _articleManagerMock.Setup(m => m.Delete(It.IsAny<int>()));

            ArticleController articleController = new(_articleManagerMock.Object, _sessionManagerMock.Object,
                _userManagerMock.Object);
            IActionResult result = articleController.Delete(session.AuthToken.ToString(), 0);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            NoContentResult noContentResult = (NoContentResult)result;

            _articleManagerMock.VerifyAll();
            _sessionManagerMock.VerifyAll();
            Assert.IsTrue(noContentResult.StatusCode == StatusCodes.Status204NoContent);
        }
    }
}
