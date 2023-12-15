using Blogs.DataAccess.Contexts;
using Blogs.Domain.BusinessEntities.Mentions;
using Blogs.Exceptions;
using Blogs.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Blogs.DataAccess.Repositories
{
    public class MentionRepository : IRepository<Mention>
    {
        protected DbContext Context { get; set; }

        public MentionRepository(DbContext context)
        {
            Context = context;
        }

        public virtual IEnumerable<Mention> GetAllByCondition(Func<Mention, bool> predicate)
        {
            return Context.Set<Mention>().AsNoTracking().AsEnumerable().Where(predicate);
        }

        public virtual Mention GetById(int id)
        {
            Mention? retrievedMention = GetAllByCondition(a => a.Id == id).FirstOrDefault();
            if (retrievedMention == null)
                throw new ResourceNotFoundException("Could not find specified mention");

            return retrievedMention;
        }

        public virtual int Store(Mention newMention)
        {
            Context.Attach(newMention.Article);
            Context.Set<Mention>().Add(newMention); //
            Context.SaveChanges();

            DbContextHelper.DetachAllEntries(Context);
            return newMention.Id;
        }

        //this method is executed only when exists a offense in the mention
        public void Update(Mention mention)
        {
            Context.Entry(mention).State = EntityState.Modified;
            Context.SaveChanges();
            Context.Entry(mention).State = EntityState.Detached;
        }

        public void Delete(Mention mention)
        {
            throw new NotImplementedException();
        }
    }
}
