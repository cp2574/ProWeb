using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Common
{
    public static class CacheHelper
    {
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <typeparam name="T">缓存值类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="value">值</param>
        /// <param name="ts">有效期</param>
        /// <returns></returns>
        public static bool Set<T>(string key, T value, TimeSpan ts)
        {

            try
            {
                System.Web.Caching.Cache cache = HttpRuntime.Cache;
                cache.Insert(key, value, null, DateTime.Now, ts);
            }
            catch (Exception)
            {

                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T">缓存值类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <returns></returns>
        public static T Get<T>(string key)
        {
            T value = default(T);
            try
            {
                System.Web.Caching.Cache cache = HttpRuntime.Cache;
                object obj = cache.Get(key);
                if (obj != null)
                {
                    return (T)obj;
                }
            }
            catch (Exception)
            {
                return value;
            }
            return value;
        }
    }
}
