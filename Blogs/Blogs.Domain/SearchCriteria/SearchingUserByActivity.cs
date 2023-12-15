using Blogs.Domain.BusinessEntities;
using Blogs.Domain.BusinessEntities.Mentions;
using Blogs.Interfaces;

namespace Blogs.Domain.SearchCriteria
{
    public class SearchingUserByActivity : ISearchCriteria<User>
    {
        public DateTime? MinDate { get; set; }
        public DateTime? MaxDate { get; set; }

        public SearchingUserByActivity()
        {
            MinDate = null;
            MaxDate = null;
        }

        public bool Criteria(User user)
        {
            var articleCriteria = new SearchingByUpdatedDate<Article> {MinDate = MinDate, MaxDate = MaxDate};
            var commentCriteria = new SearchingByUpdatedDate<Mention> {MinDate = MinDate, MaxDate = MaxDate};

            return user.PostedArticles.ToList().Exists(a => articleCriteria.Criteria(a)) ||
                   user.PostedMentions.ToList().Exists(m => m is Mention c && commentCriteria.Criteria(c));
        }
    }    
}
