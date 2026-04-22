using Microsoft.Identity.Web.Resource;
using bekokkonen.pro.MQ.Implementation;
using Microsoft.AspNetCore.Mvc;
using bekokkonen.pro.Routes.Hubs;
using bekokkonen.pro.Models;
namespace bekokkonen.pro.Routes.MapEndpoints
{
    public static partial class ApiMapper
    {
        public static void MapElectricityEndpoints(this WebApplication app)
        {
            var scopeRequiredByApi = app.Configuration["AzureAd:Scopes"] ?? "";
            var electricityEndpoints = app.MapGroup("/api/electricity").WithTags("ElectricityEndpoints").RequireAuthorization();
            electricityEndpoints.MapGet("/task", ([FromServices] MQClient mqClient, HttpContext httpContext) =>
            {
                httpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);
                return TypedResults.Ok(mqClient.Initialization?.Exception?.Message);
            })
            .WithName("GetMQClientTask");
            electricityEndpoints.MapGet("/consumption/history", ([FromServices] MQClient mqClient, HttpContext httpContext) =>
            {
                httpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);
                var historyData = mqClient.GetConsumptionDataHistory();
                return TypedResults.Ok(historyData);
            })
            .WithName("GetConsumptionHistory");
            electricityEndpoints.MapHub<ConsumptionHub>("/consumption");
            electricityEndpoints.MapGet("/status", ([FromServices] MQClient mqClient) =>
            {
                return TypedResults.Ok(mqClient.Status);
            })
            .WithName("GetMQClientStatus")
            .Produces<MQStatusEnum>(StatusCodes.Status200OK);
        }
    }
}
