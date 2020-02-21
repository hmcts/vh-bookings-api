using System;
using NUnit.Framework;
using Bookings.API.Extensions;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Threading.Tasks;
using System.Net;
using Bookings.Common;
using FluentAssertions;

namespace Bookings.UnitTests.Middleware
{
    [TestFixture]
    public class ExceptionMiddlewareTests
    {

        public Mock<IDelegateMock> RequestDelegateMock { get; set; }
        public ExceptionMiddleware ExceptionMiddleware { get; set; }
        public HttpContext _HttpContext { get; set; }
       

        [SetUp]
        public void ExceptionMiddleWareSetup()
        {
            RequestDelegateMock = new Mock<IDelegateMock>();
           _HttpContext = new DefaultHttpContext();
          
        }

        [Test]
        public  async Task Should_Invoke_Delegate()
        {
            RequestDelegateMock
                .Setup(x => x.RequestDelegate(It.IsAny<HttpContext>()))
                .Returns(Task.FromResult(0));
            ExceptionMiddleware = new ExceptionMiddleware(RequestDelegateMock.Object.RequestDelegate);
            await ExceptionMiddleware.InvokeAsync(new DefaultHttpContext());
            RequestDelegateMock.Verify(x => x.RequestDelegate(It.IsAny<HttpContext>()), Times.Once);
        }

        [Test]
        public async Task Should_return_bad_request_message()
        {

            RequestDelegateMock
                .Setup(x => x.RequestDelegate(It.IsAny<HttpContext>()))
                .Returns(Task.FromException(new BadRequestException("Error")));
            ExceptionMiddleware = new ExceptionMiddleware(RequestDelegateMock.Object.RequestDelegate);


            await ExceptionMiddleware.InvokeAsync(_HttpContext);

            Assert.AreEqual((int)HttpStatusCode.BadRequest, _HttpContext.Response.StatusCode);
            _HttpContext.Response.ContentType.Should().Be("application/json");
        }

        [Test]
        public async Task Should_return_exception_message()
        {

            RequestDelegateMock
               .Setup(x => x.RequestDelegate(It.IsAny<HttpContext>()))
               .Returns(Task.FromException(new Exception()));
            ExceptionMiddleware = new ExceptionMiddleware(RequestDelegateMock.Object.RequestDelegate);

            
            await ExceptionMiddleware.InvokeAsync(_HttpContext);

            Assert.AreEqual((int)HttpStatusCode.InternalServerError, _HttpContext.Response.StatusCode);
            _HttpContext.Response.ContentType.Should().Be("application/json");
        }

        public interface IDelegateMock
        {
            Task RequestDelegate(HttpContext context);
        }
    }
}
