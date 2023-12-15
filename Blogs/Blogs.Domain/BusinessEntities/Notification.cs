using Blogs.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace Blogs.Domain.BusinessEntities
{
    public class Notification<T>
    {
        public int Id { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public virtual T AssociatedContent { get; set; }

        [Required]
        public virtual MessageVerification State { get; set; }

        [Required]
        public virtual User Receiver { get; set; }

        public Notification()
        {
            State = MessageVerification.UnRead;
            AssociatedContent = default(T);
            Receiver = null;
            Text = string.Empty;
        }

        public virtual void ValidOrFail()
        {
            ValidContent();
            ValidText();
            ValidReceiver();
        }

        public virtual void Read()
        {
            State = MessageVerification.Read;
        }

        #region Validators

        private void ValidContent()
        {
            if (AssociatedContent == null)
                throw new InvalidRequestDataException("Must provide a associated content");
        }

        private void ValidReceiver()
        {
            if (Receiver == null)
                throw new InvalidRequestDataException("Must provide a receiver");
        }

        private void ValidText()
        {
            if (string.IsNullOrEmpty(Text))
                throw new InvalidRequestDataException("Must provide a text");
        }

        #endregion

        public override bool Equals(object? obj)
        {
            return obj != null && obj is Notification<T> notification && Id == notification.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }
}
