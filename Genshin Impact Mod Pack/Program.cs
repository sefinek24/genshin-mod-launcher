using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Genshin_Impact_Mod.Forms;
using Genshin_Impact_Mod.Forms.Errors;
using Genshin_Impact_Mod.Forms.Other;
using Genshin_Impact_Mod.Properties;
using Genshin_Impact_Mod.Scripts;

namespace Genshin_Impact_Mod
{
	internal static class Program
	{
		public static readonly string AppName = Assembly.GetExecutingAssembly().GetName().Name;
		public static readonly string AppVersion = Application.ProductVersion;
		public const string AppWebsite = "https://sefinek.net/genshin-impact-reshade";

		// Directories
		public const string Folder = @"C:\Genshin-Impact-ReShade";
		private const string PatronsDir = Folder + @"\Data\Presets\Patrons";
		public static readonly string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Genshin Impact MP by Sefinek";

		// Files
		public static readonly string UnlockerFpsPath = Folder + @"\Data\Unlocker\unlocker.config.json";
		private static readonly string LaunchCountFile = AppData + @"\launch-count.sfn";
		private static readonly string TierActivated = AppData + @"\tier-activated.sfn";

		// Other
		public static readonly string UserAgent = $"Mozilla/5.0 (compatible; GenshinModLauncher/{AppVersion}; +{AppWebsite})";
		public static int LaunchCountGlobal;

		[STAThread]
		private static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			if ((Environment.CurrentDirectory != Folder) & !Debugger.IsAttached)
			{
				new WrongDirectory { Icon = Resources.icon_52x52 }.ShowDialog();

				Environment.Exit(0);
			}

			string fileData = Utils.IsInstalledFile_Create();
			if (fileData == "false")
			{
				new NotConfigured { Icon = Resources.icon_52x52 }.ShowDialog();

				Environment.Exit(0);
			}

			if (Process.GetProcessesByName(Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly()?.Location)).Count() > 1)
			{
				MessageBox.Show($"Sorry, one instance is currently open.\n\nQuit the process with name {Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly()?.Location)} and try again.", AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
				Environment.Exit(0);
			}


			try
			{
				if (!File.Exists(LaunchCountFile))
					using (StreamWriter lcfWriter = File.CreateText(LaunchCountFile))
					{
						lcfWriter.Write("0");
					}


				if (!File.Exists(TierActivated) && Directory.Exists(PatronsDir)) Directory.Delete(PatronsDir, true);

				int number = int.Parse(File.ReadAllLines(LaunchCountFile).First()) + 1;
				LaunchCountGlobal = number;
				using (StreamWriter sw = File.CreateText(LaunchCountFile))
				{
					sw.Write(number);
				}

				switch (number)
				{
					case 4:
					case 13:
					case 42:
					case 63:
						Application.Run(new SupportMe { Icon = Resources.icon_52x52 });
						return;

					case 8:
					case 25:
					case 35:
					case 60:
					case 100:
					case 200:
						DialogResult dialogResult1 = MessageBox.Show("Do you want to send us anonymous log files?", AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
						if (dialogResult1 == DialogResult.Yes)
						{
							Discord.SendLogFiles();

							DialogResult dialogResult2 = MessageBox.Show("Thank you! Do you want to see these files?", AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
							if (dialogResult2 == DialogResult.Yes) Process.Start(Log.Folder);
						}

						break;
				}

				Application.Run(new Default { Icon = Resources.icon_52x52 });
			}
			catch (Exception e)
			{
				Log.ErrorAndExit(e);
			}
		}
	}
}