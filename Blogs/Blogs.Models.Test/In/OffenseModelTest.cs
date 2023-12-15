using Blogs.Models.In;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blogs.Models.Test.In
{
    [TestClass]
    public class OffenseModelTest
    {
        [TestMethod]
        public void ToEntityReturnsAsExpected()
        {
            var offenseModel = new OffenseModel() {Word = "Repugnante"};
            var offenseEntity = offenseModel.ToEntity();

            Assert.IsTrue(offenseModel.Word == offenseEntity.Word);
        }
    }
}
