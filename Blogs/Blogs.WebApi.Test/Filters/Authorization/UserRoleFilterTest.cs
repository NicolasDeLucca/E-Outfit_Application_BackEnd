using Blogs.WebApi.Filters.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.AspNetCore.Routing;
using Blogs.Domain.BusinessEntities;
using Blogs.Domain;
using Blogs.Interfaces;

namespace Blogs.WebApi.Test.Filters.Authorization
{
    [TestClass]
    public class UserRoleFilterTest
    {
        private Mock<AuthorizationFilterContext> _authorizationFilterContextMock;
        private Mock<IManager<Session>> _sessionManagerMock;

        [TestInitialize]
        public void SetUp()
        {
            _sessionManagerMock = new Mock<IManager<Session>>();
            _sessionManagerMock.SetupAllProperties();

            var httpContextMock = new Mock<HttpContext>();
            var httpRequestMock = new Mock<HttpRequest>();
            var serviceProviderMock = new Mock<IServiceProvider>();

            serviceProviderMock.Setup(sp => sp.GetService(typeof(IManager<Session>))).Returns(_sessionManagerMock.Object);
            httpContextMock.SetupGet(c => c.RequestServices).Returns(serviceProviderMock.Object);
            httpContextMock.SetupGet(c => c.Request).Returns(httpRequestMock.Object);

            var headers = new HeaderDictionary {{"Authorization", Guid.NewGuid().ToString()}};
            httpRequestMock.SetupGet(r => r.Headers).Returns(headers);

            ActionContext actionContext = new()
            {
                HttpContext = httpContextMock.Object,
                RouteData = new RouteData(),
                ActionDescriptor = new ActionDescriptor()
            };

            _authorizationFilterContextMock = new Mock<AuthorizationFilterContext>
                (actionContext, new List<IFilterMetadata>());
            _authorizationFilterContextMock.SetupAllProperties();
        }

        [TestMethod]
        public void InvalidUserRoleReturns401()
        {
            UserRoleFilter filter = new() {UserRole = UserRole.Admin};
            IEnumerable<Session> sessions = new List<Session>() {new Session {AuthenticatedUser = new User {Role = UserRole.Blogger}}};

            _sessionManagerMock.Setup(s => s.GetAllBy(It.IsAny<ISearchCriteria<Session>>())).Returns(sessions);
            filter.OnAuthorization(_authorizationFilterContextMock.Object);

            var result = (JsonResult) _authorizationFilterContextMock.Object.Result;
            Assert.IsTrue(result.StatusCode == StatusCodes.Status401Unauthorized);
        }

        [TestMethod]
        public void ValidUserRoleApprovesAuthorization()
        {
            UserRole anyRole = UserRole.Blogger;
            UserRoleFilter filter = new() {UserRole = anyRole};
            IEnumerable<Session> sessions = new List<Session>() {new Session {AuthenticatedUser = new User {Role = anyRole}}};

            _sessionManagerMock.Setup(s => s.GetAllBy(It.IsAny<ISearchCriteria<Session>>())).Returns(sessions);
            filter.OnAuthorization(_authorizationFilterContextMock.Object);

            Assert.IsNull(_authorizationFilterContextMock.Object.Result);
        }
    }
}
