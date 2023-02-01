using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Genshin_Impact_Mod.Scripts
{
	internal class Utils
	{
		private static readonly string Path = $@"{Program.AppData}\is-installed.sfn";

		public static void RemoveClickEvent(Label b)
		{
			FieldInfo f1 = typeof(Control).GetField("EventClick", BindingFlags.Static | BindingFlags.NonPublic);

			object obj = f1?.GetValue(b);
			PropertyInfo pi = b.GetType().GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Instance);

			EventHandlerList list = (EventHandlerList)pi?.GetValue(b, null);
			list?.RemoveHandler(obj, list[obj]);
		}

		public static string IsInstalledFile_Create()
		{
			if (!Directory.Exists(Program.AppData)) Directory.CreateDirectory(Program.AppData);

			if (!File.Exists(Path))
				using (StreamWriter sw = File.CreateText(Path))
				{
					sw.Write("false");
				}

			try
			{
				return File.ReadAllLines(Path).First();
			}
			catch (Exception e)
			{
				Log.ErrorAndExit(e);

				return null;
			}
		}
	}
}