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
using Installation.Models.Interfaces;
using Installation.Models.Notify;
using Serilog;

namespace Installation.Communication
{
    public class ServerCommunicator
    {
        public bool ClientConnected { get; set; }
        private bool isPrivileged;
        private List<IPCServer> servers = new List<IPCServer>();

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
                await server.ConnectAsync();
                Task.Run(() => server.ListenAsync());
                servers.Add(server);

            }

        }

        private async Task receiveData(string data, Guid endpointId)
        {
            var dataObject = converter.ConvertToObject(data);
            if(dataObject is Command && dataObject is IExternal)
            {
                var command = (IExternal)dataObject;
                command.EndpointId = endpointId;
                if (command.IsPrivilegedCommand)
                    if (isPrivileged)
                        await OnObjectReceived(command);
                    else
                        return;
                else
                    await OnObjectReceived(command);

            }
        }
        public async Task SendData<T>(Notify<T> notify)
        {
            await sendData(notify, notify.EndpointId);

        }
        public async Task SendData(IExternal command)
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
                var server = servers.First(x => x.endpointId == endpointId);
                Task.Run(() => server?.SendDataAsync(converter.ConvertToString(obj)));
            }
        }

    }
}
