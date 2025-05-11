using bekokkonen.pro.Models;
using Microsoft.AspNetCore.SignalR;

namespace bekokkonen.pro.Routes.Hubs
{
    public class ConsumptionHub : Hub
    {
        public async Task SendConsumptionData(ConsumptionData payload)
        {
            await Clients.All.SendAsync("broadcastConsumptionData", payload);

        }
    }
}
