using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace GzPrinter.Adaptor
{
    internal class AuthAdaptor:AdaptorBase
    {
        public AuthAdaptor() : base("http://shop.wanyouyinli.com/")
        {
        }

        public Auth GetAuth(string token,string mac,string domain)
        {
            NameValueCollection parms = new NameValueCollection
            {
                {"token",token },
                {"mac",mac },
                {"domain",domain }
            };
            string action = "/api/getrtpauth";
            string rsp = RemoteCall(action, true, "post", parms);
            if (string.IsNullOrEmpty(rsp)) return null;
            var dic=JsonConvert.DeserializeObject<Dictionary<string,object>>(rsp);
            if (dic.ContainsKey("error")) throw new System.Exception(dic["error"].ToString());
            return JsonConvert.DeserializeObject<Auth>(dic["item"].ToString());
        }
    }
}
