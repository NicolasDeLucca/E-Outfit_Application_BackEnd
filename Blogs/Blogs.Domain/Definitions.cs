using Blogs.Domain.BusinessEntities;
using Blogs.Domain.BusinessEntities.Mentions;

namespace Blogs.Domain
{
    public enum Template
    {
        Top = 1,
        Left = 2,
        TopLeft = 3,
        TopBottom = 4
    }

    public enum Visibility
    {
        Public = 1,
        Private = 2
    }

    public enum UserRole
    {
        Admin = 1,
        Blogger = 2,
        AdminBlogger = 3
    }

    public enum EntityState
    { 
        Unchanged = 1,
        Modified = 2,
        Detached = 3
    } 

    public enum MessageVerification
    {
        Read = 1,
        UnRead = 2
    }

    public readonly struct IntervalDate
    {
        public readonly DateTime MinDate;
        public readonly DateTime MaxDate;
        public IntervalDate(DateTime minDate, DateTime maxDate)
        {
            MinDate = minDate;
            MaxDate = maxDate;
        }
        public int GetIntervalDays()
        {
            return Math.Abs(MaxDate.Day - MinDate.Day);
        }
    }

    public static class Functions
    {   
        public static int Activity(User user, Func <Article, bool> articleBehaviour, Func <Mention, bool> mentionBehaviour) 
            => user.PostedArticles.Count(articleBehaviour) + user.PostedMentions.Count(mentionBehaviour);
        public static IEnumerable<User> BlogersActivity(IEnumerable<User> users, Func<Article, bool> articleBehaviour, Func<Mention, bool> mentionBehaviour)
            => users.OrderByDescending(u => Activity(u, articleBehaviour, mentionBehaviour));
        public static IEnumerable<Article> TopModifiedArticles(IEnumerable<Article> arts, int top) 
            => arts.AsEnumerable().Where(a => a.Visibility == Visibility.Public).OrderByDescending(a => a.UpdatedAt).Take(top);
        public static IEnumerable<Notification<Mention>> MentionNotificationsUnRead
            (IEnumerable<Notification<Mention>> notis) => notis.Where(n => n.State == MessageVerification.UnRead);
        public static IEnumerable<Notification<Article>> ArticleNotificationsUnRead
            (IEnumerable<Notification<Article>> notis) => notis.Where(n => n.State == MessageVerification.UnRead);
    }
}
