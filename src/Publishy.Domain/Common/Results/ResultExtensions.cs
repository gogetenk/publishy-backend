using Ardalis.Result;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Publishy.Domain.Common.Results;

public static class ResultExtensions
{
    public static IResult ToMinimalApiResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
            return Results.Ok(result.Value);

        if (result.Status == ResultStatus.NotFound)
            return Results.NotFound(new ProblemDetails
            {
                Title = "Resource not found",
                Detail = string.Join(", ", result.Errors),
                Status = StatusCodes.Status404NotFound
            });

        if (result.Status == ResultStatus.Invalid)
            return Results.BadRequest(new ProblemDetails
            {
                Title = "Validation failed",
                Detail = string.Join(", ", result.Errors),
                Status = StatusCodes.Status400BadRequest
            });

        return Results.Problem(
            title: "An error occurred",
            detail: string.Join(", ", result.Errors),
            statusCode: StatusCodes.Status500InternalServerError
        );
    }

    public static IResult ToMinimalApiResult(this Result result)
    {
        if (result.IsSuccess)
            return Results.Ok();

        if (result.Status == ResultStatus.NotFound)
            return Results.NotFound(new ProblemDetails
            {
                Title = "Resource not found",
                Detail = string.Join(", ", result.Errors),
                Status = StatusCodes.Status404NotFound
            });

        if (result.Status == ResultStatus.Invalid)
            return Results.BadRequest(new ProblemDetails
            {
                Title = "Validation failed",
                Detail = string.Join(", ", result.Errors),
                Status = StatusCodes.Status400BadRequest
            });

        return Results.Problem(
            title: "An error occurred",
            detail: string.Join(", ", result.Errors),
            statusCode: StatusCodes.Status500InternalServerError
        );
    }
}