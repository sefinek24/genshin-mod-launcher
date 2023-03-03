using System;
using System.Diagnostics;
using System.Windows.Forms;
using Genshin_Stella_Mod.Scripts;

namespace Genshin_Stella_Mod.Forms.Errors
{
    public partial class NotInstalledViaSetup : Form
    {
        public NotInstalledViaSetup()
        {
            InitializeComponent();
        }

        private void NotConfigured_Load(object sender, EventArgs e)
        {
            Log.Output($"Loaded form '{Text}'.");
            Log.ErrorAuditLog(new Exception(
                $"Launcher is not installed using our installation wizard.\n\nApplication data: {Program.AppData}"));
        }

        private void NotInstalledViaSetup_FormClosed(object sender, FormClosedEventArgs e)
        {
            Log.Output($"Closed form '{Text}'.");
        }

        private void Installer_Button(object sender, EventArgs e)
        {
            Process.Start(Program.AppWebsite);
        }

        private void Discord_Button(object sender, EventArgs e)
        {
            Process.Start(Discord.Invitation);
        }
    }
}
