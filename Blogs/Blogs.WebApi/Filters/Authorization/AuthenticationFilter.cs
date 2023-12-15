using Blogs.Domain.BusinessEntities;
using Blogs.Domain.SearchCriteria;
using Blogs.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace Blogs.WebApi.Filters.Authorization
{
    public class AuthenticationFilter : Attribute, IAuthorizationFilter
    {
        private readonly IManager<Session> _sessionService;

        public AuthenticationFilter(IManager<Session> sessionService)
        {
            _sessionService = sessionService;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            string authorizationHeader = context.HttpContext.Request.Headers["Authorization"];
            
            if (authorizationHeader == StringValues.Empty)
                context.Result = GetJsonResult("Authorization token is missing", 401);
            else if (!Guid.TryParse(authorizationHeader, out Guid token))
                context.Result = GetJsonResult("Invalid authorization token format", 400);
            else if (!IsSessionInUse(token))
                context.Result = GetJsonResult("Authorization required", 401);           
        }

        #region Helpers

        private bool IsSessionInUse(Guid token)
        {
            var searchCriteria = new SearchingSessionByToken{Token = token};
            var session = _sessionService.GetAllBy(searchCriteria).FirstOrDefault();
            return session.AuthenticatedUser != null;  
        }

        private JsonResult GetJsonResult(string message, int? statusCode)
        {
            return new JsonResult(message) {StatusCode = statusCode};
        }

        #endregion 
    }
}
