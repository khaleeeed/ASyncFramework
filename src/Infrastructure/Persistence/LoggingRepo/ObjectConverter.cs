using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ASyncFramework.Infrastructure.Persistence.LoggingRepo
{
    public class ObjectConverter
    {
        public static object ContentType(string obj, string ContentType)
        {
            try
            {
                return ContentType switch
                {
                    "application/json" => ConvertJsonToDic(obj),

                    "text/xml" => ConvertXmlToDic(obj),

                    _ => obj,
                };
            }
            catch
            {
                return obj;
            }
        }

        private static Dictionary<string, object> ConvertJsonToDic(string json)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            var jobject = JsonConvert.DeserializeObject(json) as JObject;
            foreach (KeyValuePair<string, JToken> item in jobject)
            {
                dic.Add(item.Key, GetJsonValue(item.Value));
            }
            return dic;
        }

        private static object GetJsonValue(JToken value)
        {
            bool isEnterforeach = false;
            Dictionary<string, object> dic = new Dictionary<string, object>();
            foreach (JProperty item in value)
            {
                isEnterforeach = true;
                dic.Add(item.Name, GetJsonValue(item.Value));
            }

            if (isEnterforeach)
                return dic;

            return value.Value<string>();
        }

        private static Dictionary<string, object> ConvertXmlToDic(string Xml)
        {

            XDocument doc = XDocument.Parse(Xml);
            Dictionary<string, object> dataDictionary = new Dictionary<string, object>();

            foreach (XElement element in doc.Descendants().Where(p => p.HasElements == false))
            {
                int keyInt = 0;
                string keyName = element.Name.LocalName;

                while (dataDictionary.ContainsKey(keyName))
                {
                    keyName = element.Name.LocalName + "_" + keyInt++;
                }

                dataDictionary.Add(keyName, element.Value);
            }

            return dataDictionary;
        }
    }
}