using Blogs.Domain.BusinessEntities;
using static Blogs.Instances.DomainInstances;
using Blogs.Domain.SearchCriteria;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blogs.Domain.Test.SearchCriteria
{
    [TestClass]
    public class SearchingUserByOffensiveActivityTest
    {
        private readonly DateTime deadline = new(2023, 6, 14);

        private IntervalDate _randomDateInterval;
        private SearchingUserByOffensiveActivity _searchByOffensiveActivity;
        private User _user;

        [TestInitialize]
        public void SetUp()
        {
            _randomDateInterval = CreateRandomValidDayInterval(deadline);
            _searchByOffensiveActivity = new SearchingUserByOffensiveActivity();

            _user = CreateSimpleUser(0);

            var newArticle = CreateSimpleArticle(_user, 0);
            newArticle.Text += ".. what a disgusting issue!";
            _user.PostedArticles.Add(newArticle);
            var newComment = CreateSimpleComment(_user, 0);
            newComment.Text += ".. what a disgusting article!";
            _user.PostedMentions.Add(newComment);
            newComment.Article = _user.PostedArticles.ElementAt(0);
            _user.ValidOrFail();

            var retrievedMention = _user.PostedMentions.ElementAt(0);
            var retrievedArticle = _user.PostedArticles.ElementAt(0);
            retrievedMention.ValidOrFail();
            retrievedArticle.ValidOrFail();
        }

        [TestMethod]
        public void EmptySearchCriteriaDeniesBecauseOfAnyInOffensiveUserActivity()
        {
            Assert.IsFalse(_searchByOffensiveActivity.Criteria(_user));
        }

        [TestMethod]
        public void SearchCriteriaWithOutDatesAcceptsByAnyOffensiveUser()
        {
            _searchByOffensiveActivity.OffensiveWords = new List<string>{"disgusting"};
            Assert.IsTrue(_searchByOffensiveActivity.Criteria(_user));
        }

        [TestMethod]
        public void SearchCriteriaDeniesBecauseOfApprovalCommentUserSearch()
        {
            _searchByOffensiveActivity.MinDate = _randomDateInterval.MinDate;
            _searchByOffensiveActivity.MaxDate = _randomDateInterval.MaxDate;

            _user.PostedArticles.Clear();
            var randomIntervalDays = new Random().Next(_randomDateInterval.GetIntervalDays());
            _user.PostedMentions.ElementAt(0).UpdatedAt = _randomDateInterval.MinDate.AddDays(randomIntervalDays);

            Assert.IsFalse(_searchByOffensiveActivity.Criteria(_user));
        }

        [TestMethod]
        public void SearchCriteriaAcceptsByOffensiveCommentUserSearch()
        {
            _searchByOffensiveActivity.OffensiveWords = new List<string>{"disgusting"};
            _searchByOffensiveActivity.MinDate = _randomDateInterval.MinDate;
            _searchByOffensiveActivity.MaxDate = _randomDateInterval.MaxDate;

            _user.PostedArticles.Clear();
            var randomIntervalDays = new Random().Next(_randomDateInterval.GetIntervalDays());
            _user.PostedMentions.ElementAt(0).UpdatedAt = _randomDateInterval.MinDate.AddDays(randomIntervalDays);

            Assert.IsTrue(_searchByOffensiveActivity.Criteria(_user));
        }

        [TestMethod]
        public void SearchCriteriaDeniesBecauseOfApprovalArticleUserSearch()
        {
            _searchByOffensiveActivity.MinDate = _randomDateInterval.MinDate;
            _searchByOffensiveActivity.MaxDate = _randomDateInterval.MaxDate;

            _user.PostedMentions.Clear();

            var randomIntervalDays = new Random().Next(_randomDateInterval.GetIntervalDays());
            var retrievedArticle = _user.PostedArticles.ElementAt(0);
            retrievedArticle.UpdatedAt = _randomDateInterval.MinDate.AddDays(randomIntervalDays);

            Assert.IsFalse(_searchByOffensiveActivity.Criteria(_user));
        }

        [TestMethod]
        public void SearchCriteriaAcceptsByOffensiveArticleUserSearch()
        {
            _searchByOffensiveActivity.OffensiveWords = new List<string>{"disgusting"};
            _searchByOffensiveActivity.MinDate = _randomDateInterval.MinDate;
            _searchByOffensiveActivity.MaxDate = _randomDateInterval.MaxDate;

            _user.PostedMentions.Clear();

            var randomIntervalDays = new Random().Next(_randomDateInterval.GetIntervalDays());
            var retrievedArticle = _user.PostedArticles.ElementAt(0);
            retrievedArticle.UpdatedAt = _randomDateInterval.MinDate.AddDays(randomIntervalDays);

            Assert.IsTrue(_searchByOffensiveActivity.Criteria(_user));
        }

        [TestMethod]
        public void SearchCriteriaDeniesBecauseAnySearchIsOffensive()
        {
            _searchByOffensiveActivity.MinDate = _randomDateInterval.MinDate;
            _searchByOffensiveActivity.MaxDate = _randomDateInterval.MaxDate;

            Assert.IsFalse(_searchByOffensiveActivity.Criteria(_user));
        }

        [TestMethod]
        public void SearchCriteriaDeniesBecauseItDoesntMatchBothSearchs()
        {
            _searchByOffensiveActivity.MinDate = _randomDateInterval.MinDate;
            _searchByOffensiveActivity.MaxDate = _randomDateInterval.MaxDate;
            _user.PostedArticles.Clear();
            _user.PostedMentions.Clear();

            Assert.IsFalse(_searchByOffensiveActivity.Criteria(_user));
        }
    }
}
