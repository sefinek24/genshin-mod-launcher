using System;
using System.Globalization;
using Microsoft.Win32;

namespace Genshin_Impact_Mod.Scripts
{
    internal abstract class Os
    {
        private static readonly RegistryKey SKey =
            Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows NT\CurrentVersion");

        public static readonly string Name = GetOs();
        public static readonly string Build = GetBuild();
        public static readonly string Version = GetVersion();
        public static readonly string AllInfos = $"{Name} {Version} [{Build}]";

        public static readonly string TimeZone = TimeZoneInfo.Local.ToString();
        public static readonly string Region = RegionInfo.CurrentRegion.EnglishName;

        private static string GetOs()
        {
            try
            {
                return SKey.GetValue("ProductName").ToString();
            }
            catch
            {
                return "?";
            }
        }

        private static string GetBuild()
        {
            try
            {
                return SKey.GetValue("CurrentBuild") + "." + SKey.GetValue("UBR");
            }
            catch
            {
                return "?";
            }
        }

        private static string GetVersion()
        {
            try
            {
                return SKey.GetValue("DisplayVersion").ToString();
            }
            catch
            {
                return "?";
            }
        }
    }
}
