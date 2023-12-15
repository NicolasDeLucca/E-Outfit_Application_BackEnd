using Microsoft.EntityFrameworkCore;

namespace Blogs.DataAccess.Contexts
{
    public static class DbContextHelper
    {
        public static void DetachAllEntries(DbContext dbContext)
        {
            foreach (var entry in dbContext.ChangeTracker.Entries().ToList())
            {
                dbContext.Entry(entry.Entity).State = EntityState.Detached;
            }
        }
    }
}
