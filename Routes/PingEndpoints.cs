namespace bekokkonen.pro.Routes.MapEndpoints
{
    public static partial class ApiMapper
    {
        public static void MapPingEndpoints(this WebApplication app)
        {
            var pingItems = app.MapGroup("/api/ping")
                .WithTags("PingEndpoints");

            pingItems.MapGet(
                "/",
                () => Results.Ok(new
                {
                    status = "pong",
                    timestamp = DateTimeOffset.UtcNow
                }))
                .WithName("Ping")
                .WithOpenApi(operation =>
                {
                    operation.Summary = "Simple liveness endpoint";
                    operation.Description = "Returns a basic response to indicate the API is reachable.";
                    return operation;
                }).RequireAuthorization();
        }
    }
}
