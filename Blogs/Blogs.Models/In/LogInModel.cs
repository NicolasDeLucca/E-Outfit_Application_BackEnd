using Blogs.Domain.BusinessEntities;

namespace Blogs.Models.In
{
    public class LogInModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public User ToEntity()
        {
            return new User()
            {
                UserName = UserName,
                Password = Password
            };
        }

        public override bool Equals(object? obj)
        {
            return obj != null && obj is LogInModel dTO && UserName == dTO.UserName;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(UserName);
        }

    }
}
