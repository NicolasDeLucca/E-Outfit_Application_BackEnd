using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.AspNetCore.Routing;
using Blogs.WebApi.Filters.Authorization;
using Blogs.Domain.BusinessEntities;
using Blogs.Interfaces;

namespace Blogs.WebApi.Test.Filters.Authorization
{
    [TestClass]
    public class AuthenticationFilterTest
    {
        private AuthenticationFilter _filter;
        private Mock<AuthorizationFilterContext> _authorizationFilterContextMock;
        private Mock<IManager<Session>> _sessionManagerMock;

        [TestInitialize]
        public void SetUp()
        {
            _sessionManagerMock = new Mock<IManager<Session>>();
            _filter = new AuthenticationFilter(_sessionManagerMock.Object);
            ActionContext actionContext = new()
            {
                HttpContext = new DefaultHttpContext(),
                RouteData = new RouteData(),
                ActionDescriptor = new ActionDescriptor()
            };
            _authorizationFilterContextMock = new Mock<AuthorizationFilterContext>
                (actionContext, new List<IFilterMetadata>());
        }

        [TestMethod]
        public void InvalidAuthorizationTokenFormatSentReturns400()
        {
            _authorizationFilterContextMock.SetupAllProperties();
            _authorizationFilterContextMock.Object.HttpContext.Request.Headers.Add("Authorization", "not a guid");
            _filter.OnAuthorization(_authorizationFilterContextMock.Object);

            var result = (JsonResult) _authorizationFilterContextMock.Object.Result;
            Assert.IsTrue(result.StatusCode == StatusCodes.Status400BadRequest);
        }

        [TestMethod]
        public void EmptyAuthorizationTokenSentReturns401()
        {
            _authorizationFilterContextMock.SetupAllProperties();
            _filter.OnAuthorization(_authorizationFilterContextMock.Object);

            var result = (JsonResult) _authorizationFilterContextMock.Object.Result;
            Assert.IsTrue(result.StatusCode == StatusCodes.Status401Unauthorized);
        }

        [TestMethod]
        public void InvalidAuthorizationTokenSentReturns401()
        {
            var activeSession = new Session();
            IEnumerable<Session> sessions = new List<Session>() {activeSession};

            _authorizationFilterContextMock.SetupAllProperties();
            _authorizationFilterContextMock.Object.HttpContext.Request.Headers.Add("Authorization", activeSession.AuthToken.ToString());

            _sessionManagerMock.Setup(s => s.GetAllBy(It.IsAny<ISearchCriteria<Session>>())).Returns(sessions);
            _filter.OnAuthorization(_authorizationFilterContextMock.Object);

            var result = (JsonResult) _authorizationFilterContextMock.Object.Result;
            Assert.IsTrue(result.StatusCode == StatusCodes.Status401Unauthorized);
        }

        [TestMethod]
        public void ValidAuthorizationTokenPassesFilter()
        {
            var activeSession = new Session {AuthenticatedUser = new User()};
            IEnumerable<Session> sessions = new List<Session>() {activeSession};

            _authorizationFilterContextMock.SetupAllProperties();
            _authorizationFilterContextMock.Object.HttpContext.Request.Headers.Add("Authorization", activeSession.AuthToken.ToString());

            _sessionManagerMock.Setup(s => s.GetAllBy(It.IsAny<ISearchCriteria<Session>>())).Returns(sessions);
            _filter.OnAuthorization(_authorizationFilterContextMock.Object);

            Assert.IsNull(_authorizationFilterContextMock.Object.Result);
        }
    }
}
