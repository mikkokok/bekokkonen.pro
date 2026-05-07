using bekokkonen.pro.Models;

namespace bekokkonen.pro.Routes.Hubs
{
    public interface IConsumptionHubClient
    {
        Task BroadcastConsumptionData(ConsumptionData payload);
    }
}