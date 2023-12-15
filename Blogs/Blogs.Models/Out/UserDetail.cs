using Blogs.Domain.BusinessEntities;

namespace Blogs.Models.Out
{
    public class UserDetail
    {
        public int Id { set; get; }
        public string UserName { set; get; }
        public string Email { set; get; }
        public int Role { set; get; }

        public UserDetail(User user)
        {
            Id = user.Id;
            Email = user.Email;
            UserName = user.UserName;
            Role = (int) user.Role;
        }

        public override bool Equals(object? obj)
        {
            return obj != null && obj is UserDetail detail && Id == detail.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }
}
