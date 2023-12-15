using Blogs.Domain.BusinessEntities.Mentions;

namespace Blogs.Models.Out
{
    public class MentionBasicInfo
    {
        public int Id { get; set; }
        public string Text { get; set; }
        
        public MentionBasicInfo(Mention mention)
        {
            Id = mention.Id;
            Text = mention.Text;
        }

        public override bool Equals(object? obj)
        {
            return obj != null && obj is MentionBasicInfo info && Id == info.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }
}
