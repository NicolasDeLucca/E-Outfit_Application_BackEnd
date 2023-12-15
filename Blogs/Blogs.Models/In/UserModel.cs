using Blogs.Domain;
using Blogs.Domain.BusinessEntities;

namespace Blogs.Models.In
{
    public class UserModel
    {
        public string Name { set; get; }
        public string LastName { set; get; }
        public string Email { set; get; }
        public string UserName { set; get; }
        public string Password { set; get; }    
        public int Role { set; get; }

        public User ToEntity()
        {
            return new()
            {
                Name = Name,
                LastName = LastName,
                Email = Email,
                UserName = UserName,
                Password = Password,
                Role = (UserRole) Role
            };
        }

        public override bool Equals(object? obj)
        {
            return obj!= null && obj is UserModel model && UserName == model.UserName;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(UserName);
        }

    }
}
