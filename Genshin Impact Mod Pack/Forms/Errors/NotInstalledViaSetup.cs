using System;
using System.Diagnostics;
using System.Windows.Forms;
using Genshin_Impact_Mod.Scripts;

namespace Genshin_Impact_Mod.Forms.Errors
{
	public partial class NotInstalledViaSetup : Form
	{
		public NotInstalledViaSetup()
		{
			InitializeComponent();
		}

		private void NotConfigured_Load(object sender, EventArgs e)
		{
			Log.ErrorAuditLog(new Exception($"Launcher is not installed using our installation wizard.\n\nApplication data: {Program.AppData}"));
		}

		private void DependencyList_Click(object sender, EventArgs e)
		{
			Process.Start("https://github.com/sefinek24/Genshin-Impact-ReShade#--requirements");
		}

		private void Installer_Button(object sender, EventArgs e)
		{
			Process.Start(Program.AppWebsite);
		}

		private void Discord_Button(object sender, EventArgs e)
		{
			Process.Start("https://discord.gg/SVcbaRc7gH");
		}
	}
}