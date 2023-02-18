﻿using Genshin_Impact_Mod.Properties;
using Genshin_Impact_Mod.Scripts;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Genshin_Impact_Mod.Forms.Other
{
	public partial class SupportMe : Form
	{
		private bool _mouseDown;
		private Point _offset;

		public SupportMe()
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

		private void SupportMe_Shown(object sender, EventArgs e)
		{
			Log.Output($"Loaded form '{Text}'.");
		}

		private void Exit_Click(object sender, EventArgs e)
		{
			Log.Output($"Closed form '{Text}'.");
			Close();
		}

		private void WhyNot_Click(object sender, EventArgs e)
		{
			new Default { Location = Location, StartPosition = FormStartPosition.Manual, Icon = Resources.icon_52x52 }.Show();
			MessageBox.Show("Thanks :3", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);

			Log.Output($"Clicked yes in form '{Text}'.");
			WebHook.SupportMe_AnswYes();
		}

		private void NotThisTime_Click(object sender, EventArgs e)
		{
			new Default { Location = Location, StartPosition = FormStartPosition.Manual, Icon = Resources.icon_52x52 }.Show();

			Log.Output($"Clicked no in form '{Text}'.");
			WebHook.SupportMe_AnswNo();
		}
	}
}