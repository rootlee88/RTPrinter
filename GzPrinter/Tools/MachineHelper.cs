/***********************************************************************
 *            Project: RTPrinter
 *        ProjectName: 旭日web打印服务
 *             Author: rootlee
 *              Email: 540478668@qq.com
 *         CreateTime: 2023/8/21 8:00:00
 *        Description: 暂无
 ***********************************************************************/

using System.Linq;
using System.Management;
using System.Net;

namespace GzPrinter
{
    internal class MachineHelper
    {
        /// <summary>
        /// 获取服务器mac地址
        /// </summary>
        /// <returns></returns>
        public static string GetMacAddress()
        {
            string retString = string.Empty;
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if ((bool)mo["IPEnabled"] == true)
                {
                    retString += mo["MACAddress"].ToString() + "$";
                }
            }
            return retString.TrimEnd('$');
        }

        public static string GetLocalIP4()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            var ippaddress = host
                .AddressList
                .FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
            return ippaddress.ToString();
        }
    }
}
