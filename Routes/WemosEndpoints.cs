using Microsoft.AspNetCore.Mvc;

namespace bekokkonen.pro.Routes
{
    public static partial class ApiMapper
    {
        public static void MapWemosEndpoints(this WebApplication app)
        {
            var wemosEndpoints = app.MapGroup("/api/wemos").WithTags("WemosEndpoints").RequireAuthorization();
            wemosEndpoints.MapGet("/history", ([FromServices] MQ.Implementation.MQClient mqClient) =>
            {
                var historyData = mqClient.GetWemosDataHistory();
                return TypedResults.Ok(historyData);
            });
        }
    }
}
