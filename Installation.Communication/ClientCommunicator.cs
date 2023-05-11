using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Installation.Communication
{
    public class ClientCommunicator
    {
        public delegate Task ClientConnected();
        public delegate Task ClientDisconnected();
        public event ClientConnected OnClientConnected;
        public event ClientDisconnected OnClientDisconnected;

        private IPCClient client;

        public ClientCommunicator(string pipeName, CancellationToken cancellationToken)
        {
            client = new IPCClient(pipeName, cancellationToken, receiveDataAsync);
            client.OnClientDisconnected += onClientDisconnectedAsync;
            client.OnClientConnected += onClientConnectedAsync;
        }

        public async Task ConnectAsync()
        {
            await client.ConnectAsync();
        }
        public async Task ListenAsync()
        {
            await client?.ListenAsync();
        }
        public async Task receiveDataAsync(string data, Guid endpointId)
        {

        }
        private async Task onClientDisconnectedAsync()
        {

        }
        private async Task onClientConnectedAsync()
        {

        }
    }
}
