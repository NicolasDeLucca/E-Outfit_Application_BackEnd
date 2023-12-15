using Blogs.Domain.BusinessEntities;

namespace Blogs.Models.Out
{
    public class ArticleNotificationBasicInfo
    {
        public string Text { get; set; }
        public ArticleBasicInfo ArticleModel { get; set; }

        public ArticleNotificationBasicInfo(Notification<Article> notification)
        {
            Text = notification.Text;
            ArticleModel = new ArticleBasicInfo(notification.AssociatedContent);
        }

        public override bool Equals(object? obj)
        {
            return obj != null && obj is ArticleNotificationBasicInfo info && Text == info.Text
                && ArticleModel.Id == info.ArticleModel.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Text, ArticleModel.Id);
        }
    }
}
