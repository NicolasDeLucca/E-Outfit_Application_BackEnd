using Blogs.Domain.BusinessEntities;
using Blogs.Exceptions;
using Blogs.Interfaces;
using Blogs.Models.In;
using Blogs.Models.Out;
using Blogs.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Security.Authentication;

namespace Blogs.WebApi.Test.Controllers
{
    [TestClass]
    public class SessionControllerTest
    {
        private Mock<IManager<Session>> _sessionsServiceMock;

        [TestInitialize]
        public void SetUp()
        {
            _sessionsServiceMock = new Mock<IManager<Session>>(MockBehavior.Strict); 
        }

        [TestMethod]
        public void LoginOkTest()
        {
            LogInModel loginDTO = new() {UserName = "key", Password = "value"};
            var expectedSession = new Session {AuthenticatedUser = loginDTO.ToEntity()};

            _sessionsServiceMock.Setup(s => s.Create(It.IsAny<Session>())).Returns(expectedSession);

            SessionController sessionController = new(_sessionsServiceMock.Object);
            IActionResult result = sessionController.Post(loginDTO);

            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            OkObjectResult okResult = (OkObjectResult) result;
            var responseModel = (LogInDetail) okResult.Value;

            _sessionsServiceMock.VerifyAll();

            Assert.IsTrue(okResult.StatusCode == StatusCodes.Status200OK); 
            Assert.IsTrue(new LogInDetail(expectedSession).Equals(responseModel));
        }

        [ExpectedException(typeof(InvalidCredentialException))]
        [TestMethod]
        public void LoginBadRequestTest()
        {
            LogInModel emptyLog = new();
            Session createdSession = new() {AuthenticatedUser = emptyLog.ToEntity()};

            _sessionsServiceMock.Setup(s => s.Create(createdSession)).Throws(new InvalidCredentialException());

            SessionController sessionController = new(_sessionsServiceMock.Object);
            IActionResult result = sessionController.Post(emptyLog);

            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            BadRequestObjectResult badRequestResult = (BadRequestObjectResult) result;

            _sessionsServiceMock.VerifyAll();
            Assert.IsTrue(badRequestResult.StatusCode == StatusCodes.Status400BadRequest);
        }

        [TestMethod]
        public void GetCurrentUserOkTest()
        {
            User expectedUser = new();
            IEnumerable<Session> expectedSessionsBy = new List<Session>(){new() {AuthenticatedUser = expectedUser}};

            _sessionsServiceMock.Setup(s => s.GetAllBy(It.IsAny<ISearchCriteria<Session>>())).Returns(expectedSessionsBy);

            SessionController sessionController = new(_sessionsServiceMock.Object);
            IActionResult result = sessionController.GetCurrentUser(It.IsAny<Guid>().ToString());

            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var resultOk = (OkObjectResult) result;

            var retrievedUser = (UserDetail) resultOk.Value;

            _sessionsServiceMock.VerifyAll();
            Assert.IsTrue(resultOk.StatusCode == StatusCodes.Status200OK);
            Assert.AreEqual(new UserDetail(expectedUser), retrievedUser);
        }

        [ExpectedException(typeof(InvalidRequestDataException))]
        [TestMethod]
        public void GetCurrentUserInvalidTokenBadRequestTest()
        {
            var invalidTokenString = "|_";

            SessionController sessionController = new(_sessionsServiceMock.Object);
            IActionResult result = sessionController.GetCurrentUser(invalidTokenString);

            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            BadRequestObjectResult badRequestResult = (BadRequestObjectResult) result;

            Assert.IsTrue(badRequestResult.StatusCode == StatusCodes.Status400BadRequest);
        }

        [ExpectedException(typeof(ResourceNotFoundException))]
        [TestMethod]
        public void GetCurrentUserInvalidSessionNotFoundTest()
        {
            _sessionsServiceMock.Setup(s => s.GetAllBy(It.IsAny<ISearchCriteria<Session>>())).Returns(new List<Session>());

            SessionController sessionController = new(_sessionsServiceMock.Object);
            IActionResult result = sessionController.GetCurrentUser(Guid.NewGuid().ToString());

            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            BadRequestObjectResult badRequestResult = (BadRequestObjectResult)result;

            _sessionsServiceMock.VerifyAll();
            Assert.IsTrue(badRequestResult.StatusCode == StatusCodes.Status400BadRequest);
        }

        [TestMethod]
        public void LogOutAsAdminNoContentTest()
        {
            _sessionsServiceMock.Setup(s => s.Delete(It.IsAny<int>()));

            SessionController sessionController = new(_sessionsServiceMock.Object);
            IActionResult result = sessionController.DeleteAsAdmin(0);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            NoContentResult noContentResult = (NoContentResult) result;

            _sessionsServiceMock.VerifyAll();
            Assert.IsTrue(noContentResult.StatusCode == StatusCodes.Status204NoContent);
        }

        [TestMethod]
        public void LogOutNoContentTest()
        {
            var activeSession = new Session {Id = 0};
            _sessionsServiceMock.Setup(s => s.GetAllBy(It.IsAny<ISearchCriteria<Session>>())).Returns(new List<Session>{activeSession});
            _sessionsServiceMock.Setup(s => s.Delete(activeSession.Id));

            SessionController sessionController = new(_sessionsServiceMock.Object);
            IActionResult result = sessionController.Delete(activeSession.AuthToken.ToString());

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            NoContentResult noContentResult = (NoContentResult)result;

            _sessionsServiceMock.VerifyAll();
        }
    }
}
