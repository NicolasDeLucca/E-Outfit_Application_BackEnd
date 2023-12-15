using Blogs.Domain.BusinessEntities;
using Blogs.Domain;

namespace Blogs.Importing.DTOs
{
    public class EntityMapper
    {
        public Article MapArticle(ImportedArticle importedArticle)
        {
            importedArticle.ValidOrFail();

            Article article = new()
            {
                Name = importedArticle.Name,
                Template = importedArticle.Template,
                Text = importedArticle.Text,
                State = EntityState.Unchanged,
                UpdatedAt = importedArticle.UpdatedAt,
                Visibility = Visibility.Public                
            };

            foreach(var image in importedArticle.Images)
            {
                article.Images.Add(new Image {Data = image.Data});
            }

            return article;
        }
    }
}
