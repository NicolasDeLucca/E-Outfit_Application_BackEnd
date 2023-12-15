using Blogs.Domain.BusinessEntities;
using Blogs.Interfaces;

namespace Blogs.BusinessLogic.ResourceManagers
{
    public class UserManager : IManager<User>
    {
        private readonly IRepository<User> _userRepository;

        public UserManager(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public IEnumerable<User> GetAll()
        {
            return _userRepository.GetAllByCondition(u => true);
        }

        public IEnumerable<User> GetAllBy(ISearchCriteria<User> searchCriteria)
        {
            return _userRepository.GetAllByCondition(searchCriteria.Criteria);
        }

        public User GetById(int id)
        {
            return _userRepository.GetById(id);
        }

        //sign up
        public User Create(User user)
        {
            if (user == null)
                throw new ArgumentException();

            user.ValidOrFail();

            var userId = _userRepository.Store(user);
            var createdUser = _userRepository.GetById(userId);
            return createdUser;
        }

        public void Update(User user)
        {
            var existsAssert = _userRepository.GetById(user.Id);
            _userRepository.Update(user);
        }

        // sign down
        public void Delete(int id)
        {
            var userToDelete = _userRepository.GetById(id);
            _userRepository.Delete(userToDelete);
        }

    }
}
