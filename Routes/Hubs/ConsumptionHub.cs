using Microsoft.AspNetCore.SignalR;

namespace bekokkonen.pro.Routes.Hubs
{
    public class ConsumptionHub : Hub
    {
        public async Task SendReturnDelivery(string payload)
        {
            await Clients.All.SendAsync("broadcastReturnDelivery", payload);

        }
        public async Task SendActualConsumption(string payload)
        {
            await Clients.All.SendAsync("broadcastActualConsumption", payload);

        }
    }
}
