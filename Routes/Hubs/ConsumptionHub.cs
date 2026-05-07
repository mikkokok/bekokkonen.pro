using bekokkonen.pro.Models;
using Microsoft.AspNetCore.SignalR;

namespace bekokkonen.pro.Routes.Hubs
{
    public class ConsumptionHub : Hub<IConsumptionHubClient>
    {
    }
}
