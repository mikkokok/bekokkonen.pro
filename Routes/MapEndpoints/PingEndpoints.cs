namespace bekokkonen.pro.Routes.MapEndpoints
{
    public static partial class ApiMapper
    {
        public static void MapPingEndpoints(this WebApplication app)
        {
            var pingItems = app.MapGroup("/api/ping").WithTags("PingEndpoints");
            pingItems.MapGet("/ping", () => "pong");
        }
    }
}
