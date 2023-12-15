using static Blogs.Instances.DomainInstances;
using Blogs.Models.Out;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Blogs.Domain.BusinessEntities;

namespace Blogs.Models.Test.Out
{
    [TestClass]
    public class ArticleDetailTest
    {
        [TestMethod]
        public void ArticleDetailModelCreatedAsExpected()
        {
            var newArticle = CreateSimpleArticle(new User(), 0);
            ArticleDetail articleModel = new(newArticle);

            Assert.IsTrue(articleModel.Id == newArticle.Id);
            Assert.IsTrue(articleModel.Name == newArticle.Name);
            Assert.IsTrue(articleModel.UpdatedAt == newArticle.UpdatedAt.ToString());
            Assert.IsTrue(articleModel.Visibility == (int) newArticle.Visibility);
            Assert.IsTrue(articleModel.Text == newArticle.Text);
            Assert.IsTrue(articleModel.State == (int) newArticle.State);
        }
    }
}
