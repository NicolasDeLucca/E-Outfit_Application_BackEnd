using Blogs.Domain.BusinessEntities;
using Blogs.Domain.BusinessEntities.Mentions;
using Blogs.Models.Out;
using static Blogs.Instances.DomainInstances;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blogs.Models.Test.Out
{
    [TestClass]
    public class MentionNotificationBasicInfoTest
    {
        [TestMethod]
        public void MentionNotificationModelCreatedAsExpected()
        {
            var notification = new Notification<Mention> {Text = "new mention", AssociatedContent = CreateSimpleComment(new User(), 0)};
            var notificationModel = new MentionNotificationBasicInfo(notification);

            Assert.IsTrue(notificationModel.Text == notification.Text);
            Assert.IsTrue(notificationModel.MentionModel.Equals(new MentionBasicInfo(notification.AssociatedContent)));
        }
    }
}
