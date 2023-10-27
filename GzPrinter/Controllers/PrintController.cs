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
    internal class PrintController : IController
    {
        public bool NeedAuth()
        {
            return true;
        }

        public void ProcessRequest(MyHttpServer httpServer, Dictionary<string, string> data, HttpListenerResponse resp, string route = "", string request_type = "get")
        {
            string rsp;
            if (data == null || !data.ContainsKey("html"))
            {
                rsp = JsonConvert.SerializeObject(new { error = "缺少参数html" });
            }
            else
            {
                string html = data["html"];
                bool previewFlag = false;
                if (data.ContainsKey("preview"))
                {
                    previewFlag = data["preview"] == "1";
                }
                string printerName = "";
                if (data.ContainsKey("printer"))
                {
                    printerName = data["printer"];
                }
                string title = "";
                if (data.ContainsKey("title"))
                {
                    title = data["title"];
                }
                bool landscape = false;
                if (data.ContainsKey("landscape"))
                {
                    landscape = data["landscape"] == "1";
                }
                short copies = 1;
                if (data.ContainsKey("copies"))
                {
                    copies = short.Parse(data["copies"]);
                }
                string paperName = string.Empty;
                if (data.ContainsKey("paperName"))
                {
                    paperName = data["paperName"];
                }
                List<string> htmls = new List<string>();
                if (html.StartsWith("["))
                {
                    htmls = JsonConvert.DeserializeObject<List<string>>(html);
                }
                else
                {
                    htmls.Add(html);
                }
                new PrintHelper().Print(title,htmls, previewFlag, printerName,landscape, copies, paperName);
                rsp = JsonConvert.SerializeObject(new { success = true });
            }
            httpServer.ResponData(rsp, resp);
        }
    }
}
