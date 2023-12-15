using Blogs.DataAccess.Contexts;
using Blogs.Domain.BusinessEntities;
using Blogs.Exceptions;
using Blogs.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Blogs.DataAccess.Repositories
{
    public class SessionRepository : IRepository<Session>
    {
        protected DbContext Context { get; set; }

        public SessionRepository(DbContext context)
        {
            Context = context;
        }

        public virtual IEnumerable<Session> GetAllByCondition(Func<Session, bool> predicate)
        {
            return Context.Set<Session>().AsNoTracking().
                Include(s => s.AuthenticatedUser).ThenInclude(u => u.PostedArticles).ThenInclude(a => a.Images).
                Include(s => s.AuthenticatedUser).ThenInclude(u => u.PostedMentions).ThenInclude(m => m.Article).
                Include(s => s.AuthenticatedUser).ThenInclude(u => u.MentionNotifications).ThenInclude(n => n.AssociatedContent).
                Include(s => s.AuthenticatedUser).ThenInclude(u => u.ArticleNotifications).ThenInclude(n => n.AssociatedContent).
                    AsEnumerable().Where(predicate);
        }

        public virtual Session GetById(int id)
        {
            Session? retrievedSession = GetAllByCondition(s => s.Id == id).FirstOrDefault();
            if (retrievedSession == null)
                throw new ResourceNotFoundException("Could not find specified session");

            return retrievedSession;
        }

        public virtual int Store(Session session)
        {
            Context.Attach(session.AuthenticatedUser);
            Context.Set<Session>().Add(session);
            Context.SaveChanges();

            DbContextHelper.DetachAllEntries(Context);
            return session.Id;
        }

        public void Update(Session session)
        {
            throw new NotImplementedException();
        }

        public virtual void Delete(Session session)
        {
            Context.Attach(session.AuthenticatedUser);
            Context.Set<Session>().Remove(session);
            Context.SaveChanges();

            DbContextHelper.DetachAllEntries(Context);
        }
    }
}
