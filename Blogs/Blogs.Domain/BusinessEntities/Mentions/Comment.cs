namespace Blogs.Domain.BusinessEntities.Mentions
{
    public class Comment : Mention
    {       
        public Comment()
        { 
            UpdatedAt = DateTime.Now;
        }

        public override bool Equals(object? obj)
        {
            return obj != null && obj is Comment && base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode());
        }
    }
}
