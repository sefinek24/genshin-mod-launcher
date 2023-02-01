using System;
using System.Diagnostics;
using System.Windows.Forms;
using Genshin_Impact_Mod.Scripts;

namespace Genshin_Impact_Mod.Forms.Errors
{
	public partial class NotConfigured : Form
	{
		public NotConfigured()
		{
			InitializeComponent();
		}

		private void NotConfigured_Load(object sender, EventArgs e)
		{
			Log.ErrorAuditLog(new Exception($"Launcher is not configured by our setup.\n\nApplication data: {Program.AppData}"));
		}

		private void Installer_Button(object sender, EventArgs e)
		{
			Process.Start(Program.AppWebsite);
		}

		private void Youtube_Button(object sender, EventArgs e)
		{
			Process.Start("https://www.youtube.com/watch?v=rDeO26RapAk");
		}

		private void Discord_Button(object sender, EventArgs e)
		{
			Process.Start("https://discord.gg/SVcbaRc7gH");
		}
	}
}