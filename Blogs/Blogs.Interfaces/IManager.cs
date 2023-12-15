namespace Blogs.Interfaces
{
    public interface IManager<T>
    {
        IEnumerable<T> GetAll();
        IEnumerable<T> GetAllBy(ISearchCriteria<T> searchCriteria);
        T GetById(int id);
        T Create(T t);
        void Update(T t);
        void Delete(int id);
    }
}
