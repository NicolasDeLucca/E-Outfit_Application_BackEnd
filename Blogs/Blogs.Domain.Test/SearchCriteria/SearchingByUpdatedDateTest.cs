using Blogs.Domain.BusinessEntities.Mentions;
using Blogs.Domain.BusinessEntities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Blogs.Instances.DomainInstances;
using Blogs.Domain.SearchCriteria;

namespace Blogs.Domain.Test.SearchCriteria
{
    [TestClass]
    public class SearchingByUpdatedDateTest
    {
        private const int daysMaxValue = 6000;
        private readonly DateTime deadline = new(2023, 6, 14);

        private IntervalDate _randomInterval;
        private SearchingByUpdatedDate<Article> _searchArticleByUpdatedDate;
        private SearchingByUpdatedDate<Mention> _searchCommentByUpdatedDate;
        private Mention _comment;
        private Article _article;

        [TestInitialize]
        public void SetUp()
        {
            _randomInterval = CreateRandomValidDayInterval(deadline);

            _searchArticleByUpdatedDate = new SearchingByUpdatedDate<Article>
            {
                MinDate = _randomInterval.MinDate,
                MaxDate = _randomInterval.MaxDate,
            };
            _searchCommentByUpdatedDate = new SearchingByUpdatedDate<Mention>
            {
                MinDate = _randomInterval.MinDate,
                MaxDate = _randomInterval.MaxDate,
            };

            _article = CreateSimpleArticle(new User(), 0);
            _comment = CreateSimpleComment(new User(), 0);
            _comment.Article = _article;
            
            _article.ValidOrFail();
            _comment.ValidOrFail();
        }

        [TestMethod]
        public void EmptySearchCriteriaAcceptsAny()
        {
            _searchCommentByUpdatedDate.MinDate = null;
            _searchArticleByUpdatedDate.MinDate = null;

            Assert.IsTrue(_searchCommentByUpdatedDate.Criteria(_comment));
            Assert.IsTrue(_searchArticleByUpdatedDate.Criteria(_article));
        }

        [TestMethod]
        public void EmptySearchCriteriaAcceptsAny2()
        {
            _searchCommentByUpdatedDate.MaxDate = null;
            _searchArticleByUpdatedDate.MaxDate = null;

            Assert.IsTrue(_searchCommentByUpdatedDate.Criteria(_comment));
            Assert.IsTrue(_searchArticleByUpdatedDate.Criteria(_article));
        }

        [TestMethod]
        public void EmptySearchCriteriaAcceptsAny3()
        {
            _searchCommentByUpdatedDate.MinDate = null;
            _searchCommentByUpdatedDate.MaxDate = null;

            _searchArticleByUpdatedDate.MinDate = null;
            _searchArticleByUpdatedDate.MaxDate = null;

            Assert.IsTrue(_searchCommentByUpdatedDate.Criteria(_comment));
            Assert.IsTrue(_searchArticleByUpdatedDate.Criteria(_article));
        }

        [TestMethod]
        public void SearchCriteriaAcceptsByUpdatedDateRange()
        {
            var intervalDays = _randomInterval.GetIntervalDays();
            var randomIntervalDays = new Random().Next(intervalDays);

            _comment.UpdatedAt = _randomInterval.MinDate.AddDays(randomIntervalDays);
            _article.UpdatedAt = _randomInterval.MinDate.AddDays(randomIntervalDays);

            Assert.IsTrue(_searchCommentByUpdatedDate.Criteria(_comment));
            Assert.IsTrue(_searchArticleByUpdatedDate.Criteria(_article));
        }

        [TestMethod]
        public void SearchCriteriaDeniesBecauseItDoesntMatchUpdatedDateRange1()
        {
            var randomIntervalDays = new Random().Next(1, daysMaxValue);

            _comment.UpdatedAt = _randomInterval.MaxDate.AddDays(randomIntervalDays);
            _article.UpdatedAt = _randomInterval.MaxDate.AddDays(randomIntervalDays);

            Assert.IsFalse(_searchCommentByUpdatedDate.Criteria(_comment));
            Assert.IsFalse(_searchArticleByUpdatedDate.Criteria(_article));
        }

        [TestMethod]
        public void SearchCriteriaDeniesBecauseItDoesntMatchUpdatedDateRange2()
        {
            var randomIntervalDays = new Random().Next(1, daysMaxValue);

            _comment.UpdatedAt = _randomInterval.MinDate.AddDays(-randomIntervalDays);
            _article.UpdatedAt = _randomInterval.MinDate.AddDays(-randomIntervalDays);

            Assert.IsFalse(_searchCommentByUpdatedDate.Criteria(_comment));
            Assert.IsFalse(_searchArticleByUpdatedDate.Criteria(_article));
        }

    }
}
