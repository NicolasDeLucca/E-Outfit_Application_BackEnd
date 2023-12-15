using Blogs.Domain;
using Blogs.Domain.BusinessEntities;
using Blogs.Domain.BusinessEntities.Mentions;
using Blogs.Interfaces;

namespace Blogs.BusinessLogic.ResourceManagers
{
    public class MentionManager : IManager<Mention>
    {
        private readonly IRepository<Mention> _mentionRepository;
        private readonly IRepository<Offense> _offenseRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Notification<Mention>> _mentionNotificationRepository;

        public MentionManager(IRepository<Mention> mentionRepository, IRepository<Offense> offenseRepository,
            IRepository<Notification<Mention>> mentionNotificationRepository, IRepository<User> userRepository)
        {   
            _mentionRepository = mentionRepository;
            _offenseRepository = offenseRepository;
            _mentionNotificationRepository = mentionNotificationRepository;
            _userRepository = userRepository;
        }

        public IEnumerable<Mention> GetAll()
        {
            return _mentionRepository.GetAllByCondition(m => true);
        }

        public IEnumerable<Mention> GetAllBy(ISearchCriteria<Mention> searchCriteria)
        {
            return _mentionRepository.GetAllByCondition(searchCriteria.Criteria);
        }

        public Mention GetById(int id)
        {
            return _mentionRepository.GetById(id);
        }

        public Mention Create(Mention mention)
        {
            if (mention == null || mention.Article == null)
                throw new ArgumentException();

            mention.ValidOrFail();
            if (!ApproveContent(mention)) mention.State = EntityState.Detached;
            var mentionId = _mentionRepository.Store(mention);

            if (mention.State == EntityState.Detached)
            {
                GenerateOffenseNotification(mention);
                GenerateOffenseNotificationForAdmin(mention);
            }
            if (mention is Comment com) GenerateCreatedNotification(com);
            var createdMention = _mentionRepository.GetById(mentionId);
            return createdMention;
        }

        public void Update(Mention mention)
        {
            var retrievedMention = _mentionRepository.GetById(mention.Id);
            if (!ApproveContent(retrievedMention) && ApproveContent(mention))
            {
                mention.State = EntityState.Modified;
                mention.UpdatedAt = DateTime.Now;
                _mentionRepository.Update(mention);
            }
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        private bool ApproveContent(Mention mention)
        {
            var offensiveWords = _offenseRepository.GetAllByCondition(o => true).Select
                (offense => offense.Word).ToList();
            return !mention.IsOffensive(offensiveWords);
        }

        private void GenerateOffenseNotification(Mention mention)
        {
            Notification<Mention> newNotification = new()
            {
                Receiver = mention.Author,
                Text = "Your mention has offensive words, so it has been detached",
                AssociatedContent = mention
            };
            _mentionNotificationRepository.Store(newNotification);

            mention.Author.MentionNotifications.Add(newNotification); //
            _userRepository.Update(mention.Author);
        }

        private void GenerateOffenseNotificationForAdmin(Mention mention)
        {
            foreach (var user in _userRepository.GetAllByCondition(u => u.Role == UserRole.Admin || u.Role == UserRole.AdminBlogger))
            {
                Notification<Mention> newNotification = new()
                {
                    Receiver = user,
                    Text = "This mention has offensive words, so it has been detached",
                    AssociatedContent = mention
                };
                _mentionNotificationRepository.Store(newNotification);

                user.MentionNotifications.Add(newNotification); //
                _userRepository.Update(user);
            }
        }

        private void GenerateCreatedNotification(Mention mention)
        {
            Notification<Mention> newNotification = new()
            {
                Receiver = mention.Article.Author,
                Text = "Your article has a new comment",
                AssociatedContent = mention
            };
            _mentionNotificationRepository.Store(newNotification);

            mention.Article.Author.MentionNotifications.Add(newNotification); //
            _userRepository.Update(mention.Article.Author);
        }
    }
}
