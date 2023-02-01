using System;
using System.IO;
using System.Text;
using DiscordRPC;
using DiscordRPC.Logging;
using JNogueira.Discord.Webhook.Client;

namespace Genshin_Impact_Mod.Scripts
{
	internal abstract class Discord
	{
		private const int DiscordPipe = -1;
		private static readonly LogLevel LogLevel = LogLevel.Trace;

		public static DiscordRpcClient Client;

		public static readonly RichPresence Presence = new RichPresence
		{
			Details = $"Version: v{Program.AppVersion}",
			State = "In main window 🐈",
			Assets = new Assets
			{
				LargeImageKey = "main",
				LargeImageText = "Better Graphics Quality + FPS Unlocker for Genshin Impact."
			},
			Timestamps = Timestamps.Now,
			Buttons = new[]
			{
				new Button { Label = "Official website", Url = "https://sefinek.net/genshin-impact-reshade" }
			}
		};

		public static async void WebHook_Opened(int launchCount)
		{
			// var telemetryFile = File.ReadAllLines(Tools.TelemetryFile)[0];
			// if (telemetryFile == "false") return;

			DiscordWebhookClient client = new DiscordWebhookClient(Config.WebHookUrlDefault);
			DiscordMessage message = new DiscordMessage(embeds: new[]
			{
				new DiscordMessageEmbed(color: 1492492, author: new DiscordMessageEmbedAuthor($"✅ {Os.Region}: Mod app was opened on PC [{launchCount}] - v{Program.AppVersion}"), fields: new[]
				{
					new DiscordMessageEmbedField("» OS", $"> {Os.Name} ", true),
					new DiscordMessageEmbedField("» Version", $"> {Os.Version}", true),
					new DiscordMessageEmbedField("» Build", $"> {Os.Build}", true)
				}, footer: new DiscordMessageEmbedFooter($"📅 {DateTime.Now:HH:mm:ss, dd.MM.yyyy} • {Os.TimeZone}"))
			});
			await client.SendToDiscord(message);
		}

		public static async void SupportMe_Form()
		{
			DiscordWebhookClient client = new DiscordWebhookClient(Config.WebHookUrlDefault);
			DiscordMessage message = new DiscordMessage(embeds: new[]
			{
				new DiscordMessageEmbed(color: 1492492, author: new DiscordMessageEmbedAuthor($"✅ {Os.Region}: Form Support me - v{Program.AppVersion}"), footer: new DiscordMessageEmbedFooter($"📅 {DateTime.Now:HH:mm:ss, dd.MM.yyyy} • {Os.TimeZone}"))
			});
			await client.SendToDiscord(message);
		}

		public static async void SendLogFiles()
		{
			Console.ForegroundColor = ConsoleColor.Blue;
			Console.Write("Sending... ");

			try
			{
				DiscordWebhookClient client = new DiscordWebhookClient(Config.WebHookLogFiles);

				// Content
				DiscordMessage message = new DiscordMessage($"\\💻 {Os.AllInfos}\n\\🏠 {Os.Region}\n\\📆 {DateTime.Now:HH:mm:ss, dd.MM.yyyy} • {Os.TimeZone}");

				// Files
				string content1 = "File launcher.output.log is empty.";
				if (File.Exists(Log.OutputFile)) content1 = File.ReadAllText(Log.OutputFile);
				DiscordFile attachment1 = new DiscordFile("launcher.output.log", Encoding.UTF8.GetBytes(content1));

				string content2 = "File launcher.error.log is empty.";
				if (File.Exists(Log.ErrorFile)) content2 = File.ReadAllText(Log.ErrorFile);
				DiscordFile attachment2 = new DiscordFile("launcher.error.log", Encoding.UTF8.GetBytes(content2));

				string content3 = "File installer.output.log is empty.";
				if (File.Exists(Log.InstOutputFile)) content3 = File.ReadAllText(Log.InstOutputFile);
				DiscordFile gitLog = new DiscordFile("installer.output.log", Encoding.UTF8.GetBytes(content3));

				// Send
				await client.SendToDiscord(message, new[] { attachment1, attachment2, gitLog });

				// Debug log
				Log.Output("Log files was sent to developer.");
			}
			catch (Exception ex)
			{
				Log.Error(ex);
			}
		}

		public static async void SupportMe_Form_Yes()
		{
			DiscordWebhookClient client = new DiscordWebhookClient(Config.WebHookUrlDefault);
			DiscordMessage message = new DiscordMessage(embeds: new[]
			{
				new DiscordMessageEmbed(color: 14544489, author: new DiscordMessageEmbedAuthor($"💸 {Os.Region}: Form Support me - Selected Yes - v{Program.AppVersion}"),
					footer: new DiscordMessageEmbedFooter($"📅 {DateTime.Now:HH:mm:ss, dd.MM.yyyy} • {Os.TimeZone}"))
			});
			await client.SendToDiscord(message);
		}

		public static async void SupportMe_Form_No()
		{
			DiscordWebhookClient client = new DiscordWebhookClient(Config.WebHookUrlDefault);
			DiscordMessage message = new DiscordMessage(embeds: new[]
			{
				new DiscordMessageEmbed(color: 9614021, author: new DiscordMessageEmbedAuthor($"🌨️ {Os.Region}: Form Support me - Selected No - v{Program.AppVersion}"),
					footer: new DiscordMessageEmbedFooter($"📅 {DateTime.Now:HH:mm:ss, dd.MM.yyyy} • {Os.TimeZone}"))
			});
			await client.SendToDiscord(message);
		}

		public static async void WebHook_Error(Exception ex)
		{
			// var telemetryFile = File.ReadAllLines(Tools.TelemetryFile)[0];
			// if (telemetryFile == "false") return;

			DiscordWebhookClient client = new DiscordWebhookClient(Config.WebHookUrlErrors);
			DiscordMessage message = new DiscordMessage(embeds: new[]
			{
				new DiscordMessageEmbed(color: 15743511, author: new DiscordMessageEmbedAuthor($"❌ {Os.Region}: Error, something went wrong - v{Program.AppVersion}"), description: $"```js\n{ex.Message.Replace("System.Exception: ", "")}```", fields: new[]
				{
					new DiscordMessageEmbedField("» OS", $"> {Os.Name} ", true),
					new DiscordMessageEmbedField("» Version", $"> {Os.Version}", true),
					new DiscordMessageEmbedField("» Build", $"> {Os.Build}", true)
				}, footer: new DiscordMessageEmbedFooter($"📅 {DateTime.Now:HH:mm:ss, dd.MM.yyyy} • {Os.TimeZone}"))
			});
			await client.SendToDiscord(message);
		}

		public static void RunRpc()
		{
			Client = new DiscordRpcClient(Config.AppId, DiscordPipe)
			{
				Logger = new ConsoleLogger(LogLevel, true)
			};

			Client.Logger = new ConsoleLogger { Level = LogLevel.Warning };
			Client.OnReady += (sender, msg) => Log.Output($"Connected to Discord RPC with user {msg.User.Username}.");
			Client.OnPresenceUpdate += (sender, msg) => Log.Output("Presence has been updated.");

			Client.Initialize();
			Client.SetPresence(Presence);
		}

		public static void Home()
		{
			Presence.State = "Home page 🏠";
			Client.SetPresence(Presence);
		}
	}
}