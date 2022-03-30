using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Installation.Models;
using Installation.Models.Executables;
using Newtonsoft.Json;

namespace Installation.Communication
{
    public class Communicator
    {
        //delegates for events
        public delegate Task JobReceived(Job job);
        public delegate Task ExecutableReceived(Executable executabe);

        public event JobReceived OnJobReceived;
        public event ExecutableReceived OnExecutableReceived;

        protected CancellationToken cancellationToken;
        protected PipeStream pipeStream;
        protected string pipeName = "pipe";
        
        public Communicator(CancellationToken cancellationToken)
        {
            this.cancellationToken = cancellationToken;
            
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
                await pipeStream.ReadAsync(buffer, 0, 4, cancellationToken).ConfigureAwait(false);
                int size = getMessageSize(buffer);

                buffer = new byte[size];
                await pipeStream.ReadAsync(buffer, 0, size, cancellationToken).ConfigureAwait(false);
                var dataString = convertFromByte(buffer);
                await handleIncomingData(dataString);
            }
        }
        private async Task handleIncomingData(string data)
        {
            try
            {
                var jsonObject = deserializeObject(data);
                if(jsonObject != null)
                {
                    if(jsonObject is Job && OnJobReceived != null)
                        await OnJobReceived((Job)jsonObject);
                    else if(jsonObject is Executable && OnExecutableReceived != null)
                        await OnExecutableReceived((Executable)jsonObject);
                }
                
            }
            catch (Exception ex)
            {

            }
        }

        private async Task SendData(byte[] data)
        {
            if (!pipeStream.IsConnected)
            {
                return;
            }
            await pipeStream.WriteAsync(data, 0, data.Length, cancellationToken).ConfigureAwait(false);
            await pipeStream.FlushAsync(cancellationToken).ConfigureAwait(false);

        }

        public async Task SendJob(Job job)
        {
            await SendData(convertToByte(serializeObject(job)));
        }
        public async Task SendExecutable(Executable executable)
        {
            await SendData(convertToByte(serializeObject(executable)));
        }

        private string serializeObject(object jsonObject)
        {
            return JsonConvert.SerializeObject(jsonObject);
        }
        private object deserializeObject(string dataString)
        {
            var deserializedObject = JsonConvert.DeserializeObject(dataString);
            return deserializedObject;
        }
        private string convertFromByte(byte[] byteArray)
        {
            return Encoding.ASCII.GetString(byteArray);
        }

        private int getMessageSize(byte[] byteArray)
        {
            int messageSize = BitConverter.ToInt32(byteArray, 0);
            return messageSize;
        }

        private byte[] convertToByte(string text)
        {
            byte[] messageSize = BitConverter.GetBytes(text.Length);
            byte[] messageText = Encoding.ASCII.GetBytes(text);
            byte[] message = new byte[messageSize.Length + messageText.Length];
            messageSize.CopyTo(message, 0);
            messageText.CopyTo(message, messageSize.Length);
            return message;
        }
    }
}
