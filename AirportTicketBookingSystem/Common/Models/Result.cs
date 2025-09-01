namespace AirportTicketBookingSystem.Common.Models;

public class Result
{
    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None ||
            !isSuccess && error == Error.None)
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    public static Result Success() => new(true, Error.None);

    public static Result Failure(Error error) => new(false, error);
}

public class Result<TValue> : Result
{
    private readonly TValue _value;

    protected internal Result(TValue value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    public TValue Value =>
        IsSuccess ? _value : throw new InvalidOperationException("The result does not contain a value.");

    public static Result<TValue> Success(TValue value) => new(value, true, Error.None);

    public new static Result<TValue> Failure(Error error)
    {
        return new(default!, false, error);
    }
    
    public static implicit operator Result<TValue>(Error error) =>
        new(default!, false, error);

    public static implicit operator Result<TValue>(TValue value) =>
        new(value, true, Error.None);
}