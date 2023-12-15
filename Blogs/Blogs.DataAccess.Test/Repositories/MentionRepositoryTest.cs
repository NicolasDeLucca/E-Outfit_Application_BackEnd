using Blogs.DataAccess.Contexts;
using Blogs.DataAccess.Repositories;
using Blogs.Domain.BusinessEntities;
using Blogs.Domain.BusinessEntities.Mentions;
using Blogs.Exceptions;
using Blogs.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Blogs.Instances.DomainInstances;

namespace Blogs.DataAccess.Test.Repositories
{
    [TestClass]
    public class MentionRepositoryTest
    {
        private Mention _defaultMention;

        private IRepository<Mention> _repository;
        private IRepository<Article> _articleRepository;
        private IRepository<User> _userRepository;

        private DataContext _dataContext;

        [TestInitialize]
        public void SetUp()
        {
            _dataContext = ContextFactory.GetNewContext(ContextType.SqLite);
            _repository = new MentionRepository(_dataContext);
            _userRepository = new UserRepository(_dataContext);
            _articleRepository = new ArticleRepository(_dataContext);

            _dataContext.Database.OpenConnection();
            _dataContext.Database.EnsureCreated();

            SetEnvironmentEntities();
        }

        [TestMethod]
        public void GetAllMentionsOnEmptyRepository()
        {
            var retrievedUser = _userRepository.GetById(1);
            _userRepository.Delete(retrievedUser);

            retrievedUser = _userRepository.GetById(2);
            _userRepository.Delete(retrievedUser);

            List<Mention> expectedMentions = new();
            var retrievedMentions = _repository.GetAllByCondition(m => true);
            CollectionAssert.AreEquivalent(expectedMentions, retrievedMentions.ToList());
        }

        [TestMethod]
        public void GetAllMentionsReturnsAsExpected()
        {
            var retrievedUser = _userRepository.GetById(1);
            var newArticle = CreateSimpleArticle(retrievedUser, 2);
            newArticle = UpdateArticleAlternativeKey(newArticle);
            _dataContext.Entry(retrievedUser).State = EntityState.Unchanged;
            var newArticleId = _articleRepository.Store(newArticle);

            retrievedUser = _userRepository.GetById(retrievedUser.Id);
            var newOwnMention = CreateSimpleComment(retrievedUser, 2);
            newOwnMention.Article = retrievedUser.PostedArticles.First();
            _dataContext.Entry(retrievedUser).State = EntityState.Unchanged;
            var newOwnMentionId = _repository.Store(newOwnMention);

            var retrievedMention = _repository.GetById(newOwnMentionId);
            List<Mention> expectedMentions = new() {_defaultMention, retrievedMention};
            var retrievedMentions = _repository.GetAllByCondition(m => true);
            CollectionAssert.AreEquivalent(expectedMentions, retrievedMentions.ToList());
        }

        [TestMethod]
        public void GetMentionByIdReturnsAsExpected()
        {
            var retrievedMention = _repository.GetById(_defaultMention.Id);
            Assert.IsTrue(_defaultMention.Equals(retrievedMention));
        }

        [TestMethod]
        [ExpectedException(typeof(ResourceNotFoundException))]
        public void GetMentionByIdDoesntExist()
        {
            _repository.GetById(-1);
        }

        [TestMethod]
        public void StoreMentionReturnsAsExpected()
        {
            Assert.IsTrue(_repository.GetAllByCondition(m => true).Count() == 1);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _dataContext.Database.EnsureDeleted();
        }

        #region SetUpHelper

        private void SetEnvironmentEntities()
        {
            var newUser = CreateSimpleUser(1);
            var newUserId = _userRepository.Store(newUser);

            var otherUser = CreateSimpleUser(2);
            otherUser = UpdateUserAlternativeKeys(otherUser);
            var otherUserId = _userRepository.Store(otherUser);

            var retrievedArticleBlogger = _userRepository.GetById(newUserId);
            var newArticle = CreateSimpleArticle(retrievedArticleBlogger, 1);
            _dataContext.Entry(retrievedArticleBlogger).State = EntityState.Unchanged;
            var newArticleId = _articleRepository.Store(newArticle);

            var retrievedCommentator = _userRepository.GetById(otherUserId);
            var retrievedArticle = _articleRepository.GetById(newArticleId);

            _defaultMention = CreateSimpleComment(retrievedCommentator, 1);
            _defaultMention.Article = retrievedArticle;
            _dataContext.Entry(retrievedCommentator).State = EntityState.Unchanged;
            _repository.Store(_defaultMention);
        }

        #endregion
    }
}