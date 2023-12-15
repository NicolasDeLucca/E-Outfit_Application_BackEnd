using Blogs.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace Blogs.Domain.BusinessEntities.Mentions
{
    public class Response : Mention
    {
        [Required]
        public virtual Comment AssociatedComment { get; set; }

        public Response()
        {
            UpdatedAt = DateTime.Now;
            AssociatedComment = null;
        }

        public override void ValidOrFail()
        {
            base.ValidOrFail();
            ValidateComment();
            ValidateCommentAuthor();
        }

        public override bool Equals(object? obj)
        {
            return obj != null && obj is Response && base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode());
        }

        #region Validators

        private void ValidateComment()
        {
            if (AssociatedComment == null)
                throw new InvalidRequestDataException("A response must be about a comment");
        }

        private void ValidateCommentAuthor()
        {
            if (AssociatedComment.Author != Author)
                throw new InvalidRequestDataException("A response must be about its comment author");
        }

        #endregion
    }
}
