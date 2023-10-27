using System;
using System.Web.Caching;
using System.Web;

namespace GzPrinter
{
    internal class CacheHelper
    {
        public static bool Exists(string key)
        {
            return HttpRuntime.Cache.Get(key) != null;
        }

        public static T GetFromCache<T>(string key)
        {
            return (T)HttpRuntime.Cache[key];
        }

        public static void SaveToCache<T>(string key, T value)
        {
            HttpRuntime.Cache[key] = value;
        }

        public static void SaveToCache<T>(string key, T value, CacheDependency dependency)
        {
            HttpRuntime.Cache.Insert(key, value, dependency);
        }

        public static void SaveToCache<T>(string key, T value, DateTime expires)
        {
            HttpRuntime.Cache.Insert(key, value, null, expires, System.Web.Caching.Cache.NoSlidingExpiration);
        }

        public static void Remove(string key)
        {
            HttpRuntime.Cache.Remove(key);
        }
    }
}
