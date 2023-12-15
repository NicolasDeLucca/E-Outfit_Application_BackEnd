using Blogs.Domain.BusinessEntities;

namespace Blogs.Models.Out
{
    public class LogInDetail
    {
        public int Id { get; set; }
        public string AuthToken { get; set; } 
        public string UserName { get; set; }
        public string Role { get; set; }

        public LogInDetail(Session session)
        {
            Id = session.Id;
            AuthToken = session.AuthToken.ToString();
            UserName = session.AuthenticatedUser.UserName;
            Role = session.AuthenticatedUser.Role.ToString();
        }

        public override bool Equals(object? obj)
        {
            return obj != null && obj is LogInDetail model && Id == model.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }
}
