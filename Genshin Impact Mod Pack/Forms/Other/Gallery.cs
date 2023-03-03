using System;
using System.Windows.Forms;
using DiscordRPC;
using Genshin_Stella_Mod.Scripts;
using Microsoft.Web.WebView2.Core;

namespace Genshin_Stella_Mod.Forms.Other
{
    public partial class Gallery : Form
    {
        public Gallery()
        {
            InitializeComponent();
        }

        private void Gallery_Load(object sender, EventArgs e)
        {
            InitBrowser();

            var client = Discord.Client;
            Discord.Presence.State = "Gallery page 📷";
            client.SetPresence(Discord.Presence);

            Log.Output($"Loaded form '{Text}'.");
        }

        private async void InitBrowser()
        {
            var coreWeb =
                await CoreWebView2Environment.CreateAsync(null, Program.AppData, new CoreWebView2EnvironmentOptions());
            await webView21.EnsureCoreWebView2Async(coreWeb);

            webView21.CoreWebView2.Navigate($"{Program.AppWebsite}/gallery");
        }

        private void Gallery_FormClosed(object sender, FormClosedEventArgs e)
        {
            Log.Output($"Closed form '{Text}'.");
            Discord.Home();
        }
    }
}
