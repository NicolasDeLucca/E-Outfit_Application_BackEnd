using Blogs.DataAccess.Repositories;
using Blogs.Domain.BusinessEntities;
using Blogs.Exceptions;
using static Blogs.Instances.DomainInstances;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Blogs.Domain;
using Blogs.DataAccess.Contexts;
using Blogs.Interfaces;
using Blogs.Domain.BusinessEntities.Mentions;
using EntityState = Microsoft.EntityFrameworkCore.EntityState;

namespace Blogs.DataAccess.Test.Repositories
{
    [TestClass]
    public class UserRepositoryTest
    {
        private IRepository<User> _repository;
        private IRepository<Article> _articleRepository;
        private IRepository<Notification<Mention>> _notificationRepository;
        private IRepository<Mention> _mentionRepository;

        private DataContext _dataContext;

        [TestInitialize]
        public void SetUp()
        {
            _dataContext = ContextFactory.GetNewContext(ContextType.SqLite);
            _repository = new UserRepository(_dataContext);
            _articleRepository = new ArticleRepository(_dataContext);
            _notificationRepository = new NotificationRepository<Mention>(_dataContext);
            _mentionRepository = new MentionRepository(_dataContext);  

            _dataContext.Database.OpenConnection();
            _dataContext.Database.EnsureCreated();
        }

        [TestMethod]
        public void GetAllUsersOnEmptyRepository()
        {
            List<User> expectedUsers = new();
            var retrievedUsers = _repository.GetAllByCondition(u => true);

            CollectionAssert.AreEquivalent(expectedUsers, retrievedUsers.ToList());
        }

        [TestMethod]
        public void GetAllUsersReturnsAsExpected()
        {
            var newUser = CreateSimpleUser(1);
            _repository.Store(newUser);

            var otherUser = CreateSimpleUser(2);
            otherUser = UpdateUserAlternativeKeys(otherUser);
            _repository.Store(otherUser);

            List<User> expectedUsers = new() {newUser, otherUser};
            var retrievedUsers = _repository.GetAllByCondition(u => true);
            CollectionAssert.AreEquivalent(expectedUsers, retrievedUsers.ToList());
        }

        [TestMethod]
        public void GetAllUsersIncludesRelatedEntities()
        {
            var simpleUser = CreateSimpleUser(1);
            SaveAndChargeSimpleUserWithOwnCommentary(simpleUser);

            var retrievedUser = _repository.GetById(simpleUser.Id);

            Assert.IsTrue(retrievedUser.PostedArticles != null && retrievedUser.PostedArticles.Count > 0);
            Assert.IsTrue(retrievedUser.PostedArticles.All(a => a.Images != null && a.Images.Count > 0));
            Assert.IsTrue(retrievedUser.PostedMentions != null && retrievedUser.PostedMentions.Count > 0);
            Assert.IsTrue(retrievedUser.PostedMentions.All(m => m.Article != null));
            Assert.IsTrue(retrievedUser.MentionNotifications != null && retrievedUser.MentionNotifications.Count > 0);
            Assert.IsTrue(retrievedUser.MentionNotifications.All(n => n.AssociatedContent != null));
        }

        [TestMethod]
        public void SaveUsersWithOutConcurrency()
        {
            var newReceiverUser = CreateSimpleUser(1);
            var retrievedId = _repository.Store(newReceiverUser);

            var retrievedReceiver = _repository.GetById(retrievedId);
            var newArticle = CreateSimpleArticle(retrievedReceiver, 1);
            var newImage = CreateSimpleImage(1);
            newArticle.Images.Add(newImage);
            _dataContext.Entry(retrievedReceiver).State = EntityState.Unchanged;
            var articleId = _articleRepository.Store(newArticle);

            var newCommentatorUser = CreateSimpleUser(2);
            newCommentatorUser = UpdateUserAlternativeKeys(newCommentatorUser);
            var commentatorId = _repository.Store(newCommentatorUser);

            var retrievedCommentator = _repository.GetById(commentatorId);
            var retrievedArticle = _articleRepository.GetById(articleId);
            var newComment = CreateSimpleComment(retrievedCommentator, 1);
            newComment.Article = retrievedArticle;
            _dataContext.Entry(retrievedCommentator).State = EntityState.Unchanged;
            _mentionRepository.Store(newComment);

            retrievedReceiver = _repository.GetById(retrievedId);
            retrievedCommentator = _repository.GetById(commentatorId);
            var newResponse = CreateSimpleResponse(retrievedReceiver, 2);
            newResponse.Article = retrievedArticle;
            newResponse.AssociatedComment = (Comment) retrievedCommentator.PostedMentions.First();
            _dataContext.Entry(retrievedReceiver).State = EntityState.Unchanged;
            _mentionRepository.Store(newResponse);            
        }

