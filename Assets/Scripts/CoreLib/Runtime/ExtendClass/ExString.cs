using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Corelib.Utils
{
    public static class ExString
    {
        [System.ThreadStatic]
        private static StringBuilder _sbCache;

        public static string FromList(this List<string> list)
        {
            if (list == null || list.Count == 0)
                return string.Empty;

            if (_sbCache == null)
                _sbCache = new StringBuilder(1024);
            else
                _sbCache.Clear();

            for (int i = 0; i < list.Count; i++)
            {
                if (i > 0) _sbCache.Append('\n');
                _sbCache.Append(list[i]);
            }

            return _sbCache.ToString();
        }
    }
}