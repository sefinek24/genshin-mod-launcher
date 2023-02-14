using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Genshin_Impact_Mod.Scripts
{
	internal class GetGame
	{
		private static readonly string FileWithGamePath = $@"{Program.AppData}\game-path.sfn";

		public static string Find(string type)
		{
			if (!File.Exists(FileWithGamePath))
			{
				MessageBox.Show($"File with game path was not found in:\n{FileWithGamePath}", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				Log.Output($"File with game path was not found in: {FileWithGamePath}");
				return null;
			}

			string gameDir = File.ReadAllLines(FileWithGamePath).First();
			if (!Directory.Exists(gameDir))
			{
				MessageBox.Show($"Folder does not exists.\n{gameDir}", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				Log.Output($"Directory {gameDir} does not exists.");
				return null;
			}


			switch (type)
			{
				case "giGameDir":
				{
					Log.Output($"Found Genshin Impact Game dir: {gameDir} [giGameDir]");
					return gameDir;
				}

				case "giExe":
				{
					string genshinImpactExeMain = $@"{gameDir}\GenshinImpact.exe";
					if (File.Exists(genshinImpactExeMain))
					{
						Log.Output($"Found GenshinImpact.exe in: {genshinImpactExeMain} [giExe]");
						return genshinImpactExeMain;
					}
					MessageBox.Show($"File does not exists.\n{genshinImpactExeMain}", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
					Log.Output($"File does not exists in: {genshinImpactExeMain} [giExe]");

					string genshinImpactExeYuanShen = $@"{gameDir}\YuanShen.exe";
					if (File.Exists(genshinImpactExeYuanShen))
					{
						Log.Output($"Found GenshinImpact.exe in: {genshinImpactExeMain} [giExe]");
						return genshinImpactExeYuanShen;
					}
					MessageBox.Show($"File {genshinImpactExeYuanShen} does not exists.", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
					Log.Output($"File {genshinImpactExeYuanShen} does not exists. [giExe]");

					return null;
				}

				case "giLauncher":
				{
					string mainDir = Directory.GetParent(gameDir)?.FullName;
					if (!Directory.Exists(mainDir))
					{
						MessageBox.Show($"Directory does not exists.\n{mainDir}", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
						Log.Output($"Directory does not exists in: {mainDir} [giLauncher]");
						return null;
					}

					string genshinImpactExe = $@"{mainDir}\launcher.exe";
					if (!File.Exists(genshinImpactExe) || !File.Exists(genshinImpactExe))
					{
						MessageBox.Show($"Launcher file does not exists.\n{genshinImpactExe}", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
						Log.Output($"Launcher file does not exists in: {genshinImpactExe} [giLauncher]");
						return null;
					}

					Log.Output($"Found Genshin Impact Launcher in: {genshinImpactExe} [giLauncher]");
					return genshinImpactExe;
				}

				default:
					return null;
			}
		}
	}
}