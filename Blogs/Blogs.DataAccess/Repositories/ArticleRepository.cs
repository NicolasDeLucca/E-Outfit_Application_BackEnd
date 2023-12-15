using Blogs.DataAccess.Contexts;
using Blogs.Domain.BusinessEntities;
using Blogs.Exceptions;
using Blogs.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Blogs.DataAccess.Repositories
{
    public class ArticleRepository : IRepository<Article>
    {
        protected DbContext Context { get; set; }

        public ArticleRepository(DbContext context)
        {
            Context = context;
        }

        public virtual IEnumerable<Article> GetAllByCondition(Func<Article, bool> predicate)
        {
            return Context.Set<Article>().AsNoTracking().Include(a => a.Images).Include(a => a.Mentions).
                AsEnumerable().Where(predicate);
        }

        public virtual Article GetById(int id)
        {
            Article? retrievedArticle = GetAllByCondition(a => a.Id == id).FirstOrDefault();
            if (retrievedArticle == null)
                throw new ResourceNotFoundException("Could not find specified article");

            return retrievedArticle;
        }

        public virtual int Store(Article newArticle)
        {
            Article? retrievedArticle = Context.Set<Article>().AsNoTracking().AsEnumerable().
                Where(a => a.Name == newArticle.Name).FirstOrDefault();

            if (retrievedArticle != null)
                throw new InvalidOperationException("Article already registered");

            Context.Entry(newArticle.Author).State = EntityState.Unchanged;
            Context.Set<Image>().AddRange(newArticle.Images);
            Context.Set<Article>().Add(newArticle);
            Context.SaveChanges();

            DbContextHelper.DetachAllEntries(Context);
            return newArticle.Id;
        }

        public virtual void Update(Article article)
        {
            foreach (var image in article.Images) Context.Entry(image).State = EntityState.Unchanged;
            Context.Entry(article).State = EntityState.Modified;

            Context.SaveChanges();

            foreach (var image in article.Images) Context.Entry(image).State = EntityState.Detached;
            Context.Entry(article).State = EntityState.Detached;
        }

        public virtual void Delete(Article article)
        {
            Context.AttachRange(article.Mentions);
            Context.RemoveRange(article.Images);
            Context.Remove(article);
            Context.SaveChanges();

            DbContextHelper.DetachAllEntries(Context);
        }

    }
}
