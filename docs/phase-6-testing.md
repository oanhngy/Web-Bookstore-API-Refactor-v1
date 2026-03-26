# Phase 6 — Unit Tests

## Goal
Write unit tests for Services that contain real business logic.
Tests should verify that business rules work correctly — independent of the database.

## Why unit tests are possible now
After Phase 3 and 4, Services depend on Repository *interfaces*, not concrete EF Core classes.
This means we can replace the real Repository with a fake one (mock) in tests — no database needed.

---
## Setup

Install these packages in `BookstoreWeb.Tests`:
```bash
dotnet add package xunit
dotnet add package xunit.runner.visualstudio
dotnet add package Moq
dotnet add package Microsoft.NET.Test.Sdk
```

## Directory structure (mirrors Service structure)

```
BookstoreWeb.Tests/
└── Services/
    ├── ProductServiceTests.cs
    ├── OrderServiceTests.cs
    ├── CartServiceTests.cs
    └── CategoryServiceTests.cs
```

## Test method naming: MethodName_Scenario_ExpectedResult

```
CancelOrderAsync_WhenStatusIsCompleted_ShouldThrowValidationException
CancelOrderAsync_WhenStatusIsNew_ShouldSucceed
GetByIdAsync_WhenProductNotFound_ShouldThrowNotFoundException
CreateAsync_WhenPriceIsNegative_ShouldThrowValidationException
AddToCartAsync_WhenProductExists_ShouldIncreaseQuantity
```

## Test structure: Arrange → Act → Assert

```csharp
public class OrderServiceTests
{
    // Set up mock and service once for all tests in this class
    private readonly Mock<IOrderRepository> _mockRepo;
    private readonly OrderService _service;

    public OrderServiceTests()
    {
        _mockRepo = new Mock<IOrderRepository>();
        _service  = new OrderService(_mockRepo.Object);
    }

    [Fact]
    public async Task CancelOrderAsync_WhenStatusIsCompleted_ShouldThrowValidationException()
    {
        // Arrange — set up the scenario
        var order = new Order { OrderID = 1, Status = "Completed" };
        _mockRepo
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(order);

        // Act — call the method being tested
        var act = async () => await _service.CancelOrderAsync(1);

        // Assert — verify the expected outcome
        await Assert.ThrowsAsync<ValidationException>(act);
    }

    [Fact]
    public async Task CancelOrderAsync_WhenStatusIsCheckedOut_ShouldSucceed()
    {
        // Arrange
        var order = new Order { OrderID = 1, Status = "Checked Out" };
        _mockRepo
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(order);

        // Act
        await _service.CancelOrderAsync(1);

        // Assert — verify UpdateAsync was called once with the cancelled order
        _mockRepo.Verify(r => r.UpdateAsync(It.Is<Order>(o => o.Status == "Cancelled")), Times.Once);
    }
}
```

## What to test vs what to skip

```
✅ Test — Services with business logic:
   OrderService  — state transition rules, cancellation conditions
   CartService   — total calculation, quantity update logic
   ProductService — validation rules on create/update

❌ Skip — no logic to test:
   Repositories  — just EF Core wrappers
   Controllers   — just call Services, return ActionResult
```

## How to measure coverage

```bash
# Run tests with coverage collection
dotnet test --collect:"XPlat Code Coverage"

# Install report generator (one time)
dotnet tool install -g dotnet-reportgenerator-globaltool

# Generate HTML report
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coverage-report"

# Open coverage-report/index.html in browser
```

No hard coverage target — focus on testing logic that matters, not chasing a percentage.

## Checklist before moving to Phase 7

- [ ] Test project created with xUnit and Moq packages
- [ ] Test classes created for OrderService, CartService, ProductService
- [ ] Each test follows Arrange → Act → Assert pattern
- [ ] All tests pass — `dotnet test` shows green
- [ ] No real DbContext used in any test
