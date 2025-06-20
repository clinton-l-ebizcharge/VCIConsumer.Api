using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Reflection;
using VCIConsumer.Api.Filter;
using Xunit;

namespace VCIConsumer.Api.UnitTests.FilterTests;
public class FakeEndpointFilterInvocationContext : EndpointFilterInvocationContext
{
    public FakeEndpointFilterInvocationContext(HttpContext httpContext, IList<object?> arguments)
    {
        HttpContext = httpContext;
        Arguments = arguments;
    }

    public override HttpContext HttpContext { get; }
    public override IList<object?> Arguments { get; }

    public override T GetArgument<T>(int index)
    {
        // Ensure the index is within bounds and cast the argument to the requested type.  
        if (index < 0 || index >= Arguments.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
        }

        return (T)Arguments[index]!;
    }
}

// Dummy endpoint methods so we can set metadata correctly.
public class DummyEndpoints
{
    [SkipStandardFilterAttribute]
    public static Task<object?> SkipMethod(EndpointFilterInvocationContext context)
        => Task.FromResult((object?)"ShouldNotWrap");
}

public class DummyEndpointsNonSkip
{
    // This method does not have the skip attribute.
    public static Task<object?> NormalMethod(EndpointFilterInvocationContext context)
        => Task.FromResult((object?)"NormalResult");
}

// A fake IResult implementation to simulate a scenario in which the next delegate returns an already wrapped result.
public class FakeResult : IResult
{
    public Task ExecuteAsync(HttpContext httpContext)
    {
        httpContext.Response.StatusCode = 200;
        return Task.CompletedTask;
    }
}

public class StandardApiFilterTests
{
    [Fact]
    public async Task InvokeAsync_SkipFilterAttribute_CallsNextDirectly()
    {
        // Arrange
        var loggerFactory = new LoggerFactory();
        var filter = new StandardApiFilter<object>(loggerFactory);
        var httpContext = new DefaultHttpContext { TraceIdentifier = "TestCorrelationId" };

        // Create an endpoint whose metadata contains a MethodInfo marked with SkipStandardFilterAttribute.
        MethodInfo skipMethod = typeof(DummyEndpoints)
            .GetMethod(nameof(DummyEndpoints.SkipMethod))!;
        var endpoint = new Endpoint(
            context => Task.CompletedTask,
            new EndpointMetadataCollection(skipMethod),
            "TestEndpoint");
        httpContext.SetEndpoint(endpoint);

        var context = new FakeEndpointFilterInvocationContext(httpContext, new object?[] { });

        // Setup a next delegate that returns a plain object.
        EndpointFilterDelegate next = ctx => ValueTask.FromResult((object?)"DirectResult");

        // Act
        var result = await filter.InvokeAsync(context, next);

        // Assert: the filter should detect the skip attribute and return what the next delegate returns.
        Assert.Equal("DirectResult", result);
    }

    [Fact]
    public async Task InvokeAsync_WhenNextReturnsIResult_DoesNotWrapResult()
    {
        // Arrange
        var loggerFactory = new LoggerFactory();
        var filter = new StandardApiFilter<object>(loggerFactory);
        var httpContext = new DefaultHttpContext { TraceIdentifier = "CorrId1" };

        // Create an endpoint whose MethodInfo is NOT marked with SkipStandardFilterAttribute.
        MethodInfo normalMethod = typeof(DummyEndpointsNonSkip)
            .GetMethod(nameof(DummyEndpointsNonSkip.NormalMethod))!;
        var endpoint = new Endpoint(
            context => Task.CompletedTask,
            new EndpointMetadataCollection(normalMethod),
            "TestEndpoint");
        httpContext.SetEndpoint(endpoint);

        var context = new FakeEndpointFilterInvocationContext(httpContext, new object?[] { });

        // Setup a next delegate that returns an IResult.
        var fakeResult = new FakeResult();
        EndpointFilterDelegate next = ctx => ValueTask.FromResult((object?)fakeResult);

        // Act
        var result = await filter.InvokeAsync(context, next);

        // Assert: when next returns an IResult, it should pass it through without wrapping.
        Assert.Equal(fakeResult, result);
    }

