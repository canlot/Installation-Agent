using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installation.Parser.Helpers
{
    public static class StringHelpers
    {
        public static string RemoveWrongCharacters(this string text)
        {
            var temp = text.Replace($"\"", "");
            //temp = temp.Replace('"', '');
            return temp;
        }
    }
}
