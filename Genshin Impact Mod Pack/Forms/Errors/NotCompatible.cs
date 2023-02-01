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

		private void DownloadInstaller_Click(object sender, EventArgs e)
		{
			Process.Start(Program.AppWebsite);
		}
	}
}