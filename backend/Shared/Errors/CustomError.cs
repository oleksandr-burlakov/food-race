namespace Shared.Errors;

public record CustomError(string Code, string Description, ErrorType Type = ErrorType.Failure)
{
    public static readonly CustomError None = new(string.Empty, string.Empty);

    public static readonly CustomError NullValue = new("Error.NullValue", "Null value was provided",
        ErrorType.Validation);

    public static CustomError NotFound(string code, string description)
    {
        return new CustomError(code, description, ErrorType.NotFound);
    }

    public static CustomError Unauthorized(string code, string description)
    {
        return new CustomError(code, description, ErrorType.Unauthorized);
    }

    public static CustomError Validation(string code, string description)
    {
        return new CustomError(code, description, ErrorType.Validation);
    }

    public static CustomError Conflict(string code, string description)
    {
        return new CustomError(code, description, ErrorType.Conflict);
    }
}

public enum ErrorType
{
    Failure = 0,
    Validation = 1,
    NotFound = 2,
    Conflict = 3,
    Unauthorized = 4
}