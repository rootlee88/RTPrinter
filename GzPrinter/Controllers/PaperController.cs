/***********************************************************************
 *            Project: RTPrinter
 *        ProjectName: 旭日web打印服务
 *             Author: rootlee
 *              Email: 540478668@qq.com
 *         CreateTime: 2023/8/21 8:00:00
 *        Description: 暂无
 ***********************************************************************/

using System.Collections.Generic;
using System.Net;

namespace GzPrinter.Controllers
{
    internal class PaperController : IController
    {
        public bool NeedAuth()
        {
            return true;
        }

        public void ProcessRequest(MyHttpServer httpServer, Dictionary<string, string> data, HttpListenerResponse resp, string route = "", string request_type = "get")
        {
            string printerName = "";
            if (data.ContainsKey("printer"))
            {
                printerName = data["printer"];
            }
            var ret=new PrintHelper().GetPaperSizes(printerName);
            var rsp = Newtonsoft.Json.JsonConvert.SerializeObject(new { items=ret });
            httpServer.ResponData(rsp,resp);
        }
    }
}
