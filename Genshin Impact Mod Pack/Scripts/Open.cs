using System.Diagnostics;

namespace Genshin_Impact_Mod.Scripts
{
	internal class Open
	{
		public static void InBrowser(string url)
		{
			Process.Start(url);
			Log.Output($"Opened '{url}' in default browser.");
		}
	}
}