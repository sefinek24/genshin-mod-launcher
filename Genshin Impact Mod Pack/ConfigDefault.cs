using System;
using System.Text.RegularExpressions;

namespace Genshin_Impact_Mod
{
	internal abstract class ConfigDefault
	{
		public const string AppId = "";
		private const string Url = "https://discordapp.com/api/webhooks";

		private static readonly bool DefaultChannel = !Regex.Match(Environment.UserName, "(?:sefinek)", RegexOptions.IgnoreCase | RegexOptions.Singleline).Success;

		public static readonly string WebHookUrlDefault = DefaultChannel ? $"{Url}/" : $"{Url}/";
		public static readonly string WebHookLogFiles = DefaultChannel ? $"{Url}/" : $"{Url}/";
		public static readonly string WebHookUrlErrors = DefaultChannel ? $"{Url}/" : $"{Url}/";
	}
}