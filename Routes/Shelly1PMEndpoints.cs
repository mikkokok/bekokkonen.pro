using bekokkonen.pro.Global.Enum;
using bekokkonen.pro.Models;
using bekokkonen.pro.Providers;
using Microsoft.AspNetCore.Mvc;

namespace bekokkonen.pro.Routes
{
    public static partial class ApiMapper
    {
        public static void MapShelly1PMEndpoints(this WebApplication app)
        {
            var shellyEndpoints = app.MapGroup("/api/shelly1pm").WithTags("Shelly1PMEndpoints").RequireAuthorization();

            shellyEndpoints.MapGet("/devices", ([FromServices] Shelly1PMProvider provider) =>
            {
                return TypedResults.Ok(provider.GetShelly1PMDevices());
            })
            .WithName("GetShelly1PMDevices");

            shellyEndpoints.MapPost("/{deviceName}/relay", async (
                [FromServices] Shelly1PMProvider provider,
                Shelly1PMDeviceName deviceName,
                [FromQuery] bool turnOn) =>
            {
                await provider.SetShelly1PMRelay(deviceName, turnOn);
                return TypedResults.NoContent();
            })
            .WithName("SetShelly1PMRelay");

            shellyEndpoints.MapGet("/{deviceName}/settings", async Task<IResult> (
                [FromServices] Shelly1PMProvider provider,
                Shelly1PMDeviceName deviceName) =>
            {
                try
                {
                    var settings = await provider.GetShelly1PMRelaySettings(deviceName);
                    return settings is not null
                        ? Results.Ok(settings)
                        : Results.NotFound();
                }
                catch
                {
                    return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
                }
            })
            .WithName("GetShelly1PMRelaySettings")
            .Produces<Shelly1PMRelaySettingsResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status503ServiceUnavailable);

            shellyEndpoints.MapPost("/{deviceName}/schedule", async Task<IResult> (
                [FromServices] Shelly1PMProvider provider,
                Shelly1PMDeviceName deviceName,
                [FromQuery] string schedule) =>
            {
                if (string.IsNullOrWhiteSpace(schedule))
                {
                    return Results.BadRequest("Schedule is required.");
                }

                try
                {
                    var settings = await provider.SetShelly1PMRelaySchedule(deviceName, schedule, add: true);
                    return Results.Ok(settings);
                }
                catch
                {
                    return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
                }
            })
            .WithName("AddShelly1PMRelaySchedule")
            .Produces<Shelly1PMRelaySettingsResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status503ServiceUnavailable);

            shellyEndpoints.MapDelete("/{deviceName}/schedule", async Task<IResult> (
                [FromServices] Shelly1PMProvider provider,
                Shelly1PMDeviceName deviceName,
                [FromQuery] string schedule) =>
            {
                if (string.IsNullOrWhiteSpace(schedule))
                {
                    return Results.BadRequest("Schedule is required.");
                }

                try
                {
                    var settings = await provider.SetShelly1PMRelaySchedule(deviceName, schedule, add: false);
                    return Results.Ok(settings);
                }
                catch
                {
                    return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
                }
            })
            .WithName("RemoveShelly1PMRelaySchedule")
            .Produces<Shelly1PMRelaySettingsResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status503ServiceUnavailable);
        }
    }
}