    [Fact]
    public async Task InvokeAsync_WhenNextReturnsNonIResult_WrapsInApiResponse()
    {
        // Arrange
        var loggerFactory = new LoggerFactory();
        var filter = new StandardApiFilter<object>(loggerFactory);
        var httpContext = new DefaultHttpContext { TraceIdentifier = "CorrId2" };

        // Create an endpoint whose MethodInfo is not marked with SkipStandardFilterAttribute.
        MethodInfo normalMethod = typeof(DummyEndpointsNonSkip)
            .GetMethod(nameof(DummyEndpointsNonSkip.NormalMethod))!;
        var endpoint = new Endpoint(
            context => Task.CompletedTask,
            new EndpointMetadataCollection(normalMethod),
            "TestEndpoint");
        httpContext.SetEndpoint(endpoint);

        var context = new FakeEndpointFilterInvocationContext(httpContext, new object?[] { });

        // Setup a next delegate returning a plain object (non-IResult).
        EndpointFilterDelegate next = ctx => ValueTask.FromResult((object?)"WrappedResult");

        // Act
        var result = await filter.InvokeAsync(context, next);

        // Assert: Since next returned a non-IResult, the filter should wrap it in an ApiResponse using Results.Ok.
        // We simply verify that the returned object implements IResult.
        Assert.IsAssignableFrom<IResult>(result);
    }

