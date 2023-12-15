using Blogs.Domain;
using Blogs.Domain.BusinessEntities;
using Blogs.Interfaces;

namespace Blogs.BusinessLogic.ResourceManagers
{
    public class ArticleManager : IManager<Article>
    {
        private readonly IRepository<Article> _articleRepository;
        private readonly IRepository<Offense> _offenseRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Notification<Article>> _articleNotificationRepository;

        public ArticleManager(IRepository<Article> articleRepository, IRepository<Offense> offenseRepository,
            IRepository<Notification<Article>> articleNotificationRepository, IRepository<User> userRepository)
        {
            _articleRepository = articleRepository;
            _offenseRepository = offenseRepository;
            _articleNotificationRepository = articleNotificationRepository;
            _userRepository = userRepository;
        }

        public IEnumerable<Article> GetAll()
        {
            return _articleRepository.GetAllByCondition(a => true);
        }

        public IEnumerable<Article> GetAllBy(ISearchCriteria<Article> searchCriteria)
        {
            return _articleRepository.GetAllByCondition(searchCriteria.Criteria);
        }

        public Article GetById(int id)
        {
            return _articleRepository.GetById(id);
        }

        public Article Create(Article article)
        {
            if (article == null)
                throw new ArgumentException();

            article.ValidOrFail();
            if (!ApproveContent(article)) article.State = EntityState.Detached;
            var articleId = _articleRepository.Store(article);

            if (article.State == EntityState.Detached)
            {
                GenerateOffenseNotification(article);
                GenerateOffenseNotificationForAdmin(article);
            }

            var createdArticle = _articleRepository.GetById(articleId);
            return createdArticle;
        }

        public void Update(Article article)
        {
            var existsAssert = _articleRepository.GetById(article.Id);
            if (ApproveContent(article)) article.State = EntityState.Modified;
            else
            {
                article.State = EntityState.Detached;
                GenerateOffenseNotification(article);
                GenerateOffenseNotificationForAdmin(article);
            }
            article.UpdatedAt = DateTime.Now;
            _articleRepository.Update(article);
        }

        public void Delete(int id)
        {
            var articleToDelete = _articleRepository.GetById(id);
            _articleRepository.Delete(articleToDelete);
        }

        private bool ApproveContent(Article article)
        {
            var offensiveWords = _offenseRepository.GetAllByCondition(o => true).Select
                (offense => offense.Word).ToList();
            return !article.IsOffensive(offensiveWords);
        }

        private void GenerateOffenseNotification(Article article)
        {
            Notification<Article> newNotification = new()
            {
                Receiver = article.Author,
                Text = "Your article has offensive words, so it has been detached",
                AssociatedContent = article
            };
            _articleNotificationRepository.Store(newNotification);

            article.Author.ArticleNotifications.Add(newNotification); //
            _userRepository.Update(article.Author);
        }

        private void GenerateOffenseNotificationForAdmin(Article article)
        {
            foreach (var user in _userRepository.GetAllByCondition(u => u.Role == UserRole.Admin || u.Role == UserRole.AdminBlogger))
            {
                Notification<Article> newNotification = new()
                {
                    Receiver = user,
                    Text = "This article has offensive words, so it has been detached",
                    AssociatedContent = article
                };
                _articleNotificationRepository.Store(newNotification);

                user.ArticleNotifications.Add(newNotification); //
                _userRepository.Update(user);
            }
        }
    }
}
