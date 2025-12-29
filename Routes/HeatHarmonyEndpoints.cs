using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using bekokkonen.pro.Providers;
using bekokkonen.pro.Global;
using bekokkonen.pro.Models.HeatHarmony;
using bekokkonen.pro.Global.Config;
using System.Text.Json;

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
                return requestProvider.GetAsync<PingResponse>(HttpClientConst.HeatHarmony, $"{heatHarmonyUrl}/appstatus/ping")
                    .ContinueWith(task => task.Result is not null
                        ? Results.Ok(task.Result)
                        : Results.StatusCode(503));
            }).WithName("GetAppHealthStatus")
              .RequireAuthorization();
            heatHarmonyEndpoints.MapGet("/appstatus/uptime", ([FromServices] IRequestProvider requestProvider) =>
            {
                return requestProvider.GetAsync<UptimeResponse>(HttpClientConst.HeatHarmony, $"{heatHarmonyUrl}/appstatus/uptime")
                    .ContinueWith(task => task.Result is not null
                        ? Results.Ok(task.Result)
                        : Results.StatusCode(503));
            }).WithName("GetAppUptime")
            .RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/em/latest", ([FromServices] IRequestProvider requestProvider) =>
            {
                return requestProvider.GetAsync<EmLatestResponse>(HttpClientConst.HeatHarmony, $"{heatHarmonyUrl}/em/latest")
                    .ContinueWith(task => task.Result is not null
                        ? Results.Ok(task.Result)
                        : Results.StatusCode(503));
            }).WithName("GetLatestEM")
            .RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/em/changes", ([FromServices] IRequestProvider requestProvider) =>
            {
                return requestProvider.GetAsync<IEnumerable<HarmonyChange>>(HttpClientConst.HeatHarmony, $"{heatHarmonyUrl}/em/changes")
                    .ContinueWith(task => task.Result is not null
                        ? Results.Ok(task.Result)
                        : Results.StatusCode(503));
            }).WithName("GetEMChanges")
            .RequireAuthorization();

            heatHarmonyEndpoints.MapPost("/em/enable", async ([FromServices] IRequestProvider requestProvider) =>
            {
                try
                {
                    await requestProvider.PostAsync(HttpClientConst.HeatHarmony, $"{heatHarmonyUrl}/em/enable");
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
                    await requestProvider.PostAsync(HttpClientConst.HeatHarmony, $"{heatHarmonyUrl}/em/disable");
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
                    var response = await requestProvider.PostAsync<EmOverrideGetResponse>(HttpClientConst.HeatHarmony, $"{heatHarmonyUrl}/em/override/enable/{hours}");
                    return Results.Ok(response);
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
                    var response = await requestProvider.PostAsync<EmOverrideGetResponse>(HttpClientConst.HeatHarmony, $"{heatHarmonyUrl}/em/override/disable/{hours}");
                    return Results.Ok(response);
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
                return await requestProvider.GetAsync<EmOverrideStatus>(HttpClientConst.HeatHarmony, $"{heatHarmonyUrl}/em/override/status")
                .ContinueWith(task => task.Result is not null
                ? Results.Ok(task.Result)
                : Results.StatusCode(503));
            }).WithName("GetEMOverrideStatus")
            .RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/heatautomation/status", async ([FromServices] IRequestProvider requestProvider) =>
            {
                return await requestProvider.GetAsync<HeatAutomationStatusResponse>(HttpClientConst.HeatHarmony, $"{heatHarmonyUrl}/heatautomation/status")
                    .ContinueWith(task => task.Result is not null
                        ? Results.Ok(task.Result)
                        : Results.StatusCode(503));
            }).WithName("GetHeatAutomationStatus")
            .RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/heatautomation/tasks", async ([FromServices] IRequestProvider requestProvider) =>
            {
                return await requestProvider.GetAsync<HeatAutomationTasksResponse>(HttpClientConst.HeatHarmony, $"{heatHarmonyUrl}/heatautomation/tasks")
                    .ContinueWith(task => task.Result is not null
                        ? Results.Ok(task.Result)
                        : Results.StatusCode(503));
            }).WithName("GetHeatAutomationTaskStatus")
            .RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/heatautomation/override", async ([FromServices] IRequestProvider requestProvider) =>
            {
                return await requestProvider.GetAsync<HeatAutomationOverrideResponse>(HttpClientConst.HeatHarmony, $"{heatHarmonyUrl}/heatautomation/override")
                    .ContinueWith(task => task.Result is not null
                        ? Results.Ok(task.Result)
                        : Results.StatusCode(503));
            }).WithName("GetOverrideStatus")
            .RequireAuthorization();

            heatHarmonyEndpoints.MapPost("/heatautomation/override",
                async ([FromServices] IRequestProvider requestProvider, TemperatureOverride body) =>
                {
                    try
                    {
                        var (statusCode, content) = await requestProvider.PostRawAsync(
                            HttpClientConst.HeatHarmony,
                            $"{heatHarmonyUrl}/heatautomation/override",
                            body);

                        if (string.IsNullOrWhiteSpace(content))
                        {
                            return Results.StatusCode(statusCode);
                        }

                        if (statusCode == StatusCodes.Status202Accepted)
                        {
                            var okPayload = JsonSerializer.Deserialize<HeatAutomationOverrideResponse>(content);
                            return TypedResults.Accepted(string.Empty, okPayload);
                        }

                        if (statusCode is StatusCodes.Status400BadRequest or StatusCodes.Status409Conflict)
                        {
                            var errorPayload = JsonSerializer.Deserialize<HeatAutomationErrorResponse>(content);
                            return TypedResults.Json(errorPayload, statusCode: statusCode);
                        }

                        var unknown = JsonSerializer.Deserialize<object>(content);
                        return TypedResults.Json(unknown, statusCode: statusCode);
                    }
                    catch (Exception ex)
                    {
                        app.Logger.LogError(ex, "Error occurred while proxying HeatAutomation override");
                        return Results.StatusCode(StatusCodes.Status500InternalServerError);
                    }
                })
                .WithName("SetOverrideTemp")
                .RequireAuthorization();

            heatHarmonyEndpoints.MapDelete("/heatautomation/override", async ([FromServices] IRequestProvider requestProvider) =>
            {
                try
                {
                    var response = await requestProvider.DeleteAsync<HeatAutomationRemoveOverrideResponse>(HttpClientConst.HeatHarmony, $"{heatHarmonyUrl}/heatautomation/override");
                    return Results.Ok(response);
                }
                catch (Exception ex)
                {
                    app.Logger.LogError(ex, "Error occurred while overriding (disable) EM water heating");
                    return Results.StatusCode(500);
                }
            })
            .WithName("CancelOverrideTemp")
            .RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/heishamon/latest", async ([FromServices] IRequestProvider requestProvider) =>
            {
                try
                {
                    var response = await requestProvider.GetAsync<object>(HttpClientConst.HeatHarmony, $"{heatHarmonyUrl}/heishamon/latest");
                    return Results.Ok(response);
                }
                catch (Exception ex)
                {
                    app.Logger.LogError(ex, "Error occurred while fetching latest HeishaMon readings");
                    return Results.StatusCode(500);
                }
            })
            .WithName("GetLatestHeishaMonReadings")
            .RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/heishamon/task", async ([FromServices] IRequestProvider requestProvider) =>
            {
                try
                {
                    var response = await requestProvider.GetAsync<object>(HttpClientConst.HeatHarmony, $"{heatHarmonyUrl}/heishamon/task");
                    return Results.Ok(response);
                }
                catch (Exception ex)
                {
                    app.Logger.LogError(ex, "Error occurred while fetching HeishaMon task");
                    return Results.StatusCode(500);
                }
            })
            .WithName("GetHeishaMonProviderTask")
            .RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/heishamon/status", async ([FromServices] IRequestProvider requestProvider) =>
            {
                try
                {
                    var response = await requestProvider.GetAsync<object>(HttpClientConst.HeatHarmony, $"{heatHarmonyUrl}/heishamon/status");
                    return Results.Ok(response);
                }
                catch (Exception ex)
                {
                    app.Logger.LogError(ex, "Error occurred while fetching HeishaMon status");
                    return Results.StatusCode(500);
                }
            })
            .WithName("GetHeishaMonProviderStatus")
            .RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/ouman/latest", async ([FromServices] IRequestProvider requestProvider) =>
            {
                try
                {
                    var response = await requestProvider.GetAsync<object>(HttpClientConst.HeatHarmony, $"{heatHarmonyUrl}/ouman/latest");
                    return Results.Ok(response);
                }
                catch (Exception ex)
                {
                    app.Logger.LogError(ex, "Error occurred while fetching latest Ouman readings");
                    return Results.StatusCode(500);
                }
            })
            .WithName("GetLatestOumanReadings")
            .RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/ouman/status", async ([FromServices] IRequestProvider requestProvider) =>
            {
                try
                {
                    var response = await requestProvider.GetAsync<object>(HttpClientConst.HeatHarmony, $"{heatHarmonyUrl}/ouman/status");
                    return Results.Ok(response);
                }
                catch (Exception ex)
                {
                    app.Logger.LogError(ex, "Error occurred while fetching Ouman status");
                    return Results.StatusCode(500);
                }
            })
            .WithName("GetOumanStatus")
            .RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/ouman/task", async ([FromServices] IRequestProvider requestProvider) =>
            {
                try
                {
                    var response = await requestProvider.GetAsync<object>(HttpClientConst.HeatHarmony, $"{heatHarmonyUrl}/ouman/task");
                    return Results.Ok(response);
                }
                catch (Exception ex)
                {
                    app.Logger.LogError(ex, "Error occurred while fetching Ouman task");
                    return Results.StatusCode(500);
                }
            })
            .WithName("GetOumanProviderTask")
            .RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/prices/today", async ([FromServices] IRequestProvider requestProvider) =>
            {
                try
                {
                    var response = await requestProvider.GetAsync<object>(HttpClientConst.HeatHarmony, $"{heatHarmonyUrl}/prices/today");
                    return Results.Ok(response);
                }
                catch (Exception ex)
                {
                    app.Logger.LogError(ex, "Error occurred while fetching today's prices");
                    return Results.StatusCode(500);
                }
            })
            .WithName("GetTodayPrices")
            .RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/prices/tomorrow", async ([FromServices] IRequestProvider requestProvider) =>
            {
                try
                {
                    var response = await requestProvider.GetAsync<object>(HttpClientConst.HeatHarmony, $"{heatHarmonyUrl}/prices/tomorrow");
                    return Results.Ok(response);
                }
                catch (Exception ex)
                {
                    app.Logger.LogError(ex, "Error occurred while fetching tomorrow's prices");
                    return Results.StatusCode(500);
                }
            })
            .WithName("GetTomorrowPrices")
            .RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/prices/lowperiods/today", async ([FromServices] IRequestProvider requestProvider) =>
            {
                try
                {
                    var response = await requestProvider.GetAsync<object>(HttpClientConst.HeatHarmony, $"{heatHarmonyUrl}/prices/lowperiods/today");
                    return Results.Ok(response);
                }
                catch (Exception ex)
                {
                    app.Logger.LogError(ex, "Error occurred while fetching today's low periods");
                    return Results.StatusCode(500);
                }
            })
            .WithName("GetTodayLowPeriods")
            .RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/prices/lowperiods/all", async ([FromServices] IRequestProvider requestProvider) =>
            {
                try
                {
                    var response = await requestProvider.GetAsync<object>(HttpClientConst.HeatHarmony, $"{heatHarmonyUrl}/prices/lowperiods/all");
                    return Results.Ok(response);
                }
                catch (Exception ex)
                {
                    app.Logger.LogError(ex, "Error occurred while fetching all low periods");
                    return Results.StatusCode(500);
                }
            })
            .WithName("GetAllLowPeriods")
            .RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/prices/nightperiod", async ([FromServices] IRequestProvider requestProvider) =>
            {
                try
                {
                    var response = await requestProvider.GetAsync<object>(HttpClientConst.HeatHarmony, $"{heatHarmonyUrl}/prices/nightperiod");
                    return Results.Ok(response);
                }
                catch (Exception ex)
                {
                    app.Logger.LogError(ex, "Error occurred while fetching night period prices");
                    return Results.StatusCode(500);
                }
            })
            .WithName("GetNightPeriod")
            .RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/trv/latest", async ([FromServices] IRequestProvider requestProvider) =>
            {
                try
                {
                    var response = await requestProvider.GetAsync<object>(HttpClientConst.HeatHarmony, $"{heatHarmonyUrl}/trv/latest");
                    return Results.Ok(response);
                }
                catch (Exception ex)
                {
                    app.Logger.LogError(ex, "Error occurred while fetching latest TRV readings");
                    return Results.StatusCode(500);
                }
            })
            .WithName("GetLatestTRVReadings")
            .RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/trv/task", async ([FromServices] IRequestProvider requestProvider) =>
            {
                try
                {
                    var response = await requestProvider.GetAsync<object>(HttpClientConst.HeatHarmony, $"{heatHarmonyUrl}/trv/task");
                    return Results.Ok(response);
                }
                catch (Exception ex)
                {
                    app.Logger.LogError(ex, "Error occurred while fetching TRV provider task");
                    return Results.StatusCode(500);
                }
            })
            .WithName("GetTRVProviderTask")
            .RequireAuthorization();
        }
    }
}
