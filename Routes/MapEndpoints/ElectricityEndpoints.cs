using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Identity.Web.Resource;
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

        }
    }
}
