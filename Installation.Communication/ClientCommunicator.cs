using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Installation.Communication
{
    public class ClientCommunicator : Communicator
    {
        public delegate Task ClientConnected();
        public event ClientConnected OnClientConnected;
        public ClientCommunicator(CancellationToken cancellationToken) : base(cancellationToken)
        {

        }

        public async Task ConnectAsync()
        {
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

            }
        }
    }
}
