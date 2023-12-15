using Blogs.Domain.BusinessEntities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Blogs.Exceptions;

namespace Blogs.Domain.Test.BusinessEntities
{
    [TestClass]
    public class ImageTest
    {
        private Image _validImage;

        [TestInitialize]
        public void SetUp()
        {
            _validImage = new Image()
            {
                Id = 1,
                Data = "url image",
                Article = new Article()
            };
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRequestDataException))]
        public void ImageWithNoDataFailsValidation()
        {
            _validImage.Data = null;
            _validImage.ValidOrFail();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRequestDataException))]
        public void ImageWithNoArticleFailsValidation()
        {
            _validImage.Article = null;
            _validImage.ValidOrFail();
        }

        [TestMethod]
        public void ValidImagePassesValidation()
        {
            _validImage.ValidOrFail();
        }
    }
}
