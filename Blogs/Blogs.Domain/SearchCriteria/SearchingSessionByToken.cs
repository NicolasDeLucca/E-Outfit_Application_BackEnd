using Blogs.Domain.BusinessEntities;
using Blogs.Interfaces;

namespace Blogs.Domain.SearchCriteria
{
    public class SearchingSessionByToken : ISearchCriteria<Session>
    {
        public SearchingSessionByToken() { }

        public Guid Token { get; set; }

        public bool Criteria(Session session)
        {   
            return session.AuthToken.Equals(Token);
        }
    }
}
