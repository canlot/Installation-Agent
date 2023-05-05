using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace Installation.Communication
{
    public class ServerCommunicator : Communicator
    {
        public bool ClientConnected { get; set; }
        public ServerCommunicator(CancellationToken cancellationToken) : base(cancellationToken)
        {
            ClientConnected = false;
        }
        public async Task ListenAsync()
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;
                var pipeSecurity = new PipeSecurity();
                pipeSecurity.AddAccessRule(new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null), PipeAccessRights.ReadWrite, AccessControlType.Allow));
                

                using (pipeStream = new NamedPipeServerStream(pipeName, PipeDirection.InOut, 254, PipeTransmissionMode.Byte, PipeOptions.Asynchronous, 0, 0, pipeSecurity))
                {
                
                        try
                        {
                            await (pipeStream as NamedPipeServerStream).WaitForConnectionAsync(cancellationToken);
                            if (pipeStream != null)
                            {
                                if (pipeStream.IsConnected)
                                {
                                    Log.Debug("Client connected to Pipe");
                                    ClientConnected = true;
                                    
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Debug(ex, "Waiting for connections aborted (error expected)");
                        }

                        await ReadAsync();
                        ClientConnected = false;
                }
            }

        }


    }
}
