using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Installation.Models;
using Installation.Models.Helpers;
using Installation.Models.Notify;
using Serilog;

namespace Installation.Communication
{
    public class ServerCommunicator
    {
        public bool ClientConnected { get; set; }
        private bool isPrivileged;
        private List<IPCServer> servers;

        public delegate Task ObjectReceived(object obj);
        public event ObjectReceived OnObjectReceived;
        private string pipeName;

        private CancellationToken cancellationToken;

        private ObjectConverter converter;
        public ServerCommunicator(CancellationToken cancellationToken, string pipeName, bool isPrivileged = false)
        {
            ClientConnected = false;
            this.converter = new ObjectConverter();
            this.pipeName = pipeName;
            this.isPrivileged = isPrivileged;
            this.cancellationToken = cancellationToken;
        }
        public async Task ListenAsync()
        {
            while(true)
            {
                var server = new IPCServer(pipeName, cancellationToken, receiveData);
                Task.Run(() => server.ConnectAsync());
                Task.Run(() => server.ListenAsync());
                servers.Add(server);


            }

        }

        private async Task receiveData(string data)
        {
            await OnObjectReceived(converter.ConvertToObject(data));
        }
        public async Task SendData<T>(Notify<T> notify)
        {
            await sendData(notify, notify.EndpointId);

        }
        public async Task SendData(Command command)
        {
            await sendData(command, command.EndpointId);
        }
        private async Task sendData(object obj, Guid endpointId)
        {
            if (obj == null || endpointId.NullOrEmpty())
                return;
            if (endpointId.IsBroadcast())
            {
                Task.Run(() =>
                servers.ForEach((server) =>
                {
                    server.SendDataAsync(converter.ConvertToString(obj));
                }));
            }
            else
            {
                var server = servers.First(x => x.ConnectionId == endpointId);
                Task.Run(() => server?.SendDataAsync(converter.ConvertToString(obj)));
            }
        }

    }
}
