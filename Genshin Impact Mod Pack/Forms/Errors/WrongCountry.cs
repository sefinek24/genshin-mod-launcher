using System;
using System.Windows.Forms;
using Genshin_Impact_Mod.Scripts;
using Microsoft.Toolkit.Uwp.Notifications;

namespace Genshin_Impact_Mod.Forms.Errors
{
    public partial class WrongCountry : Form
    {
        public WrongCountry()
        {
            InitializeComponent();
        }

        private void WrongCountry_Shown(object sender, EventArgs e)
        {
            try
            {
                new ToastContentBuilder().AddText("kekw").AddText("axaxaxaxaxaxaxaxa, slava poland").Show();
            }
            catch (Exception ex)
            {
                Log.ErrorAuditLog(ex);
            }

            Log.Output($"Loaded form '{Text}'.");
        }

        private void WrongCountry_FormClosed(object sender, FormClosedEventArgs e)
        {
            Log.Output($"Closed form '{Text}'.");
        }
    }
}
