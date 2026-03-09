using employee_webapp.Models;
using Xunit;

namespace employee_webapp.Tests;

public class EmployeeModelTests
{
    [Fact]
    public void Employee_CanSetAndGetProperties()
    {
        // Arrange + Act
        var employee = new Employee
        {
            Id = 1,
            Name = "Anil Kumar",
            Role = "DevOps Engineer"
        };

        // Assert
        Assert.Equal(1, employee.Id);
        Assert.Equal("Anil Kumar", employee.Name);
        Assert.Equal("DevOps Engineer", employee.Role);
    }

    [Fact]
    public void Employee_DefaultValues_AreEmpty()
    {
        // Arrange + Act
        var employee = new Employee();

        // Assert
        Assert.Equal(0, employee.Id);
        Assert.Null(employee.Name);
        Assert.Null(employee.Role);
    }
}
