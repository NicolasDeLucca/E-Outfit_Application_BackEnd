using Blogs.Domain.BusinessEntities;
using static Blogs.Instances.DomainInstances;
using Blogs.Interfaces;
using Blogs.Models.Out;
using Blogs.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Blogs.Models.In;
using Blogs.Domain.BusinessEntities.Mentions;

namespace Blogs.WebApi.Test.Controllers
{
    [TestClass]
    public class OffenseControllerTest
    {
        private Mock<IManager<Offense>> _offenseManagerMock;
        private Mock<IManager<Article>> _articleManagerMock;
        private Mock<IManager<Mention>> _mentionManagerMock;

        [TestInitialize]
        public void SetUp()
        {
            _offenseManagerMock = new Mock<IManager<Offense>>(MockBehavior.Strict);
            _articleManagerMock = new Mock<IManager<Article>>(MockBehavior.Strict);
            _mentionManagerMock = new Mock<IManager<Mention>>(MockBehavior.Strict);
        }

        [TestMethod]
        public void GetAllOffensesOkTest()
        {
            List<Offense> retrievedOffenses = new() {CreateSimpleOffense(0)};
            List<OffenseDetail> offenseDetail = new();
            foreach (Offense of in retrievedOffenses) offenseDetail.Add(new OffenseDetail(of));

            _offenseManagerMock.Setup(m => m.GetAll()).Returns(retrievedOffenses);

            OffenseController offenseController = new(_offenseManagerMock.Object, _articleManagerMock.Object,
                _mentionManagerMock.Object);
            IActionResult result = offenseController.GetAll();

            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            OkObjectResult okResult = (OkObjectResult)result;

            var responseModel = (IEnumerable<OffenseDetail>) okResult.Value;

            _offenseManagerMock.VerifyAll();

            Assert.IsTrue(okResult.StatusCode == StatusCodes.Status200OK);
            CollectionAssert.AreEquivalent(offenseDetail, responseModel.ToList());
        }

        [TestMethod]
        public void PostOffenseCreatedTest()
        {
            var offenseModel = new OffenseModel() {Word = "Repugnante"};
            var offenseEntity = offenseModel.ToEntity();
            var offenseDetail = new OffenseDetail(offenseEntity);

            _offenseManagerMock.Setup(m => m.Create(It.IsAny<Offense>())).Returns(offenseEntity);
            _articleManagerMock.Setup(m => m.GetAll()).Returns(new List<Article>());
            _mentionManagerMock.Setup(m => m.GetAll()).Returns(new List<Mention>());

            OffenseController offenseController = new(_offenseManagerMock.Object, _articleManagerMock.Object,
                _mentionManagerMock.Object);
            IActionResult result = offenseController.Post(offenseModel);

            Assert.IsInstanceOfType(result, typeof(CreatedResult));
            CreatedResult createdResult = (CreatedResult)result;
            var responseModel = (OffenseDetail) createdResult.Value;

            _offenseManagerMock.VerifyAll();
            _articleManagerMock.VerifyAll();
            _mentionManagerMock.VerifyAll();

            Assert.IsTrue(createdResult.StatusCode == StatusCodes.Status201Created);
            Assert.IsTrue(offenseDetail.Equals(responseModel));
        }

        [TestMethod]
        public void PutOffenseByIdNoContentTest()
        {
            var offenseModel = new OffenseModel() {Word = "Repugnante"};

            _offenseManagerMock.Setup(m => m.Update(It.IsAny<Offense>()));
            _articleManagerMock.Setup(m => m.GetAll()).Returns(new List<Article>());
            _mentionManagerMock.Setup(m => m.GetAll()).Returns(new List<Mention>());

            OffenseController offenseController = new(_offenseManagerMock.Object, _articleManagerMock.Object,
                _mentionManagerMock.Object);
            IActionResult result = offenseController.Put(0, offenseModel);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            NoContentResult noContentResult = (NoContentResult)result;

            _offenseManagerMock.VerifyAll();
            _articleManagerMock.VerifyAll();
            _mentionManagerMock.VerifyAll();

            Assert.IsTrue(noContentResult.StatusCode == StatusCodes.Status204NoContent);
        }

        [TestMethod]
        public void DeleteOffenseByIdNoContentTest()
        {
            _offenseManagerMock.Setup(m => m.Delete(It.IsAny<int>()));

            OffenseController offenseController = new(_offenseManagerMock.Object, _articleManagerMock.Object,
                _mentionManagerMock.Object);
            IActionResult result = offenseController.Delete(0);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            NoContentResult noContentResult = (NoContentResult)result;

            _offenseManagerMock.VerifyAll();
            Assert.IsTrue(noContentResult.StatusCode == StatusCodes.Status204NoContent);
        }


    }
}
