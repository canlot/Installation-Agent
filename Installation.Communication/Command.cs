using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Installation.Communication
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Command
    {
        [EnumMember(Value = "SendExecutables")]
        SendExecutables
    }
}
