using Blogs.DataAccess.Contexts;
using Blogs.DataAccess.Repositories;
using static Blogs.Instances.DomainInstances;
using Blogs.Exceptions;
using Blogs.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Blogs.Domain.BusinessEntities.Mentions;
using Microsoft.EntityFrameworkCore;
using Blogs.Domain.BusinessEntities;

namespace Blogs.DataAccess.Test.Repositories
{
    [TestClass]
    public class NotificationRepositoryTest
    {
        //análogo a notificaciones sobre menciones
        private Notification<Mention> _defaultNotification;

        private IRepository<Notification<Mention>> _repository;
        private IRepository<Mention> _mentionRepository;
        private IRepository<User> _userRepository;
        private IRepository<Article> _articleRepository;

        private DataContext _dataContext;

        [TestInitialize]
        public void SetUp()
        {
            _dataContext = ContextFactory.GetNewContext(ContextType.SqLite);
            _repository = new NotificationRepository<Mention>(_dataContext);
            _userRepository = new UserRepository(_dataContext);
            _articleRepository = new ArticleRepository(_dataContext);
            _mentionRepository = new MentionRepository(_dataContext);

            _dataContext.Database.OpenConnection();
            _dataContext.Database.EnsureCreated();

            SetEnvironmentEntities();
        }

        [TestMethod]
        public void GetAllNotificationsOnEmptyRepository()
        {
            var retrievedUser = _userRepository.GetById(1);
            _userRepository.Delete(retrievedUser);

            List<Notification<Mention>> expectedNotifications = new();
            var retrievedNotifications = _repository.GetAllByCondition(n => true);
            CollectionAssert.AreEquivalent(expectedNotifications, retrievedNotifications.ToList());
        }

        [TestMethod]
        public void GetAllNotificationsReturnsAsExpected()
        {
            SetSecondRoundOfUserRelatedEntities();
            var newNotification = _repository.GetById(2);

            List<Notification<Mention>> expectedNotifications = new() {_defaultNotification, newNotification};
            var retrievedNotifications = _repository.GetAllByCondition(n => true);
            CollectionAssert.AreEquivalent(expectedNotifications, retrievedNotifications.ToList());
        }

        [TestMethod]
        public void GetAllNotificationsIncludesRelatedEntities()
        {
            SetSecondRoundOfUserRelatedEntities();
            var retrievedNotifications = _repository.GetAllByCondition(n => true);
            
            Assert.IsTrue(retrievedNotifications.All(n => n.AssociatedContent != null));
        }

        [TestMethod]
        public void GetNotificationByIdReturnsAsExpected()
        {
            var retrievedNotification = _repository.GetById(_defaultNotification.Id);
            Assert.IsTrue(_defaultNotification.Equals(retrievedNotification));
        }

        [TestMethod]
        [ExpectedException(typeof(ResourceNotFoundException))]
        public void GetNotificationByIdDoesntExist()
        {
            _repository.GetById(-1);
        }

        [TestMethod]
        public void StoreNotificationReturnsAsExpected()
        {
            Assert.IsTrue(_repository.GetAllByCondition(n => true).Count() == 1);
        }

        [TestMethod]
        public void DeleteNotificationDoesAsExpected()
        {
            _repository.Delete(_defaultNotification);

            Assert.IsTrue(_repository.GetAllByCondition(n => true).Count() == 0);
            Assert.IsNotNull(_mentionRepository.GetById(1));
        }

        [TestCleanup]
        public void CleanUp()
        {
            _dataContext.Database.EnsureDeleted();
        }

        #region Helpers

        private void SetEnvironmentEntities()
        {
            var newUser = CreateSimpleUser(1);
            var articleBloggerId = _userRepository.Store(newUser);

            var otherUser = CreateSimpleUser(2);
            otherUser = UpdateUserAlternativeKeys(otherUser);
            var otherUserId = _userRepository.Store(otherUser);

            var retrievedArticleBlogger = _userRepository.GetById(articleBloggerId);
            var newArticle = CreateSimpleArticle(retrievedArticleBlogger, 1);
            _dataContext.Entry(retrievedArticleBlogger).State = EntityState.Unchanged;
            var newArticleId = _articleRepository.Store(newArticle);

            var retrievedCommentator = _userRepository.GetById(otherUserId);
            var retrievedArticle = _articleRepository.GetById(newArticleId);
            var newComment = CreateSimpleComment(retrievedCommentator, 1);
            newComment.Article = retrievedArticle;
            _dataContext.Entry(retrievedCommentator).State = EntityState.Unchanged;

            var newCommentId = _mentionRepository.Store(newComment);

            retrievedArticleBlogger = _userRepository.GetById(articleBloggerId);
            var retrievedComment = _mentionRepository.GetById(newCommentId);
            _defaultNotification = CreateSimpleNotification<Mention>(retrievedArticleBlogger, "..", 1);
            _defaultNotification.AssociatedContent = retrievedComment;
            _dataContext.Entry(retrievedArticleBlogger).State = EntityState.Unchanged;
            _repository.Store(_defaultNotification);
        }

        private void SetSecondRoundOfUserRelatedEntities()
        {
            var retrievedArticleBlogger = _userRepository.GetById(1);
            var newArticle = CreateSimpleArticle(retrievedArticleBlogger, 2);
            newArticle = UpdateArticleAlternativeKey(newArticle);
            _dataContext.Entry(retrievedArticleBlogger).State = EntityState.Unchanged;
            _articleRepository.Store(newArticle);

            retrievedArticleBlogger = _userRepository.GetById(retrievedArticleBlogger.Id);
            var newOwnComment = CreateSimpleComment(retrievedArticleBlogger, 2);
            newOwnComment.Article = newArticle;
            var newOwnCommentId = _mentionRepository.Store(newOwnComment);

            retrievedArticleBlogger = _userRepository.GetById(retrievedArticleBlogger.Id);
            var newNotification = CreateSimpleNotification<Mention>(retrievedArticleBlogger, "..", 2);
            newNotification.AssociatedContent = retrievedArticleBlogger.PostedMentions.FirstOrDefault(m => m.Id == newOwnCommentId);
            _dataContext.Entry(retrievedArticleBlogger).State = EntityState.Unchanged;
            _repository.Store(newNotification);
        }

        #endregion
    }
}
