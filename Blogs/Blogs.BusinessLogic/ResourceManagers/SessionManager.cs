using Blogs.Domain.BusinessEntities;
using Blogs.Interfaces;
using System.Security.Authentication;

namespace Blogs.BusinessLogic.ResourceManagers
{
    public class SessionManager : IManager<Session>
    {
        private readonly IRepository<Session> _sessionRepository;
        private readonly IRepository<User> _userRepository;

        public SessionManager(IRepository<Session> sessionRepository, IRepository<User> userRepository)
        {
            _sessionRepository = sessionRepository;
            _userRepository = userRepository;
        }

        public IEnumerable<Session> GetAll()
        {
            return _sessionRepository.GetAllByCondition(s => true);
        }

        public IEnumerable<Session> GetAllBy(ISearchCriteria<Session> searchCriteria)
        {
            return _sessionRepository.GetAllByCondition(searchCriteria.Criteria);
        }

        // log/sign in
        public Session Create(Session session)
        {
            if (session == null || session.AuthenticatedUser == null)
                throw new InvalidCredentialException("No credentials provided");

            var retrievedUser = _userRepository.GetAllByCondition(u => u.UserName == session.AuthenticatedUser.UserName && 
                u.Password == session.AuthenticatedUser.Password).FirstOrDefault();
            if (retrievedUser == null)
                throw new InvalidCredentialException("Invalid credentials");

            var existingSession = _sessionRepository.GetAllByCondition(s => s.AuthenticatedUser.Id == retrievedUser.Id).FirstOrDefault();
            if (existingSession != null)
                return existingSession;

            session.AuthenticatedUser = retrievedUser;
            int sessionId = _sessionRepository.Store(session);
            return GetById(sessionId);
        }

        public Session GetById(int id)
        {
            return _sessionRepository.GetById(id);
        }

        public void Update(Session session)
        {
            var existsAssert = _sessionRepository.GetById(session.Id);
            _sessionRepository.Update(session);
        }

        // log/sign out
        public void Delete(int id)
        {
            Session sessionToDelete = _sessionRepository.GetById(id);
            _sessionRepository.Delete(sessionToDelete);
        }

    }
}
