using Genshin_Impact_Mod.Scripts;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Genshin_Impact_Mod.Forms.Errors
{
	public partial class NotCompatible : Form
	{
		public NotCompatible()
		{
			InitializeComponent();
		}

		private void NotCompatible_Shown(object sender, EventArgs e)
		{
			Log.Output($"Loaded form '{Text}'.");
		}

		private void NotCompatible_Closed(object sender, FormClosedEventArgs e)
		{
			Log.Output($"Closed form '{Text}'.");
		}

		private void DownloadInstaller_Click(object sender, EventArgs e)
		{
			Process.Start(Program.AppWebsite);
		}
	}
}