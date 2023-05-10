using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace Installation.Communication
{
    public abstract class IPCBase
    {


        protected CancellationToken cancellationToken;
        protected PipeStream pipeStream;
        protected string pipeName;
        public Guid endpointId = new Guid();

        private Func<string, Guid, Task> ReceivedData;
        
        public IPCBase(string pipeName, CancellationToken cancellationToken, Func<string, Guid, Task> receivedData)
        {
            this.cancellationToken = cancellationToken;
            this.pipeName = pipeName;
            ReceivedData = receivedData;
        }
        protected async Task ReadAsync()
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    pipeStream.Dispose();
                    break;
                }
                if (!pipeStream.IsConnected)
                {
                    break;
                }
                byte[] buffer = new byte[4];
                int bytes = await pipeStream.ReadAsync(buffer, 0, 4, cancellationToken).ConfigureAwait(false);
                if (bytes != 4)
                {
                    Log.Debug("Less than initial 4 bytes received");
                    if (!pipeStream.IsConnected)
                        Log.Debug("Client disconnected");
                    break;
                }
                int size = getMessageSize(buffer);

                buffer = new byte[size];
                bytes = await pipeStream.ReadAsync(buffer, 0, size, cancellationToken).ConfigureAwait(false);
                if (bytes != size)
                {
                    Log.Debug("Less than the size: {size} of the message bytes received", size);
                    if (!pipeStream.IsConnected)
                        Log.Debug("Client disconnected");
                    break;
                }
                var dataString = convertFromByte(buffer);
                await handleIncomingDataAsync(dataString);
            }
        }
        private async Task handleIncomingDataAsync(string data)
        {
            Log.Debug("Data received");

            await ReceivedData?.Invoke(data, endpointId);
        }

        public async Task SendDataAsync(string data)
        {
            if (!pipeStream.IsConnected)
            {
                return;
            }
            await pipeStream.WriteAsync(convertToByte(data), 0, data.Length, cancellationToken).ConfigureAwait(false);
            await pipeStream.FlushAsync(cancellationToken).ConfigureAwait(false);

        }

        private string convertFromByte(byte[] byteArray)
        {
            return Encoding.UTF8.GetString(byteArray);
        }

        private int getMessageSize(byte[] byteArray)
        {
            int messageSize = BitConverter.ToInt32(byteArray, 0);
            return messageSize;
        }

        private byte[] convertToByte(string text)
        {
            byte[] messageText = Encoding.UTF8.GetBytes(text);
            byte[] messageSize = BitConverter.GetBytes(messageText.Length);
            byte[] message = new byte[messageSize.Length + messageText.Length];
            messageSize.CopyTo(message, 0);
            messageText.CopyTo(message, messageSize.Length);
            return message;
        }
    }
}
