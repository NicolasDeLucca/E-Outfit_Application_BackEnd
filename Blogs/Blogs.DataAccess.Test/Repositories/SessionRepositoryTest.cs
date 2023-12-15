using Blogs.DataAccess.Repositories;
using Blogs.Domain.BusinessEntities;
using static Blogs.Instances.DomainInstances;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Blogs.DataAccess.Contexts;
using Blogs.Interfaces;
using Blogs.Domain.BusinessEntities.Mentions;

namespace Blogs.DataAccess.Test.Repositories
{
    [TestClass]
    public class SessionRepositoryTest
    {
        private Session _defaultSession;

        private IRepository<Notification<Mention>> _notificationRepository;
        private IRepository<Mention> _mentionRepository;
        private IRepository<Article> _articleRepository;
        private IRepository<Session> _repository;
        private IRepository<User> _userRepository;
        private DataContext _dataContext;

        [TestInitialize]
        public void SetUp()
        {
            _dataContext = ContextFactory.GetNewContext(ContextType.SqLite);

            _notificationRepository = new NotificationRepository<Mention>(_dataContext);
            _mentionRepository = new MentionRepository(_dataContext);
            _repository = new SessionRepository(_dataContext);   
            _userRepository = new UserRepository(_dataContext);
            _articleRepository = new ArticleRepository(_dataContext);

            _dataContext.Database.OpenConnection();
            _dataContext.Database.EnsureCreated();

            SetEnvironmentEntities();
        }

        [TestMethod]
        public void GetAllSessionsOnEmptyRepository()
        {
            var retrievedSession = _repository.GetById(_defaultSession.Id);
            _repository.Delete(retrievedSession);

            List<Session> expectedSessions = new();
            var retrievedSessions = _repository.GetAllByCondition(s => true);
            CollectionAssert.AreEquivalent(expectedSessions, retrievedSessions.ToList());
        }

        [TestMethod]
        public void GetAllSessionsReturnsAsExpected()
        {
            var newUser = CreateSimpleUser(2);
            var newUserId = _userRepository.Store(newUser);

            var retrievedUser = _userRepository.GetById(newUserId);
            var newSession = new Session {Id = 2, AuthenticatedUser = retrievedUser};
            var newSessionId = _repository.Store(newSession);

            var retrievedSession = _repository.GetById(newSessionId);
            var defaultSession = _repository.GetById(_defaultSession.Id);
            List<Session> expectedSessions = new() {defaultSession, retrievedSession};
            var retrievedSessions = _repository.GetAllByCondition(s => true);
            CollectionAssert.AreEquivalent(expectedSessions, retrievedSessions.ToList());
        }

        [TestMethod]
        public void StoreSessionDoesAsExpected()
        {
            Assert.IsTrue(_repository.GetAllByCondition(u => true).Count() == 1);
        }

        [TestMethod]
        public void GetSessionByIdReturnsAsExpected()
        {
            var retrievedSession = _repository.GetById(_defaultSession.Id);
            Assert.IsTrue(_defaultSession.Equals(retrievedSession));
        }

        [TestMethod]
        public void GetSessionByIdIncludesAuthenticatedUser()
        {
            var retrievedSession = _repository.GetById(_defaultSession.Id);
            var retrievedUser = retrievedSession.AuthenticatedUser;
            Assert.IsTrue(retrievedUser != null);
            Assert.IsTrue(retrievedUser.PostedArticles != null && retrievedUser.PostedArticles.Count > 0);
            Assert.IsTrue(retrievedUser.PostedArticles.All(a => a.Images != null && a.Images.Count > 0));
            Assert.IsTrue(retrievedUser.PostedMentions != null && retrievedUser.PostedMentions.Count > 0);
            Assert.IsTrue(retrievedUser.PostedMentions.All(m => m.Article != null));
            Assert.IsTrue(retrievedUser.MentionNotifications != null && retrievedUser.MentionNotifications.Count > 0);
            Assert.IsTrue(retrievedUser.MentionNotifications.All(n => n.AssociatedContent != null));
        }

        [TestMethod]
        public void DeleteSessionDoesAsExpected()
        {
            var retrievedSession = _repository.GetById(_defaultSession.Id);
            _repository.Delete(retrievedSession);

            Assert.IsTrue(_repository.GetAllByCondition(u => true).Count() == 0);
            Assert.IsNotNull(_userRepository.GetById(retrievedSession.AuthenticatedUser.Id));
        }

        [TestCleanup]
        public void CleanUp()
        {
            _dataContext.Database.EnsureDeleted();
        }

        #region SetUpHelpers

        private void SetEnvironmentEntities()
        {
            var newUser = CreateAndStoreChargedUserWithOwnCommentary(1);

            _defaultSession = new Session {Id = 1, AuthenticatedUser = newUser};
            _repository.Store(_defaultSession);
        }

        private User CreateAndStoreChargedUserWithOwnCommentary(int id)
        {
            var newUser = CreateSimpleUser(id);
            var newUserId = _userRepository.Store(newUser);

            var retrievedUser = _userRepository.GetById(newUserId);
            var newArticle = CreateSimpleArticle(retrievedUser, id);
            var newImage = CreateSimpleImage(id);
            newArticle.Images.Add(newImage);
            _dataContext.Entry(retrievedUser).State = EntityState.Unchanged;
            var articleId = _articleRepository.Store(newArticle);

            retrievedUser = _userRepository.GetById(newUserId);
            var retrievedArticle = _articleRepository.GetById(articleId);
            var newComment = CreateSimpleComment(retrievedUser, id);
            newComment.Article = retrievedArticle;
            _dataContext.Entry(retrievedUser).State = EntityState.Unchanged;
            var commentId = _mentionRepository.Store(newComment);

            retrievedUser = _userRepository.GetById(newUserId);
            retrievedArticle = retrievedUser.PostedArticles.First();
            var retrievedComment = (Comment) _mentionRepository.GetById(commentId);
            retrievedArticle.Mentions.Add(retrievedComment);
            var newResponse = CreateSimpleResponse(retrievedUser, id+1);
            newResponse.Article = retrievedArticle;
            newResponse.AssociatedComment = retrievedComment;
            _dataContext.Entry(retrievedUser).State = EntityState.Unchanged;
            _mentionRepository.Store(newResponse);

            retrievedUser = _userRepository.GetById(newUserId);
            var newComNotification = CreateSimpleNotification<Mention>(retrievedUser, "..", id);
            newComNotification.AssociatedContent = retrievedUser.PostedMentions.FirstOrDefault(m => m.Id == 1);
            _dataContext.Entry(retrievedUser).State = EntityState.Unchanged;
            _notificationRepository.Store(newComNotification);

            return retrievedUser;
        }

        #endregion

    }
}
