using FruitShelf.Api.Middleware;

namespace FruitShelf.Api.Tests.Middleware;

using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;

public class RequestLoggingMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_LogsStartAndCompletion_WhenNoException()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<RequestLoggingMiddleware>>();
        var middleware = new RequestLoggingMiddleware(
            next: async ctx =>
            {
                ctx.Response.StatusCode = 200;
                await Task.CompletedTask;
            },
            loggerMock.Object);

        var context = new DefaultHttpContext
        {
            Request =
            {
                Method = "GET",
                Path = "/test",
                QueryString = new QueryString("?q=1")
            },
            Response =
            {
                Body = new MemoryStream()
            }
        };

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        VerifyMockLoggerCalled(loggerMock, "Starting GET /test?q=1", LogLevel.Information);

        VerifyMockLoggerCalled(loggerMock, "Completed 200 in", LogLevel.Information);
    }


    [Fact]
    public async Task InvokeAsync_LogsErrorAndRethrows_WhenExceptionThrown()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<RequestLoggingMiddleware>>();
        var exception = new InvalidOperationException("Simulate some exception occurred");
        var middleware = new RequestLoggingMiddleware(
            next: ctx => throw exception,
            loggerMock.Object);

        var context = new DefaultHttpContext
        {
            Request =
            {
                Method = "POST",
                Path = "/fail"
            },
            Response =
            {
                Body = new MemoryStream()
            }
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => middleware.InvokeAsync(context));

        Assert.Same(exception, ex);

        // Error log
        VerifyMockLoggerCalled(loggerMock, "Exception thrown in POST /fail after", LogLevel.Error);
    }

    private static void VerifyMockLoggerCalled(Mock<ILogger<RequestLoggingMiddleware>> loggerMock,
        string expectedLogMessage, LogLevel logLevel)
    {
        loggerMock.Verify(
            x => x.Log(
                logLevel,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(expectedLogMessage)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }
}