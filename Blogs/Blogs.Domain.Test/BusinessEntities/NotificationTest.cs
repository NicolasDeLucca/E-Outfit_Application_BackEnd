using Blogs.Domain.BusinessEntities;
using Blogs.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blogs.Domain.Test.BusinessEntities
{
    [TestClass]
    public class NotificationTest
    {
        private Notification<object?> _validNotification;

        [TestInitialize]
        public void SetUp()
        {
            _validNotification = new Notification<object?>()
            {
                Id = 1,
                Text = "...",
                AssociatedContent = new object(),
                Receiver = new User()
            };
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRequestDataException))]
        public void NotificationWithNoTypeAssociatedFailsValidation()
        {
            _validNotification.AssociatedContent = null;
            _validNotification.ValidOrFail();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRequestDataException))]
        public void NotificationWithNoReceiverFailsValidation()
        {
            _validNotification.Receiver = null;
            _validNotification.ValidOrFail();
        }

        [TestMethod]
        public void ValidNotificationPassesValidation()
        {
            _validNotification.ValidOrFail();
        }

    }
}
