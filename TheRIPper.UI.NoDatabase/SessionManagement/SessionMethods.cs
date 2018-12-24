using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheRIPper.UI.NoDatabase.SessionManagement
{
    public static class SessionMethods
    {
        public static void Set<T>(this ISession session, string key, T value) {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T Get<T>(this ISession session, string key) {
            return session.GetString(key) == null ? default(T) :
                JsonConvert.DeserializeObject<T>(session.GetString(key));
        }
    }
}
