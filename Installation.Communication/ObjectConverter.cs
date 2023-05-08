using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Installation.Models;
using Newtonsoft.Json;
using Serilog;

namespace Installation.Communication
{
    public class ObjectConverter
    {
        private Encoding encoding;

        public ObjectConverter()
        {
            this.encoding = Encoding.UTF8;
        }
        public ObjectConverter(Encoding encoding)
        {
            this.encoding = encoding;
        }
        public byte[] ConvertToByte(object jsonObject)
        {
            Log.Verbose("Using Encoding: {encoding}", encoding.EncodingName);
            string jsonText = serializeObject(jsonObject);
            byte[] messageText = encoding.GetBytes(jsonText);
            return messageText;

        }
        public string ConvertToString(object jsonObject)
        {
            return serializeObject(jsonObject);

        }
        public object ConvertToObject(byte[] data)
        {
            Log.Verbose("Using Encoding: {encoding}", encoding.EncodingName);
            string dataText = encoding.GetString(data);
            return deserializeObject(dataText);
        }
        public object ConvertToObject(string data)
        {
            return deserializeObject(data);
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
            return JsonConvert.DeserializeObject(dataString, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
        }

    }
}
