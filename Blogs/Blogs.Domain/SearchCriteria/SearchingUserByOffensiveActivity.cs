using Blogs.Domain.BusinessEntities;
using Blogs.Interfaces;

namespace Blogs.Domain.SearchCriteria
{
    public class SearchingUserByOffensiveActivity : ISearchCriteria<User>
    {
        public DateTime? MinDate { get; set; }
        public DateTime? MaxDate { get; set; }
        public List<string> OffensiveWords { get; set; }

        public SearchingUserByOffensiveActivity()
        {
            OffensiveWords = new();
        }

        public bool Criteria(User user)
        {
            var userActivityCriteria = new SearchingUserByActivity() {MinDate = MinDate, MaxDate = MaxDate};
            var hasOffensiveArticles = user.PostedArticles.ToList().Exists(a => a.IsOffensive(OffensiveWords));
            var hasOffensiveMentions = user.PostedMentions.ToList().Exists(m => m.IsOffensive(OffensiveWords));

            return userActivityCriteria.Criteria(user) && (hasOffensiveArticles || hasOffensiveMentions);
        }
    }
}
