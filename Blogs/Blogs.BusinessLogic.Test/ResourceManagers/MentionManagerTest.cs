using Blogs.BusinessLogic.ResourceManagers;
using Blogs.Domain.BusinessEntities;
using Blogs.Domain.BusinessEntities.Mentions;
using Blogs.Domain.SearchCriteria;
using Blogs.Exceptions;
using Blogs.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections;

namespace Blogs.BusinessLogic.Test.ResourceManagers
{
    [TestClass]
    public class MentionManagerTest
    {
        private Mock<Article> _articleMock;
        private Mock<Mention> _mentionMock;
        private Mock<IRepository<Mention>> _repositoryMock;
        private Mock<IRepository<Offense>> _offenseRepositoryMock;
        private Mock<IRepository<Notification<Mention>>> _notificationRepositoryMock;
        private Mock<IRepository<User>> _userRepositoryMock;

        [TestInitialize]
        public void Setup()
        {
            _repositoryMock = new Mock<IRepository<Mention>>(MockBehavior.Strict);
            _articleMock = new Mock<Article>(MockBehavior.Strict);
            _mentionMock = new Mock<Mention>(MockBehavior.Strict);
            _offenseRepositoryMock = new Mock<IRepository<Offense>>(MockBehavior.Strict);
            _notificationRepositoryMock = new Mock<IRepository<Notification<Mention>>>(MockBehavior.Strict);
            _userRepositoryMock = new Mock<IRepository<User>>(MockBehavior.Strict);
        }

        [TestMethod]
        public void GetMentionsReturnsAsExpected()
        {
            var mentions = It.IsAny<List<Mention>>();
            _repositoryMock.Setup(r => r.GetAllByCondition(It.IsAny<Func<Mention, bool>>())).Returns(mentions);

            MentionManager mentionManager = new(_repositoryMock.Object, _offenseRepositoryMock.Object, 
                _notificationRepositoryMock.Object, _userRepositoryMock.Object);
            var retrievedMentions = mentionManager.GetAll();

            _repositoryMock.VerifyAll();
            CollectionAssert.AreEquivalent((ICollection) retrievedMentions, mentions);
        }

        [TestMethod]
        public void GetMentionsByConditionReturnsAsExpected()
        {
            List<Mention> mentions = new();
            SearchingMentionByType<Comment> mentionSearchCriteria = new();

            _repositoryMock.Setup(r => r.GetAllByCondition(mentionSearchCriteria.Criteria)).Returns(mentions);

            MentionManager mentionManager = new(_repositoryMock.Object, _offenseRepositoryMock.Object,
                _notificationRepositoryMock.Object, _userRepositoryMock.Object);
            var retrievedMentions = mentionManager.GetAllBy(mentionSearchCriteria);

            _repositoryMock.VerifyAll();
            CollectionAssert.AreEquivalent((ICollection)retrievedMentions, mentions);
        }

        [TestMethod]
        public void GeMentionByIdReturnsAsExpected()
        {
            Mention expectedMention = new() {Id = 0};

            _repositoryMock.Setup(r => r.GetById(0)).Returns(expectedMention);
            MentionManager mentionManager = new(_repositoryMock.Object, _offenseRepositoryMock.Object,
                _notificationRepositoryMock.Object, _userRepositoryMock.Object);
            var retrievedMention = mentionManager.GetById(0);

            _repositoryMock.VerifyAll();
            Assert.IsTrue(expectedMention.Equals(retrievedMention));
        }

