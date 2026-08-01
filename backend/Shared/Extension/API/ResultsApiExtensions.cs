using Microsoft.AspNetCore.Http;
using Shared.Errors;
using Shared.Results;

namespace Shared.Extension;

public static class ResultsApiExtensions
{
    public static IResult ToHttpResponse<T>(this Result<T> result)
    {
        if (result.IsSuccess) return Microsoft.AspNetCore.Http.Results.Ok(result.Value);

        return MatchErrorToProblemDetails(result.Error);
    }

    public static IResult ToHttpResponse(this Result result)
    {
        if (result.IsSuccess) return Microsoft.AspNetCore.Http.Results.NoContent();

        return MatchErrorToProblemDetails(result.Error);
    }

    private static IResult MatchErrorToProblemDetails(CustomError error)
    {
        var statusCode = error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };

        return Microsoft.AspNetCore.Http.Results.Problem(
            statusCode: statusCode,
            title: error.Code,
            detail: error.Description,
            type: GetRfcType(statusCode)
        );
    }

    private static string GetRfcType(int statusCode)
    {
        return statusCode switch
        {
            400 => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            401 => "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1",
            404 => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            409 => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
            _ => "https://tools.ietf.org/html/rfc7231#section-6.6.1"
        };
    }
}