using Blogs.Instances;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blogs.Models.Test.In
{
    [TestClass]
    public class UserModelTest
    {
        [TestMethod]
        public void ToEntityReturnsAsExpected()
        {
            var userModel = ModelInstances.CreateUserModel();
            var user = userModel.ToEntity();

            Assert.IsTrue(userModel.Name == user.Name);
            Assert.IsTrue(userModel.LastName == user.LastName);
            Assert.IsTrue(userModel.Email == user.Email);
            Assert.IsTrue(userModel.UserName == user.UserName);
            Assert.IsTrue(userModel.Role == (int) user.Role);
            Assert.IsTrue(userModel.Password == user.Password);
        }
    }
}
