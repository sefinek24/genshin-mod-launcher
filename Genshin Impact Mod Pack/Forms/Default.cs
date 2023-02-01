using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ByteSizeLib;
using Genshin_Impact_Mod.Forms.Errors;
using Genshin_Impact_Mod.Forms.Other;
using Genshin_Impact_Mod.Models;
using Genshin_Impact_Mod.Properties;
using Genshin_Impact_Mod.Scripts;
using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json;

// 1 = ReShade + FPS Unlocker
// 2 = ReShade
// 3 = FPS Unlocker

namespace Genshin_Impact_Mod.Forms
{
	public partial class Default : Form
	{
		private static readonly string LaunchModeFile = $@"{Program.AppData}\launch-mode.sfn";
		private static readonly string SetupPathExe = $"{Path.GetTempPath()}Genshin_Impact_Mod_Setup.exe";
		public static bool UpdateIsAvailable;
		private int _bgInt;

		private bool _mouseDown;
		private Point _offset;

		public Default()
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

		private void ShowPanels(bool show)
		{
			panel3.Visible = show;
			panel4.Visible = show;
			panel5.Visible = show;
		}

		private void ChangeBackground_Click(object sender, EventArgs e)
		{
			_bgInt++;

			string fileName;
			switch (_bgInt)
			{
				case 1:
					fileName = "collei_1";
					ShowPanels(true);
					break;
				case 2:
					fileName = "kokomi_3-1";
					ShowPanels(false);
					break;
				case 3:
					fileName = "collei_3";
					break;
				case 4:
					fileName = "collei_2";
					break;
				case 5:
					fileName = "hutao_3";
					break;
				default:
					BackgroundImage = new Bitmap(Resources.kokomi_1);
					_bgInt = 0;
					return;
			}

			BackgroundImage = new Bitmap($@"{Program.Folder}\Screenshots\Default\Backgrounds\{fileName}.png");
		}

		private void Exit_Click(object sender, EventArgs e)
		{
			Close();
		}

		// Check updates...
		private async Task<int> CheckUpdates()
		{
			updates_Label.LinkColor = Color.White;
			updates_Label.Text = @"Checking for updates...";
			Log.Output("Checking for new updates...");

			try
			{
				WebClient client = new WebClient();
				client.Headers.Add("user-agent", Program.UserAgent);
				string json = await client.DownloadStringTaskAsync("https://api.sefinek.net/api/v1/genshin-impact-reshade/launcher/version");
				LauncherApi res = JsonConvert.DeserializeObject<LauncherApi>(json);

				if (res.Version[0] != Program.AppVersion[0])
				{
					new NotCompatible { Icon = Resources.icon_52x52 }.ShowDialog();

					Log.Output($"New major version is available: v{Program.AppVersion} → v{res.Version} ({res.Date})");
					UpdateIsAvailable = true;
					Environment.Exit(0);
				}

				if (res.Version != Program.AppVersion)
				{
					// 1
					version_Label.Text = $@"v{Program.AppVersion} → v{res.Version}";

					// 2
					updates_Label.LinkColor = Color.Cyan;
					updates_Label.Text = @"Click here to update";
					update_Icon.Image = Resources.icons8_download_from_the_cloud;
					Utils.RemoveClickEvent(updates_Label);
					updates_Label.Click += Update_Event;

					// 3
					WebClient wc = new WebClient();
					wc.Headers.Add("user-agent", Program.UserAgent);
					wc.OpenRead("https://cdn.sefinek.net/resources/genshin-impact-reshade/launcher/download.exe");
					string updateSize = ByteSize.FromBytes(Convert.ToInt64(wc.ResponseHeaders["Content-Length"])).MegaBytes.ToString("00.00");
					status_Label.Text += $"[i] New version from {res.Date} is available.\n[i] Update size: {updateSize} MB\n";

					// 4
					UpdateIsAvailable = true;

					try
					{
						ToastContentBuilder builder = new ToastContentBuilder().AddText("📥 We found new updates").AddText("New version is available.");
						builder.Show();
					}
					catch (Exception e)
					{
						Log.ErrorAuditLog(e);
					}

					Log.Output($"New release is available: v{Program.AppVersion} → v{res.Version} [{updateSize} MB]");

					progressBar1.Hide();
					label3.Hide();
					label3.Text = "Preparning... If process is stuck, reopen launcher.";
					pictureBox3.Show();
					settings_Label.Show();
					pictureBox6.Show();
					createShortcut_Label.Show();
					pictureBox11.Show();
					linkLabel5.Show();
					pictureBox4.Show();
					website_Label.Show();
					progressBar1.Value = 0;
					return 1;
				}

				updates_Label.Text = @"Check for updates";
				update_Icon.Image = Resources.icons8_available_updates;
				return 0;
			}
			catch (Exception e)
			{
				updates_Label.LinkColor = Color.Red;
				updates_Label.Text = @"Ohh, something went wrong";
				status_Label.Text = $"[✖] {e.Message}";

				Log.ErrorAuditLog(new Exception($"Something went wrong while checking for new updates.\n\n{e}"));
				Log.Output(e.Message);
				return -1;
			}
		}

