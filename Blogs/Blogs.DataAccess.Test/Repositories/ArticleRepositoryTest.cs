using Blogs.DataAccess.Contexts;
using Blogs.DataAccess.Repositories;
using Blogs.Domain.BusinessEntities;
using static Blogs.Instances.DomainInstances;
using Blogs.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Blogs.Exceptions;
using Blogs.Domain;
using Blogs.Domain.BusinessEntities.Mentions;
using EntityState = Microsoft.EntityFrameworkCore.EntityState;

namespace Blogs.DataAccess.Test.Repositories
{
    [TestClass]
    public class ArticleRepositoryTest
    {
        private Article _defaultArticle;

        private IRepository<Article> _repository;
        private IRepository<User> _userRepository;
        private IRepository<Mention> _mentionRepository;

        private DataContext _dataContext;

        [TestInitialize]
        public void SetUp()
        {
            _dataContext = ContextFactory.GetNewContext(ContextType.SqLite);
            _repository = new ArticleRepository(_dataContext);
            _userRepository = new UserRepository(_dataContext);
            _mentionRepository = new MentionRepository(_dataContext);

            _dataContext.Database.OpenConnection();
            _dataContext.Database.EnsureCreated();

            SetEnvironmentEntities();
        }

        [TestMethod]
        public void GetAllArticlesOnEmptyRepository()
        {
            var retrievedUser = _userRepository.GetById(1);
            _userRepository.Delete(retrievedUser);
     
            List<Article> expectedArticles = new();
            var retrievedArticles = _repository.GetAllByCondition(a => true);
            CollectionAssert.AreEquivalent(expectedArticles, retrievedArticles.ToList());
        }

        [TestMethod]
        public void GetAllArticlesReturnsAsExpected()
        {
            SetSecondRoundOfUserRelatedEntities();
            var newArticle = _repository.GetById(2);

            List<Article> expectedArticles = new() {_defaultArticle, newArticle};
            var retrievedArticles = _repository.GetAllByCondition(a => true);
            CollectionAssert.AreEquivalent(expectedArticles, retrievedArticles.ToList());
        }

        [TestMethod]
        public void GetAllArticlesIncludesRelatedEntities()
        {
            SetSecondRoundOfUserRelatedEntities();

            var retrievedArticles = _repository.GetAllByCondition(a => true);
            Assert.IsTrue(retrievedArticles.All(a => a.Images != null && a.Images.Count > 0));
            Assert.IsTrue(retrievedArticles.All(a => a.Mentions != null && a.Mentions.Count > 0));
        }

        [TestMethod]
        public void GetArticleByIdReturnsAsExpected()
        {
            var retrievedArticle = _repository.GetById(_defaultArticle.Id);
            Assert.IsTrue(_defaultArticle.Equals(retrievedArticle));
        }

        [TestMethod]
        [ExpectedException(typeof(ResourceNotFoundException))]
        public void GetArticleByIdDoesntExist()
        {
            _repository.GetById(-1);
        }

        [TestMethod]
        public void StoreArticleReturnsAsExpected()
        {
            Assert.IsTrue(_repository.GetAllByCondition(a => true).Count() == 1);
        }

        [TestMethod]
        public void UpdateArticleDoesAsExpected()
        {
            var newVisibility = Visibility.Public;
            _defaultArticle.Visibility = newVisibility;
            _repository.Update(_defaultArticle);

            var updatedArticle = _repository.GetById(_defaultArticle.Id);
            Assert.AreEqual(_defaultArticle, updatedArticle);
            Assert.IsTrue(updatedArticle.Visibility == newVisibility);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void StoreArticleWithSameAlternativeKeyFails()
        {
            var newUser = _userRepository.GetById(1);
            CreateAndStoreChargedArticle(newUser, 2, false);
        }

        [TestMethod]
        public void DeleteArticleDoesAsExpected()
        {
            _repository.Delete(_defaultArticle);
            Assert.IsTrue(_repository.GetAllByCondition(a => true).Count() == 0);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _dataContext.Database.EnsureDeleted();
        }

        #region Helpers

        private void SetEnvironmentEntities()
        {
            var newUser = CreateSimpleUser(1);
            var newUserId = _userRepository.Store(newUser);

            var retrievedUser = _userRepository.GetById(newUserId);
            CreateAndStoreChargedArticle(retrievedUser, 1, false);
            _defaultArticle = _repository.GetById(1);
        }

        private void SetSecondRoundOfUserRelatedEntities()
        {
            var retrievedUser = _userRepository.GetById(1);
            CreateAndStoreChargedArticle(retrievedUser, 2, true);
        }

        private void CreateAndStoreChargedArticle(User author, int id, bool changeAKs)
        {
            var newArticle = CreateSimpleArticle(author, id);
            if (changeAKs) newArticle = UpdateArticleAlternativeKey(newArticle);
            var newImage = CreateSimpleImage(id);
            newArticle.Images.Add(newImage);
            _dataContext.Entry(author).State = EntityState.Unchanged;
            _repository.Store(newArticle);

            var retrievedAuthor = _userRepository.GetById(author.Id);
            Mention newMention = CreateSimpleComment(retrievedAuthor, id);
            newMention.Article = retrievedAuthor.PostedArticles.First();
            _dataContext.Entry(retrievedAuthor).State = EntityState.Unchanged;
            _mentionRepository.Store(newMention);
        }

        #endregion
    }
}
