using static Blogs.Instances.DomainInstances;
using Blogs.Domain.BusinessEntities;
using Blogs.Models.Out;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blogs.Models.Test.Out
{
    [TestClass]
    public class UserDetailTest
    {
        [TestMethod]
        public void UserDetailModelCreatedAsExpected()
        {
            User newUser = CreateSimpleUser(0);
            UserDetail userModel = new(newUser);

            Assert.IsTrue(userModel.Id == newUser.Id);
            Assert.IsTrue(userModel.Email == newUser.Email);
            Assert.IsTrue(userModel.UserName == newUser.UserName);
            Assert.IsTrue(userModel.Role == (int) newUser.Role);
        }
    }
}
