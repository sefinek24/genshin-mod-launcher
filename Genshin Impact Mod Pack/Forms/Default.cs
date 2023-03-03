using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using ByteSizeLib;
using Genshin_Stella_Mod.Forms.Errors;
using Genshin_Stella_Mod.Forms.Other;
using Genshin_Stella_Mod.Models;
using Genshin_Stella_Mod.Properties;
using Genshin_Stella_Mod.Scripts;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.WindowsAPICodePack.Taskbar;
using Newtonsoft.Json;

// 1 = ReShade + FPS Unlocker
// 2 = ReShade
// 3 = FPS Unlocker

namespace Genshin_Stella_Mod.Forms
{
    public partial class Default : Form
    {
        private static readonly string LaunchModeFile = $@"{Program.AppData}\launch-mode.sfn";
        private static readonly string SetupPathExe = $"{Path.GetTempPath()}Genshin_Stella_Mod_Setup.exe";
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
            var currentScreenPos = PointToScreen(e.Location);
            Location = new Point(currentScreenPos.X - _offset.X, currentScreenPos.Y - _offset.Y);
        }

        private void MouseUp_Event(object sender, MouseEventArgs e)
        {
            _mouseDown = false;
        }

        private void ShowPanels(bool data)
        {
            panel3.Visible = data;
            panel4.Visible = data;
            panel5.Visible = data;
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
                    fileName = "kokomi_1";
                    _bgInt = 0;
                    break;
            }

            var path = Path.Combine(Program.Folder, "screenshots", "default", "backgrounds", $"{fileName}.png");

            if (!File.Exists(path))
            {
                fileName = "kokomi_1";
                path = Path.Combine(Program.Folder, "screenshots", "default", "backgrounds", $"{fileName}.png");
                _bgInt = 0;
            }

            BackgroundImage = new Bitmap(path);

            Log.Output($"Changed app background to '{path}'. ID: {_bgInt}");
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Default_FormClosing(object sender, FormClosingEventArgs e)
        {
            Log.Output($"Closing form '{Text}'.");
        }

        private void Default_FormClosed(object sender, FormClosedEventArgs e)
        {
            Log.Output("Closed.");
        }

        private async Task<int> CheckUpdates()
        {
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Indeterminate);

            updates_Label.LinkColor = Color.White;
            updates_Label.Text = @"Checking for updates...";
            Log.Output("Checking for new versions...");

