using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;

namespace GzPrinter.Adaptor
{
    internal class AdaptorBase
    {
        private string server = string.Empty;

        /// <summary>
        /// 缓存过期时间，分钟
        /// </summary>
        public int CacheTimeout { get; set; } = 5;

        public AdaptorBase(string server)
        {
            this.server = server;
        }

        protected WebClient GetWebClient()
        {
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            wc.Proxy = null;
            //wc.UseDefaultCredentials= true;
            ServicePointManager.DefaultConnectionLimit = 100000;
            ServicePointManager.Expect100Continue = false;
            return wc;
        }

        protected string RemoteCall(string action,
            bool useCache = true,
            string method = "get",
            NameValueCollection parms = null
            )
        {
            string rsp = string.Empty;
            string err = string.Empty;
            using (WebClient wc = GetWebClient())
            {
                try
                {
                    if (useCache && CacheHelper.Exists(action))
                    {
                        rsp = CacheHelper.GetFromCache<string>(action);
                    }
                    else
                    {
                        var api = server + action;
                        if (method == "get")
                        {
                            if (parms != null)
                            {
                                string query = ToQuery(parms);
                                if (api.Contains("?"))
                                {
                                    api = api + "&" + query;
                                }
                                else
                                {
                                    api = api + "?" + query;
                                }
                            }
                            rsp = wc.DownloadString(api);
                        }
                        else
                        {
                            wc.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");
                            var temp = wc.UploadValues(api, parms);
                            rsp = Encoding.UTF8.GetString(temp);
                        }
                        if (useCache && !string.IsNullOrEmpty(rsp) && !rsp.Contains("error"))
                            CacheHelper.SaveToCache(action, rsp, DateTime.Now.AddMinutes(CacheTimeout));
                    }
                }
                catch (WebException ex)
                {
                    err = ex.Message;
                }
                catch (Exception ex)
                {
                    err = ex.Message;
                }
            }
            if (err.Length > 0) rsp = JsonConvert.SerializeObject(new { error = err });
            return rsp;
        }

        protected bool RemoteCallAsync(string action,
            Action<string> callback,
            bool useCache = true,
            string method = "get",
            NameValueCollection parms = null)
        {
            bool ret = false;
            var task = System.Threading.Tasks.Task.Factory.StartNew(() => {
                callback(RemoteCall(action, useCache, method, parms));
            });
            ret = task.Wait(TimeSpan.FromSeconds(30));
            return ret;
        }

        protected string ToQuery(NameValueCollection items)
        {
            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach (var key in items.AllKeys)
            {
                if (!first)
                {
                    sb.Append("&");
                }
                else
                {
                    first = false;
                }
                sb.Append(key + "=" + items[key]);
            }
            return sb.ToString();
        }
    }
}
