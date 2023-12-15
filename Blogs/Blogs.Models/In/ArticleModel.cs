using Blogs.Domain;
using Blogs.Domain.BusinessEntities;

namespace Blogs.Models.In
{
    public class ArticleModel
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public int Template { get; set; }
        public int Visibility { get; set; }
        public int State { get; set; }
        public List<string> ImagesData { get; set; }

        public Article ToEntity()
        {
            List<Image> images = new();
            foreach (string data in ImagesData) images.Add(new Image {Data = data});

            return new()
            {
                Name = Name,
                Text = Text,
                Visibility = (Visibility) Visibility,
                Template = (Template) Template,
                State = (EntityState) State,
                Images = images
            };
        }

        public override bool Equals(object? obj)
        {
            return obj != null && obj is ArticleModel model &&
                   Name == model.Name &&
                   Text == model.Text &&
                   Template == model.Template &&
                   Visibility == model.Visibility &&
                   State == model.State &&
                   EqualityComparer<List<string>>.Default.Equals(ImagesData, model.ImagesData);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Text, Template, Visibility, State, ImagesData);
        }
    }
}
