using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using DiscordRPC;
using Genshin_Impact_Mod.Scripts;

namespace Genshin_Impact_Mod.Forms
{
	public partial class Links : Form
	{
		private bool _mouseDown;
		private Point _offset;

		public Links()
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
			Log.Output($"Closed form '{Text}'.");
			Close();

			Discord.Home();
		}

		private void URLs_Shown(object sender, EventArgs e)
		{
			DiscordRpcClient client = Discord.Client;
			Discord.Presence.State = "URLs page 🌍";
			client.SetPresence(Discord.Presence);

			Log.Output($"Loaded form '{Text}'.");
		}

		private void TIMap_Click(object sender, EventArgs e)
		{
			Open.InBrowser("https://act.hoyolab.com/ys/app/interactive-map/index.html");
		}

		private void GIInterWorldMap_Click(object sender, EventArgs e)
		{
			Open.InBrowser("https://genshin-impact-map.appsample.com");
		}

		private void GIBTierList_Click(object sender, EventArgs e)
		{
			Open.InBrowser("https://genshin.gg/tier-list");
		}

		private void TLBCHD_Click(object sender, EventArgs e)
		{
			Open.InBrowser("https://game8.co/games/Genshin-Impact/archives/297465");
		}

		private void Uptimerobot_Click(object sender, EventArgs e)
		{
			Open.InBrowser("https://stats.uptimerobot.com/kLXYEukEwW");
		}

		private void Api_Click(object sender, EventArgs e)
		{
			Open.InBrowser("https://api.sefinek.net");
		}

		private void Cdn_Click(object sender, EventArgs e)
		{
			Open.InBrowser("https://cdn.sefinek.net");
		}
	}
}