using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Blogs.Domain.BusinessEntities;
using Blogs.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Blogs.Models.Out;
using static Blogs.Instances.DomainInstances;
using static Blogs.Instances.ModelInstances;
using Blogs.Interfaces;
using Blogs.Domain.SearchCriteria;

namespace Blogs.WebApi.Test.Controllers
{
    [TestClass]
    public class UserControllerTest
    {
        private Mock<IManager<User>> _usersManagerMock;
        private Mock<IManager<Offense>> _offensesManagerMock;
        private Mock<IManager<Session>> _sessionsManagerMock;

        [TestInitialize]
        public void SetUp()
        {
            _usersManagerMock = new Mock<IManager<User>>(MockBehavior.Strict);
            _offensesManagerMock = new Mock<IManager<Offense>>(MockBehavior.Strict);
            _sessionsManagerMock = new Mock<IManager<Session>>(MockBehavior.Strict);
        }

        [TestMethod]
        public void GetAllUsersOkTest()
        {
            List<User> retrievedUsers = new() {CreateSimpleUser(0)};
            List<UserBasicInfo> usersBasicInfo = new();
            foreach (User u in retrievedUsers) usersBasicInfo.Add(new UserBasicInfo(u));

            _usersManagerMock.Setup(m => m.GetAll()).Returns(retrievedUsers);
            UserController userController = new(_usersManagerMock.Object, _offensesManagerMock.Object,
                _sessionsManagerMock.Object);

            IActionResult result = userController.GetAll();
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            OkObjectResult okResult = (OkObjectResult) result;

            var responseModel = (IEnumerable<UserBasicInfo>) okResult.Value;

            _usersManagerMock.VerifyAll();

            Assert.IsTrue(okResult.StatusCode == StatusCodes.Status200OK);
            CollectionAssert.AreEquivalent(usersBasicInfo, responseModel.ToList());
        }

        [TestMethod]
        public void GetAllUsersByActivityOkTest()
        {
            List<User> retrievedUsers = new() {CreateSimpleUser(0)};
            List<UserBasicInfo> usersBasicInfo = new();
            foreach (User u in retrievedUsers) usersBasicInfo.Add(new UserBasicInfo(u));

            _usersManagerMock.Setup(m => m.GetAllBy(It.IsAny<SearchingUserByActivity>())).Returns(retrievedUsers);
            UserController userController = new(_usersManagerMock.Object, _offensesManagerMock.Object, 
                _sessionsManagerMock.Object);

            IActionResult result = userController.GetAllByActivity(DateTime.MinValue.ToString(), DateTime.MaxValue.ToString()); 
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            OkObjectResult okResult = (OkObjectResult) result;
            var responseModel = (IEnumerable<UserBasicInfo>) okResult.Value;

            _usersManagerMock.VerifyAll();

            Assert.IsTrue(okResult.StatusCode == StatusCodes.Status200OK);
            CollectionAssert.AreEquivalent(usersBasicInfo, responseModel.ToList());
        }

        [TestMethod]
        public void GetUserByIdOkTest()
        {
            var retrievedUser = CreateSimpleUser(0);

            _usersManagerMock.Setup(m => m.GetById(It.IsAny<int>())).Returns(retrievedUser);
            UserController userController = new(_usersManagerMock.Object, _offensesManagerMock.Object,
                _sessionsManagerMock.Object);
            IActionResult result = userController.GetById(0);

            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            OkObjectResult okResult = (OkObjectResult) result;
            var responseModel = (UserDetail) okResult.Value;

            _usersManagerMock.VerifyAll();
            
            Assert.IsTrue(okResult.StatusCode == StatusCodes.Status200OK);
            Assert.IsTrue(new UserDetail(retrievedUser).Equals(responseModel));
        }

        [TestMethod]
        public void PostUserCreatedTest()
        {
            var userModel = CreateUserModel();
            UserDetail userDetail = new(userModel.ToEntity());
            
            _usersManagerMock.Setup(m => m.Create(It.IsAny<User>())).Returns(userModel.ToEntity());
            UserController userController = new(_usersManagerMock.Object, _offensesManagerMock.Object,
                _sessionsManagerMock.Object);
            IActionResult result = userController.Post(userModel);

            Assert.IsInstanceOfType(result, typeof(CreatedResult));
            CreatedResult createdResult = (CreatedResult) result;
            var responseModel = (UserDetail) createdResult.Value;

            _usersManagerMock.VerifyAll();

            Assert.IsTrue(createdResult.StatusCode == StatusCodes.Status201Created);
            Assert.IsTrue(userDetail.Equals(responseModel));
        }

        [TestMethod]
        public void PutUserByIdNoContentTest()
        {
            _usersManagerMock.Setup(m => m.Update(It.IsAny<User>()));

            UserController userController = new(_usersManagerMock.Object, _offensesManagerMock.Object,
                _sessionsManagerMock.Object);
            IActionResult result = userController.PutAsAdmin(0, new());

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            NoContentResult noContentResult = (NoContentResult) result;

            _usersManagerMock.VerifyAll();
            Assert.IsTrue(noContentResult.StatusCode == StatusCodes.Status204NoContent);
        }

        [TestMethod]
        public void PutMySelfNoContentTest()
        {
            var userModel = CreateUserModel();
            var userEntity = userModel.ToEntity();
            Session activeSession = new() {AuthenticatedUser = userEntity};
            string authorization = activeSession.AuthToken.ToString();

            _sessionsManagerMock.Setup(m => m.GetAllBy(It.IsAny<ISearchCriteria<Session>>())).Returns(new List<Session>{activeSession});
            _usersManagerMock.Setup(m => m.Update(It.IsAny<User>()));

            UserController userController = new(_usersManagerMock.Object, _offensesManagerMock.Object,
                _sessionsManagerMock.Object);
            IActionResult result = userController.Put(authorization, userModel);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            NoContentResult noContentResult = (NoContentResult)result;

            _sessionsManagerMock.VerifyAll();
            _usersManagerMock.VerifyAll();
            Assert.IsTrue(noContentResult.StatusCode == StatusCodes.Status204NoContent);
        }

        [TestMethod]
        public void DeleteAsAdminUserByIdNoContentTest()
        {
            _usersManagerMock.Setup(m => m.Delete(It.IsAny<int>()));

            UserController userController = new(_usersManagerMock.Object, _offensesManagerMock.Object,
                _sessionsManagerMock.Object);
            IActionResult result = userController.DeleteAsAdmin(0);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            NoContentResult noContentResult = (NoContentResult) result;

            _usersManagerMock.VerifyAll();
            Assert.IsTrue(noContentResult.StatusCode == StatusCodes.Status204NoContent);
        }

        [TestMethod]
        public void DeleteUserByIdNoContentTest()
        {
            var session = new Session {AuthenticatedUser = new User {Id = 0}};
            _sessionsManagerMock.Setup(m => m.GetAllBy(It.IsAny<ISearchCriteria<Session>>())).Returns(new List<Session> {session});
            _usersManagerMock.Setup(m => m.Delete(It.IsAny<int>()));

            UserController userController = new(_usersManagerMock.Object, _offensesManagerMock.Object,
                _sessionsManagerMock.Object);
            IActionResult result = userController.Delete(session.AuthToken.ToString());

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            NoContentResult noContentResult = (NoContentResult)result;

            _sessionsManagerMock.VerifyAll();
            _usersManagerMock.VerifyAll();
            Assert.IsTrue(noContentResult.StatusCode == StatusCodes.Status204NoContent);
        }
    }
}