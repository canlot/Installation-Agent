using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installation.Models.Helpers
{
    public static class DictionaryHelper
    {
        public static string ReturnValueOrSubstringOrFirstValue(this Dictionary<string, string> dic, string skey)
        {
            string key = dic.Keys.First();
            var foundItems = dic.Where(r => r.Key == skey);
            if (foundItems.Count() == 0)
            {
                var foundItemsStartWithKey = dic.Where(entry => entry.Key.StartsWith(skey.Split('-')[0]) == true); // if main language is equal, like en-GB <-> en-US because en <-> en without dialect, only en works as well

                if (foundItemsStartWithKey.Count() == 0)
                    return dic[key];
                else
                    return foundItemsStartWithKey.First().Value;
            }
            else
            {
                return foundItems.First().Value;
            }
        }
    }
}
