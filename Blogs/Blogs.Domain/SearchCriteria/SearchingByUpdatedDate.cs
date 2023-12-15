using Blogs.Domain.BusinessEntities;
using Blogs.Domain.BusinessEntities.Mentions;
using Blogs.Interfaces;

namespace Blogs.Domain.SearchCriteria
{
    public class SearchingByUpdatedDate<T> : ISearchCriteria<T>
    {
        public DateTime? MinDate { get; set; }
        public DateTime? MaxDate { get; set; }

        public SearchingByUpdatedDate() { }

        public bool Criteria(T t)
        {
            return MinDate == null || MaxDate == null || t is Article art && ArticleCriteria(art)
                 || t is Mention men && MentionCriteria(men) || t is Mention com && CommentCriteria(com)
                 || t is Response res && ResponseCriteria(res);                
        }

        #region Helpers

        private bool ArticleCriteria(Article art)
        {
            return MinDate <= art.UpdatedAt && art.UpdatedAt <= MaxDate;
        }

        private bool MentionCriteria(Mention men)
        {
            return MinDate <= men.UpdatedAt && men.UpdatedAt <= MaxDate;
        }

        private bool ResponseCriteria(Response res)
        {
            return MinDate <= res.UpdatedAt && res.UpdatedAt <= MaxDate;
        }

        private bool CommentCriteria(Mention com)
        {
            return MinDate <= com.UpdatedAt && com.UpdatedAt <= MaxDate;
        }

        #endregion
    }
}
