using Blogs.Domain.BusinessEntities;
using Blogs.Domain.BusinessEntities.Mentions;
using Blogs.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blogs.Domain.Test.BusinessEntities
{
    [TestClass]
    public class MentionTest
    {
        private Mention _validMention;

        [TestInitialize]
        public void SetUp()
        {
            var author = new User {Id = 1};
            _validMention = new Response
            {
                Id = 1,
                Text = "Nice article",
                Article = new Article(),
                Author = author,
                AssociatedComment = new Comment{Author = author}
            };
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRequestDataException))]
        public void MentionWithNoTextFailsValidation()
        {
            _validMention.Text = null;
            _validMention.ValidOrFail();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRequestDataException))]
        public void MentionWithNoArticleAssociatedFailsValidation()
        {
            _validMention.Article = null;
            _validMention.ValidOrFail();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRequestDataException))]
        public void MentionWithNoAuthorAssociatedFailsValidation()
        {
            _validMention.Author = null;
            _validMention.ValidOrFail();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRequestDataException))]
        public void ResponseWithNoCommentAssociatedFailsValidation()
        {
            Response response = (Response) _validMention;
            response.AssociatedComment = null;
            response.ValidOrFail();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRequestDataException))]
        public void ResponseWithNotTheSameAuthorAsItsCommentFailsValidation()
        {
            Response response = (Response)_validMention;
            response.AssociatedComment.Author = new User {Id = 2};
            response.ValidOrFail();
        }

        [TestMethod]
        public void ValidMentionPassesValidation()
        {
            _validMention.ValidOrFail();
        }

    }
}
