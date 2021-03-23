using System;
using System.Collections.Generic;

namespace RC
{
    public static class Logger
    {
        static List<string> EnabledMethods = new List<string>();
        static bool LogginEnabled = false;

        public static void EnableLogging(bool enable)
        {
            LogginEnabled = enable;
        }

        public static void DebugMethod(string method)
        {
            EnabledMethods.Add(method);
        }

        public static void Log(string msg, [System.Runtime.CompilerServices.CallerMemberName] string caller = "")
        {
            if (LogginEnabled && (EnabledMethods.Count == 0 || EnabledMethods.Contains(caller)))
            {
                Console.WriteLine("[" + caller + "] " + msg);
            }
        }
    }
}
