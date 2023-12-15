using Blogs.Exceptions;
using Blogs.WebApi.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Security.Authentication;

namespace Blogs.WebApi.Test.Filters
{
    [TestClass]
    public class ExceptionFilterTest
    {
        private ExceptionFilter _exceptionFilter;
        private Mock<ExceptionContext> _exceptionContextMock;

        [TestInitialize]
        public void SetUp()
        {
            _exceptionFilter = new ExceptionFilter();
            ActionContext actionContext = new ActionContext()
            {
                HttpContext = new DefaultHttpContext(),
                RouteData = new RouteData(),
                ActionDescriptor = new ActionDescriptor()
            };
            _exceptionContextMock = new Mock<ExceptionContext>(actionContext, new List<IFilterMetadata>());
        }

        [TestMethod]
        public void InvalidRequestDataReturnsErrorCode400()
        {
            string excMessage = "Invalid request";
            _exceptionContextMock.SetupAllProperties();
            _exceptionContextMock.Setup(c => c.Exception).Returns(new InvalidRequestDataException(excMessage));

            _exceptionFilter.OnException(_exceptionContextMock.Object);
            var result = (JsonResult) _exceptionContextMock.Object.Result;
            string expectedValue = result.Value.ToString();

            Assert.IsTrue(result.StatusCode == StatusCodes.Status400BadRequest);
            Assert.IsTrue(excMessage.Equals(expectedValue));
        }

        [TestMethod]
        public void InvalidCredentialReturnsErrorCode401()
        {
            string excMessage = "Invalid credentials";
            _exceptionContextMock.SetupAllProperties();
            _exceptionContextMock.Setup(c => c.Exception).Returns(new InvalidCredentialException(excMessage));

            _exceptionFilter.OnException(_exceptionContextMock.Object);
            var result = (JsonResult)_exceptionContextMock.Object.Result;
            string expectedValue = result.Value.ToString();

            Assert.IsTrue(result.StatusCode == StatusCodes.Status401Unauthorized);
            Assert.IsTrue(excMessage.Equals(expectedValue));
        }

        [TestMethod]
        public void ResourceNotFoundReturnsErrorCode404()
        {
            string excMessage = "Couldnt find any resource";
            _exceptionContextMock.SetupAllProperties();
            _exceptionContextMock.Setup(c => c.Exception).Returns(new ResourceNotFoundException(excMessage));

            _exceptionFilter.OnException(_exceptionContextMock.Object);
            var result = (JsonResult) _exceptionContextMock.Object.Result;
            string expectedValue = result.Value.ToString();

            Assert.IsTrue(result.StatusCode == StatusCodes.Status404NotFound);
            Assert.IsTrue(excMessage.Equals(expectedValue));
        }

        [TestMethod]
        public void InvalidOperationReturnsErrorCode409()
        {
            string excMessage = "Invalid operation";
            _exceptionContextMock.SetupAllProperties();
            _exceptionContextMock.Setup(c => c.Exception).Returns(new InvalidOperationException(excMessage));

            _exceptionFilter.OnException(_exceptionContextMock.Object);
            var result = (JsonResult) _exceptionContextMock.Object.Result;
            string expectedValue = result.Value.ToString();

            Assert.IsTrue(result.StatusCode == StatusCodes.Status409Conflict);
            Assert.IsTrue(excMessage.Equals(expectedValue));
        }

        [TestMethod]
        public void BaseExceptionReturnsErrorCode500()
        {
            string excMessage = "Issues encountered, try again later";
            _exceptionContextMock.SetupAllProperties();
            _exceptionContextMock.Setup(c => c.Exception).Returns(new Exception());

            _exceptionFilter.OnException(_exceptionContextMock.Object);
            var result = (JsonResult) _exceptionContextMock.Object.Result;
            string expectedValue = result.Value.ToString();

            Assert.IsTrue(result.StatusCode == StatusCodes.Status500InternalServerError);
            Assert.IsTrue(excMessage.Equals(expectedValue));
        }
    }
}
