using Blogs.Domain;
using Blogs.Domain.BusinessEntities;
using Blogs.Domain.BusinessEntities.Mentions;
using Blogs.Models.In;
using System.Diagnostics.CodeAnalysis;

namespace Blogs.Instances
{
    [ExcludeFromCodeCoverage]
    public static class ModelInstances
    {
        public static MentionModel CreateCommentModel(User author, Article article)
        {
            var articleModel = GetArticleModel(article);
            var authorModel = GetUserModel(author);

            return new MentionModel()
            {
                Text = "...",
                AuthorModel = authorModel,
                CommentModel = null,
                ArticleModel = articleModel
            };
        }

        public static MentionModel CreateResponseModel(User author, Comment replyAbout)
        {
            var commentModel = GetCommentModel(replyAbout);
            var authorModel = GetUserModel(author);

            return new MentionModel()
            {
                Text = "nice comment",
                AuthorModel = authorModel,
                CommentModel = commentModel,
                ArticleModel = commentModel.ArticleModel
            };
        }

        public static ArticleModel CreateArticleModel()
        {
            return new ArticleModel()
            {
                Name = "Football News",
                Text = "......",
                Visibility = (int) Visibility.Private,
                Template = (int) Template.TopLeft,
                ImagesData = new List<string>() {}
            };
        }

        public static UserModel CreateUserModel()
        {
            return new UserModel()
            { 
                Name = "Nicolas",
                LastName = "Rodriguez",
                Email = "nicorodriguez@gmail.com",
                UserName = "NicoRodri",
                Role = (int) UserRole.Blogger,
                Password = "pepito123"
            };
        }

        public static LogInModel CreateLogModel()
        {
            return new()
            {
                UserName = "NGonza",
                Password = "ngon_1234"
            };
        }

        public static UserModel GetUserModel(User user)
        {
            return new()
            {
                Email = user.Email,
                LastName = user.LastName,
                Name = user.Name,
                Password = user.Password,
                Role = (int) user.Role,
                UserName = user.UserName
            };
        }

        private static MentionModel GetCommentModel(Comment comment)
        {
            var userModel = GetUserModel(comment.Author);
            var articleModel = GetArticleModel(comment.Article);

            return new()
            {
                Text = comment.Text,
                AuthorModel = userModel,
                CommentModel = null,
                ArticleModel = articleModel
            };
        }

        public static ArticleModel GetArticleModel(Article article)
        {
            if (article == null)
                return null;

            var imagesdata = article.Images?.Select(i => i.Data).ToList();

            return new()
            {
                Name = article.Name,
                State = (int) article.State,
                Template = (int) article.Template,
                Visibility = (int) article.Visibility,
                Text = article.Text,
                ImagesData = imagesdata,
            };
        }

    }
}
        
