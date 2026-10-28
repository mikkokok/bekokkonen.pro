using bekokkonen.pro.Models.HeatHarmony;
using bekokkonen.pro.Providers;
using Microsoft.AspNetCore.Mvc;

namespace bekokkonen.pro.Routes
{
    public static partial class ApiMapper
    {
        public static void MapRestlessFalconEndpoints(this WebApplication app)
        {
            var restlessFalconEndpoints = app.MapGroup("/api/restlessfalcon").WithTags("RestlessFalconEndpoints").RequireAuthorization();

            restlessFalconEndpoints.MapGet("/prices", async Task<IResult> (
                [FromServices] RestlessFalconProvider provider,
                [FromQuery] int days = 0) =>
            {
                if (days < 0)
                {
                    return Results.BadRequest("Days must be zero or greater.");
                }

                try
                {
                    var prices = await provider.GetElectricityPrices(days);
                    return Results.Ok(prices);
                }
                catch
                {
                    return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
                }
            })
            .WithName("GetRestlessFalconElectricityPrices")
            .Produces<IEnumerable<ElectricityPrice>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status503ServiceUnavailable);
        }
    }
}
