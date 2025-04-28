using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Identity.Web.Resource;
using bekokkonen.pro.MQ.Implementation;
using NuGet.Protocol;
using Microsoft.AspNetCore.Mvc;
using bekokkonen.pro.Routes.Hubs;
namespace bekokkonen.pro.Routes.MapEndpoints
{
    public static partial class ApiMapper
    {
        public static void MapElectricityEndpoints(this WebApplication app)
        {
            var scopeRequiredByApi = app.Configuration["AzureAd:Scopes"] ?? "";
            var electricityItems = app.MapGroup("/api/electricity").WithTags("ElectricityEndpoints");
            electricityItems.MapGet("/", (HttpContext httpContext) =>
            {
                httpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);
                return TypedResults.Ok("jees");
            })
            .WithName("GetAllElectricityEndpoints")
            .WithOpenApi()
            .RequireAuthorization();

            electricityItems.MapGet("/task", ([FromServices] MQClient mqClient, HttpContext httpContext) =>
            {
                httpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);
                return TypedResults.Ok(mqClient.Initialization.Exception?.Message);
            })
            .WithName("GetMQClientTask")
            .WithOpenApi()
            .RequireAuthorization();

            electricityItems.MapHub<ConsumptionHub>("/consumption")
                .WithOpenApi()
                .RequireAuthorization();
        }
    }
}
