using Blogs.Models.Out;
using static Blogs.Instances.DomainInstances;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Blogs.Domain.BusinessEntities;

namespace Blogs.Models.Test.Out
{
    [TestClass]
    public class MentionBasicInfoTest
    {
        [TestMethod]
        public void MentionBasicInfoModelCreatedAsExpected()
        {
            var mention = CreateSimpleComment(new User(), 0);
            mention.Article = CreateSimpleArticle(new User(), 0);
            MentionBasicInfo mentionModel = new(mention);

            Assert.IsTrue(mentionModel.Id == mention.Id);
            Assert.IsTrue(mentionModel.Text == mention.Text);
        }
    }
}
