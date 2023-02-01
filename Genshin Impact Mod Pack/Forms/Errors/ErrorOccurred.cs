using System;
using System.Diagnostics;
using System.Windows.Forms;
using Genshin_Impact_Mod.Scripts;

namespace Genshin_Impact_Mod.Forms.Errors
{
	public partial class ErrorOccurred : Form
	{
		public ErrorOccurred()
		{
			InitializeComponent();
		}

		private void SeeLogs_Button(object sender, EventArgs e)
		{
			Process.Start(Log.Folder);
		}

		private void Installer_Button(object sender, EventArgs e)
		{
			Process.Start(Program.AppWebsite);
		}

		private void Discord_Button(object sender, EventArgs e)
		{
			Process.Start("https://discord.gg/SVcbaRc7gH");
		}

		private async void SfcScan_Click(object sender, EventArgs e)
		{
			await Cmd.Execute("wt", $@"{Program.Folder}\Data\Cmd\scan_sys_files.cmd", null, false, true, false);
		}
	}
}