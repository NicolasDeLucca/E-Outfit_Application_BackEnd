using Blogs.Domain;
using Blogs.Domain.BusinessEntities.Mentions;
using Blogs.Exceptions;

namespace Blogs.Models.In
{
    public class MentionModel
    {
        public MentionModel? CommentModel { get; set; } = null;
        public UserModel AuthorModel { get; set; } 
        public ArticleModel ArticleModel { get; set; }
        public int State { get; set; }
        public string Text { get; set; }

        public Mention ToEntity()
        {
            if (CommentModel == null)
                return new Comment {Article = ArticleModel.ToEntity(), Text = Text, Author = AuthorModel.ToEntity(), State = (EntityState) State};
            else 
                if (CommentModel.CommentModel == null)
                    return new Response {Article = ArticleModel.ToEntity(), Text = Text, Author = AuthorModel.ToEntity(), State = (EntityState) State, 
                        AssociatedComment = (Comment) CommentModel.ToEntity()};
                else 
                    throw new InvalidRequestDataException("A response cannot have another response associated, must be a comment associated");
        }
        public override bool Equals(object? obj)
        {
            return obj is MentionModel model && State == model.State && Text == model.Text &&
                AuthorModel.Equals(model.AuthorModel) && ArticleModel.Equals(model.ArticleModel) &&
                CommentModel.Equals(model.CommentModel);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(State, Text, AuthorModel, ArticleModel, CommentModel);
        }
    }
}
