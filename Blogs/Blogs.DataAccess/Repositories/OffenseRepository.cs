using Blogs.Domain.BusinessEntities;
using Blogs.Exceptions;
using Blogs.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Blogs.DataAccess.Repositories
{
    public class OffenseRepository : IRepository<Offense>
    {
        protected DbContext Context { get; set; }

        public OffenseRepository(DbContext context)
        {
            Context = context;
        }

        public IEnumerable<Offense> GetAllByCondition(Func<Offense, bool> predicate)
        {
            return Context.Set<Offense>().AsNoTracking().AsEnumerable().Where(predicate);
        }

        public int Store(Offense offense)
        {
            Context.Set<Offense>().Add(offense);
            Context.SaveChanges();
            Context.Entry(offense).State = EntityState.Detached;

            return offense.Id;
        }

        public void Update(Offense offense)
        {
            Context.Entry(offense).State = EntityState.Modified;
            Context.SaveChanges();
            Context.Entry(offense).State = EntityState.Detached;
        }

        public void Delete(Offense offense)
        {
            Context.Attach(offense);
            Context.Remove(offense);
            Context.SaveChanges();

            Context.Entry(offense).State = EntityState.Detached;
        }

        public Offense GetById(int id)
        {
            Offense? retrievedOffense = GetAllByCondition(a => a.Id == id).FirstOrDefault();
            if (retrievedOffense == null)
                throw new ResourceNotFoundException("Could not find specified offense");

            return retrievedOffense;
        }
    }
}
