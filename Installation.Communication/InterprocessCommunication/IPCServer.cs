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
    public class IPCServer : IPCBase
    {
        public bool ClientConnected { get; set; }
        public Guid ConnectionId = new Guid();
        private PipeSecurity pipeSecurity = new PipeSecurity();
        public IPCServer(string pipeName, CancellationToken cancellationToken, 
            Func<string, Task> receivedData, bool privilegedCommunication = false) : base(pipeName, cancellationToken, receivedData)
        {
            ClientConnected = false;
            
            if(privilegedCommunication) 
                pipeSecurity.AddAccessRule(new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.AccountAdministratorSid, null), PipeAccessRights.ReadWrite, AccessControlType.Allow));
            else
                pipeSecurity.AddAccessRule(new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null), PipeAccessRights.ReadWrite, AccessControlType.Allow));

        }
        public async Task ConnectAsync()
        {
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
                    ClientConnected = false;
                }
            }
        }
        public async Task ListenAsync()
        {
            await ReadAsync();
            ClientConnected = false;
        }
    }
}
