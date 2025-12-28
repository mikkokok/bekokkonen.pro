using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using bekokkonen.pro.Providers;
using bekokkonen.pro.Global;
using bekokkonen.pro.Models.HeatHarmony;
using bekokkonen.pro.Global.Config;

namespace bekokkonen.pro.Routes.MapEndpoints
{
    public static partial class ApiMapper
    {
        public static void MapHeatHarmonyEndpoints(this WebApplication app)
        {
            var heatHarmonyEndpoints = app.MapGroup("/api/heatharmony").WithTags("HeatHarmonyEndpoints");
            var heatHarmonyUrl = GlobalConfig.HeatHarmonyConfig!.BaseUrl;

            heatHarmonyEndpoints.MapGet("/appstatus/ping", ([FromServices] IRequestProvider requestProvider) =>
            {
                return requestProvider.GetAsync<object>(HttpClientConst.HeatHarmony, $"{heatHarmonyUrl}/appstatus/ping")
                    .ContinueWith(task => task.Result is not null
                        ? Results.Ok(task.Result)
                        : Results.StatusCode(503));
            }).WithName("GetAppHealthStatus")
              .RequireAuthorization();
            heatHarmonyEndpoints.MapGet("/appstatus/uptime", ([FromServices] IRequestProvider requestProvider) =>
            {
                return requestProvider.GetAsync<object>(HttpClientConst.HeatHarmony, $"{heatHarmonyUrl}/appstatus/uptime")
                    .ContinueWith(task => task.Result is not null
                        ? Results.Ok(task.Result)
                        : Results.StatusCode(503));
            }).WithName("GetAppUptime")
            .RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/em/latest", ([FromServices] IRequestProvider requestProvider) =>
            {
                return requestProvider.GetAsync<object>(HttpClientConst.HeatHarmony, $"{heatHarmonyUrl}/em/latest")
                    .ContinueWith(task => task.Result is not null
                        ? Results.Ok(task.Result)
                        : Results.StatusCode(503));
            }).WithName("GetLatestEM")
            .RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/em/changes", ([FromServices] IRequestProvider requestProvider) =>
            {
                return requestProvider.GetAsync<object>(HttpClientConst.HeatHarmony, $"{heatHarmonyUrl}/em/changes")
                    .ContinueWith(task => task.Result is not null
                        ? Results.Ok(task.Result)
                        : Results.StatusCode(503));
            }).WithName("GetEMChanges")
            .RequireAuthorization();

            heatHarmonyEndpoints.MapPost("/em/enable", async ([FromServices] IRequestProvider requestProvider) =>
            {
                try
                {
                    await requestProvider.PostAsync<object>(HttpClientConst.HeatHarmony, $"{heatHarmonyUrl}/em/enable");
                    return Results.Ok();
                }
                catch (Exception ex)
                {
                    app.Logger.LogError(ex, "Error occurred while enabling EM water heating");
                    return Results.StatusCode(500);
                }
            }).WithName("EnableEMWaterHeating")
            .RequireAuthorization();

            heatHarmonyEndpoints.MapPost("/em/disable", async ([FromServices] IRequestProvider requestProvider) =>
            {
                try
                {
                    await requestProvider.PostAsync<object>(HttpClientConst.HeatHarmony, $"{heatHarmonyUrl}/em/disable");
                    return Results.Ok();
                }
                catch (Exception ex)
                {
                    app.Logger.LogError(ex, "Error occurred while disabling EM water heating");
                    return Results.StatusCode(500);
                }
            }).WithName("DisableEMWaterHeating")
            .RequireAuthorization();

            heatHarmonyEndpoints.MapDelete("/em/override/delete", async ([FromServices] IRequestProvider requestProvider) =>
            {
                try
                {
                    await requestProvider.DeleteAsync(HttpClientConst.HeatHarmony, $"{heatHarmonyUrl}/em/delete");
                    return Results.Ok();
                }
                catch (Exception ex)
                {
                    app.Logger.LogError(ex, "Error occurred while removing EM water heating override");
                    return Results.StatusCode(500);
                }
            })
                .WithName("ClearEMOverride");

            heatHarmonyEndpoints.MapPost("/em/override/enable/{hours:int?}", async ([FromServices] IRequestProvider requestProvider, int? hours) =>
            {
                try
                {
                    await requestProvider.PostAsync<object>(HttpClientConst.HeatHarmony, $"{heatHarmonyUrl}/em/override/enable/{hours}");
                    return Results.Ok();
                }
                catch (Exception ex)
                {
                    app.Logger.LogError(ex, "Error occurred while overriding (enable) EM water heating");
                    return Results.StatusCode(500);
                }
            }).WithName("OverrideEMEnableWaterHeating")
            .RequireAuthorization();

