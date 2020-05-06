using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using tacchograaph_reader.Core.Commands.DddFiles;

namespace TachographReader.Web.hubs
{
    public class SocialDataHub :Hub, INotificationHandler<ProgressNotification>
    {
        public Task SendProgress( decimal value)
        {
            return Clients.Client(Context.ConnectionId).SendAsync("ReceiveProgress", value);
        }

        public Task Handle(ProgressNotification notification, CancellationToken cancellationToken)
        {
            return SignalRHubManager.Clients.Client(SignalRHubManager.ConnectionId).SendAsync("ReceiveProgress", notification.Value);
        
        }

        public override Task OnConnectedAsync()
        {
            SignalRHubManager.ConnectionId = Context.ConnectionId;
            SignalRHubManager.Clients = Clients;
            return base.OnConnectedAsync();
        }
    }
}
