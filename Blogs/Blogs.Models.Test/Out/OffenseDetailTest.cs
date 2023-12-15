using Blogs.Models.Out;
using static Blogs.Instances.DomainInstances;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blogs.Models.Test.Out
{
    [TestClass]
    public class OffenseDetailTest
    {
        [TestMethod]
        public void OffenseDetailModelCreatedAsExpected()
        {
            var newOffense = CreateSimpleOffense(0);
            OffenseDetail offenseModel = new(newOffense);

            Assert.IsTrue(offenseModel.Id == newOffense.Id);
            Assert.IsTrue(offenseModel.Word == newOffense.Word);
        }
    }
}
