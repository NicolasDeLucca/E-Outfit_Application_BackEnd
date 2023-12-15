using Blogs.Domain.BusinessEntities;
using Blogs.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blogs.Domain.Test.BusinessEntities
{
    [TestClass]
    public class ArticleTest
    {
        private Article _validArticle;

        [TestInitialize]
        public void SetUp()
        {
            _validArticle = new Article()
            {
                Id = 1,
                Name = "F1 News",
                Text = ".......",
                Template = Template.TopLeft,
                Author = new User()
            };
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRequestDataException))]
        public void ArticleWithNoNameFailsValidation()
        {
            _validArticle.Name = null;
            _validArticle.ValidOrFail();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRequestDataException))]
        public void ArticleWithNoTemplateFailsValidation()
        {
            _validArticle.Template = null;
            _validArticle.ValidOrFail();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRequestDataException))]
        public void ArticleWithInvalidTemplateFailsValidation()
        {
            _validArticle.Template = Template.TopBottom;
            _validArticle.ValidOrFail();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRequestDataException))]
        public void ArticleWithNoAuthorFailsValidation()
        {
            _validArticle.Author = null;
            _validArticle.ValidOrFail();
        }

        [TestMethod]
        public void ValidArticlePassesValidation()
        {
            _validArticle.ValidOrFail();
        }
    }
}
