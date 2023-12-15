using static Blogs.Instances.ModelInstances;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blogs.Models.Test.In
{
    [TestClass]
    public class LogInModelTest
    {
        [TestMethod]
        public void ToEntityReturnsAsExpected()
        {
            var log = CreateLogModel();
            var user = log.ToEntity();

            Assert.IsTrue(log.UserName == user.UserName);
            Assert.IsTrue(log.Password == user.Password);
        }
    }
}