            try
            {
                var client = new WebClient();
                client.Headers.Add("user-agent", Program.UserAgent);
                var json = await client.DownloadStringTaskAsync(
                    "https://api.sefinek.net/api/v2/genshin-impact-reshade/launcher/version");
                var res = JsonConvert.DeserializeObject<LauncherApi>(json);

                if (res.Version[0] != Program.AppVersion[0])
                {
                    version_Label.Text = $"v{Program.AppVersion} → v{res.Version}";

                    updates_Label.LinkColor = Color.Cyan;
                    updates_Label.Text = "Major version is available";
                    update_Icon.Image = Resources.icons8_download_from_the_cloud;

                    Log.Output(
                        $"New major version from {res.Date} is available: v{Program.AppVersion} → v{res.Version}");

                    UpdateIsAvailable = true;
                    TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
                    new NotCompatible { Icon = Resources.icon_52x52 }.ShowDialog();

                    Environment.Exit(0);
                }

                if (res.Version != Program.AppVersion)
                {
                    // 1
                    version_Label.Text = $"v{Program.AppVersion} → v{res.Version}";

                    // 2
                    updates_Label.LinkColor = Color.Cyan;
                    updates_Label.Text = "Click here to update";
                    update_Icon.Image = Resources.icons8_download_from_the_cloud;
                    Utils.RemoveClickEvent(updates_Label);
                    updates_Label.Click += Update_Event;

                    // 3
                    var wc = new WebClient();
                    wc.Headers.Add("user-agent", Program.UserAgent);
                    wc.OpenRead("https://cdn.sefinek.net/resources/genshin-impact-reshade/launcher/download.exe");
                    var updateSize = ByteSize.FromBytes(Convert.ToInt64(wc.ResponseHeaders["Content-Length"])).MegaBytes
                        .ToString("00.00");
                    status_Label.Text +=
                        $"[i] New version from {res.Date} is available.\n[i] Update size: {updateSize} MB\n";

                    // 4
                    UpdateIsAvailable = true;

                    try
                    {
                        new ToastContentBuilder().AddText("📥 We found new updates")
                            .AddText("New version is available.").Show();
                    }
                    catch (Exception e)
                    {
                        Log.ErrorAuditLog(e);
                    }

                    Log.Output(
                        $"New release from {res.Date} is available: v{Program.AppVersion} → v{res.Version} [{updateSize} MB]");

                    // Hide and show elements
                    progressBar1.Hide();
                    label3.Hide();
                    label3.Text = "🍰 Preparning... If process is stuck, reopen launcher.";
                    pictureBox3.Show();
                    settings_Label.Show();
                    pictureBox6.Show();
                    createShortcut_Label.Show();
                    pictureBox11.Show();
                    linkLabel5.Show();
                    pictureBox4.Show();
                    website_Label.Show();
                    progressBar1.Value = 0;

                    TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
                    return 1;
                }

                updates_Label.Text = "Check for updates";
                update_Icon.Image = Resources.icons8_available_updates;

                Log.Output($"Not found any new updates. Your installed version: v{Program.AppVersion}");
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
                return 0;
            }
            catch (Exception e)
            {
                updates_Label.LinkColor = Color.Red;
                updates_Label.Text = "Ohh, something went wrong";
                status_Label.Text += $"[x] {e.Message}\n";

                Log.ErrorAuditLog(new Exception($"Something went wrong while checking for new updates.\n\n{e}"));
                Log.Output(e.Message);
                return -1;
            }
        }

        private async void Main_Shown(object sender, EventArgs e)
        {
            version_Label.Text = $@"v{Program.AppVersion}";
            Log.Output($"Loaded form '{Text}'.");

            if (!File.Exists($@"{Program.Folder}\data\unlocker\unlockfps_clr.exe"))
                status_Label.Text += "[WARN]: data/unlocker/unlockfps_clr.exe was not found.\n";
            if (!File.Exists($@"{Program.Folder}\data\reshade\inject64.exe"))
                status_Label.Text += "[WARN]: data/reshade/inject64.exe was not found.\n";
            if (!File.Exists($@"{Program.Folder}\data\reshade\ReShade64.dll"))
                status_Label.Text += "[WARN]: data/reshade/ReShade64.dll was not found.\n";
            if (!File.Exists(Program.UnlockerFpsPath))
            {
                status_Label.Text += "[i] Downloading config file for FPS Unlocker...\n";
                Log.Output("Downloading config file for FPS Unlocker...");

                try
                {
                    var client = new WebClient();
                    client.Headers.Add("user-agent", Program.UserAgent);
                    await client.DownloadFileTaskAsync(
                        "https://cdn.sefinek.net/resources/genshin-impact-reshade/unlocker-config",
                        Program.UnlockerFpsPath);

                    var fpsUnlockerCfg = File.ReadAllText(Program.UnlockerFpsPath);
                    File.WriteAllText(Program.UnlockerFpsPath,
                        fpsUnlockerCfg.Replace("{GamePath}",
                            @"C:\\Program Files\\Genshin Impact\\Genshin Impact game\\GenshinImpact.exe"));

                    status_Label.Text += "[✓] Success!\n";
                    Log.Output("Done.");
                }
                catch (Exception ex)
                {
                    status_Label.Text += $"[✖] {ex.Message}\n";

                    Log.Output($"Failed. {ex.Message}");
                    Log.ErrorAuditLog(ex);
                }
            }

            if (status_Label.Text.Length > 0) Log.ErrorAuditLog(new Exception(status_Label.Text));

            if (File.Exists(SetupPathExe))
            {
                File.Delete(SetupPathExe);
                status_Label.Text += "[i] Deleted old setup file from temp directory.\n";
                Log.Output($"Deleted old setup file from temp folder: {SetupPathExe}");
            }

            await CheckUpdates();

            Discord.RunRpc();

            if (Debugger.IsAttached) return;
            WebHook.OpenedLauncher(Program.LaunchCountGlobal);

            // Music
            try
            {
                new SoundPlayer { SoundLocation = @"data\music.wav" }.Play();
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
            Utils.OpenUrl("https://github.com/sefinek24/Genshin-Impact-ReShade");
        }

        private void AnimeGirl1_DoubleClick(object sender, EventArgs e)
        {
            MessageBox.Show("(^o.o^) ugh??", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Question);
        }

        private void AnimeGirl2_DoubleClick(object sender, EventArgs e)
        {
            MessageBox.Show("ฅ^˙Ⱉ˙^ฅ rawr!", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void AnimeGirl3_DoubleClick(object sender, EventArgs e)
        {
            MessageBox.Show("(^=_=^)", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);

            try
            {
                var client = new WebClient();
                client.Headers.Add("user-agent", Program.UserAgent);
                var json = await client.DownloadStringTaskAsync("https://api.alexflipnote.dev/sadcat");
                var res = JsonConvert.DeserializeObject<AlexflipnoteApi>(json);

                Utils.OpenUrl(res.File);
                Log.Output($"GET https://api.alexflipnote.dev/sadcat - {res.File}");
            }
            catch (Exception ex)
            {
                Log.ErrorAuditLog(ex);
            }
        }

        // ------- Start the game -------
        private async void StartGame_Click(object sender, EventArgs e)
        {
            using (var sw = new StreamWriter(LaunchModeFile))
            {
                await sw.WriteAsync("1");
            }

            await Cmd.Execute("wt", $@"{Program.Folder}\data\cmd\start.cmd", null, true, false, false);
        }

        private async void OnlyReShade_Click(object sender, EventArgs e)
        {
            using (var sw = new StreamWriter(LaunchModeFile))
            {
                await sw.WriteAsync("2");
            }

            await Cmd.Execute("wt", $@"{Program.Folder}\data\cmd\start.cmd", null, false, false, false);

            var path = Utils.GetGame("giLauncher");
            if (path == null) return;
            await Cmd.Execute(path, null, null, true, false, false);
        }

        private async void OnlyUnlocker_Click(object sender, EventArgs e)
        {
            using (var sw = new StreamWriter(LaunchModeFile))
            {
                await sw.WriteAsync("3");
            }

            await Cmd.Execute("wt", $@"{Program.Folder}\data\cmd\start.cmd", null, true, false, false);
        }

        private async void OpenGILauncher_Click(object sender, EventArgs e)
        {
            var path = Utils.GetGame("giLauncher");
            if (path == null) return;

            await Cmd.Execute(path, null, null, false, true, false);
        }

        // ------- Footer -------
        private void Patron_Click(object sender, EventArgs e)
        {
            Utils.OpenUrl("https://www.patreon.com/sefinek");
        }

        private void SupportMe_Click(object sender, EventArgs e)
        {
            Utils.OpenUrl("https://sefinek.net/support-me");
        }

        private void Discord_Button(object sender, EventArgs e)
        {
            Utils.OpenUrl(Discord.Invitation);
        }

        private void YouTube_Button(object sender, EventArgs e)
        {
            Utils.OpenUrl("https://www.youtube.com/@sefinek");
        }

        private void Settings_Click(object sender, EventArgs e)
        {
            Application.OpenForms.OfType<Tools>().FirstOrDefault()?.Close();
            new Tools { Location = Location, Icon = Resources.icon_52x52 }.Show();
        }

        private void TutorialHelp_Click(object sender, EventArgs e)
        {
            Application.OpenForms.OfType<Tutorial>().FirstOrDefault()?.Close();
            new Tutorial { Location = Location, Icon = Resources.icon_52x52 }.Show();
        }

        private void URLs_Click(object sender, EventArgs e)
        {
            Application.OpenForms.OfType<Links>().FirstOrDefault()?.Close();
            new Links { Location = Location, Icon = Resources.icon_52x52 }.Show();
        }

        private void Website_Click(object sender, EventArgs e)
        {
            Utils.OpenUrl(Program.AppWebsite);
        }

        private void Version_Click(object sender, EventArgs e)
        {
            Utils.OpenUrl("https://github.com/sefinek24/Genshin-Impact-ReShade/blob/main/CHANGELOG.md");
        }

        private void Beta_Click(object sender, EventArgs e)
        {
            Utils.OpenUrl("https://www.youtube.com/watch?v=VzdQ6eFWeR0");
        }

        private void MadeBySefinek_Click(object sender, EventArgs e)
        {
            MessageBox.Show(@"It's just text. What more do you want?", Program.AppName, MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            Utils.OpenUrl("https://www.youtube.com/watch?v=FJVRE-CNafU");
        }

        private async void CheckUpdates_Click(object sender, EventArgs e)
        {
            var update = await CheckUpdates();
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

            try
            {
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

            var client = new WebClient();
            client.Headers.Add("user-agent", Program.UserAgent);
            client.DownloadProgressChanged += Client_DownloadProgressChanged;
            client.DownloadFileCompleted += Client_DownloadFileCompleted;
            client.DownloadFileAsync(
                new Uri("https://cdn.sefinek.net/resources/genshin-impact-reshade/launcher/download.exe"),
                SetupPathExe);
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            BeginInvoke((MethodInvoker)(() =>
            {
                var progress = e.TotalBytesToReceive > 0
                    ? (int)Math.Floor(e.BytesReceived / (double)e.TotalBytesToReceive * 100)
                    : 0;
                progressBar1.Value = progress;
                TaskbarManager.Instance.SetProgressValue(progress, 100);

                label3.Text =
                    $"📥 Downloading... {ByteSize.FromBytes(e.BytesReceived).MegaBytes:00.00} MB of {ByteSize.FromBytes(e.TotalBytesToReceive).MegaBytes:000.00} MB";
            }));
        }

        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            BeginInvoke((Action)(() =>
            {
                RunSetup();
                label3.Text = "✔ Successfully! Now it's time to install.";
            }));
        }

        private async void RunSetup()
        {
            if (!Directory.Exists($@"{Log.Folder}\updates")) Directory.CreateDirectory($@"{Log.Folder}\updates");

            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Indeterminate);
            var date = DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss");
            await Cmd.Execute(SetupPathExe, $"/UPDATE /NORESTART /LOG=\"{Log.Folder}\\updates\\{date}.log\"", null,
                false, true, true);

            TaskbarManager.Instance.SetProgressValue(100, 100);
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Paused);

            label3.Text = "✖ Bruh. Operation was canceled.";
            updates_Label.LinkColor = Color.Red;
            updates_Label.Text = "Aborted - try again";
            updates_Label.Click += CheckUpdates_Click;
        }

        private void Paimon_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms.OfType<RandomThings>().FirstOrDefault() != null) return;
            var randomThingsForm = new RandomThings { Location = Location, Icon = Resources.icon_52x52 };
            randomThingsForm.Show();
        }
    }
}
