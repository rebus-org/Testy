using System;

namespace Testy.Internals
{
    static class Shims
    {
        public static string CurrentBaseDirectory()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }
    }
}