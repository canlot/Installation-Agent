using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Installation.Communication
{
    public class ServerCommunicator : Communicator
    {
        public ServerCommunicator(CancellationToken cancellationToken) : base(cancellationToken)
        {

        }
        public async Task ListenAsync()
        {
            using (pipeStream = new NamedPipeServerStream(pipeName, PipeDirection.InOut, 254, PipeTransmissionMode.Byte, PipeOptions.Asynchronous))
            {
                await (pipeStream as NamedPipeServerStream).WaitForConnectionAsync();
                await ReadAsync();
            }
            
        }
    }
}
