using Blogs.Domain.BusinessEntities;
using Blogs.Instances;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blogs.Models.Test.In
{
    [TestClass]
    public class ArticleModelTest
    {
        [TestMethod]
        public void ToEntityReturnsAsExpected()
        {
            var articleModel = ModelInstances.CreateArticleModel();
            var article = articleModel.ToEntity();

            Assert.IsTrue(articleModel.Name == article.Name);
            Assert.IsTrue(articleModel.Text == article.Text);
            Assert.IsTrue(articleModel.Visibility == (int) article.Visibility);
            Assert.IsTrue(articleModel.Template == (int) article.Template);
            Assert.IsTrue(articleModel.State == (int) article.State);
            CollectionAssert.AreEqual(articleModel.ImagesData, article.Images.Select(i => i.Data).ToList());
        }

    }
}
