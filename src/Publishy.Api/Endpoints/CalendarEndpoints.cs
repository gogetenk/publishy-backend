using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Publishy.Application.UseCases.Commands.CreateCalendar;
using Publishy.Application.UseCases.Commands.UpdateCalendar;
using Publishy.Application.UseCases.Queries.GetCalendarById;
using Publishy.Application.UseCases.Queries.GetCalendars;

namespace Publishy.Api.Endpoints;

public static class CalendarEndpoints
{
    public static IEndpointRouteBuilder MapCalendarEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/calendars")
            .WithTags("Calendars")
            .WithOpenApi();

        // GET /calendars
        group.MapGet("/", async ([FromServices] IMediator mediator, [FromQuery] int? page, [FromQuery] int? pageSize,
            [FromQuery] string? projectId, [FromQuery] string? status) =>
        {
            var query = new GetCalendarsQuery(page ?? 1, pageSize ?? 10, projectId, status);
            var response = await mediator.SendRequest(query);
            return response.ToMinimalApiResult();
        })
        .WithName("GetCalendars")
        .WithSummary("Retrieve the list of calendars")
        .WithDescription("Fetches all calendars with their details.")
        .Produces<Result<GetCalendarsResponse>>()
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // POST /calendars
        group.MapPost("/", async ([FromServices] IMediator mediator, CreateCalendarCommand command) =>
        {
            var response = await mediator.SendRequest(command);
            return response.ToMinimalApiResult();
        })
        .WithName("CreateCalendar")
        .WithSummary("Create a new calendar")
        .WithDescription("Creates a new calendar with the provided information.")
        .Produces<Result<CalendarResponse>>(StatusCodes.Status201Created)
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // GET /calendars/{calendarId}
        group.MapGet("/{calendarId}", async ([FromServices] IMediator mediator, string calendarId) =>
        {
            var query = new GetCalendarByIdQuery(calendarId);
            var response = await mediator.SendRequest(query);
            return response.ToMinimalApiResult();
        })
        .WithName("GetCalendarById")
        .WithSummary("Retrieve calendar details")
        .WithDescription("Fetches details of a specific calendar.")
        .Produces<Result<CalendarResponse>>()
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // PUT /calendars/{calendarId}
        group.MapPut("/{calendarId}", async ([FromServices] IMediator mediator, string calendarId, UpdateCalendarCommand command) =>
        {
            command = command with { CalendarId = calendarId };
            var response = await mediator.SendRequest(command);
            return response.ToMinimalApiResult();
        })
        .WithName("UpdateCalendar")
        .WithSummary("Update a calendar")
        .WithDescription("Updates information of an existing calendar.")
        .Produces<Result<CalendarResponse>>()
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        return app;
    }
}