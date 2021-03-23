using System;
using System.Collections.Generic;

namespace RC
{
    public static class Logger
    {
        static List<string> EnabledMethods = new List<string>();

        public static void DebugMethod(string method)
        {
            EnabledMethods.Add(method);
        }

        public static void Log(string msg, [System.Runtime.CompilerServices.CallerMemberName] string caller = "")
        {
            if (EnabledMethods.Count == 0 || EnabledMethods.Contains(caller))
            {
                Console.WriteLine("[" + caller + "] " + msg);
            }
        }
    }
}
