namespace Blogs.Interfaces
{
    public interface ISearchCriteria<T>
    {
       bool Criteria(T entity);
    }
}


