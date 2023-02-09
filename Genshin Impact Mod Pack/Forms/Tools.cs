using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using DiscordRPC;
using Genshin_Impact_Mod.Scripts;
using IWshRuntimeLibrary;
using File = System.IO.File;

namespace Genshin_Impact_Mod.Forms
{
	public partial class Tools : Form
	{
		private bool _mouseDown;
		private Point _offset;

		public Tools()
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

		private void Utils_Shown(object sender, EventArgs e)
		{
			version_Label.Text = $@"v{Program.AppVersion}";

			DiscordRpcClient client = Discord.Client;
			Discord.Presence.State = "Tools page 🔧";
			client.SetPresence(Discord.Presence);

			Log.Output($"Loaded form '{Text}'.");
		}

		private static bool CreateShortcut()
		{
			try
			{
				object shDesktop = "Desktop";
				WshShell shell = new WshShell();
				string shortcutAddress = (string)shell.SpecialFolders.Item(ref shDesktop) + @"\Genshin Impact Mod.lnk";
				IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutAddress);

				shortcut.Description = "Run official mod launcher made by Sefinek.";
				shortcut.IconLocation = Environment.CurrentDirectory + @"\icons\52x52.ico";
				shortcut.WorkingDirectory = Environment.CurrentDirectory;
				shortcut.TargetPath = Assembly.GetExecutingAssembly().Location;
				shortcut.Save();

				return true;
			}
			catch (Exception e)
			{
				Log.Error(new Exception($"An error occurred while creating the shortcut.\n\n{e}"));

				return false;
			}
		}

		// ----------------- Utils -----------------
		private void CreateShortcut_Button(object sender, EventArgs e)
		{
			bool success = CreateShortcut();
			if (success) MessageBox.Show(@"The shortcut has been successfully created.", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private async void ScanSysFiles_Click(object sender, EventArgs e)
		{
			await Cmd.Execute("wt", $@"{Program.Folder}\Data\Cmd\scan_sys_files.cmd", null, false, true, false);
		}

		private async void ReShadeConfig_Click(object sender, EventArgs e)
		{
			string gamePath = File.ReadAllText($@"{Program.AppData}\game-path.sfn");
			string gameDir = Directory.GetParent(gamePath)?.FullName;
			string cfgFile = $@"{gameDir}\ReShade.ini";

			if (!Directory.Exists(gameDir) || !File.Exists(cfgFile))
				MessageBox.Show($@"ReShade config file was not found in {cfgFile}.", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			else
				await Cmd.Execute("notepad", cfgFile, null, false, true, false);
		}

		private async void UnlockerConfig_Click(object sender, EventArgs e)
		{
			await Cmd.Execute("notepad", $@"{Program.Folder}\Data\FPS Unlocker\unlocker.config.json", null, false, true, false);
		}

		// ----------------- Cache -----------------
		private async void DeleteCache_Button(object sender, EventArgs e)
		{
			await Cmd.Execute("wt", $@"{Program.Folder}\Data\Cmd\delete_cache.cmd", null, false, true, false);
		}

		private async void DeleteWebViewCache_Click(object sender, EventArgs e)
		{
			await Cmd.Execute("wt", $@"{Program.Folder}\Data\Cmd\delete_webview_cache.cmd", null, false, true, false);
		}

		// ----------------- Logs -----------------
		private async void LauncherLogs_Click(object sender, EventArgs e)
		{
			await Cmd.Execute("notepad", $@"{Log.Folder}\launcher.output.log", null, false, true, false);
		}

		private async void ErrorLogs_Click(object sender, EventArgs e)
		{
			await Cmd.Execute("notepad", $@"{Log.Folder}\launcher.error.log", null, false, true, false);
		}

		private async void ReShadeLogs_Button(object sender, EventArgs e)
		{
			string gamePath = File.ReadAllText($@"{Program.AppData}\game-path.sfn");
			string gameDir = Directory.GetParent(gamePath)?.FullName;
			string logFile = $@"{gameDir}\ReShade.log";

			if (!Directory.Exists(gameDir) || !File.Exists(logFile))
				MessageBox.Show($@"ReShade log file was not found in {logFile}.", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			else
				await Cmd.Execute("notepad", logFile, null, false, true, false);
		}

		private async void InstallationLog_Button(object sender, EventArgs e)
		{
			await Cmd.Execute("notepad", $@"{Log.Folder}\installer.output.log", null, false, true, false);
		}

		private async void InstallationErrorLog_Button(object sender, EventArgs e)
		{
			await Cmd.Execute("notepad", $@"{Log.Folder}\installer.error.log", null, false, true, false);
		}
	}
}