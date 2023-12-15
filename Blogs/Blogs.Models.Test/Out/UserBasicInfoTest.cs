using static Blogs.Instances.DomainInstances;
using Blogs.Domain.BusinessEntities;
using Blogs.Models.Out;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blogs.Models.Test.Out
{
    [TestClass]
    public class UserBasicInfoTest
    {
        [TestMethod]
        public void UserBasicInfoModelCreatedAsExpected()
        {
            User newUser = CreateSimpleUser(0);
            UserBasicInfo userModel = new(newUser);

            Assert.IsTrue(userModel.Id == newUser.Id);
            Assert.IsTrue(userModel.Email == newUser.Email);
        }
    }
}
