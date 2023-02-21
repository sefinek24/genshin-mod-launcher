using System;
using System.Text.RegularExpressions;

namespace Genshin_Impact_Mod
{
    internal abstract class ConfigDefault
    {
        public const string AppId = "";
        private const string Url = "https://discordapp.com/api/webhooks";

        private static readonly bool Channel = !Regex.Match(Environment.UserName, "(?:sefinek)",
            RegexOptions.IgnoreCase | RegexOptions.Singleline).Success;

        public static readonly string WebHookUrlDefault = Channel ? $"{Url}/" : $"{Url}/";
        public static readonly string WebHookLogFiles = Channel ? $"{Url}/" : $"{Url}/";
        public static readonly string WebHookUrlErrors = Channel ? $"{Url}/" : $"{Url}/";
    }
}
