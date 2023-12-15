using static Blogs.Instances.ModelInstances;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Blogs.Domain.BusinessEntities.Mentions;
using static Blogs.Instances.DomainInstances;

namespace Blogs.Models.Test.In
{
    [TestClass]
    public class MentionModelTest
    {
        [TestMethod]
        public void CommentToEntityReturnsAsExpected()
        {
            var author = CreateSimpleUser(0);
            var authorArticle = CreateSimpleArticle(author, 0);
            author.PostedArticles.Add(authorArticle);
            
            var authorModel = GetUserModel(author);

            var mentionModel = CreateCommentModel(author, authorArticle);
            var mention = mentionModel.ToEntity();

            Assert.IsTrue(mentionModel.Text == mention.Text);
            Assert.IsTrue(mentionModel.State == (int)mention.State);
            Assert.IsTrue(mentionModel.CommentModel == null);
            Assert.IsTrue(mentionModel.AuthorModel.Equals(authorModel));
        }

        [TestMethod]
        public void ResponseToEntityReturnsAsExpected()
        {
            var author = CreateSimpleUser(0);
            var authorArticle = CreateSimpleArticle(author, 0);
            author.PostedArticles.Add(authorArticle);
            var com = CreateSimpleComment(author, 0);
            com.Article = authorArticle;

            var authorModel = GetUserModel(author);

            var mentionModel = CreateResponseModel(author, com);
            var mention = mentionModel.ToEntity();

            Assert.IsTrue(mentionModel.Text == mention.Text);
            Assert.IsTrue(mentionModel.State == (int)mention.State);
            Assert.IsTrue(((Response)mention).AssociatedComment.Equals(com));
            Assert.IsTrue(mentionModel.AuthorModel.Equals(authorModel));
        }

    }
}
