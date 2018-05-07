using System;

namespace Testy
{
    static class Shims
    {
        public static string CurrentBaseDirectory()
        {
#if NETFULL
            return AppDomain.CurrentDomain.BaseDirectory;
#else
            return AppContext.BaseDirectory;
#endif
        }
    }
}