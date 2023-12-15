using Blogs.DataAccess.Contexts;
using Blogs.DataAccess.Repositories;
using Blogs.Domain.BusinessEntities;
using Blogs.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blogs.DataAccess.Test.Repositories
{
    [TestClass]
    public class OffenseRepositoryTest
    {
        private Offense _defaultOffense;
        private IRepository<Offense> _repository;

        private DataContext _dataContext;

        [TestInitialize]
        public void SetUp()
        {
            _dataContext = ContextFactory.GetNewContext(ContextType.SqLite);
            _repository = new OffenseRepository(_dataContext);

            _dataContext.Database.OpenConnection();
            _dataContext.Database.EnsureCreated();

            _defaultOffense = new Offense {Id = 1, Word = "Espantoso"};
            _repository.Store(_defaultOffense);
        }

        [TestMethod]
        public void GetAllOffensesReturnsAsExpected()
        {
            List<Offense> expectedOffenses = new(){_defaultOffense};
            var retrievedOffenses = _repository.GetAllByCondition(o => true);
            CollectionAssert.AreEquivalent(expectedOffenses, retrievedOffenses.ToList());
        }

        [TestMethod]
        public void GetOffenseByIdReturnsAsExpected()
        {
            var retrievedOffense = _repository.GetById(_defaultOffense.Id);
            Assert.IsTrue(retrievedOffense.Equals(_defaultOffense));
        }

        [TestMethod]
        public void StoreOffenseReturnsAsExpected()
        {
            Assert.IsTrue(_repository.GetAllByCondition(o => true).Count() == 1);
        }

        [TestMethod]
        public void UpdateOffenseDoesAsExpected()
        {
            var newWord = "Desagradable";
            _defaultOffense.Word = newWord;
            _repository.Update(_defaultOffense);

            var updatedOffense = _repository.GetAllByCondition(o => o.Id == _defaultOffense.Id).First();
            Assert.AreEqual(_defaultOffense, updatedOffense);
            Assert.IsTrue(updatedOffense.Word == newWord);
        }

        [TestMethod]
        public void DeleteOffenseDoesAsExpected()
        {
            _repository.Delete(_defaultOffense);
            Assert.IsTrue(_repository.GetAllByCondition(o => true).Count() == 0);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _dataContext.Database.EnsureDeleted();
        }
    }
}