        [TestMethod]
        public void GetUserByIdReturnsAsExpected()
        {
            var expectedUser = CreateSimpleUser(1);
            _repository.Store(expectedUser);

            var retrievedUser = _repository.GetById(1);
            Assert.IsTrue(expectedUser.Equals(retrievedUser));
        }

        [TestMethod]
        [ExpectedException(typeof(ResourceNotFoundException))]
        public void GetUserByIdDoesntExist()
        {
            _repository.GetById(-1);
        }

        [TestMethod]
        public void UpdateUserDoesAsExpected()
        {
            var newRole = UserRole.Admin;

            var newUser = CreateSimpleUser(1);
            var newUserId = _repository.Store(newUser);

            var userToUpdate = _repository.GetById(newUserId);
            userToUpdate.Role = newRole;
            _repository.Update(userToUpdate);

            var updatedUser = _repository.GetById(newUserId);
            Assert.AreEqual(userToUpdate, updatedUser);
            Assert.IsTrue(updatedUser.Role == newRole);
        }

        [TestMethod]
        public void StoreUserReturnsAsExpected()
        {
            var newUser = CreateSimpleUser(1);
            _repository.Store(newUser);

            Assert.IsTrue(_repository.GetAllByCondition(u => true).Count() == 1);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void StoreUserWithSameAlternativeKeysFails()
        {
            var newUser = CreateSimpleUser(1);
            _repository.Store(newUser);

            var otherUser = CreateSimpleUser(2);
            _repository.Store(otherUser);
        }

        [TestMethod]
        public void DeleteUserDoesAsExpected()
        {
            var newUser = CreateSimpleUser(1);
            SaveAndChargeSimpleUserWithOwnCommentary(newUser);

            var userToBeDeleted = _repository.GetById(newUser.Id);
            _repository.Delete(userToBeDeleted);

            Assert.IsTrue(_repository.GetAllByCondition(u => true).Count() == 0);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _dataContext.Database.EnsureDeleted();
        }

        #region Helper

        private void SaveAndChargeSimpleUserWithOwnCommentary(User simpleUser)
        {
            var userId = _repository.Store(simpleUser);

            var retrievedUser = _repository.GetById(userId);
            var newArticle = CreateSimpleArticle(retrievedUser, 1);
            var newImage = CreateSimpleImage(1);
            newArticle.Images.Add(newImage);
            _dataContext.Entry(retrievedUser).State = EntityState.Unchanged;
            _articleRepository.Store(newArticle);

            retrievedUser = _repository.GetById(userId);
            var newResponse = CreateSimpleResponse(retrievedUser, 1);
            var retrievedArticle = retrievedUser.PostedArticles.First();
            newResponse.Article = retrievedArticle;
            var newComment = CreateSimpleComment(retrievedUser, 2);
            newComment.Article = retrievedArticle;
            var commentId = _mentionRepository.Store(newComment);

            retrievedUser = _repository.GetById(userId);
            var newComNotification = CreateSimpleNotification<Mention>(retrievedUser,"..", 1);
            var retrievedComment = retrievedUser.PostedMentions.Where(m => m.Id == commentId).First();
            newComNotification.AssociatedContent = retrievedComment;
            _dataContext.Entry(retrievedUser).State = EntityState.Unchanged;
            _notificationRepository.Store(newComNotification);
        }

        #endregion
    }
}