            heatHarmonyEndpoints.MapPost("/em/override/disable/{hours:int?}", async ([FromServices] IRequestProvider requestProvider, int? hours) =>
            {
                try
                {
                    await requestProvider.PostAsync<object>(HttpClientConst.HeatHarmony, $"{heatHarmonyUrl}/em/override/disable/{hours}");
                    return Results.Ok();
                }
                catch (Exception ex)
                {
                    app.Logger.LogError(ex, "Error occurred while overriding (disable) EM water heating");
                    return Results.StatusCode(500);
                }
            }).WithName("OverrideEMDisableWaterHeating")
            .RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/em/override/status", async ([FromServices] IRequestProvider requestProvider) =>
            {
                return await requestProvider.GetAsync<object>(HttpClientConst.HeatHarmony, $"{heatHarmonyUrl}/appstatus/ping")
                .ContinueWith(task => task.Result is not null
                ? Results.Ok(task.Result)
                : Results.StatusCode(503));
            }).WithName("GetEMOverrideStatus")
            .RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/heatautomation/status", async ([FromServices] IRequestProvider requestProvider) =>
            {
                return await requestProvider.GetAsync<object>(HttpClientConst.HeatHarmony, $"{heatHarmonyUrl}/status")
                    .ContinueWith(task => task.Result is not null
                        ? Results.Ok(task.Result)
                        : Results.StatusCode(503));
            }).WithName("GetHeatAutomationStatus")
            .RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/heatautomation/tasks", async ([FromServices] IRequestProvider requestProvider) =>
            {
                return await requestProvider.GetAsync<object>(HttpClientConst.HeatHarmony, $"{heatHarmonyUrl}/tasks")
                    .ContinueWith(task => task.Result is not null
                        ? Results.Ok(task.Result)
                        : Results.StatusCode(503));
            }).WithName("GetHeatAutomationTaskStatus")
            .RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/heatautomation/override", async ([FromServices] IRequestProvider requestProvider) =>
            {
                return await requestProvider.GetAsync<object>(HttpClientConst.HeatHarmony, $"{heatHarmonyUrl}/override")
                    .ContinueWith(task => task.Result is not null
                        ? Results.Ok(task.Result)
                        : Results.StatusCode(503));
            }).WithName("GetOverrideStatus")
            .RequireAuthorization();

            heatHarmonyEndpoints.MapPost("/heatautomation/override", async ([FromServices] IRequestProvider requestProvider, TemperatureOverride body) =>
            {
                var response = await requestProvider.PostAsync<TemperatureOverride, object>(
                    HttpClientConst.HeatHarmony,
                    $"{heatHarmonyUrl}/override",
                    body);
                return Results.Accepted();
            })
            .WithName("SetOverrideTemp")
            .RequireAuthorization();

            heatHarmonyEndpoints.MapDelete("/heatautomation/override", async ([FromServices] IRequestProvider requestProvider) =>
            {
                try
                {
                    var response = await requestProvider.DeleteAsync(HttpClientConst.HeatHarmony, $"{heatHarmonyUrl}/override");
                    return Results.Ok(response);
                }
                catch (Exception ex)
                {
                    app.Logger.LogError(ex, "Error occurred while overriding (disable) EM water heating");
                    return Results.StatusCode(500);
                }
            })
                .WithName("CancelOverrideTemp").RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/heishamon/latest", () => Results.Ok())
                .WithName("GetLatestHeishaMonReadings").RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/heishamon/task", () => Results.Ok())
                .WithName("GetHeishaMonProviderTask").RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/heishamon/status", () => Results.Ok())
                .WithName("GetHeishaMonProviderStatus").RequireAuthorization();

            heatHarmonyEndpoints.MapPut("/heishamon/target/{temperature:int}", (int temperature) =>
            {
                if (temperature is < 20 or > 70)
                    return Results.BadRequest("temperature out of range");
                return Results.Accepted();
            }).WithName("SetHeishaMonTargetTemperature").RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/ouman/latest", () => Results.Ok())
                .WithName("GetLatestOumanReadings").RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/ouman/status", () => Results.Ok())
                .WithName("GetOumanStatus").RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/ouman/task", () => Results.Ok())
                .WithName("GetOumanProviderTask").RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/prices/today", () => Results.Ok())
                .WithName("GetTodayPrices").RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/prices/tomorrow", () => Results.Ok())
                .WithName("GetTomorrowPrices").RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/prices/lowperiods/today", () => Results.Ok())
                .WithName("GetTodayLowPeriods").RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/prices/lowperiods/all", () => Results.Ok())
                .WithName("GetAllLowPeriods").RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/prices/nightperiod", () => Results.Ok())
                .WithName("GetNightPeriod").RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/trv/latest", () => Results.Ok())
                .WithName("GetLatestTRVReadings").RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/trv/task", () => Results.Ok())
                .WithName("GetTRVProviderTask").RequireAuthorization();
        }
    }
}
