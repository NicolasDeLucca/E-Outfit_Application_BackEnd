namespace Blogs.Interfaces
{
    public interface IRepository<T>
    {
        IEnumerable<T> GetAllByCondition(Func<T, bool> predicate);
        T GetById(int id);
        int Store(T t);
        void Update(T t);
        void Delete(T t);
    }

}
