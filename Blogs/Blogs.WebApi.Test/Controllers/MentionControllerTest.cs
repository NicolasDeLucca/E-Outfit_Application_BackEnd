using Blogs.Domain.BusinessEntities.Mentions;
using Blogs.Interfaces;
using static Blogs.Instances.DomainInstances;
using static Blogs.Instances.ModelInstances;
using Blogs.Models.Out;
using Blogs.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Blogs.Domain.BusinessEntities;
using Blogs.Models.In;

namespace Blogs.WebApi.Test.Controllers
{
    [TestClass]
    public class MentionControllerTest
    {
        private Mock<IManager<Mention>> _mentionManagerMock;
        private Mock<IManager<Session>> _sessionServiceMock;

        [TestInitialize]
        public void SetUp()
        {
            _mentionManagerMock = new Mock<IManager<Mention>>(MockBehavior.Strict);
            _sessionServiceMock = new Mock<IManager<Session>>(MockBehavior.Strict);
        }

        [TestMethod]
        public void GetAllMentionsOkTest()
        {
            var newComment = CreateSimpleComment(new User(), 0);
            newComment.Article = CreateSimpleArticle(new User(), 0);
            List<Mention> retrievedMentions = new() {newComment};
            List<MentionBasicInfo> mentionsBasicInfo = new();
            foreach (var m in retrievedMentions) mentionsBasicInfo.Add(new MentionBasicInfo(m));

            _mentionManagerMock.Setup(m => m.GetAll()).Returns(retrievedMentions);
            MentionController mentionController = new(_mentionManagerMock.Object, _sessionServiceMock.Object);
            IActionResult result = mentionController.GetAll();

            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            OkObjectResult okResult = (OkObjectResult)result;
            var responseModel = (IEnumerable<MentionBasicInfo>)okResult.Value;

            _mentionManagerMock.VerifyAll();

            Assert.IsTrue(okResult.StatusCode == StatusCodes.Status200OK);
            CollectionAssert.AreEquivalent(mentionsBasicInfo, responseModel.ToList());
        }

        [TestMethod]
        public void GetMentionByIdOkTest()
        {
            Mention retrievedMention = CreateSimpleComment(new User(), 0);
            retrievedMention.Article = CreateSimpleArticle(new User(), 0);

            _mentionManagerMock.Setup(m => m.GetById(It.IsAny<int>())).Returns(retrievedMention);
            
            MentionController mentionController = new(_mentionManagerMock.Object, _sessionServiceMock.Object);
            IActionResult result = mentionController.GetById(0);

            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            OkObjectResult okResult = (OkObjectResult)result;
            var responseModel = (MentionDetail) okResult.Value;

            _mentionManagerMock.VerifyAll();

            Assert.IsTrue(okResult.StatusCode == StatusCodes.Status200OK);
            Assert.IsTrue(new MentionDetail(retrievedMention).Equals(responseModel));
        }

        [TestMethod]
        public void PostMentionCreatedTest()
        {
            var user = new User();
            var userArticle = CreateSimpleArticle(user, 0);
            user.PostedArticles.Add(userArticle);
            MentionModel model = CreateCommentModel(user, userArticle);
            Mention retrievedMention = model.ToEntity();
            Session currentSession = new() {AuthenticatedUser = user};
            IEnumerable<Session> sessions = new List<Session> {currentSession};

            _mentionManagerMock.Setup(m => m.Create(It.IsAny<Mention>())).Returns(retrievedMention);
            _sessionServiceMock.Setup(s => s.GetAllBy(It.IsAny<ISearchCriteria<Session>>())).Returns(sessions);

            MentionController mentionController = new(_mentionManagerMock.Object, _sessionServiceMock.Object);
            IActionResult result = mentionController.Post(currentSession.AuthToken.ToString(), model);

            Assert.IsInstanceOfType(result, typeof(CreatedResult));
            CreatedResult createdResult = (CreatedResult)result;
            var responseModel = (MentionDetail) createdResult.Value;

            _mentionManagerMock.VerifyAll();
            _sessionServiceMock.VerifyAll();

            Assert.IsTrue(createdResult.StatusCode == StatusCodes.Status201Created);
            Assert.IsTrue(new MentionDetail(retrievedMention).Equals(responseModel));
        }

    }
}
