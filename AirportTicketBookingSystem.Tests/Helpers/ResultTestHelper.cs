using AirportTicketBookingSystem.Common.Models;

namespace AirportTicketBookingSystem.Tests.Helpers;

public static class ResultTestHelper
{
    public static Result<TValue> CreateSuccess<TValue>(TValue value) => Result<TValue>.Success(value);
    public static Result CreateSuccess() => Result.Success();
    public static Result CreateFailure(Error error) => Result.Failure(error);
}