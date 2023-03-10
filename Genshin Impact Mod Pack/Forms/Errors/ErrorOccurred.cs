using System;
using System.Diagnostics;
using System.Windows.Forms;
using Genshin_Stella_Mod.Scripts;

namespace Genshin_Stella_Mod.Forms.Errors
{
    public partial class ErrorOccurred : Form
    {
        public ErrorOccurred()
        {
            InitializeComponent();
        }

        private void ErrorOccurred_Shown(object sender, EventArgs e)
        {
            Log.Output($"Loaded form '{Text}'.");
        }

        private void ErrorOccurred_FormClosed(object sender, FormClosedEventArgs e)
        {
            Log.Output($"Closed form '{Text}'.");
        }

        private void SeeLogs_Button(object sender, EventArgs e)
        {
            Process.Start(Log.Folder);
        }

        private void Reinstall_Button(object sender, EventArgs e)
        {
            Process.Start(Program.AppWebsite);
        }

        private void Discord_Button(object sender, EventArgs e)
        {
            Process.Start(Discord.Invitation);
        }

        private async void SfcScan_Click(object sender, EventArgs e)
        {
            await Cmd.Execute("wt", $@"{Program.Folder}\data\cmd\scan_sys_files.cmd", null, false, true, false);
        }
    }
}
