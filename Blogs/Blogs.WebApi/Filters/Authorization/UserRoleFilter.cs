using Blogs.Domain;
using Blogs.Domain.BusinessEntities;
using Blogs.Domain.SearchCriteria;
using Blogs.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Blogs.WebApi.Filters.Authorization
{
    public class UserRoleFilter : Attribute, IAuthorizationFilter
    {
        private IManager<Session> _sessionService;

        public UserRole UserRole { get; set; }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            _sessionService = (IManager<Session>) context.HttpContext.RequestServices.GetService(typeof(IManager<Session>));
            string authorizationHeader = context.HttpContext.Request.Headers["Authorization"];
            
            Guid.TryParse(authorizationHeader, out Guid token);
            if (!IsRoleAuthorized(token))
                context.Result = GetJsonResult("Unauthorized role", 401);
        }

        #region Helpers

        private JsonResult GetJsonResult(string message, int? statusCode)
        {
            return new JsonResult(message) {StatusCode = statusCode};
        }

        private bool IsRoleAuthorized(Guid token)
        {
            var searchCriteria = new SearchingSessionByToken {Token = token};
            var currentUser = _sessionService.GetAllBy(searchCriteria).FirstOrDefault().AuthenticatedUser;
            return currentUser.Role.ToString().Contains(UserRole.ToString());
        }

        #endregion
    }
}
