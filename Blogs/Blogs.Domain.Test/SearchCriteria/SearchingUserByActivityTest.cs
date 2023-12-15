using Blogs.Domain.BusinessEntities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Blogs.Instances.DomainInstances;
using Blogs.Domain.SearchCriteria;

namespace Blogs.Domain.Test.SearchCriteria
{
    [TestClass]
    public class SearchingUserByActivityTest
    {
        private readonly DateTime deadline = new(2023, 6, 14);

        private IntervalDate _randomDateInterval;
        private SearchingUserByActivity _searchByActivity;
        private User _user;

        [TestInitialize]
        public void SetUp()
        {
            _randomDateInterval = CreateRandomValidDayInterval(deadline);
            _searchByActivity = new SearchingUserByActivity();

            _user = CreateSimpleUser(0);
            _user.PostedArticles.Add(CreateSimpleArticle(_user, 0));
            var newComment = CreateSimpleComment(_user, 0);
            _user.PostedMentions.Add(newComment);
            newComment.Article = _user.PostedArticles.ElementAt(0);
            _user.ValidOrFail();

            var retrievedMention = _user.PostedMentions.ElementAt(0);
            var retrievedArticle = _user.PostedArticles.ElementAt(0);
            retrievedMention.ValidOrFail();
            retrievedArticle.ValidOrFail();
        }

        [TestMethod]
        public void EmptySearchCriteriaAcceptsAnyUser()
        {
            Assert.IsTrue(_searchByActivity.Criteria(_user));
        }

        [TestMethod]
        public void SearchCriteriaAcceptsByCommentUserSearch()
        {
            _searchByActivity.MinDate = _randomDateInterval.MinDate;
            _searchByActivity.MaxDate = _randomDateInterval.MaxDate;

            _user.PostedArticles.Clear();
            var randomIntervalDays = new Random().Next(_randomDateInterval.GetIntervalDays());
            _user.PostedMentions.ElementAt(0).UpdatedAt = _randomDateInterval.MinDate.AddDays(randomIntervalDays);

            Assert.IsTrue(_searchByActivity.Criteria(_user));
        }

        [TestMethod]
        public void SearchCriteriaAcceptsByArticleUserSearch()
        {
            _searchByActivity.MinDate = _randomDateInterval.MinDate;
            _searchByActivity.MaxDate = _randomDateInterval.MaxDate;

            _user.PostedMentions.Clear();

            var randomIntervalDays = new Random().Next(_randomDateInterval.GetIntervalDays());
            var retrievedArticle = _user.PostedArticles.ElementAt(0);
            retrievedArticle.UpdatedAt = _randomDateInterval.MinDate.AddDays(randomIntervalDays);

            Assert.IsTrue(_searchByActivity.Criteria(_user));
        }

        [TestMethod]
        public void SearchCriteriaDeniesBecauseItDoesntMatchBothSearchs()
        {
            _searchByActivity.MinDate = _randomDateInterval.MinDate;
            _searchByActivity.MaxDate = _randomDateInterval.MaxDate;
            _user.PostedArticles.Clear();
            _user.PostedMentions.Clear();

            Assert.IsFalse(_searchByActivity.Criteria(_user));
        }
    }
}
