using Microsoft.AspNetCore.Mvc;
using employee_webapp.Controllers;
using employee_webapp.Models;
using Xunit;

namespace employee_webapp.Tests;

public class EmployeeControllerTests
{
    // EmployeeController has NO constructor arguments
    // so we just new it up directly — simple!
    private readonly EmployeeController _controller = new();

    [Fact]
    public void Index_ReturnsViewResult()
    {
        // Act
        var result = _controller.Index();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Index_ReturnsViewWithEmployeesList()
    {
        // Act
        var result = _controller.Index() as ViewResult;

        // Assert
        Assert.NotNull(result);
        // Model should be a List<Employee>
        Assert.IsType<List<Employee>>(result.Model);
    }

    [Fact]
    public void Create_Get_ReturnsViewResult()
    {
        // Act
        var result = _controller.Create();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Create_Post_ValidEmployee_RedirectsToIndex()
    {
        // Arrange — create a valid employee matching your model
        var employee = new Employee
        {
            Id = 1,
            Name = "Anil Kumar",
            Role = "DevOps Engineer"
        };

        // Act
        var result = _controller.Create(employee);

        // Assert — should redirect back to Index
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
    }

    [Fact]
    public void Create_Post_AddsEmployeeToList()
    {
        // Arrange
        var employee = new Employee
        {
            Id = 2,
            Name = "Test User",
            Role = "Developer"
        };

        // Act
        _controller.Create(employee);
        var indexResult = _controller.Index() as ViewResult;
        var employees = indexResult?.Model as List<Employee>;

        // Assert — employee was actually added
        Assert.NotNull(employees);
        Assert.Contains(employees, e => e.Name == "Test User");
    }
}
