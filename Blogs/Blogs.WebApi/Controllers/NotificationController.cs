using static Blogs.Domain.Functions;
using Blogs.Domain.BusinessEntities.Mentions;
using Blogs.Interfaces;
using Blogs.WebApi.Filters.Authorization;
using Microsoft.AspNetCore.Mvc;
using Blogs.Domain.BusinessEntities;
using Microsoft.AspNetCore.Cors;
using Blogs.Models.Out;
using System.Net;

namespace Blogs.WebApi.Controllers
{
    [EnableCors("AllowEverything")]
    [ServiceFilter(typeof(AuthenticationFilter))]
    [Route("api/notifications")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IManager<Notification<Mention>> _mentionNotificationManager;
        private readonly IManager<Notification<Article>> _articleNotificationManager;
        private readonly IManager<Session> _sessionService;

        public NotificationController(IManager<Notification<Mention>> mentionNotificationManager,
             IManager<Notification<Article>> articleNotificationManager, IManager<Session> sessionService)
        {
            _mentionNotificationManager = mentionNotificationManager;
            _articleNotificationManager = articleNotificationManager;
            _sessionService = sessionService;
        }

        [HttpGet("mentions")]
        public IActionResult GetAllUnReadMentions()
        {
            var retrievedNotifications = _mentionNotificationManager.GetAll();
            var unReadNotifications = MentionNotificationsUnRead(retrievedNotifications);
            var notificationsBasicInfo = unReadNotifications.Select(n => new MentionNotificationBasicInfo(n)).ToList();

            return Ok(notificationsBasicInfo);
        }

        [HttpDelete("mentions/{id:int}")]
        public IActionResult DeleteMentionNotification([FromHeader] string Authorization, [FromRoute] int id)
        {
            Session currentSession = ControllerHelper.GetSession(_sessionService, Authorization);
            var authorUser = currentSession.AuthenticatedUser;

            var notificationToDelete = authorUser.MentionNotifications.ToList().Find(n => n.Id == id);
            if (notificationToDelete == null)
                return NotFound("Could not find specified notification");

            _mentionNotificationManager.Delete(id);
            return NoContent();
        }

        [HttpGet("articles")]
        public IActionResult GetAllUnReadArticles()
        {
            var retrievedNotifications = _articleNotificationManager.GetAll();
            var unReadNotifications = ArticleNotificationsUnRead(retrievedNotifications);
            var notificationsBasicInfo = unReadNotifications.Select(n => new ArticleNotificationBasicInfo(n)).ToList();

            return Ok(notificationsBasicInfo);
        }

        [HttpDelete("articles/{id:int}")]
        public IActionResult DeleteArticleNotification([FromHeader] string Authorization, [FromRoute] int id)
        {
            Session currentSession = ControllerHelper.GetSession(_sessionService, Authorization);
            var authorUser = currentSession.AuthenticatedUser;

            var notificationToDelete = authorUser.MentionNotifications.ToList().Find(n => n.Id == id);
            if (notificationToDelete == null)
                return NotFound("Could not find specified notification");

            _mentionNotificationManager.Delete(id);
            return NoContent();
        }
    }
}
