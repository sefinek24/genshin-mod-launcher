using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DiscordRPC;
using Genshin_Impact_Mod.Forms.Other;
using Genshin_Impact_Mod.Properties;
using Genshin_Impact_Mod.Scripts;
using Microsoft.Web.WebView2.Core;

namespace Genshin_Impact_Mod.Forms
{
	public partial class Tutorial : Form
	{
		private bool _mouseDown;
		private Point _offset;

		public Tutorial()
		{
			InitializeComponent();
		}

		private void MouseDown_Event(object sender, MouseEventArgs e)
		{
			_offset.X = e.X;
			_offset.Y = e.Y;
			_mouseDown = true;
		}

		private void MouseMove_Event(object sender, MouseEventArgs e)
		{
			if (!_mouseDown) return;
			Point currentScreenPos = PointToScreen(e.Location);
			Location = new Point(currentScreenPos.X - _offset.X, currentScreenPos.Y - _offset.Y);
		}

		private void MouseUp_Event(object sender, MouseEventArgs e)
		{
			_mouseDown = false;
		}

		private void Exit_Click(object sender, EventArgs e)
		{
			Close();
			Log.Output($"Closed form '{Text}'.");

			Discord.Home();
		}

		private async void Tutorial_Shown(object sender, EventArgs e)
		{
			try
			{
				CoreWebView2Environment cwv2Environment = await CoreWebView2Environment.CreateAsync(null, Program.AppData, new CoreWebView2EnvironmentOptions());
				await webView21.EnsureCoreWebView2Async(cwv2Environment);

				webView21.CoreWebView2.Navigate("https://www.youtube.com/embed/rDeO26RapAk");
			}
			catch (Exception ex)
			{
				WebView2.HandleError(ex);
			}

			DiscordRpcClient client = Discord.Client;
			Discord.Presence.State = "Tutorial page 📝";
			client.SetPresence(Discord.Presence);

			Log.Output($"Loaded form '{Text}'.");
		}

		private void ViewInBrowser_Click(object sender, EventArgs e)
		{
			Process.Start("https://www.youtube.com/watch?v=rDeO26RapAk");
		}

		private void Gallery_Button(object sender, EventArgs e)
		{
			Gallery frm = new Gallery { Icon = Resources.icon_52x52 };
			if (Application.OpenForms.OfType<Gallery>().Count() == 1) Application.OpenForms.OfType<Gallery>().First().Close();
			frm.Show();
		}

		private void Discord_Button(object sender, EventArgs e)
		{
			Process.Start("https://discord.gg/SVcbaRc7gH");
		}

		private void Website_Button(object sender, EventArgs e)
		{
			Process.Start(Program.AppWebsite);
		}
	}
}