using bekokkonen.pro.Global;
using bekokkonen.pro.Global.Config;
using bekokkonen.pro.Models.HeatHarmony;
using bekokkonen.pro.Providers;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace bekokkonen.pro.Routes.MapEndpoints
{
    public static partial class ApiMapper
    {
        public static void MapHeatHarmonyEndpoints(this WebApplication app)
        {
            var heatHarmonyEndpoints = app.MapGroup("/api/heatharmony").WithTags("HeatHarmonyEndpoints");
            var heatHarmonyUrl = GlobalConfig.HeatHarmonyConfig!.BaseUrl;

            static async Task<IResult> ProxyGetNullable<T>(IRequestProvider requestProvider, string clientName, string url)
            {
                var result = await requestProvider.GetAsync<T>(clientName, url);
                return result is not null
                    ? Results.Ok(result)
                    : Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            static async Task<IResult> ProxyGet<T>(IRequestProvider requestProvider, string clientName, string url)
            {
                try
                {
                    var result = await requestProvider.GetAsync<T>(clientName, url);
                    return Results.Ok(result);
                }
                catch
                {
                    return Results.StatusCode(StatusCodes.Status500InternalServerError);
                }
            }

            heatHarmonyEndpoints.MapGet("/appstatus/ping",
                ([FromServices] IRequestProvider requestProvider) =>
                    ProxyGetNullable<PingResponse>(
                        requestProvider,
                        HttpClientConst.HeatHarmony,
                        $"{heatHarmonyUrl}/appstatus/ping"))
                .WithName("GetAppHealthStatus")
                .Produces<PingResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status503ServiceUnavailable)
                .RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/appstatus/uptime",
                ([FromServices] IRequestProvider requestProvider) =>
                    ProxyGetNullable<UptimeResponse>(
                        requestProvider,
                        HttpClientConst.HeatHarmony,
                        $"{heatHarmonyUrl}/appstatus/uptime"))
                .WithName("GetAppUptime")
                .Produces<UptimeResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status503ServiceUnavailable)
                .RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/em/latest",
                ([FromServices] IRequestProvider requestProvider) =>
                    ProxyGetNullable<EmLatestResponse>(
                        requestProvider,
                        HttpClientConst.HeatHarmony,
                        $"{heatHarmonyUrl}/em/latest"))
                .WithName("GetLatestEM")
                .Produces<EmLatestResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status503ServiceUnavailable)
                .RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/em/changes",
                ([FromServices] IRequestProvider requestProvider) =>
                    ProxyGetNullable<EmChangesResponse>(
                        requestProvider,
                        HttpClientConst.HeatHarmony,
                        $"{heatHarmonyUrl}/em/changes"))
                .WithName("GetEMChanges")
                .Produces<EmChangesResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status503ServiceUnavailable)
                .RequireAuthorization();

            heatHarmonyEndpoints.MapPost("/em/enable",
                async ([FromServices] IRequestProvider requestProvider) =>
                {
                    try
                    {
                        await requestProvider.PostAsync(HttpClientConst.HeatHarmony, $"{heatHarmonyUrl}/em/enable");
                        return Results.Accepted();
                    }
                    catch (Exception ex)
                    {
                        app.Logger.LogError(ex, "Error occurred while enabling EM water heating");
                        return Results.StatusCode(StatusCodes.Status500InternalServerError);
                    }
                })
                .WithName("EnableEMWaterHeating")
                .Produces(StatusCodes.Status202Accepted)
                .Produces(StatusCodes.Status500InternalServerError)
                .RequireAuthorization();

            heatHarmonyEndpoints.MapPost("/em/disable",
                async ([FromServices] IRequestProvider requestProvider) =>
                {
                    try
                    {
                        await requestProvider.PostAsync(HttpClientConst.HeatHarmony, $"{heatHarmonyUrl}/em/disable");
                        return Results.Accepted();
                    }
                    catch (Exception ex)
                    {
                        app.Logger.LogError(ex, "Error occurred while disabling EM water heating");
                        return Results.StatusCode(StatusCodes.Status500InternalServerError);
                    }
                })
                .WithName("DisableEMWaterHeating")
                .Produces(StatusCodes.Status202Accepted)
                .Produces(StatusCodes.Status500InternalServerError)
                .RequireAuthorization();

            heatHarmonyEndpoints.MapDelete("/em/override/delete",
                async ([FromServices] IRequestProvider requestProvider) =>
                {
                    try
                    {
                        await requestProvider.DeleteAsync<object>(HttpClientConst.HeatHarmony, $"{heatHarmonyUrl}/em/override/delete");
                        return Results.Ok();
                    }
                    catch (Exception ex)
                    {
                        app.Logger.LogError(ex, "Error occurred while removing EM water heating override");
                        return Results.StatusCode(StatusCodes.Status500InternalServerError);
                    }
                })
                .WithName("ClearEMOverride")
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status500InternalServerError)
                .RequireAuthorization();

            heatHarmonyEndpoints.MapPost("/em/override/enable/{hours:int}",
                async ([FromServices] IRequestProvider requestProvider, int hours) =>
                {
                    try
                    {
                        var response = await requestProvider.PostAsync<EmOverrideResultResponse>(
                            HttpClientConst.HeatHarmony,
                            $"{heatHarmonyUrl}/em/override/enable/{hours}");
                        return Results.Accepted(null, response);
                    }
                    catch (Exception ex)
                    {
                        app.Logger.LogError(ex, "Error occurred while overriding (enable) EM water heating");
                        return Results.StatusCode(StatusCodes.Status500InternalServerError);
                    }
                })
                .WithName("OverrideEMEnableWaterHeating")
                .Produces<EmOverrideResultResponse>(StatusCodes.Status202Accepted)
                .Produces(StatusCodes.Status500InternalServerError)
                .RequireAuthorization();

            heatHarmonyEndpoints.MapPost("/em/override/disable/{hours:int}",
                async ([FromServices] IRequestProvider requestProvider, int hours) =>
                {
                    try
                    {
                        var response = await requestProvider.PostAsync<EmOverrideResultResponse>(
                            HttpClientConst.HeatHarmony,
                            $"{heatHarmonyUrl}/em/override/disable/{hours}");
                        return Results.Accepted(null, response);
                    }
                    catch (Exception ex)
                    {
                        app.Logger.LogError(ex, "Error occurred while overriding (disable) EM water heating");
                        return Results.StatusCode(StatusCodes.Status500InternalServerError);
                    }
                })
                .WithName("OverrideEMDisableWaterHeating")
                .Produces<EmOverrideResultResponse>(StatusCodes.Status202Accepted)
                .Produces(StatusCodes.Status500InternalServerError)
                .RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/em/override/status",
                ([FromServices] IRequestProvider requestProvider) =>
                    ProxyGetNullable<EmOverrideStatusResponse>(
                        requestProvider,
                        HttpClientConst.HeatHarmony,
                        $"{heatHarmonyUrl}/em/override/status"))
                .WithName("GetEMOverrideStatus")
                .Produces<EmOverrideStatusResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status503ServiceUnavailable)
                .RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/heatautomation/status",
                ([FromServices] IRequestProvider requestProvider) =>
                    ProxyGetNullable<HeatAutomationStatusResponse>(
                        requestProvider,
                        HttpClientConst.HeatHarmony,
                        $"{heatHarmonyUrl}/heatautomation/status"))
                .WithName("GetHeatAutomationStatus")
                .Produces<HeatAutomationStatusResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status503ServiceUnavailable)
                .RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/heatautomation/tasks",
                ([FromServices] IRequestProvider requestProvider) =>
                    ProxyGetNullable<HeatAutomationTasksResponse>(
                        requestProvider,
                        HttpClientConst.HeatHarmony,
                        $"{heatHarmonyUrl}/heatautomation/tasks"))
                .WithName("GetHeatAutomationTaskStatus")
                .Produces<HeatAutomationTasksResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status503ServiceUnavailable)
                .RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/heatautomation/override",
                ([FromServices] IRequestProvider requestProvider) =>
                    ProxyGetNullable<HeatAutomationOverrideResponse>(
                        requestProvider,
                        HttpClientConst.HeatHarmony,
                        $"{heatHarmonyUrl}/heatautomation/override"))
                .WithName("GetOverrideStatus")
                .Produces<HeatAutomationOverrideResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status503ServiceUnavailable)
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
                .Produces<HeatAutomationOverrideResponse>(StatusCodes.Status202Accepted)
                .Produces<HeatAutomationErrorResponse>(StatusCodes.Status400BadRequest)
                .Produces<HeatAutomationErrorResponse>(StatusCodes.Status409Conflict)
                .Produces(StatusCodes.Status500InternalServerError)
                .RequireAuthorization();

            heatHarmonyEndpoints.MapDelete("/heatautomation/override",
                async ([FromServices] IRequestProvider requestProvider) =>
                {
                    try
                    {
                        var response = await requestProvider.DeleteAsync<HeatAutomationRemoveOverrideResponse>(
                            HttpClientConst.HeatHarmony,
                            $"{heatHarmonyUrl}/heatautomation/override");
                        return Results.Ok(response);
                    }
                    catch (Exception ex)
                    {
                        app.Logger.LogError(ex, "Error occurred while cancelling HeatAutomation override");
                        return Results.StatusCode(StatusCodes.Status500InternalServerError);
                    }
                })
                .WithName("CancelOverrideTemp")
                .Produces<HeatAutomationRemoveOverrideResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status500InternalServerError)
                .RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/heishamon/latest",
                ([FromServices] IRequestProvider requestProvider) =>
                    ProxyGet<HeishaMonLatestResponse>(
                        requestProvider,
                        HttpClientConst.HeatHarmony,
                        $"{heatHarmonyUrl}/heishamon/latest"))
                .WithName("GetLatestHeishaMonReadings")
                .Produces<HeishaMonLatestResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status500InternalServerError)
                .RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/heishamon/task",
                ([FromServices] IRequestProvider requestProvider) =>
                    ProxyGet<HeishaMonTaskResponse>(
                        requestProvider,
                        HttpClientConst.HeatHarmony,
                        $"{heatHarmonyUrl}/heishamon/task"))
                .WithName("GetHeishaMonProviderTask")
                .Produces<HeishaMonTaskResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status500InternalServerError)
                .RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/heishamon/status",
                ([FromServices] IRequestProvider requestProvider) =>
                    ProxyGet<HeishaMonStatusResponse>(
                        requestProvider,
                        HttpClientConst.HeatHarmony,
                        $"{heatHarmonyUrl}/heishamon/status"))
                .WithName("GetHeishaMonProviderStatus")
                .Produces<HeishaMonStatusResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status500InternalServerError)
                .RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/ouman/latest",
                ([FromServices] IRequestProvider requestProvider) =>
                    ProxyGet<OumanLatestResponse>(
                        requestProvider,
                        HttpClientConst.HeatHarmony,
                        $"{heatHarmonyUrl}/ouman/latest"))
                .WithName("GetLatestOumanReadings")
                .Produces<OumanLatestResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status500InternalServerError)
                .RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/ouman/status",
                ([FromServices] IRequestProvider requestProvider) =>
                    ProxyGet<OumanStatusResponse>(
                        requestProvider,
                        HttpClientConst.HeatHarmony,
                        $"{heatHarmonyUrl}/ouman/status"))
                .WithName("GetOumanStatus")
                .Produces<OumanStatusResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status500InternalServerError)
                .RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/ouman/task",
                ([FromServices] IRequestProvider requestProvider) =>
                    ProxyGet<OumanTaskResponse>(
                        requestProvider,
                        HttpClientConst.HeatHarmony,
                        $"{heatHarmonyUrl}/ouman/task"))
                .WithName("GetOumanProviderTask")
                .Produces<OumanTaskResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status500InternalServerError)
                .RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/prices/today",
                ([FromServices] IRequestProvider requestProvider) =>
                    ProxyGet<PriceTodayResponse>(
                        requestProvider,
                        HttpClientConst.HeatHarmony,
                        $"{heatHarmonyUrl}/prices/today"))
                .WithName("GetTodayPrices")
                .Produces<PriceTodayResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status500InternalServerError)
                .RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/prices/tomorrow",
                ([FromServices] IRequestProvider requestProvider) =>
                    ProxyGet<PriceTomorrowResponse>(
                        requestProvider,
                        HttpClientConst.HeatHarmony,
                        $"{heatHarmonyUrl}/prices/tomorrow"))
                .WithName("GetTomorrowPrices")
                .Produces<PriceTomorrowResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status500InternalServerError)
                .RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/prices/lowperiods/today",
                ([FromServices] IRequestProvider requestProvider) =>
                    ProxyGet<TodayLowPeriodsResponse>(
                        requestProvider,
                        HttpClientConst.HeatHarmony,
                        $"{heatHarmonyUrl}/prices/lowperiods/today"))
                .WithName("GetTodayLowPeriods")
                .Produces<TodayLowPeriodsResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status500InternalServerError)
                .RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/prices/lowperiods/all",
                ([FromServices] IRequestProvider requestProvider) =>
                    ProxyGet<AllLowPeriodsResponse>(
                        requestProvider,
                        HttpClientConst.HeatHarmony,
                        $"{heatHarmonyUrl}/prices/lowperiods/all"))
                .WithName("GetAllLowPeriods")
                .Produces<AllLowPeriodsResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status500InternalServerError)
                .RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/prices/nightperiod",
                ([FromServices] IRequestProvider requestProvider) =>
                    ProxyGet<NightPeriodResponse>(
                        requestProvider,
                        HttpClientConst.HeatHarmony,
                        $"{heatHarmonyUrl}/prices/nightperiod"))
                .WithName("GetNightPeriod")
                .Produces<NightPeriodResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status500InternalServerError)
                .RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/trv/latest",
                ([FromServices] IRequestProvider requestProvider) =>
                    ProxyGet<TRVLatestResponse>(
                        requestProvider,
                        HttpClientConst.HeatHarmony,
                        $"{heatHarmonyUrl}/trv/latest"))
                .WithName("GetLatestTRVReadings")
                .Produces<TRVLatestResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status500InternalServerError)
                .RequireAuthorization();

            heatHarmonyEndpoints.MapGet("/trv/task",
                ([FromServices] IRequestProvider requestProvider) =>
                    ProxyGet<TRVTaskResponse>(
                        requestProvider,
                        HttpClientConst.HeatHarmony,
                        $"{heatHarmonyUrl}/trv/task"))
                .WithName("GetTRVProviderTask")
                .Produces<TRVTaskResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status500InternalServerError)
                .RequireAuthorization();
        }
    }
}