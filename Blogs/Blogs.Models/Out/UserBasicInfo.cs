using Blogs.Domain.BusinessEntities;

namespace Blogs.Models.Out
{
    public class UserBasicInfo
    {
        public int Id { get; set; }
        public string Email { set; get; }

        public UserBasicInfo(User user)
        {
            Id = user.Id;
            Email = user.Email;
        }

        public override bool Equals(object? obj)
        {
            return obj != null && obj is UserBasicInfo info && Id == info.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }
}
