using Blogs.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace Blogs.Domain.BusinessEntities
{
    public class Image
    {
        public int Id { get; set; }

        [Required]
        public string Data { get; set; }

        [Required]
        public virtual Article Article { get; set; }

        public Image()
        {
            Article = null;
        }

        public virtual void ValidOrFail()
        {
            ValidateData();
            ValidateArticle();
        }

        #region Validation

        private void ValidateData()
        {
            if (Data == null)
                throw new InvalidRequestDataException("Must provide image data");
        }

        private void ValidateArticle()
        {
            if (Article == null)
                throw new InvalidRequestDataException("Must provide image article");
        }

        #endregion

        public override bool Equals(object? obj)
        {
            return obj != null && obj is Image image && Id == image.Id ;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }
}
