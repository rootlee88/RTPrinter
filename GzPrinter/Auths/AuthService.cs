using GzPrinter.Adaptor;
using System;

namespace GzPrinter
{
    internal static class AuthService
    {
        private static AuthAdaptor _authAdaptor=new AuthAdaptor();

        public static void CheckAuth(string token,string mac,string domain)
        {
            Auth info=_authAdaptor.GetAuth(token, mac, domain);
            if (info == null) throw new Exception("获取授权信息失败，请稍候重试");
            if (info.IsExpired()) throw new Exception("授权已过期");
        } 
    }
}
