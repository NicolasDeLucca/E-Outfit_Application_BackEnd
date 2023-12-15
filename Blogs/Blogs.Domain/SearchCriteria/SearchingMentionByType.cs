using Blogs.Domain.BusinessEntities.Mentions;
using Blogs.Interfaces;

namespace Blogs.Domain.SearchCriteria
{
    public class SearchingMentionByType<T> : ISearchCriteria<Mention>
    {
        public SearchingMentionByType() {}

        public bool Criteria(Mention mention)
        {
            return mention is T; 
        }
    }
}
