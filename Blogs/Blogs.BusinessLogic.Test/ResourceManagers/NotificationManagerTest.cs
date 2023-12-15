using Blogs.BusinessLogic.ResourceManagers;
using Blogs.Domain.BusinessEntities.Mentions;
using Blogs.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections;
using Blogs.Domain.BusinessEntities;
using Blogs.Exceptions;

namespace Blogs.BusinessLogic.Test.ResourceManagers
{
    [TestClass]
    public class NotificationManagerTest
    {
        //test de notificaciones sobre menciones, sobre articulos es análogo
        private Mock<IRepository<Notification<Mention>>> _repositoryMock;

        [TestInitialize]
        public void Setup()
        {
            _repositoryMock = new Mock<IRepository<Notification<Mention>>>(MockBehavior.Strict);
        }

        [TestMethod]
        public void GetNotificationsReturnsAsExpected()
        {
            var notifications = It.IsAny<List<Notification<Mention>>>();
            _repositoryMock.Setup(r => r.GetAllByCondition(It.IsAny<Func<Notification<Mention>, bool>>())).
                Returns(notifications);

            NotificationManager<Mention> notificationManager = new(_repositoryMock.Object);
            var retrievedNotifications = notificationManager.GetAll();

            _repositoryMock.VerifyAll();
            CollectionAssert.AreEquivalent((ICollection)retrievedNotifications, notifications);
        }

        [TestMethod]
        public void DeleteNotificationDoesAsExpected()
        {
            Notification<Mention> expectedNotification = new(){Id = 0, Receiver = new User()};

            _repositoryMock.Setup(r => r.GetById(0)).Returns(expectedNotification);
            _repositoryMock.Setup(r => r.Delete(expectedNotification));

            NotificationManager<Mention> notificationManager = new(_repositoryMock.Object);
            notificationManager.Delete(0);

            _repositoryMock.VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(ResourceNotFoundException))]
        public void DeleteNonExistentNotificationFails()
        {
            _repositoryMock.Setup(r => r.GetById(It.IsAny<int>())).Throws(new ResourceNotFoundException("Could not find specified notification"));

            NotificationManager<Mention> notificationManager = new(_repositoryMock.Object);
            notificationManager.Delete(0);

            _repositoryMock.VerifyAll();
        }
    }
}
