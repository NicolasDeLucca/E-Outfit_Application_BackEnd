using Blogs.Models.Out;
using static Blogs.Instances.DomainInstances;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Blogs.Domain.BusinessEntities;

namespace Blogs.Models.Test.Out
{
    [TestClass]
    public class ArticleBasicInfoTest
    {
        [TestMethod]
        public void ArticleBasicInfoModelCreatedAsExpected()
        {
            var article = CreateSimpleArticle(new User(), 0);
            ArticleBasicInfo articleModel = new(article);

            Assert.IsTrue(articleModel.Id == article.Id);
            Assert.IsTrue(articleModel.Name == article.Name);
        }
    }
}
