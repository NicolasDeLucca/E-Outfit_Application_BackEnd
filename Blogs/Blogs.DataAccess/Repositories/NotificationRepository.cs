using Blogs.DataAccess.Contexts;
using EntityState = Microsoft.EntityFrameworkCore.EntityState;
using Blogs.Domain;
using Blogs.Domain.BusinessEntities;
using Blogs.Exceptions;
using Blogs.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Blogs.DataAccess.Repositories
{
    public class NotificationRepository<T> : IRepository<Notification<T>>
    {
        protected DbContext Context { get; set; }

        public NotificationRepository(DbContext context)
        {
            Context = context;
        }

        public IEnumerable<Notification<T>> GetAllByCondition(Func<Notification<T>, bool> predicate)
        {
            var notifications = Context.Set<Notification<T>>().AsNoTracking().Include(n => n.AssociatedContent).
                AsEnumerable().Where(predicate);
            
            ReadNotifications(predicate);
            return notifications;
        }

        public virtual Notification<T> GetById(int id)
        {
            Notification<T>? retrievedNotification = Context.Set<Notification<T>>().
                AsNoTracking().AsEnumerable().Where(a => a.Id == id).FirstOrDefault();

            if (retrievedNotification == null)
                throw new ResourceNotFoundException("Could not find specified notification");

            return retrievedNotification;
        }

        public virtual int Store(Notification<T> newNotification)
        {
            Context.Attach(newNotification.AssociatedContent);
            Context.Set<Notification<T>>().Add(newNotification);
            Context.SaveChanges();

            DbContextHelper.DetachAllEntries(Context);
            return newNotification.Id;
        }

        public virtual void Delete(Notification<T> notification)
        {
            Context.Attach(notification.AssociatedContent);
            Context.Set<Notification<T>>().Remove(notification);
            Context.SaveChanges();

            DbContextHelper.DetachAllEntries(Context);
        }

        public void Update(Notification<T> notification)
        {
            throw new NotImplementedException();
        }

        private void ReadNotifications(Func<Notification<T>, bool> predicate)
        {
            var notifications = Context.Set<Notification<T>>().AsNoTracking().Include(n => n.AssociatedContent).
                AsEnumerable().Where(predicate);

            foreach (var notification in notifications)
            {
                if (notification.State == MessageVerification.UnRead)
                {
                    notification.Read();
                    Context.Entry(notification).State = EntityState.Modified;
                }
            }
            Context.SaveChanges();
            DbContextHelper.DetachAllEntries(Context);
        }
    }
}
