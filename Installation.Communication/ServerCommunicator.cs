using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace Installation.Communication
{
    public class ServerCommunicator : Communicator
    {
        public ServerCommunicator(CancellationToken cancellationToken) : base(cancellationToken)
        {

        }
        public async Task ListenAsync()
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;
                using (pipeStream = new NamedPipeServerStream(pipeName, PipeDirection.InOut, 254, PipeTransmissionMode.Byte, PipeOptions.Asynchronous))
                {
                
                        try
                        {
                            await (pipeStream as NamedPipeServerStream).WaitForConnectionAsync(cancellationToken);
                            if (pipeStream != null)
                            {
                                if (pipeStream.IsConnected)
                                    Log.Debug("Client connected to Pipe");
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Debug(ex, "Waiting for connections aborted (error expected)");
                        }

                        await ReadAsync();
                
                }
            }

        }
    }
}
