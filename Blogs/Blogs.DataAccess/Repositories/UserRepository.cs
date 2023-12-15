using Blogs.Domain.BusinessEntities;
using Blogs.Exceptions;
using Microsoft.EntityFrameworkCore;
using Blogs.Interfaces;
using Blogs.Domain.BusinessEntities.Mentions;
using Blogs.DataAccess.Contexts;

namespace Blogs.DataAccess.Repositories
{
    public class UserRepository : IRepository<User>
    {
        protected DbContext Context { get; set; }

        public UserRepository(DbContext context)
        {
            Context = context;
        }

        public virtual IEnumerable<User> GetAllByCondition(Func<User, bool> predicate)
        {
            return Context.Set<User>().AsNoTracking().
                Include(u => u.PostedArticles).ThenInclude(a => a.Images).
                Include(u => u.PostedMentions).ThenInclude(m => m.Article).ThenInclude(a => a.Author).
                Include(u => u.MentionNotifications).ThenInclude(n => n.AssociatedContent).
                Include(u => u.ArticleNotifications).ThenInclude(n => n.AssociatedContent).
                    AsEnumerable().Where(predicate);
        }

        public virtual User GetById(int id)
        {
            User? retrievedUser = GetAllByCondition(u => u.Id == id).FirstOrDefault();
            if (retrievedUser == null)
                throw new ResourceNotFoundException("Could not find specified user");

            return retrievedUser;
        }

        public virtual int Store(User newUser)
        {
            User? retrievedUser = Context.Set<User>().AsNoTracking().AsEnumerable().
                Where(u => u.UserName == newUser.UserName || u.Email == newUser.Email).FirstOrDefault();

            if (retrievedUser != null)
                throw new InvalidOperationException("User already registered");

            Context.Set<User>().Add(newUser);
            Context.SaveChanges();

            DbContextHelper.DetachAllEntries(Context);
            return newUser.Id;
        }

        public virtual void Update(User user)
        {
            Context.Set<User>().Update(user);
            Context.SaveChanges();
            DbContextHelper.DetachAllEntries(Context);
        }

        public virtual void Delete(User user)
        {
            DeleteUserSession(user);
            Context.RemoveRange(user.ArticleNotifications); //

            foreach (var art in user.PostedArticles)
            {
                Context.RemoveRange(art.Images);
                Context.Remove(art);
            }
            Context.RemoveRange(user.MentionNotifications);
            var responses = Context.Set<Mention>().OfType<Response>().AsNoTracking().AsEnumerable().Where(r => r.Author == user);
            Context.RemoveRange(responses);
            var comments = Context.Set<Mention>().OfType<Comment>().AsNoTracking().AsEnumerable().Where(r => r.Author == user);
            Context.RemoveRange(comments);
            
            Context.Set<User>().Remove(user);
            Context.SaveChanges();
            DbContextHelper.DetachAllEntries(Context);
        }

        protected virtual void DeleteUserSession(User user)
        {
            var session = Context.Set<Session>().AsNoTracking().AsEnumerable().FirstOrDefault();

            if (session != null)
            {
                Context.Remove(session);
                Context.SaveChanges();
            }
        }
    }
}