        [TestMethod]
        public void CreateMentionReturnsAsExpected()
        {
            User newAdmin = new(){Role = Domain.UserRole.Admin};
            Offense newOffense = new(){Id = 0, Word = "Denigrante"};
            Mention mentionCreated = new Comment() {Id = 0, Article = new(){Author = new User()}};
            Notification<Mention> newOffenseNotification = new() {Id = 0, Text = "Your mention has offensive words, so it has been detached",
                    AssociatedContent = mentionCreated, Receiver = mentionCreated.Author};
            Notification<Mention> newOffenseNotificationForAdmin = new() {Id = 1, Text = "This mention has offensive words, so it has been detached",
                    AssociatedContent = mentionCreated, Receiver = newAdmin};
            Notification<Mention> newCreatedNotification = new() {Id = 2, Text = "Your article has a new comment",
                    AssociatedContent = mentionCreated, Receiver = mentionCreated.Article.Author};

            _articleMock.SetupAllProperties();
            _mentionMock.SetupAllProperties();
            _mentionMock.Object.Article = _articleMock.Object;

            _mentionMock.Setup(a => a.ValidOrFail());
            _offenseRepositoryMock.Setup(r => r.GetAllByCondition(It.IsAny<Func<Offense, bool>>())).Returns(new List<Offense>{newOffense});
            _mentionMock.Setup(a => a.IsOffensive(new List<string>{newOffense.Word})).Returns(true);
            _repositoryMock.Setup(r => r.Store(_mentionMock.Object)).Returns(0);
            _notificationRepositoryMock.Setup(r => r.Store(It.IsAny<Notification<Mention>>())).Returns(0);
            _userRepositoryMock.Setup(r => r.GetAllByCondition(It.IsAny<Func<User, bool>>())).Returns(new List<User>{newAdmin});
            _notificationRepositoryMock.Setup(r => r.Store(It.IsAny<Notification<Mention>>())).Returns(1);
            _notificationRepositoryMock.Setup(r => r.Store(It.IsAny<Notification<Mention>>())).Returns(2);
            _repositoryMock.Setup(r => r.GetById(0)).Returns(mentionCreated);

            MentionManager mentionManager = new(_repositoryMock.Object, _offenseRepositoryMock.Object,
                _notificationRepositoryMock.Object, _userRepositoryMock.Object);
            var retrievedMention = mentionManager.Create(_mentionMock.Object);

            _repositoryMock.VerifyAll();
            _mentionMock.VerifyAll();
            _offenseRepositoryMock.VerifyAll();
            _notificationRepositoryMock.VerifyAll();
            _userRepositoryMock.VerifyAll();

            Assert.AreEqual(mentionCreated, retrievedMention);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateEmptyMentionFails()
        {
            MentionManager mentionManager = new(_repositoryMock.Object, _offenseRepositoryMock.Object,
                _notificationRepositoryMock.Object, _userRepositoryMock.Object);
            mentionManager.Create(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateEmptyArticleMentionFails()
        {
            Mention mentionCreated = new() {Id = 0};

            _mentionMock.SetupAllProperties();

            _mentionMock.Setup(a => a.ValidOrFail());
            _repositoryMock.Setup(r => r.Store(_mentionMock.Object)).Returns(0);
            _repositoryMock.Setup(r => r.GetById(0)).Returns(mentionCreated);

            MentionManager mentionManager = new(_repositoryMock.Object, _offenseRepositoryMock.Object,
                _notificationRepositoryMock.Object, _userRepositoryMock.Object);
            var retrievedMention = mentionManager.Create(_mentionMock.Object);
        }

        [TestMethod]
        public void UpdateMentionReturnsAsExpected()
        {
            Offense newOffense = new(){Id = 0, Word = "Denigrante"};
            List<string> offensiveWords = new(){newOffense.Word};

            var retrievedMentionMock = new Mock<Mention>(MockBehavior.Strict);
            retrievedMentionMock.SetupAllProperties();
            _mentionMock.SetupAllProperties();

            _repositoryMock.Setup(r => r.GetById(0)).Returns(retrievedMentionMock.Object);
            _offenseRepositoryMock.Setup(r => r.GetAllByCondition(It.IsAny<Func<Offense, bool>>())).Returns(new List<Offense>{newOffense});
            retrievedMentionMock.Setup(m => m.IsOffensive(offensiveWords)).Returns(true);
            _mentionMock.Setup(m => m.IsOffensive(offensiveWords)).Returns(false);
            _repositoryMock.Setup(r => r.Update(_mentionMock.Object));

            MentionManager mentionManager = new(_repositoryMock.Object, _offenseRepositoryMock.Object,
                _notificationRepositoryMock.Object, _userRepositoryMock.Object);

            mentionManager.Update(_mentionMock.Object);

            retrievedMentionMock.VerifyAll();
            _mentionMock.VerifyAll();
            _repositoryMock.VerifyAll();
            _offenseRepositoryMock.VerifyAll();
        }
       
        [TestMethod]
        [ExpectedException(typeof(ResourceNotFoundException))]
        public void UpdateNonExistentMentionFails()
        {
            _repositoryMock.Setup(r => r.GetById(It.IsAny<int>())).Throws(new ResourceNotFoundException("Could not find specified mention"));

            MentionManager mentionManager = new(_repositoryMock.Object, _offenseRepositoryMock.Object,
                _notificationRepositoryMock.Object, _userRepositoryMock.Object);
            mentionManager.Update(new());

            _repositoryMock.VerifyAll();
        }
    }
}
