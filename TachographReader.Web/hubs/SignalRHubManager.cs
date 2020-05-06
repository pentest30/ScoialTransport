using Microsoft.AspNetCore.SignalR;

namespace TachographReader.Web.hubs
{
    public static class SignalRHubManager
    {
        public static string ConnectionId { get; set; }
        public static IHubCallerClients Clients { get; set; }
    }
}
