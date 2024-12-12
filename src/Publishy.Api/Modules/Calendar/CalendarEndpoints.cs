using Ardalis.Result;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Publishy.Api.Modules.Calendar.Commands;
using Publishy.Api.Modules.Calendar.Queries;
using Publishy.Api.Modules.Calendar.Responses;
using Publishy.Domain.Common.Results;

namespace Publishy.Api.Modules.Calendar;

public static class CalendarEndpoints
{
    public static IEndpointRouteBuilder MapCalendarEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/calendar")
            .WithTags("Calendar")
            .WithOpenApi();

        // GET /calendar/monthly
        group.MapGet("/monthly", async (IMediator mediator, string? month) =>
        {
            var query = new GetMonthlyCalendarQuery(month);
            var response = await mediator.SendRequest(query);
            return response.ToMinimalApiResult();
        })
        .WithName("GetMonthlyCalendar")
        .WithSummary("Retrieve the monthly publication calendar view")
        .WithDescription("Fetches the scheduled publications for a specific month. If no month is specified, defaults to the current month.")
        .Produces<Result<MonthlyCalendarResponse>>()
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // PUT /calendar/posts/{postId}
        group.MapPut("/posts/{postId}", async (IMediator mediator, string postId, UpdatePostCommand command) =>
        {
            command = command with { PostId = postId };
            var response = await mediator.SendRequest(command);
            return response.ToMinimalApiResult();
        })
        .WithName("UpdateCalendarPost")
        .WithSummary("Update a scheduled post")
        .WithDescription("Updates details of a scheduled publication.")
        .Produces<Result<CalendarPostResponse>>()
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // DELETE /calendar/posts/{postId}
        group.MapDelete("/posts/{postId}", async (IMediator mediator, string postId) =>
        {
            var command = new CancelPostCommand(postId);
            var response = await mediator.SendRequest(command);
            return response.ToMinimalApiResult();
        })
        .WithName("CancelCalendarPost")
        .WithSummary("Cancel a scheduled post")
        .WithDescription("Deletes a publication from the calendar.")
        .Produces<Result>(StatusCodes.Status204NoContent)
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        return app;
    }
}