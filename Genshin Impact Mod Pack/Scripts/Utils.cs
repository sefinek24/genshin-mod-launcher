using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace Genshin_Impact_Mod.Scripts
{
	internal class Utils
	{
		public static void RemoveClickEvent(Label b)
		{
			FieldInfo f1 = typeof(Control).GetField("EventClick", BindingFlags.Static | BindingFlags.NonPublic);

			object obj = f1?.GetValue(b);
			PropertyInfo pi = b.GetType().GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Instance);

			EventHandlerList list = (EventHandlerList)pi?.GetValue(b, null);
			list?.RemoveHandler(obj, list[obj]);
		}

		public static void OpenUrl(string url)
		{
			Process.Start(url);
			Log.Output($"Opened '{url}' in default browser.");
		}
	}
}