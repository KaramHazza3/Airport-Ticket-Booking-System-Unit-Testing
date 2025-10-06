using AirportTicketBookingSystem.Common.Models;
using AirportTicketBookingSystem.Tests.Helpers;

namespace AirportTicketBookingSystem.Tests;

public class ResultTests
{
    [Fact]
    public void CreateSuccess_ShouldCreateSuccessfulResult()
    {
        var result = ResultTestHelper.CreateSuccess();
        
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
    }
    
    [Fact]
    public void CreateSuccessWithValue_ShouldCreateSuccessfulResultWithValue()
    {
        var result = ResultTestHelper.CreateSuccess("Success");
        
        Assert.Equal("Success", result.Value);
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
    }
    
    [Fact]
    public void CreateFailure_ShouldCreateFailureResult()
    {
        var result = ResultTestHelper.CreateFailure(new Error("Tests.NotGood", "This is not good"));

        Assert.NotNull(result.Error);
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
    }
}