/***********************************************************************
 *            Project: RTPrinter
 *        ProjectName: 旭日web打印服务
 *             Author: rootlee
 *              Email: 540478668@qq.com
 *         CreateTime: 2023/8/21 8:00:00
 *        Description: 暂无
 ***********************************************************************/

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;

namespace GzPrinter.Controllers
{
    internal class HomeController : IController
    {
        public bool NeedAuth()
        {
            return false;
        }

        public void ProcessRequest(MyHttpServer httpServer, Dictionary<string, string> data, HttpListenerResponse resp, string route = "", string request_type = "get")
        {
            Dictionary<string, string> resp_data = new Dictionary<string, string>();
            resp_data.Add("code", "1");
            resp_data.Add("data", "");
            resp_data.Add("time", "12345");
            resp_data.Add("msg", "ok ");
            string resp_json = JsonConvert.SerializeObject(resp_data);
            //输出结果
            httpServer.ResponData(resp_json, resp);
        }
    }
}
