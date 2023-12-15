using Blogs.Domain.BusinessEntities;
using Blogs.Interfaces;

namespace Blogs.Domain.SearchCriteria
{
    public class SearchingArticleByText : ISearchCriteria<Article>
    {
        public string Text { get; set; }

        public SearchingArticleByText() { }

        public bool Criteria(Article article)
        {
            return Text == null || article.Text.Contains(Text)
                 || article.Name != null && article.Name.Contains(Text);
        }
    }
}
