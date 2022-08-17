using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Installation.Models;
using Newtonsoft.Json;
using Serilog;

namespace Installation.Communication
{
    public abstract class Communicator
    {
        //delegates for events
        public delegate Task JobReceived(Job job);
        public delegate Task ExecutableReceived(Executable executabe);
        public delegate Task CommandReceived(Command command);

        public event JobReceived OnJobReceived;
        public event ExecutableReceived OnExecutableReceived;
        public event CommandReceived OnCommandReceived;

        protected CancellationToken cancellationToken;
        protected PipeStream pipeStream;
        protected string pipeName = "pipe";
        
        public Communicator(CancellationToken cancellationToken)
        {
            this.cancellationToken = cancellationToken;
            
        }

        private async Task handleIncomingDataAsync(string data)
        {
            Log.Debug("Data received");
            File.WriteAllText("data.json", data);
            try
            {
                var jsonObject = deserializeObject(data);
                if(jsonObject != null)
                {
                    Log.Verbose("jsonObject is not null and Type {type}", jsonObject.GetType().Name);
                    if(jsonObject is Job && OnJobReceived != null)
                        await OnJobReceived((Job)jsonObject);
                    else if(jsonObject is Executable && OnExecutableReceived != null)
                        await OnExecutableReceived((Executable)jsonObject);
                    else if(jsonObject is Command && OnCommandReceived != null)
                        await OnCommandReceived((Command)jsonObject);
                }
                
            }
            catch (Exception ex)
            {

            }
        }

        private async Task sendDataAsync(byte[] data)
        {
            if (!pipeStream.IsConnected)
            {
                return;
            }
            await pipeStream.WriteAsync(data, 0, data.Length, cancellationToken).ConfigureAwait(false);
            await pipeStream.FlushAsync(cancellationToken).ConfigureAwait(false);

        }
        public async Task SendCommandAsync(Command command)
        {
            await sendDataAsync(convertToByte(serializeObject(command)));
        }
        public async Task SendJobAsync(Job job)
        {
            await sendDataAsync(convertToByte(serializeObject(job)));
            Log.Debug("Job {id} sent", job.JobID);
        }
        public async Task SendExecutableAsync(Executable executable)
        {
            await sendDataAsync(convertToByte(serializeObject(executable)));
            Log.Debug("Executable {id} sent", executable.Id);
        }

        private string serializeObject(object jsonObject)
        {
            return JsonConvert.SerializeObject(jsonObject, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
        }
        private object deserializeObject(string dataString)
        {
            Log.Verbose(dataString);
            var deserializedObject = JsonConvert.DeserializeObject(dataString, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });

            if (deserializedObject.GetType() == typeof(string)) // Hack, should be fixed
            {
                Command command;
                if(Enum.TryParse((string)deserializedObject, true, out command))
                {
                    return command;
                }
                //var command = Enum.TryParse<Command>(deserializedObject.ToString(), true, out _);
            }
            
            return deserializedObject;
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
