using Blogs.Domain.BusinessEntities.Mentions;
using Blogs.Domain.SearchCriteria;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blogs.Domain.Test.SearchCriteria
{
    [TestClass]
    public class SearchingByMentionTypeTest
    {
        [TestMethod]
        public void SearchCriteriaAcceptsByComment()
        {
           var men = new Comment(); 
           SearchingMentionByType<Comment> _searchByComment = new();
           Assert.IsTrue(_searchByComment.Criteria(men));
        }

        [TestMethod]
        public void SearchCriteriaAcceptsByResponse()
        {
            var men = new Response();
            men.AssociatedComment = new Comment();
            SearchingMentionByType<Response> _searchByResponse = new();
            Assert.IsTrue(_searchByResponse.Criteria(men));
        }
    }
}
