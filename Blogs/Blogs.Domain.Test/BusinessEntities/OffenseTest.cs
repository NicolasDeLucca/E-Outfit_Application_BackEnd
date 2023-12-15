using Blogs.Domain.BusinessEntities;
using Blogs.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blogs.Domain.Test.BusinessEntities
{
    [TestClass]
    public class OffenseTest
    {
        private Offense _validOffense;

        [TestInitialize]
        public void SetUp()
        {
            _validOffense = new Offense {Id = 1, Word = "denigrante"};
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRequestDataException))]
        public void OffenseWithNoWordFailsValidation()
        {
            _validOffense.Word = null;
            _validOffense.ValidOrFail();
        }

        [TestMethod]
        public void ValidOffensePassesValidation()
        {
            _validOffense.ValidOrFail();
        }

    }
}
