using Blogs.Domain.BusinessEntities;
using Blogs.Domain.BusinessEntities.Mentions;

namespace Blogs.Models.Out
{
    public class MentionNotificationBasicInfo
    {
        public string Text { get; set; }
        public MentionBasicInfo MentionModel { get; set; }

        public MentionNotificationBasicInfo(Notification<Mention> notification)
        {
            Text = notification.Text;
            MentionModel = new MentionBasicInfo(notification.AssociatedContent);
        }

        public override bool Equals(object? obj)
        {
            return obj != null && obj is MentionNotificationBasicInfo info && Text == info.Text 
                && MentionModel.Id == info.MentionModel.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Text, MentionModel.Id);
        }
    }
}
