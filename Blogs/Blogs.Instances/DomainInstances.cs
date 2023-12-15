using Blogs.Domain;
using Blogs.Domain.BusinessEntities;
using Blogs.Domain.BusinessEntities.Mentions;
using System.Diagnostics.CodeAnalysis;

namespace Blogs.Instances
{
    [ExcludeFromCodeCoverage]
    public static class DomainInstances
    {
        private const int _daysIntervalRange = 6000;

        public static Article CreateSimpleArticle(User author, int id)
        {
            return new() {Id = id, Name = "Global News", Text = ".......", Template = Template.Top,
                Author = author};
        }

        public static Image CreateSimpleImage(int id)
        {
            return new() {Id = id, Data = "url image"};
        }

        public static Comment CreateSimpleComment(User author, int id)
        {
            return new() {Id = id, Text = "Thanks", Author = author};
        }

        public static Notification<T> CreateSimpleNotification<T> (User notifyTo, string text, int id)
        {
            return new() {Id = id, Receiver = notifyTo, Text = text};
        }

        public static Response CreateSimpleResponse(User author, int id)
        {
            return new() {Id = id, Text = "Your Welcome", Author = author};
        }

        public static User CreateSimpleUser(int id)
        {
            return new() {Id = id, Name = "Nicolas", LastName = "Gonzalez", Email = "ngonzalez@gmail.com",
                Password = "ngon_1234", UserName = "NGonza", Role = UserRole.Admin};
        }

        public static IntervalDate CreateRandomValidDayInterval(DateTime bottomDate)
        {
            var randomMinDay = bottomDate.AddDays(new Random().Next(_daysIntervalRange));
            var randomMaxDay = randomMinDay.AddDays(new Random().Next(_daysIntervalRange));
            return new IntervalDate(randomMinDay, randomMaxDay);
        }

        public static User UpdateUserAlternativeKeys(User user)
        {
            user.UserName = "NGonza2";
            user.Email = "ngonzalez2@gmail.com";
            return user;
        }

        public static Article UpdateArticleAlternativeKey(Article article)
        {
            article.Name = "Fake News";
            return article;
        }

        public static Offense CreateSimpleOffense(int id)
        {
            return new() {Id = id, Word = "Desagradable"};
        }

    }
}
