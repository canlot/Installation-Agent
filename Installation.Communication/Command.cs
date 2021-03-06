using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Installation.Communication
{
    [JsonConverter(typeof(StringEnumConverter))] //Hacky Way, should be wrapped inside a class
    public enum Command
    {
        [EnumMember(Value = "SendExecutables")]
        SendExecutables
    }
}
