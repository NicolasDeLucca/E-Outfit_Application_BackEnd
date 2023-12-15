using Blogs.Domain.BusinessEntities;

namespace Blogs.Models.Out
{
    public class ArticleBasicInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ArticleBasicInfo(Article article)
        {
            Id = article.Id;
            Name = article.Name;
        }

        public override bool Equals(object? obj)
        {
            return obj != null && obj is ArticleBasicInfo info && Id == info.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }
}
