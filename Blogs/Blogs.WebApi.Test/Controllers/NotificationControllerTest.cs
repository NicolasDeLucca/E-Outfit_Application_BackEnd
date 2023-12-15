using static Blogs.Instances.DomainInstances;
using static Blogs.Domain.Functions;
using Blogs.Domain.BusinessEntities.Mentions;
using Blogs.Interfaces;
using Blogs.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Blogs.Domain.BusinessEntities;
using Blogs.Models.Out;

namespace Blogs.WebApi.Test.Controllers
{
    [TestClass]
    public class NotificationControllerTest
    {
        //los tests de notificaciones de articulos son análogos 
        private Mock<IManager<Notification<Mention>>> _mentionNotificationsManagerMock;
        private Mock<IManager<Notification<Article>>> _articleNotificationsManagerMock;
        private Mock<IManager<Session>> _sessionManagerMock;

        [TestInitialize]
        public void SetUp()
        {
            _mentionNotificationsManagerMock = new Mock<IManager<Notification<Mention>>>(MockBehavior.Strict);
            _articleNotificationsManagerMock = new Mock<IManager<Notification<Article>>>(MockBehavior.Strict);
            _sessionManagerMock = new Mock<IManager<Session>>(MockBehavior.Strict);
        }

        [TestMethod]
        public void GetAllUnReadMentionNotificationsOkTest()
        {
            var user = new User();
            var notification = CreateSimpleNotification<Mention>(user, "offensive mention", 0);
            notification.AssociatedContent = CreateSimpleComment(user, 0);
            IEnumerable<Notification<Mention>> retrievedNotifications = new List<Notification<Mention>>{notification};

            _mentionNotificationsManagerMock.Setup(m => m.GetAll()).Returns(retrievedNotifications);

            NotificationController notificationController = new(_mentionNotificationsManagerMock.Object, 
                _articleNotificationsManagerMock.Object, _sessionManagerMock.Object);
            IActionResult result = notificationController.GetAllUnReadMentions();

            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            OkObjectResult okResult = (OkObjectResult) result;
            var response = (List<MentionNotificationBasicInfo>) okResult.Value;

            _mentionNotificationsManagerMock.VerifyAll();

            var unReadNotifications = MentionNotificationsUnRead(retrievedNotifications);
            var mentionsNotificationBasicInfos = unReadNotifications.Select(n => new MentionNotificationBasicInfo(n)).ToList();
            
            Assert.IsTrue(okResult.StatusCode == StatusCodes.Status200OK);
            CollectionAssert.AreEquivalent(mentionsNotificationBasicInfos, response);
        }

        [TestMethod]
        public void DeleteMentionNotificationByIdNoContentTest()
        {
            var authUser = new User {Id = 0};
            authUser.MentionNotifications.Add(new(){Id = 0});
            var session = new Session {AuthenticatedUser = authUser};

            _sessionManagerMock.Setup(m => m.GetAllBy(It.IsAny<ISearchCriteria<Session>>())).Returns(new List<Session> {session});
            _mentionNotificationsManagerMock.Setup(m => m.Delete(It.IsAny<int>()));

            NotificationController notificationController = new(_mentionNotificationsManagerMock.Object,
                _articleNotificationsManagerMock.Object, _sessionManagerMock.Object);
            IActionResult result = notificationController.DeleteMentionNotification(session.AuthToken.ToString(), 0);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            NoContentResult noContentResult = (NoContentResult)result;

            _sessionManagerMock.VerifyAll();
            _mentionNotificationsManagerMock.VerifyAll();

            Assert.IsTrue(noContentResult.StatusCode == StatusCodes.Status204NoContent);
        }
    }
}
