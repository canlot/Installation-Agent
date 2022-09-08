using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Installation.Communication
{
    public class IPCClient : IPCBase
    {
        public delegate Task ClientConnected();
        public delegate Task ClientDisconnected();
        public event ClientConnected OnClientConnected;
        public event ClientDisconnected OnClientDisconnected;

        public IPCClient(string pipeName, CancellationToken cancellationToken, Func<string, Task> receivedData) : base(pipeName, cancellationToken, receivedData)
        {

        }

        public async Task ConnectAsync()
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;
                using (pipeStream = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous))
                {
                    try
                    {
                        await (pipeStream as NamedPipeClientStream).ConnectAsync(cancellationToken);
                    }
                    catch (Exception ex)
                    {

                    }
                    await OnClientConnected();
                    await ReadAsync();
                    await OnClientDisconnected();
                }
            }
        }
    }
}
