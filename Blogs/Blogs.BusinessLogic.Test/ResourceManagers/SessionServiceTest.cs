using Blogs.BusinessLogic.ResourceManagers;
using Blogs.Domain.BusinessEntities;
using Blogs.Interfaces;
using static Blogs.Instances.DomainInstances;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections;
using System.Security.Authentication;
using Blogs.Domain.SearchCriteria;

namespace Blogs.BusinessLogic.Test.ResourceManagers
{
    [TestClass]
    public class SessionServiceTest
    {
        private Mock<IRepository<User>> _userRepositoryMock;
        private Mock<IRepository<Session>> _sessionRepositoryMock;

        [TestInitialize]
        public void SetUp()
        {
            _sessionRepositoryMock = new Mock<IRepository<Session>>(MockBehavior.Strict);
            _userRepositoryMock = new Mock<IRepository<User>>(MockBehavior.Strict);
        }

        [TestMethod]
        public void CreateAnExistingSessionReturnsAsExpected()
        {
            var authenticatedUser = new User {Id = 0, UserName = "pepe", Password = "abc123"};
            var sessionCreated = new Session {AuthenticatedUser = authenticatedUser};
            var users = new List<User> {authenticatedUser};
            var sessions = new List<Session> {sessionCreated};

            _userRepositoryMock.Setup(r => r.GetAllByCondition(It.IsAny<Func<User, bool>>())).Returns(users);
            _sessionRepositoryMock.Setup(r => r.GetAllByCondition(It.IsAny<Func<Session, bool>>())).Returns(sessions);

            SessionManager sessionManager = new(_sessionRepositoryMock.Object, _userRepositoryMock.Object);
            var retrievedSession = sessionManager.Create(sessionCreated);

            _userRepositoryMock.VerifyAll();
            _sessionRepositoryMock.VerifyAll();
            Assert.AreEqual(sessionCreated, retrievedSession);
        }

        [TestMethod]
        public void CreateNewSessionReturnsAsExpected()
        {
            var authenticatedUser = new User {Id = 0, UserName = "pepe", Password = "abc123"};
            var sessionCreated = new Session {Id = 0, AuthenticatedUser = authenticatedUser};
            var users = new List<User> {authenticatedUser};
            var sessions = new List<Session> {};

            _userRepositoryMock.Setup(r => r.GetAllByCondition(It.IsAny<Func<User, bool>>())).Returns(users);
            _sessionRepositoryMock.Setup(r => r.GetAllByCondition(It.IsAny<Func<Session, bool>>())).Returns(sessions);
            _sessionRepositoryMock.Setup(r => r.Store(It.IsAny<Session>())).Returns(0);
            _sessionRepositoryMock.Setup(r => r.GetById(It.IsAny<int>())).Returns(sessionCreated);

            SessionManager sessionManager = new(_sessionRepositoryMock.Object, _userRepositoryMock.Object);
            var retrievedSession = sessionManager.Create(sessionCreated);

            _userRepositoryMock.VerifyAll();
            _sessionRepositoryMock.VerifyAll();
            Assert.AreEqual(sessionCreated, retrievedSession);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCredentialException))]
        public void CreateSessionRequiresUserDoesAsExpected()
        {
            SessionManager sessionManager = new(_sessionRepositoryMock.Object, _userRepositoryMock.Object);
            var retrievedGuid = sessionManager.Create(null);
        }

        [TestMethod]
        public void GetAllSessionsByReturnsAsExpected()
        {
            Session actualSession = new();
            List<Session> actualSessions = new() {actualSession};

            _sessionRepositoryMock.Setup(r => r.GetAllByCondition(It.IsAny<Func<Session, bool>>())).
                 Returns(actualSessions);

            SessionManager sessionManager = new(_sessionRepositoryMock.Object, _userRepositoryMock.Object);            
            var searchCriteria = new SearchingSessionByToken{Token = actualSession.AuthToken};
            var retrievedSessions = sessionManager.GetAllBy(searchCriteria);

            _sessionRepositoryMock.VerifyAll();
            Assert.AreEqual(actualSessions, retrievedSessions);
        }

        [TestMethod]
        public void GetAllSessionsReturnsAsExpected()
        {
            var sessions = It.IsAny<List<Session>>();
            _sessionRepositoryMock.Setup(r => r.GetAllByCondition(It.IsAny<Func<Session, bool>>())).Returns(sessions);

            SessionManager sessionService = new(_sessionRepositoryMock.Object, _userRepositoryMock.Object);
            var retrievedSessions = sessionService.GetAll();

            _sessionRepositoryMock.VerifyAll();
            CollectionAssert.AreEquivalent((ICollection) retrievedSessions, sessions);
        }

        [TestMethod]
        public void GetSessionByIdReturnsAsExpected()
        {
            var expectedSession = new Session();
            _sessionRepositoryMock.Setup(r => r.GetById(It.IsAny<int>())).Returns(expectedSession);

            SessionManager sessionService = new(_sessionRepositoryMock.Object, _userRepositoryMock.Object);
            var retrievedSession = sessionService.GetById(0);

            _sessionRepositoryMock.VerifyAll();
            Assert.AreEqual(retrievedSession, expectedSession);
        }

        [TestMethod]
        public void UpdateAuthenticatedUserDoesAsExpected()
        {
            Session sessionToUpdate = new(){Id = 0, AuthenticatedUser = CreateSimpleUser(0)};

            _sessionRepositoryMock.Setup(r => r.GetById(It.IsAny<int>())).Returns(sessionToUpdate);
            _sessionRepositoryMock.Setup(r => r.Update(sessionToUpdate));
            _sessionRepositoryMock.Setup(r => r.GetAllByCondition(It.IsAny<Func<Session, bool>>())).
                Returns(new List<Session>{sessionToUpdate});

            sessionToUpdate.AuthenticatedUser.LastName = "my mother lastname";

            SessionManager sessionService = new(_sessionRepositoryMock.Object, _userRepositoryMock.Object);
            sessionService.Update(sessionToUpdate);

            var searchCriteria = new SearchingSessionByToken {Token = sessionToUpdate.AuthToken};
            var retrievedAuthenticatedUser = sessionService.GetAllBy(searchCriteria).FirstOrDefault().
                AuthenticatedUser;

            _sessionRepositoryMock.VerifyAll();
            Assert.IsTrue(retrievedAuthenticatedUser.LastName == "my mother lastname");
        }

        [TestMethod]
        public void DeleteSessionDoesAsExpected()
        {
            Session sessionToDelete = new(){Id = 0};

            _sessionRepositoryMock.Setup(r => r.GetById(It.IsAny<int>())).Returns(sessionToDelete);
            _sessionRepositoryMock.Setup(r => r.Delete(sessionToDelete));

            SessionManager sessionManager = new(_sessionRepositoryMock.Object, _userRepositoryMock.Object);
            sessionManager.Delete(0);

            _sessionRepositoryMock.VerifyAll();
        }
    }
}
