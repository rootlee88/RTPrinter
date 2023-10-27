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
    internal class PrintersController : IController
    {
        public bool NeedAuth()
        {
            return true;
        }

        public void ProcessRequest(MyHttpServer httpServer, Dictionary<string, string> data, HttpListenerResponse resp, string route = "", string request_type = "get")
        {
            var printers = new PrintHelper().GetInstalledPrinters();
            string rsp = JsonConvert.SerializeObject(new { items = printers });
            httpServer.ResponData(rsp, resp);
        }
    }
}
