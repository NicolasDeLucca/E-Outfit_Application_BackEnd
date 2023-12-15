using Blogs.BusinessLogic.ResourceManagers;
using Blogs.Domain.BusinessEntities;
using Blogs.Domain.BusinessEntities.Mentions;
using Blogs.Exceptions;
using Blogs.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections;

namespace Blogs.BusinessLogic.Test.ResourceManagers
{
    [TestClass]
    public class OffenseManagerTest
    {
        private Mock<IRepository<Offense>> _offenseRepositoryMock;
        private Mock<Offense> _offenseMock;


        [TestInitialize]
        public void SetUp()
        {
            _offenseRepositoryMock = new Mock<IRepository<Offense>>(MockBehavior.Strict);
            _offenseMock = new Mock<Offense>();
        }

        [TestMethod]
        public void CreateOffenseReturnsAsExpected()
        {
            var offenseCreated = new Offense {Id = 0, Word = "Disgusting"};

            _offenseMock.Setup(o => o.ValidOrFail());
            _offenseRepositoryMock.Setup(r => r.Store(It.IsAny<Offense>())).Returns(0);
            _offenseRepositoryMock.Setup(r => r.GetById(It.IsAny<int>())).Returns(offenseCreated);

            OffenseManager offenseManager = new(_offenseRepositoryMock.Object);
            var retrievedOffense = offenseManager.Create(offenseCreated);

            _offenseRepositoryMock.VerifyAll();
            Assert.AreEqual(offenseCreated, retrievedOffense);
        }

        [TestMethod]
        public void GetAllOffensesReturnsAsExpected()
        {
            var offenses = It.IsAny<List<Offense>>();
            _offenseRepositoryMock.Setup(r => r.GetAllByCondition(It.IsAny<Func<Offense, bool>>())).Returns(offenses);

            OffenseManager offenseManager = new(_offenseRepositoryMock.Object);
            var retrievedOffenses = offenseManager.GetAll();

            _offenseRepositoryMock.VerifyAll();
            CollectionAssert.AreEquivalent((ICollection) retrievedOffenses, offenses);
        }

        [TestMethod]
        public void GetOffenseByIdReturnsAsExpected()
        {
            var offense = new Offense{Id = 0, Word = "Deplorable"};
            _offenseRepositoryMock.Setup(r => r.GetById(It.IsAny<int>())).Returns(offense);

            OffenseManager offenseManager = new(_offenseRepositoryMock.Object);
            var retrievedOffense = offenseManager.GetById(0);

            _offenseRepositoryMock.VerifyAll();
            Assert.AreEqual(retrievedOffense, offense);
        }

        [TestMethod]
        public void UpdateWordDoesAsExpected()
        {
            Offense offenseToUpdate = new(){Id = 0, Word = "Horrible"};

            _offenseRepositoryMock.Setup(r => r.GetById(It.IsAny<int>())).Returns(offenseToUpdate);
            _offenseRepositoryMock.Setup(r => r.Update(offenseToUpdate));

            offenseToUpdate.Word = "Asqueroso";

            OffenseManager offenseManager = new(_offenseRepositoryMock.Object);
            offenseManager.Update(offenseToUpdate);

            var updatedOffense = _offenseRepositoryMock.Object.GetById(offenseToUpdate.Id);

            _offenseRepositoryMock.VerifyAll();
            Assert.IsTrue(updatedOffense.Word == "Asqueroso");
        }

        [TestMethod]
        [ExpectedException(typeof(ResourceNotFoundException))]
        public void UpdateNonExistentOffenseFails()
        {
            _offenseRepositoryMock.Setup(r => r.GetById(It.IsAny<int>())).Throws(new ResourceNotFoundException("Could not find specified offense"));

            OffenseManager offenseManager = new(_offenseRepositoryMock.Object);
            offenseManager.Update(new());

            _offenseRepositoryMock.VerifyAll();
        }

        [TestMethod]
        public void DeleteOffenseDoesAsExpected()
        {
            Offense offenseToDelete = new(){Id = 0};

            _offenseRepositoryMock.Setup(r => r.GetById(It.IsAny<int>())).Returns(offenseToDelete);
            _offenseRepositoryMock.Setup(r => r.Delete(It.IsAny<Offense>()));

            OffenseManager offenseManager = new(_offenseRepositoryMock.Object);
            offenseManager.Delete(0);

            _offenseRepositoryMock.VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(ResourceNotFoundException))]
        public void DeleteNonExistentOffenseFails()
        {
            _offenseRepositoryMock.Setup(r => r.GetById(It.IsAny<int>())).Throws(new ResourceNotFoundException("Could not find specified offense"));

            OffenseManager offenseManager = new(_offenseRepositoryMock.Object);
            offenseManager.Delete(0);

            _offenseRepositoryMock.VerifyAll();
        }

    }
}
