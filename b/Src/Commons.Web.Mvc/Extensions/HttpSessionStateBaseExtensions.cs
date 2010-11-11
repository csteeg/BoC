using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace System.Web
{
    public static class HttpSessionStateBaseExtensions
    {
        public static T Get<T>(this HttpSessionStateBase session, string key) where T: class
        {
            if (session != null && session[key] != null && session[key] is T)
            {
                return (T)session[key];
            }
            return null as T;
        }

        public static void Set<T>(this HttpSessionStateBase session, string key, T value) where T : class
        {
            if (session != null)
            {
                session[key] = value;
            }
        }
    }
}
