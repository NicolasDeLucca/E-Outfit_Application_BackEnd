using Blogs.Domain.BusinessEntities;
using Blogs.Interfaces;

namespace Blogs.BusinessLogic.ResourceManagers
{
    public class OffenseManager : IManager<Offense>
    {
        private readonly IRepository<Offense> _offenseRepository;

        public OffenseManager(IRepository<Offense> offenseRepository)
        {
            _offenseRepository = offenseRepository;
        }

        public IEnumerable<Offense> GetAll()
        {
            return _offenseRepository.GetAllByCondition(o => true);
        }

        public Offense Create(Offense offense)
        {
            if (offense == null)
                throw new ArgumentException();

            offense.ValidOrFail();

            var offenseId = _offenseRepository.Store(offense);
            var createdOffense = _offenseRepository.GetById(offenseId);
            return createdOffense;
        }

        public void Update(Offense offense)
        {
            var existsAssert = _offenseRepository.GetById(offense.Id);
            _offenseRepository.Update(offense);
        }

        public void Delete(int id)
        {
            var offenseToDelete = _offenseRepository.GetById(id);
            _offenseRepository.Delete(offenseToDelete);
        }

        public Offense GetById(int id)
        {
            return _offenseRepository.GetById(id);
        }

        public IEnumerable<Offense> GetAllBy(ISearchCriteria<Offense> searchCriteria)
        {
            throw new NotImplementedException();
        }
    }
}