		private async void Main_Shown(object sender, EventArgs e)
		{
			version_Label.Text = $@"v{Program.AppVersion}";
			Log.Output($"Loaded form '{Text}'.");

			if (!File.Exists($@"{Program.Folder}\Data\Unlocker\unlockfps_clr.exe")) status_Label.Text += "[WARN]: Data/Unlocker/unlockfps_clr.exe was not found.\n";
			if (!File.Exists($@"{Program.Folder}\Data\ReShade\inject64.exe")) status_Label.Text += "[WARN]: Data/ReShade/inject64.exe was not found.\n";
			if (!File.Exists($@"{Program.Folder}\Data\ReShade\ReShade64.dll")) status_Label.Text += "[WARN]: Data/ReShade/ReShade64.dll was not found.\n";
			if (!File.Exists(Program.UnlockerFpsPath))
			{
				status_Label.Text += "[i] Downloading config file for FPS Unlocker...\n";

				try
				{
					WebClient client = new WebClient();
					client.Headers.Add("user-agent", Program.UserAgent);
					await client.DownloadFileTaskAsync("https://cdn.sefinek.net/resources/genshin-impact-reshade/unlocker-config", Program.UnlockerFpsPath);

					string fpsUnlockerCfg = File.ReadAllText(Program.UnlockerFpsPath);
					File.WriteAllText(Program.UnlockerFpsPath, fpsUnlockerCfg.Replace("{GamePath}", @"C:\\Program Files\\Genshin Impact\\Genshin Impact game\\GenshinImpact.exe"));

					status_Label.Text += @"[✔] Success!\n";
				}
				catch (Exception ex)
				{
					status_Label.Text += $"[✖] {ex.Message}\n";
				}
			}

			if (status_Label.Text.Length > 0) Log.ErrorAuditLog(new Exception(status_Label.Text));

			if (File.Exists(SetupPathExe))
			{
				File.Delete(SetupPathExe);
				status_Label.Text += "[i] Deleted old setup file from temp directory.\n";
			}

			await CheckUpdates();

			Discord.RunRpc();

			if (Debugger.IsAttached) return;
			Discord.WebHook_Opened(Program.LaunchCountGlobal);

			// Music
			try
			{
				SoundPlayer player = new SoundPlayer { SoundLocation = @"Data\music.wav" };
				player.Play();
			}
			catch (Exception e2)
			{
				status_Label.Text += $"[✖]: {e2.Message}\n";
				Log.ErrorAuditLog(e2);
			}
		}

		// ------- Body -------
		private void GitHub_Click(object sender, EventArgs e)
		{
			Process.Start("https://github.com/sefinek24/Genshin-Impact-ReShade");
		}

