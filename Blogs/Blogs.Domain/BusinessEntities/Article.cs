using Blogs.Domain.BusinessEntities.Mentions;
using static Blogs.Domain.Utilities;
using DefaultTemplate = Blogs.Domain.Template;
using Blogs.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace Blogs.Domain.BusinessEntities
{
    public class Article
    {
        private DefaultTemplate? _template;

        public int Id { get; set; }

        public virtual EntityState State { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        [Required]
        public virtual DefaultTemplate? Template
        { 
            get => _template;
            set {ValidateTemplateSet(value); _template = value;} 
        }

        [Required]
        public virtual Visibility Visibility { get; set; }
        
        [Required]
        public virtual User Author { get; set; }

        public virtual ICollection<Mention> Mentions { get; set; }
        public virtual ICollection<Image> Images { get; set; }

        public Article() 
        {
            State = EntityState.Unchanged;
            UpdatedAt = DateTime.Now;
            Visibility = Visibility.Private;
            Images = new List<Image>();
            Mentions = new List<Mention>();
            Author = null;
            Template = null;
        }

        public virtual void ValidOrFail()
        {
            ValidateName();
            ValidateTemplate();
            ValidateAuthor();
        }

        #region Validators

        private void ValidateName()
        {
            if (Name == null)
                throw new InvalidRequestDataException("Must provide an article name");
        }

        private void ValidateTemplate()
        {
            if (Template == null)
                throw new InvalidRequestDataException("Must provide a template position");
            ValidateTemplateSet(Template);
        }

        private void ValidateTemplateSet(DefaultTemplate? template)
        {
            if (template == DefaultTemplate.TopBottom && Images.Count < 2)
                throw new InvalidRequestDataException("This template requires two images");
        }

        private void ValidateAuthor()
        {
            if (Author == null)
                throw new InvalidRequestDataException("Must provide an author");
        }

        #endregion

        public virtual bool IsOffensive(List<string> offensiveWords)
        {
            ValidOrFail();
            return offensiveWords.Count > 0 && Text != null && (ExistsWord(offensiveWords, Text) ||
                ExistsWord(offensiveWords, Name));
        }

        public override bool Equals(object? obj)
        {
            return obj != null && obj is Article article && Id == article.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }
}
