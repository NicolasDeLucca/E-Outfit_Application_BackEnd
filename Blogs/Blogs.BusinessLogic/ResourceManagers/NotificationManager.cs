using Blogs.Domain.BusinessEntities;
using Blogs.Interfaces;

namespace Blogs.BusinessLogic.ResourceManagers
{
    public class NotificationManager<T>: IManager<Notification<T>>
    {
        private readonly IRepository<Notification<T>> _notificationRepository;
        
        public NotificationManager(IRepository<Notification<T>> notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public IEnumerable<Notification<T>> GetAll()
        {
            return _notificationRepository.GetAllByCondition(n => true);
        }

        public void Delete(int id)
        {
            var notificationToDelete = _notificationRepository.GetById(id);
            _notificationRepository.Delete(notificationToDelete);
        }

        public IEnumerable<Notification<T>> GetAllBy(ISearchCriteria<Notification<T>> searchCriteria)
        {
            throw new NotImplementedException();
        }

        public void Update(Notification<T> notComment)
        {
            throw new NotImplementedException();
        }
        
        public Notification<T> Create(Notification<T> notification)
        {
            throw new NotImplementedException();
        }

        public Notification<T> GetById(int id)
        {
            throw new NotImplementedException();
        }

    }
}
