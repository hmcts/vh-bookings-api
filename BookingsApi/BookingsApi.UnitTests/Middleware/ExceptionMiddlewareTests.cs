using System.Collections.Generic;
using System.IO;
using BookingsApi.Extensions;
using Microsoft.AspNetCore.Http;
using System.Net;
using BookingsApi.Common;
using BookingsApi.DAL.Exceptions;
using BookingsApi.Domain.Validations;
using Microsoft.Extensions.Logging;

namespace BookingsApi.UnitTests.Middleware
{
    [TestFixture]
    public class ExceptionMiddlewareTests
    {
        public Mock<IDelegateMock> RequestDelegateMock { get; set; }
        public Mock<ILogger<ExceptionMiddleware>> LoggerMock { get; set; }
        public ExceptionMiddleware ExceptionMiddleware { get; set; }
        public HttpContext _HttpContext { get; set; }
       

        [SetUp]
        public void ExceptionMiddleWareSetup()
        {
            RequestDelegateMock = new Mock<IDelegateMock>();
            LoggerMock = new Mock<ILogger<ExceptionMiddleware>>();
           _HttpContext = new DefaultHttpContext();
           _HttpContext.Response.Body = new MemoryStream();
        }

        

        [Test]
        public  async Task Should_Invoke_Delegate()
        {
            RequestDelegateMock
                .Setup(x => x.RequestDelegate(It.IsAny<HttpContext>()))
                .Returns(Task.FromResult(0));
            ExceptionMiddleware = new ExceptionMiddleware(RequestDelegateMock.Object.RequestDelegate, LoggerMock.Object);
            await ExceptionMiddleware.InvokeAsync(new DefaultHttpContext());
            RequestDelegateMock.Verify(x => x.RequestDelegate(It.IsAny<HttpContext>()), Times.Once);
        }

        [Test]
        public void should_return_not_found_when_EntityNotFoundException_is_thrown()
        {
            RequestDelegateMock
                .Setup(x => x.RequestDelegate(It.IsAny<HttpContext>()))
                .Returns(Task.FromException(new JusticeUserNotFoundException("random@test.om")));
            ExceptionMiddleware = new ExceptionMiddleware(RequestDelegateMock.Object.RequestDelegate, LoggerMock.Object);

            ExceptionMiddleware.InvokeAsync(_HttpContext).Wait();
            _HttpContext.Response.StatusCode.Should().Be((int) HttpStatusCode.NotFound);
            _HttpContext.Response.ContentType.Should().Be("application/json; charset=utf-8");
        }

        [Test]
        public void should_return_bad_request_when_DomainRuleException_is_thrown()
        {
            RequestDelegateMock
                .Setup(x => x.RequestDelegate(It.IsAny<HttpContext>()))
                .Returns(Task.FromException(new DomainRuleException("Error", "Error Test message")));
            ExceptionMiddleware = new ExceptionMiddleware(RequestDelegateMock.Object.RequestDelegate, LoggerMock.Object);

            ExceptionMiddleware.InvokeAsync(_HttpContext).Wait();
            _HttpContext.Response.StatusCode.Should().Be((int) HttpStatusCode.BadRequest);
            _HttpContext.Response.ContentType.Should().Be("application/json; charset=utf-8");
        }

        [Test]
        public async Task Should_return_exception_message()
        {

            RequestDelegateMock
               .Setup(x => x.RequestDelegate(It.IsAny<HttpContext>()))
               .Returns(Task.FromException(new Exception()));
            ExceptionMiddleware = new ExceptionMiddleware(RequestDelegateMock.Object.RequestDelegate, LoggerMock.Object);

            
            await ExceptionMiddleware.InvokeAsync(_HttpContext);

            _HttpContext.Response.StatusCode.Should().Be((int) HttpStatusCode.InternalServerError);
            _HttpContext.Response.ContentType.Should().Be("application/json; charset=utf-8");
        }

        [Test]
        public async Task Should_return_nested_exception_messages()
        {
            var inner = new FormatException("Format issue");
            var exception = new FileNotFoundException("File issue", inner);
            RequestDelegateMock
                .Setup(x => x.RequestDelegate(It.IsAny<HttpContext>()))
                .Returns(Task.FromException(exception));
            ExceptionMiddleware = new ExceptionMiddleware(RequestDelegateMock.Object.RequestDelegate, LoggerMock.Object);
            
            await ExceptionMiddleware.InvokeAsync(_HttpContext);

            _HttpContext.Response.StatusCode.Should().Be((int) HttpStatusCode.InternalServerError);
            _HttpContext.Response.ContentType.Should().Be("application/json; charset=utf-8");
            
            _HttpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            var body = await new StreamReader(_HttpContext.Response.Body).ReadToEndAsync();
            body.Should().Contain(exception.Message).And.Contain(inner.Message);
        }
        
        public interface IDelegateMock
        {
            Task RequestDelegate(HttpContext context);
        }
    }
}
