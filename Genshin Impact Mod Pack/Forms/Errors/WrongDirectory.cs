using System;
using System.Diagnostics;
using System.Windows.Forms;
using Genshin_Impact_Mod.Scripts;

namespace Genshin_Impact_Mod.Forms.Errors
{
	public partial class WrongDirectory : Form
	{
		public WrongDirectory()
		{
			InitializeComponent();
		}

		private void WrongDir_Shown(object sender, EventArgs e)
		{
			Log.Output($"Closed form '{Text}'.");
			Log.ErrorAuditLog(new Exception($"Invalid application path.\n\nYour: {Environment.CurrentDirectory}\nRequired: {Program.Folder}"));
		}

		private void WrongDir_FormClosed(object sender, FormClosedEventArgs e)
		{
			Log.Output($"Closed form '{Text}'.");
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