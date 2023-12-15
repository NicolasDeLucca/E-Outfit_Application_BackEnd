using System.Collections;
using Blogs.BusinessLogic.ResourceManagers;
using Blogs.Domain.BusinessEntities;
using Blogs.Domain.SearchCriteria;
using Blogs.Exceptions;
using Blogs.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Blogs.BusinessLogic.Test.ResourceManagers
{
    [TestClass]
    public class ArticleManagerTest
    {
        private Mock<Article> _articleMock;
        private Mock<IRepository<Article>> _repositoryMock;
        private Mock<IRepository<Offense>> _offenseRepositoryMock;
        private Mock<IRepository<Notification<Article>>> _notificationRepositoryMock;
        private Mock<IRepository<User>> _userRepositoryMock;

        [TestInitialize]
        public void Setup()
        {
            _repositoryMock = new Mock<IRepository<Article>>(MockBehavior.Strict);
            _articleMock = new Mock<Article>(MockBehavior.Strict);
            _offenseRepositoryMock = new Mock<IRepository<Offense>>(MockBehavior.Strict);
            _notificationRepositoryMock = new Mock<IRepository<Notification<Article>>>(MockBehavior.Strict);
            _userRepositoryMock = new Mock<IRepository<User>>(MockBehavior.Strict);
        }

        [TestMethod]
        public void GetArticlesReturnsAsExpected()
        {
            var articles = It.IsAny<List<Article>>();
            _repositoryMock.Setup(r => r.GetAllByCondition(It.IsAny<Func<Article, bool>>())).Returns(articles);

            ArticleManager articleManager = new(_repositoryMock.Object, _offenseRepositoryMock.Object,
                _notificationRepositoryMock.Object, _userRepositoryMock.Object);
            var retrievedArticles = articleManager.GetAll();

            _repositoryMock.VerifyAll();
            CollectionAssert.AreEquivalent((ICollection)retrievedArticles, articles);
        }

        [TestMethod]
        public void GetArticlesByConditionReturnsAsExpected()
        {
            List<Article> articles = new();
            SearchingArticleByText articleSearchCriteria = new();

            _repositoryMock.Setup(r => r.GetAllByCondition(articleSearchCriteria.Criteria)).Returns(articles);

            ArticleManager articleManager = new(_repositoryMock.Object, _offenseRepositoryMock.Object,
                _notificationRepositoryMock.Object, _userRepositoryMock.Object);
            var retrievedArticles = articleManager.GetAllBy(articleSearchCriteria);

            _repositoryMock.VerifyAll();
            CollectionAssert.AreEquivalent((ICollection)retrievedArticles, articles);
        }

        [TestMethod]
        public void GetArticleByIdReturnsAsExpected()
        {
            Article expectedArticle = new(){Id = 0, Author = new User()};

            _repositoryMock.Setup(r => r.GetById(0)).Returns(expectedArticle);
            ArticleManager articleManager = new(_repositoryMock.Object, _offenseRepositoryMock.Object,
                _notificationRepositoryMock.Object, _userRepositoryMock.Object);
            var retrievedArticle = articleManager.GetById(0);

            _repositoryMock.VerifyAll();
            Assert.IsTrue(expectedArticle.Equals(retrievedArticle));
        }

        [TestMethod]
        public void CreateArticleReturnsAsExpected()
        {
            User newAdmin = new() {Role = Domain.UserRole.Admin};
            Offense newOffense = new(){Id = 0, Word = "Denigrante"};
            Article articleCreated = new(){Id = 0, Text = "Denigrante esta situacion", Author = new()};
            Notification<Article> newOffenseNotification = new() {Id = 0, Text = "Your article has offensive words, so it has been detached",
                    AssociatedContent = articleCreated, Receiver = articleCreated.Author};
            Notification<Article> newOffenseNotificationForAdmin = new() {Id = 1, Text = "This article has offensive words, so it has been detached",
                    AssociatedContent = articleCreated, Receiver = newAdmin};

            _articleMock.SetupAllProperties();
            _articleMock.Setup(a => a.ValidOrFail());

            _offenseRepositoryMock.Setup(r => r.GetAllByCondition(It.IsAny<Func<Offense, bool>>())).Returns(new List<Offense>{newOffense});
            _articleMock.Setup(a => a.IsOffensive(new List<string> {"Denigrante"})).Returns(true);
            _repositoryMock.Setup(r => r.Store(_articleMock.Object)).Returns(0);
            _notificationRepositoryMock.Setup(r => r.Store(It.IsAny<Notification<Article>>())).Returns(0);
            _userRepositoryMock.Setup(r => r.GetAllByCondition(It.IsAny<Func<User, bool>>())).Returns(new List<User>{newAdmin});
            _notificationRepositoryMock.Setup(r => r.Store(It.IsAny<Notification<Article>>())).Returns(1);
            _repositoryMock.Setup(r => r.GetById(0)).Returns(articleCreated);

            ArticleManager articleManager = new(_repositoryMock.Object, _offenseRepositoryMock.Object,
                _notificationRepositoryMock.Object, _userRepositoryMock.Object);
            var retrievedArticle = articleManager.Create(_articleMock.Object);

            _repositoryMock.VerifyAll();
            _articleMock.VerifyAll();
            _userRepositoryMock.VerifyAll();
            _notificationRepositoryMock.VerifyAll();
            _offenseRepositoryMock.VerifyAll();

            Assert.AreEqual(articleCreated, retrievedArticle);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateEmptyArticleFails()
        {
            ArticleManager articleManager = new(_repositoryMock.Object, _offenseRepositoryMock.Object,
                _notificationRepositoryMock.Object, _userRepositoryMock.Object);
            articleManager.Create(null);
        }

        [TestMethod]
        public void UpdateArticleDoesAsExpected()
        {
            User newAdmin = new() {Role = Domain.UserRole.Admin};
            Offense newOffense = new() {Id = 0, Word = "Denigrante"};
            Article articleToUpdate = new() {Id = 0, Name = "This Article..", Template = Domain.Template.Top, Text = "Denigrante esta situacion", Author = new()};
            Notification<Article> newOffenseNotification = new() {Id = 0, Text = "Your article has offensive words, so it has been detached",
                    AssociatedContent = articleToUpdate, Receiver = articleToUpdate.Author};
            Notification<Article> newOffenseNotificationForAdmin = new() {Id = 1, Text = "This article has offensive words, so it has been detached",
                    AssociatedContent = articleToUpdate, Receiver = newAdmin};

            _articleMock.SetupAllProperties();
            _repositoryMock.Setup(r => r.GetById(0)).Returns(articleToUpdate);
            _articleMock.Setup(a => a.IsOffensive(new List<string>{"Denigrante"})).Returns(true);
            _offenseRepositoryMock.Setup(r => r.GetAllByCondition(It.IsAny<Func<Offense, bool>>())).Returns(new List<Offense>{newOffense});
            _notificationRepositoryMock.Setup(r => r.Store(It.IsAny<Notification<Article>>())).Returns(0);
            _userRepositoryMock.Setup(r => r.GetAllByCondition(It.IsAny<Func<User, bool>>())).Returns(new List<User>{newAdmin});
            _notificationRepositoryMock.Setup(r => r.Store(It.IsAny<Notification<Article>>())).Returns(1);
            _repositoryMock.Setup(r => r.Update(_articleMock.Object));

            articleToUpdate.Name = "new name";
            articleToUpdate.Text = "text";

            ArticleManager articleManager = new(_repositoryMock.Object, _offenseRepositoryMock.Object,
                _notificationRepositoryMock.Object, _userRepositoryMock.Object);
            
            articleManager.Update(_articleMock.Object);
            var retrievedArticle = articleManager.GetById(0);

            _repositoryMock.VerifyAll();
            _articleMock.VerifyAll();
            _userRepositoryMock.VerifyAll();
            _notificationRepositoryMock.VerifyAll();
            _offenseRepositoryMock.VerifyAll();

            Assert.IsTrue(retrievedArticle.Name == "new name");
            Assert.IsTrue(retrievedArticle.Text == "text");
        }

        [TestMethod]
        [ExpectedException(typeof(ResourceNotFoundException))]
        public void UpdateNonExistentArticleFails()
        {
            _repositoryMock.Setup(r => r.GetById(It.IsAny<int>())).Throws(new ResourceNotFoundException("Could not find specified article"));

            ArticleManager articleManager = new(_repositoryMock.Object, _offenseRepositoryMock.Object,
                _notificationRepositoryMock.Object, _userRepositoryMock.Object);
            articleManager.Update(new());

            _repositoryMock.VerifyAll();

        }

        [TestMethod]
        public void DeleteArticleDoesAsExpected()
        {
            Article expectedArticle = new(){Id = 0, Author = new User()};

            _repositoryMock.Setup(r => r.GetById(0)).Returns(expectedArticle);
            _repositoryMock.Setup(r => r.Delete(expectedArticle));

            ArticleManager articleManager = new(_repositoryMock.Object, _offenseRepositoryMock.Object,
                _notificationRepositoryMock.Object, _userRepositoryMock.Object);
            articleManager.Delete(0);

            _repositoryMock.VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(ResourceNotFoundException))]
        public void DeleteNonExistentArticleFails()
        {
            _repositoryMock.Setup(r => r.GetById(It.IsAny<int>())).Throws(new ResourceNotFoundException("Could not find specified article"));

            ArticleManager articleManager = new(_repositoryMock.Object, _offenseRepositoryMock.Object,
                _notificationRepositoryMock.Object, _userRepositoryMock.Object);
            articleManager.Delete(0);

            _repositoryMock.VerifyAll();
        }
    }
}
