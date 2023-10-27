/***********************************************************************
 *            Project: RTPrinter
 *        ProjectName: 旭日web打印服务
 *             Author: rootlee
 *              Email: 540478668@qq.com
 *         CreateTime: 2023/8/21 8:00:00
 *        Description: 暂无
 ***********************************************************************/

using System.Collections.Generic;

namespace GzPrinter.Controllers
{
    internal class RoutesFactory
    {
        private static Dictionary<string, IController> _routes = new Dictionary<string, IController>();

        public static void Init()
        {
            _routes = new Dictionary<string, IController>
            {
                {"/index",new HomeController() },
                {"/print",new PrintController() },
                {"/printers",new PrintersController() },
                { "/paper",new PaperController()},
                { "/dpi",new DpiController()},
                {"/bad",new BadController() },
            };
        }

        public static IController GetBadController()
        {
            return new BadController();
        }

        public static IController GetController(string route)
        {
            string key = route.ToLower();
            if (_routes.ContainsKey(route))
            {
                return _routes[route];
            }
            return GetBadController();
        }

        public static List<string> GetRouteKeys()
        {
            var keys = new List<string>();
            foreach(var pair in _routes)
            {
                keys.Add(pair.Key);
            }
            return keys;
        }

        public static void Register(string route,IController controller)
        {
            if (!_routes.ContainsKey(route))
            {
                _routes.Add(route, controller);
            }
            else
            {
                _routes[route] = controller;
            }
        }
    }
}
