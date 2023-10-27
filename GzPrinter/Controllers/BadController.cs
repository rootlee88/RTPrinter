using System.Collections.Generic;
using System.Net;

/***********************************************************************
 *            Project: RTPrinter
 *        ProjectName: 旭日web打印服务
 *             Author: rootlee
 *              Email: 540478668@qq.com
 *         CreateTime: 2023/8/21 8:00:00
 *        Description: 暂无
 ***********************************************************************/

namespace GzPrinter.Controllers
{
    internal class BadController : IController
    {
        public bool NeedAuth()
        {
            return false;
        }

        public void ProcessRequest(MyHttpServer httpServer, Dictionary<string, string> data, HttpListenerResponse resp, string route = "", string request_type = "get")
        {
            httpServer.ResponData("404", resp);
        }
    }
}
