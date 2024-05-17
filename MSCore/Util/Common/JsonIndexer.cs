using System.Collections.Generic;
using MSCore.Util.Newtonsoft;
using Newtonsoft.Json.Linq;

namespace MSCore.Util.Common
{
    public class JsonIndexer
    {
        private Dictionary<string, JToken> jsons = new Dictionary<string, JToken>();

        public JToken this[string index]
        {
            get
            {
                JToken result;
                if (jsons.TryGetValue(index, out result))
                {
                    return result;
                }
                return null;
            }
            set
            {
                jsons[index] = value;
            }
        }

        public T GetObj<T>(string path)
        {
            if (string.IsNullOrEmpty(path))
                return default;

            JToken result;
            if (jsons.TryGetValue(path, out result))
            {
                return result.Deserialize<T>();
            }
            return default; ;
        }
    }
}
