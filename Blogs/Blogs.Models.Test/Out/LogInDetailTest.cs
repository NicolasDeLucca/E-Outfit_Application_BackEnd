using Blogs.Domain.BusinessEntities;
using static Blogs.Instances.DomainInstances;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Blogs.Models.Out;

namespace Blogs.Models.Test.Out
{
    [TestClass]
    public class LogInDetailTest
    {
        [TestMethod]
        public void SessionModelCreatedAsExpected()
        {
            var newSession = new Session {Id = 0, AuthenticatedUser = CreateSimpleUser(0)};
            LogInDetail logInDetail = new(newSession);

            Assert.IsTrue(newSession.Id == logInDetail.Id);
            Assert.IsTrue(newSession.AuthenticatedUser.Role.ToString() == logInDetail.Role.ToString());
            Assert.IsTrue(newSession.AuthToken.ToString() == logInDetail.AuthToken.ToString());
            Assert.IsTrue(newSession.AuthenticatedUser.UserName == logInDetail.UserName);
        }
    }
}