		private void AnimeGirl1_DoubleClick(object sender, EventArgs e)
		{
			MessageBox.Show(@"(^o.o^) ugh??", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Question);
		}

		private void AnimeGirl2_DoubleClick(object sender, EventArgs e)
		{
			MessageBox.Show(@"ฅ^˙Ⱉ˙^ฅ rawr!", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void AnimeGirl3_DoubleClick(object sender, EventArgs e)
		{
			MessageBox.Show(@"(^=_=^)", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);

			WebClient client = new WebClient();
			client.Headers.Add("user-agent", Program.UserAgent);
			string json = client.DownloadString("https://api.alexflipnote.dev/sadcat");
			AlexflipnoteApi res = JsonConvert.DeserializeObject<AlexflipnoteApi>(json);

			Process.Start(res.File);
		}

		// ------- Start the game -------
		private async void StartGame_Click(object sender, EventArgs e)
		{
			using (StreamWriter sw = File.CreateText(LaunchModeFile))
			{
				await sw.WriteAsync("1");
			}

			await Cmd.Execute("wt", $@"{Program.Folder}\Data\Cmd\start.cmd", null, true, false, false);
		}

		private async void OnlyReShade_Click(object sender, EventArgs e)
		{
			using (StreamWriter sw = File.CreateText(LaunchModeFile))
			{
				await sw.WriteAsync("2");
			}

			await Cmd.Execute("wt", $@"{Program.Folder}\Data\Cmd\start.cmd", null, true, false, false);
		}

		private async void OnlyUnlocker_Click(object sender, EventArgs e)
		{
			using (StreamWriter sw = File.CreateText(LaunchModeFile))
			{
				await sw.WriteAsync("3");
			}

			await Cmd.Execute("wt", $@"{Program.Folder}\Data\Cmd\start.cmd", null, true, false, false);
		}

		private async void OpenGILauncher_Click(object sender, EventArgs e)
		{
			string gamePath = File.ReadAllLines($@"{Program.AppData}\game-path.sfn").First();
			string gameDir = Directory.GetParent(gamePath)?.FullName;
			string mainDir = Directory.GetParent(gameDir)?.FullName;

			if (!Directory.Exists(mainDir) || !File.Exists($@"{mainDir}\launcher.exe"))
			{
				MessageBox.Show($"Your game was not found in {mainDir}.", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			await Cmd.Execute($@"{mainDir}\launcher.exe", null, null, false, true, false);
		}

		// ------- Footer -------
		private void Patron_Click(object sender, EventArgs e)
		{
			Process.Start("https://www.patreon.com/sefinek");
		}

		private void KoFi_Click(object sender, EventArgs e)
		{
			Process.Start("https://ko-fi.com/sefinek");
		}

		private void Discord_Button(object sender, EventArgs e)
		{
			Process.Start("https://discord.com/invite/SVcbaRc7gH");
		}

		private void YouTube_Button(object sender, EventArgs e)
		{
			Process.Start("https://www.youtube.com/@sefinek");
		}

		private void Settings_Click(object sender, EventArgs e)
		{
			Tools frm = new Tools { Location = Location, StartPosition = FormStartPosition.Manual, Icon = Resources.icon_52x52 };
			if (Application.OpenForms.OfType<Tools>().Count() == 1) Application.OpenForms.OfType<Tools>().First().Close();
			frm.Show();
		}

		private void TutorialHelp_Click(object sender, EventArgs e)
		{
			Tutorial frm = new Tutorial { Location = Location, StartPosition = FormStartPosition.Manual, Icon = Resources.icon_52x52 };
			if (Application.OpenForms.OfType<Tutorial>().Count() == 1) Application.OpenForms.OfType<Tutorial>().First().Close();
			frm.Show();
		}

		private void URLs_Click(object sender, EventArgs e)
		{
			Links frm = new Links { Location = Location, Icon = Resources.icon_52x52 };
			if (Application.OpenForms.OfType<Links>().Count() == 1) Application.OpenForms.OfType<Links>().First().Close();
			frm.Show();
		}

		private void Website_Click(object sender, EventArgs e)
		{
			Process.Start(Program.AppWebsite);
		}

		private void Version_Click(object sender, EventArgs e)
		{
			Process.Start("https://github.com/sefinek24/Genshin-Impact-ReShade/blob/main/CHANGELOG.md");
		}

		private async void CheckUpdates_Click(object sender, EventArgs e)
		{
			int update = await CheckUpdates();
			if (update != 0) return;

			updates_Label.LinkColor = Color.LawnGreen;
			updates_Label.Text = @"You have the latest version";
			update_Icon.Image = Resources.icons8_available_updates;

			Log.Output("You have the latest release.");
		}

		private void Update_Event(object sender, EventArgs e)
		{
			updates_Label.LinkColor = Color.DodgerBlue;
			updates_Label.Text = @"Updating. Please wait...";
			Utils.RemoveClickEvent(updates_Label);

			try
			{
				progressBar1.Show();
				label3.Show();

				pictureBox3.Hide();
				settings_Label.Hide();
				pictureBox6.Hide();
				createShortcut_Label.Hide();
				pictureBox11.Hide();
				linkLabel5.Hide();
				pictureBox4.Hide();
				website_Label.Hide();

				StartDownload();
			}
			catch (Exception ex)
			{
				Log.Error(ex);
			}
		}

		private void StartDownload()
		{
			if (File.Exists(SetupPathExe))
			{
				File.Delete(SetupPathExe);
				status_Label.Text += "[i] Deleted old setup file from temp directory.\n";
			}

			Thread thread = new Thread(() =>
			{
				WebClient client = new WebClient();
				client.Headers.Add("user-agent", Program.UserAgent);
				client.DownloadProgressChanged += Client_DownloadProgressChanged;
				client.DownloadFileCompleted += Client_DownloadFileCompleted;
				client.DownloadFileAsync(new Uri("https://cdn.sefinek.net/resources/genshin-impact-reshade/launcher/download.exe"), SetupPathExe);
			});
			thread.Start();
		}

		private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
		{
			BeginInvoke((MethodInvoker)delegate
			{
				label3.Text = $"Downloading... {ByteSize.FromBytes(e.BytesReceived).MegaBytes:00.00} MB of {ByteSize.FromBytes(e.TotalBytesToReceive).MegaBytes:000.00} MB";

				double bytesIn = double.Parse(e.BytesReceived.ToString());
				double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
				progressBar1.Value = int.Parse(Math.Truncate(bytesIn / totalBytes * 100).ToString(CultureInfo.InvariantCulture));
			});
		}

		private async void RunSetup()
		{
			if (!Directory.Exists($@"{Program.AppData}\updates")) Directory.CreateDirectory($@"{Program.AppData}\updates");

			await Cmd.Execute(SetupPathExe, $"/LOG=\"{Program.AppData}\\updates\\1.log", null, false, true, true);

			label3.Text = "✖ Bruh. Operation was canceled.";

			updates_Label.LinkColor = Color.Red;
			updates_Label.Text = @"Aborted - try again";
			updates_Label.Click += CheckUpdates_Click;
		}

		private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
		{
			BeginInvoke((MethodInvoker)delegate
			{
				RunSetup();

				label3.Text = "✔ Successfully! Now it's time to install.";
			});
		}

		private void Paimon_Click(object sender, EventArgs e)
		{
			RandomImg frm = new RandomImg { Location = Location, Icon = Resources.icon_52x52 };
			if (Application.OpenForms.OfType<RandomImg>().Count() == 1) Application.OpenForms.OfType<RandomImg>().First().Close();
			frm.Show();
		}
	}
}