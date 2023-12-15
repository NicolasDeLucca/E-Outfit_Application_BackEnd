using Microsoft.VisualStudio.TestTools.UnitTesting;
using Blogs.Domain.SearchCriteria;
using Blogs.Domain.BusinessEntities;

namespace Blogs.Domain.Test.SearchCriteria
{
    [TestClass]
    public class SearchingArticleByTextTest
    {
        private SearchingArticleByText _searchByText;
        private Article _article;

        [TestInitialize]
        public void SetUp()
        {
            _searchByText = new SearchingArticleByText();
            _article = new Article() {Name = "F1 News", Text = "the race yesterday was incredible!", 
                    Template = Template.Top, Author = new User()};
            _article.ValidOrFail();
        }

        [TestMethod]
        public void EmptySearchCriteriaAcceptsAnyArticle()
        {
            Assert.IsTrue(_searchByText.Criteria(_article));
        }

        [TestMethod]
        public void SearchCriteriaAcceptsByTitleText()
        {
            _searchByText.Text = "F1";
            Assert.IsTrue(_searchByText.Criteria(_article));
        }

        [TestMethod]
        public void SearchCriteriaAcceptsByContentText()
        {
            _searchByText.Text = "race";
            Assert.IsTrue(_searchByText.Criteria(_article));
        }

        [TestMethod]
        public void SearchCriteriaDeniesBecauseItDoesntMatchArticleText()
        {
            _searchByText.Text = "cat";
            Assert.IsFalse(_searchByText.Criteria(_article));
        }
    }
}
