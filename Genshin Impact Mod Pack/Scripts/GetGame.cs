using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
				return null;
			}

			string gameDir = File.ReadAllLines(FileWithGamePath).First();
			if (!Directory.Exists(gameDir))
			{
				MessageBox.Show($"Directory {gameDir} does not exists.", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return null;
			}


			switch (type)
			{
				case "giGameDir":
				{
					return gameDir;
				}
				case "giExe":
				{
					string genshinImpactExe = $@"{gameDir}\GenshinImpact.exe";
					if (!File.Exists(genshinImpactExe))
					{
						MessageBox.Show($"File {genshinImpactExe} does not exists.", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);

						genshinImpactExe = $@"{gameDir}\YuanShen.exe";
						if (!File.Exists(genshinImpactExe))
						{
							MessageBox.Show($"File {genshinImpactExe} does not exists.", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return null;
						}

						return genshinImpactExe;
					}

					return genshinImpactExe;
				}
				case "giLauncher":
				{
					string mainDir = Directory.GetParent(gameDir)?.FullName;
					if (!Directory.Exists(mainDir))
					{
						MessageBox.Show($"Directory {mainDir} does not exists.", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
						return null;
					}

					string genshinImpactExe = $@"{mainDir}\launcher.exe";
					if (!File.Exists(genshinImpactExe) || !File.Exists(genshinImpactExe))
					{
						MessageBox.Show($"File {genshinImpactExe} does not exists.", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
						return null;
					}

					return genshinImpactExe;
				}
				default:
					return null;
			}
		}
	}
}