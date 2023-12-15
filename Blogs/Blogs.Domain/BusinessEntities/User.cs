using Blogs.Domain.BusinessEntities.Mentions;
using Blogs.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Blogs.Domain.BusinessEntities
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        [JsonIgnore]
        [Required]
        public string Password { get; set; }

        [Required]
        [MaxLength(12)]
        public string UserName { get; set; }

        [Required]
        public virtual UserRole Role { get; set; }
 
        public virtual ICollection<Article> PostedArticles { get; set; }
        public virtual ICollection<Mention> PostedMentions { get; set; }
        public virtual ICollection<Notification<Article>> ArticleNotifications { get; set; }
        public virtual ICollection<Notification<Mention>> MentionNotifications { get; set; }

        public User() 
        {
            PostedArticles = new List<Article>();
            PostedMentions = new List<Mention>();
            MentionNotifications = new List<Notification<Mention>>();
            ArticleNotifications = new List<Notification<Article>>();
        }

        public virtual void ValidOrFail()
        {
            ValidateName();
            ValidateLastName();
            ValidateEmail();
            ValidatePassword();
            ValidateUserName();
        }

        #region Validators

        private void ValidateName()
        {
            if (Name == null)
                throw new InvalidRequestDataException("Must provide a name");
        }

        private void ValidateLastName()
        {
            if (LastName == null)
                throw new InvalidRequestDataException("Must provide a lastname");
        }

        private void ValidateUserName()
        {   
            var regexPattern = "^[a-zA-Z0-9\\S]+$";
            Regex userNameRegex = new Regex(regexPattern);

            if (UserName == null || UserName.Length > 12 || !userNameRegex.IsMatch(UserName))
                throw new InvalidRequestDataException("Invalid username");
        }

        private void ValidatePassword()
        {
            if (Password == null)
                throw new InvalidRequestDataException("Must provide a user password");
        }

        private void ValidateEmail()
        {
            var regexPattern = "\\b[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}\\b";
            Regex emailRegex = new Regex(regexPattern);

            if (Email == null || !emailRegex.IsMatch(Email) || Email.Contains(" "))
                throw new InvalidRequestDataException("Invalid email");
        }

        #endregion

        public override bool Equals(object? obj)
        {
            return obj != null && obj is User user && Id == user.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

    }
}
