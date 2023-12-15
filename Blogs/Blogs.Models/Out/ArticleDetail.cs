using Blogs.Domain.BusinessEntities;

namespace Blogs.Models.Out
{
    public class ArticleDetail
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public string UpdatedAt { get; set; }
        public int Visibility { get; set; }
        public int Template { get; set; }
        public int State { get; set; }

        public ArticleDetail(Article article)
        {
            Id = article.Id;
            Name = article.Name;
            Text = article.Text;
            Visibility = (int) article.Visibility;
            UpdatedAt = article.UpdatedAt.ToString();
            State = (int) article.State;
            Template = (int) article.Template;
        }

        public override bool Equals(object? obj)
        {
            return obj != null && obj is ArticleDetail detail && Id == detail.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }
}
