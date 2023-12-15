using Blogs.Domain.BusinessEntities;
using Blogs.Models.Out;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Blogs.Instances.DomainInstances;

namespace Blogs.Models.Test.Out
{
    [TestClass]
    public class MentionDetailTest
    {
        [TestMethod]
        public void MentionModelCreatedAsExpected()
        {
            var author = new User();
            var response = CreateSimpleResponse(author, 0);
            response.Article = CreateSimpleArticle(author, 0);
            var mentionModel = new MentionDetail(response);

            Assert.IsTrue(mentionModel.Text == response.Text);
            Assert.IsTrue(mentionModel.UpdatedAt == response.UpdatedAt.ToString());
            Assert.IsTrue(mentionModel.ArticleId.Equals(response.Article.Id));
        }
    }
}
