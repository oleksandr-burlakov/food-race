using Shared.Errors;

namespace Shared.Results;

public class Result
{
    protected Result(bool isSuccess, CustomError error)
    {
        if ((isSuccess && error != CustomError.None) ||
            (!isSuccess && error == CustomError.None))
            throw new InvalidOperationException();

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public CustomError Error { get; }

    public static Result Success()
    {
        return new Result(true, CustomError.None);
    }

    public static Result Failure(CustomError error)
    {
        return new Result(false, error);
    }

    public static Result<TValue> Success<TValue>(TValue value)
    {
        return new Result<TValue>(value, true, CustomError.None);
    }

    public static Result<TValue> Failure<TValue>(CustomError error)
    {
        return new Result<TValue>(default, false, error);
    }
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    protected internal Result(TValue? value, bool isSuccess, CustomError error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("The value of a failure result cannot be accessed.");

    public static implicit operator Result<TValue>(TValue value)
    {
        return Success(value);
    }

    public static implicit operator Result<TValue>(CustomError error)
    {
        return Failure<TValue>(error);
    }
}