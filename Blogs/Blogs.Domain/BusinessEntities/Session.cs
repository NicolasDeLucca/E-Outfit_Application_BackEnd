using System.ComponentModel.DataAnnotations;

namespace Blogs.Domain.BusinessEntities
{
    public class Session
    {
        public int Id { get; set; }

        [Required]
        public Guid AuthToken { get; set; }

        [Required]
        public virtual User AuthenticatedUser { get; set; }

        public Session()
        {
            AuthToken = Guid.NewGuid();
            AuthenticatedUser = null;
        }

        public override bool Equals(object? obj)
        {
            return obj != null && obj is Session session && Id == session.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }
}
