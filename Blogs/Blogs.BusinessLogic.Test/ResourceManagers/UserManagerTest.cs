using System.Collections;
using Blogs.BusinessLogic.ResourceManagers;
using Blogs.Domain.BusinessEntities;
using Blogs.Domain.SearchCriteria;
using static Blogs.Instances.DomainInstances;
using Blogs.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Blogs.Domain.BusinessEntities.Mentions;
using Blogs.Exceptions;

namespace Blogs.BusinessLogic.Test.ResourceManagers
{
    [TestClass]
    public class UserManagerTest
    {
        private Mock<User> _userMock;
        private Mock<IRepository<User>> _repositoryMock;

        [TestInitialize]
        public void Setup()
        {
            _repositoryMock = new Mock<IRepository<User>>(MockBehavior.Strict);
            _userMock = new Mock<User>(MockBehavior.Strict);
        }

        [TestMethod]
        public void GetUsersReturnsAsExpected()
        {
            var users = It.IsAny<List<User>>();
            _repositoryMock.Setup(r => r.GetAllByCondition(It.IsAny<Func<User, bool>>())).Returns(users);

            UserManager userManager = new(_repositoryMock.Object);
            var retrievedUsers = userManager.GetAll();

            _repositoryMock.VerifyAll();
            CollectionAssert.AreEquivalent((ICollection) retrievedUsers, users);
        }

        [TestMethod]
        public void GetUsersByConditionReturnsAsExpected()
        {
            List<User> users = new();
            SearchingUserByActivity UserSearchCriteria = new();
           
            _repositoryMock.Setup(r => r.GetAllByCondition(UserSearchCriteria.Criteria)).Returns(users);

            UserManager userManager = new(_repositoryMock.Object);
            var retrievedUsers = userManager.GetAllBy(UserSearchCriteria);

            _repositoryMock.VerifyAll();
            CollectionAssert.AreEquivalent((ICollection) retrievedUsers, users);
        }

        [TestMethod]
        public void GetUserByIdReturnsAsExpected()
        {
            User expectedUser = new() {Id = 0};

            _repositoryMock.Setup(r => r.GetById(0)).Returns(expectedUser);
            UserManager userManager = new(_repositoryMock.Object);
            var retrievedUser = userManager.GetById(0);

            _repositoryMock.VerifyAll();
            Assert.IsTrue(expectedUser.Equals(retrievedUser));
        }

        [TestMethod]
        public void CreateUserReturnsAsExpected()
        {
            User userCreated = new() {Id = 0};

            _userMock.SetupAllProperties();
            _userMock.Setup(u => u.ValidOrFail());
            _repositoryMock.Setup(r => r.Store(_userMock.Object)).Returns(0);
            _repositoryMock.Setup(r => r.GetById(0)).Returns(userCreated);
            
            UserManager userManager = new(_repositoryMock.Object);
            var retrievedUser = userManager.Create(_userMock.Object);

            _repositoryMock.VerifyAll();
            _userMock.VerifyAll();
            Assert.AreEqual(userCreated, retrievedUser);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateEmptyUserFails()
        {
            UserManager userManager = new(_repositoryMock.Object);
            userManager.Create(null);
        }

        [TestMethod]
        public void UpdateUserDoesAsExpected()
        {
            User userToUpdate = CreateChargedUserWithOwnCommentary(0);

            _repositoryMock.Setup(r => r.GetById(userToUpdate.Id)).Returns(userToUpdate);
            _repositoryMock.Setup(r => r.Update(userToUpdate));

            userToUpdate.Name = "new name";
            userToUpdate.LastName = "new lastname";

            UserManager userManager = new(_repositoryMock.Object);
            userManager.Update(userToUpdate);

            var retrievedUser = userManager.GetById(userToUpdate.Id);

            _repositoryMock.VerifyAll();
            Assert.IsTrue(retrievedUser.Name == "new name");
            Assert.IsTrue(retrievedUser.LastName == "new lastname");
        }

        [TestMethod]
        [ExpectedException(typeof(ResourceNotFoundException))]
        public void UpdateNonExistentUserFails()
        {
           _repositoryMock.Setup(r => r.GetById(It.IsAny<int>())).Throws(new ResourceNotFoundException("Could not find specified user"));

            UserManager userManager = new(_repositoryMock.Object);
            userManager.Update(new());

            _repositoryMock.VerifyAll();
        }

        [TestMethod]
        public void DeleteUserDoesAsExpected()
        {
            User expectedUser = new() {Id = 0};   

            _repositoryMock.Setup(r => r.GetById(0)).Returns(expectedUser);
            _repositoryMock.Setup(r => r.Delete(expectedUser));

            UserManager userManager = new(_repositoryMock.Object);
            userManager.Delete(0);

            _repositoryMock.VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(ResourceNotFoundException))]
        public void DeleteNonExistentUserFails()
        {
            _repositoryMock.Setup(r => r.GetById(It.IsAny<int>())).Throws(new ResourceNotFoundException("Could not find specified user"));

            UserManager userManager = new(_repositoryMock.Object);
            userManager.Delete(0);

            _repositoryMock.VerifyAll();
        }

        #region InstanceHelpers

        private User CreateChargedUserWithOwnCommentary(int id)
        {
            var newUser = CreateSimpleUser(id);

            var newArticle = CreateSimpleArticle(newUser, id);
            var newImage = CreateSimpleImage(id);
            newArticle.Images.Add(newImage);

            newImage.Article = newArticle;

            var newResponse = CreateSimpleResponse(newUser, id + 1);
            newResponse.Article = newArticle;

            var newComment = CreateSimpleComment(newUser, id);
            newComment.Article = newArticle;

            newResponse.AssociatedComment = newComment;

            var newComNotification = CreateSimpleNotification<Mention>(newUser, "..", id);
            newComNotification.AssociatedContent = newComment;

            newUser.PostedArticles.Add(newArticle);
            newUser.PostedMentions.Add(newComment);
            newUser.MentionNotifications.Add(newComNotification);

            return newUser;
        }

        #endregion

    }
}