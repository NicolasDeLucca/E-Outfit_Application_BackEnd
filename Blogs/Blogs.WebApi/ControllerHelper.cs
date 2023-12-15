using Blogs.Domain.BusinessEntities;
using Blogs.Domain.SearchCriteria;
using Blogs.Exceptions;
using Blogs.Interfaces;

namespace Blogs.WebApi
{
    public static class ControllerHelper
    {
        public static Session GetSession(IManager<Session> sessionService, string authorization)
        {
            var isValidGuid = Guid.TryParse(authorization, out Guid token);
            if (!isValidGuid)
                throw new InvalidRequestDataException("Invalid token");

            var searchCriteria = new SearchingSessionByToken {Token = token};
            var actualSession = sessionService.GetAllBy(searchCriteria).FirstOrDefault();

            if (actualSession == null)
                throw new ResourceNotFoundException("Could not find specified session");

            return actualSession;
        }
    }
}