    [Fact]
    public async Task InvokeAsync_WhenExceptionIsThrown_ReturnsErrorResponse()
    {
        // Arrange
        var loggerMock = new Mock<ILoggerFactory>();
        var logger = new Mock<ILogger<StandardApiFilter<object>>>();
        loggerMock.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(logger.Object);

        var filter = new StandardApiFilter<object>(loggerMock.Object);
        var httpContext = new DefaultHttpContext { TraceIdentifier = "CorrId3" };

        MethodInfo normalMethod = typeof(DummyEndpointsNonSkip)
            .GetMethod(nameof(DummyEndpointsNonSkip.NormalMethod))!;
        var endpoint = new Endpoint(
            context => Task.CompletedTask,
            new EndpointMetadataCollection(normalMethod),
            "TestEndpoint");
        httpContext.SetEndpoint(endpoint);

        var context = new FakeEndpointFilterInvocationContext(httpContext, new object?[] { });

        // Setup a next delegate that throws an exception.
        EndpointFilterDelegate next = ctx => throw new Exception("Test exception");

        // Act
        var result = await filter.InvokeAsync(context, next);

        // Assert
        // The failure branch should log the error and return a JSON error response.
        logger.Verify(x => x.Log(
            It.Is<LogLevel>(level => level == LogLevel.Error),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((state, t) => state.ToString().Contains("Unhandled error")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
        Assert.IsAssignableFrom<IResult>(result);
    }
}

public class StandardValidatedApiFilterTests
{
    // A simple model for testing.
    public class TestModel
    {
        public string Value { get; set; } = string.Empty;
    }

    [Fact]
    public async Task InvokeAsync_WhenValidationFails_ReturnsFailureResponse()
    {
        // Arrange
        var validatorMock = new Mock<IValidator<TestModel>>();
        var failure = new ValidationFailure("Value", "Value is required");
        var validationResult = new ValidationResult(new List<ValidationFailure> { failure });
        validatorMock.Setup(x => x.ValidateAsync(It.IsAny<TestModel>(), default))
                     .ReturnsAsync(validationResult);

        var filter = new StandardValidatedApiFilter<TestModel>(validatorMock.Object);
        var httpContext = new DefaultHttpContext { TraceIdentifier = "CorrValid1" };
        var model = new TestModel { Value = "" };

        var context = new FakeEndpointFilterInvocationContext(httpContext, new object?[] { model });

        var nextCalled = false;
        EndpointFilterDelegate next = ctx =>
        {
            nextCalled = true;
            return ValueTask.FromResult((object?)"Should not be called");
        };

        // Act
        var result = await filter.InvokeAsync(context, next);

        // Assert
        // When validation fails, the filter should not call the next delegate.
        Assert.False(nextCalled);
        // And the response should be a failure response (via ToFailureResponse).
        Assert.IsAssignableFrom<IResult>(result);
    }

    [Fact]
    public async Task InvokeAsync_WhenValidationSucceeds_CallsNextAndWrapsResponse()
    {
        // Arrange
        var validatorMock = new Mock<IValidator<TestModel>>();
        // An empty ValidationResult indicates success.
        var validationResult = new ValidationResult();
        validatorMock.Setup(x => x.ValidateAsync(It.IsAny<TestModel>(), default))
                     .ReturnsAsync(validationResult);

        var filter = new StandardValidatedApiFilter<TestModel>(validatorMock.Object);
        var httpContext = new DefaultHttpContext { TraceIdentifier = "CorrValid2" };
        var model = new TestModel { Value = "Valid" };

        var context = new FakeEndpointFilterInvocationContext(httpContext, new object?[] { model });

        var nextCalled = false;
        EndpointFilterDelegate next = ctx =>
        {
            nextCalled = true;
            // Return a non-IResult value so that ToSuccessResponse is used.
            return ValueTask.FromResult((object?)"NextResult");
        };

        // Act
        var result = await filter.InvokeAsync(context, next);

        // Assert
        Assert.True(nextCalled);
        Assert.IsAssignableFrom<IResult>(result);
    }
}

public class ValidateSimpleInputFilterTests
{
    [Fact]
    public async Task InvokeAsync_WhenInputIsValid_CallsNextDelegate()
    {
        // Arrange  
        Func<string, (bool, string?)> predicate = input => (true, null);
        var loggerMock = new Mock<ILogger<ValidateSimpleInputFilter<string>>>();

        var filter = new ValidateSimpleInputFilter<string>(predicate, loggerMock.Object);
        var httpContext = new DefaultHttpContext { TraceIdentifier = "Simple1" };
        var testInput = "ValidInput";
        var context = new FakeEndpointFilterInvocationContext(httpContext, new object?[] { testInput });

        var nextCalled = false;
        EndpointFilterDelegate next = ctx =>
        {
            nextCalled = true;
            return ValueTask.FromResult((object?)"NextValue"); // Changed to ValueTask.FromResult
        };

        // Act
        var result = await filter.InvokeAsync(context, next);

        // Assert
        Assert.True(nextCalled);
        Assert.Equal("NextValue", result);
    }

    [Fact]
    public async Task InvokeAsync_WhenInputIsInvalid_ReturnsValidationFailure()
    {
        // Arrange
        Func<string, (bool, string?)> predicate = input => (false, "Invalid input provided");
        var loggerMock = new Mock<ILogger<ValidateSimpleInputFilter<string>>>();

        var filter = new ValidateSimpleInputFilter<string>(predicate, loggerMock.Object);
        var httpContext = new DefaultHttpContext { TraceIdentifier = "Simple2" };
        var testInput = "InvalidInput";
        var context = new FakeEndpointFilterInvocationContext(httpContext, new object?[] { testInput });

        var nextCalled = false;
        EndpointFilterDelegate next = ctx =>
        {
            nextCalled = true;
            return ValueTask.FromResult((object?)"Should not be reached"); // Changed to ValueTask.FromResult
        };

        // Act
        var result = await filter.InvokeAsync(context, next);

        // Assert
        Assert.False(nextCalled);
        Assert.IsAssignableFrom<IResult>(result);
        loggerMock.Verify(x => x.Log(
            It.Is<LogLevel>(level => level == LogLevel.Warning),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((state, t) => state.ToString().Contains("Invalid input provided")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}

