using Blogs.Domain;
using Blogs.Exceptions;
using static Blogs.Domain.Utilities;

namespace Blogs.Importing.DTOs
{
    public class ImportedArticle
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public DateTime UpdatedAt { get; }
        public Template Template { get; set; }
        public List<ImportedImage> Images { get; set; }

        public ImportedArticle()
        {
            UpdatedAt = DateTime.Now;
            Images = new();
        }

        public void ValidOrFail()
        {
            foreach (ImportedImage image in Images) image.ValidOrFail();
            ValidateName();
            ValidateTemplate();
        }

        public bool IsOffensive(List<string> offensiveWords)
        {
            ValidOrFail();
            return offensiveWords != null && Text != null && (ExistsWord(offensiveWords, Text) ||
                ExistsWord(offensiveWords, Name));
        }

        private void ValidateName()
        {
            if (Name == null)
                throw new InvalidRequestDataException("Must provide an article name");
        }

        private void ValidateTemplate()
        {
            if (Template == Template.TopBottom && Images.Count < 2)
                throw new InvalidRequestDataException("This template requires two images");
        }

        public class ImportedImage
        {
            public string Data { get; set; }

            public void ValidOrFail()
            {
                if (Data == null)
                    throw new InvalidRequestDataException("Must provide image data");
            }

            public override bool Equals(object? obj)
            {
                return obj != null && obj is ImportedImage image && Data == image.Data;
            }
        }

        public override bool Equals(object? obj)
        {
            return obj != null && obj is ImportedArticle article && Name == article.Name
                && Text == article.Text && UpdatedAt == article.UpdatedAt
                && Template == article.Template && Images.SequenceEqual(article.Images);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Text, UpdatedAt, Template, Images);
        }

    }
}
