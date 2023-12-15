using Blogs.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace Blogs.Domain.BusinessEntities
{
    public class Offense
    {
        public int Id { get; set; }

        [Required]
        public string Word { get; set; }

        public Offense()
        {
            Word = null;
        }

        public virtual void ValidOrFail()
        {
            ValidateWord();
        }

        #region Validators

        private void ValidateWord()
        {
            if (Word == null)
                throw new InvalidRequestDataException("Must provide a offensive word");
        }

        #endregion

        public override bool Equals(object? obj)
        {
            return obj != null && obj is Offense offense && Id == offense.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }
}
