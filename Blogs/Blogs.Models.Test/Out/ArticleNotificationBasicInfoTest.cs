using Blogs.Domain.BusinessEntities;
using Blogs.Models.Out;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Blogs.Instances.DomainInstances;

namespace Blogs.Models.Test.Out
{
    [TestClass]
    public class ArticleNotificationBasicInfoTest
    {
        [TestMethod]
        public void ArticleNotificationModelCreatedAsExpected()
        {
            var notification = new Notification<Article> {Text = "offensive article", AssociatedContent = CreateSimpleArticle(new User(), 0)};
            var notificationModel = new ArticleNotificationBasicInfo(notification);

            Assert.IsTrue(notificationModel.Text == notification.Text);
            Assert.IsTrue(notificationModel.ArticleModel.Equals(new ArticleBasicInfo(notification.AssociatedContent)));
        }

    }
}
