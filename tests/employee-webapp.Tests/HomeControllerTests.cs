using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using employee_webapp.Controllers;
using Xunit;

namespace employee_webapp.Tests;

public class HomeControllerTests
{
    // HomeController needs ILogger — we use NullLogger
    // NullLogger = real ILogger that just discards all logs
    // perfect for unit tests, no mocking library needed
    private readonly HomeController _controller = new(
        NullLogger<HomeController>.Instance
    );

    [Fact]
    public void Index_ReturnsViewResult()
    {
        // Act
        var result = _controller.Index();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Privacy_ReturnsViewResult()
    {
        // Act
        var result = _controller.Privacy();

        // Assert
        Assert.IsType<ViewResult>(result);
    }
}
