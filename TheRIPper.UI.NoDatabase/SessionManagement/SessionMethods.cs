using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheRIPper.UI.NoDatabase.SessionManagement
{
    public static class SessionMethods
    {
        public static void Set<T>(this ISession session, string key, T value, bool useSession, IMemoryCache _cache) {
            if (useSession)
            {
                session.SetString(key, JsonConvert.SerializeObject(value));
            }
            else {
                _cache.Set(key, value);
            }
        }

        public static T Get<T>(this ISession session, string key, bool useSession, IMemoryCache _cache) {
            if (useSession)
            {
                return session.GetString(key) == null ? default(T) :
                        JsonConvert.DeserializeObject<T>(session.GetString(key));
            }
            else {
                T outputV;
                _cache.TryGetValue(key, out outputV);
                return outputV;
            }
        }
    }
}
