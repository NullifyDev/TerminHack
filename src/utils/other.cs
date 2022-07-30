using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TerminHack
{
    public static partial class Utils
    {
        public static void Sleep(int ms) => Thread.Sleep(ms);
        public static bool IsInternetConnected() => System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
        public static void OpenURL(string url)  { 
            if (System.OperatingSystem.IsLinux()) {
                Process.Start("xdg-open", "https://e621.net");
            } else if (System.OperatingSystem.IsWindows()) {
                Process.Start("start ", "firefox https://e621.net");
            }
        }
    }
}
