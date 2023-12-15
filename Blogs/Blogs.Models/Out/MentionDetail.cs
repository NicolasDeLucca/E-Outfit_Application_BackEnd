using Blogs.Domain.BusinessEntities.Mentions;

namespace Blogs.Models.Out
{
    public class MentionDetail
    {
        public string Text { get; set; }
        public string UpdatedAt { get; set; }
        public int ArticleId { get; set; }

        public MentionDetail(Mention mention)
        {
            Text = mention.Text;
            UpdatedAt = mention.UpdatedAt.ToString();
            ArticleId = mention.Article.Id;
        }

        public override bool Equals(object? obj)
        {
            return obj != null && obj is MentionDetail detail && Text == detail.Text &&
                ArticleId == detail.ArticleId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Text, ArticleId);
        }
    }
}
