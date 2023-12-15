using Blogs.Exceptions;
using static Blogs.Domain.Utilities;
using System.ComponentModel.DataAnnotations;

namespace Blogs.Domain.BusinessEntities.Mentions
{
    public class Mention
    {
        public int Id { get; set; }

        public virtual EntityState State { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public virtual DateTime UpdatedAt { get; set; }
        
        [Required]
        public virtual Article Article { get; set; }

        [Required]
        public virtual User Author { get; set; }

        public Mention()
        {
            State = EntityState.Unchanged;
            Article = null;
            Author = null;
        }

        public virtual void ValidOrFail()
        {
            ValidateText();
            ValidateArticle();
            ValidateAuthor();
        }

        #region Validators

        protected void ValidateText()
        {
            if (Text == null)
                throw new InvalidRequestDataException("Must provide some text");
        }

        protected void ValidateArticle()
        {
            if (Article == null)
                throw new InvalidRequestDataException("Must provide an article");
        }

        protected void ValidateAuthor()
        {
            if (Author == null)
                throw new InvalidRequestDataException("Must provide an author");
        }

        #endregion

        public virtual bool IsOffensive(List<string> offensiveWords)
        {
            ValidOrFail();
            return offensiveWords.Count > 0 && ExistsWord(offensiveWords, Text);
        }

        public override bool Equals(object? obj)
        {
            return obj != null && obj is Mention mention && Id == mention.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }
}
