using System;
using System.IO;
using System.Reflection;
using System.Text;
using DiscordRPC;
using DiscordRPC.Logging;
using JNogueira.Discord.Webhook.Client;

namespace Genshin_Impact_Mod.Scripts
{
	internal abstract class Discord
	{
		public static readonly string Invitation = "https://discord.com/invite/SVcbaRc7gH";

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
				LargeImageText = "Better graphics quality & FPS unlocker for Genshin Impact."
			},
			Timestamps = Timestamps.Now,
			Buttons = new[]
			{
				new Button { Label = "Official website", Url = Program.AppWebsite }
			}
		};

		public static void RunRpc()
		{
			Client = new DiscordRpcClient(Config.AppId, DiscordPipe) { Logger = new ConsoleLogger(LogLevel, true) };

			Client.Logger = new ConsoleLogger { Level = LogLevel.Warning };
			Client.OnReady += (sender, msg) => Log.Output($"Connected to Discord RPC with user {msg.User.Username}.");
			Client.OnPresenceUpdate += (sender, msg) => Log.Output("Presence has been updated.");

			Client.Initialize();
			Client.SetPresence(Presence);
		}

		public static void Home()
		{
			Presence.State = "In main window 🏠";
			Client.SetPresence(Presence);
		}
	}

	internal abstract class WebHook
	{
		public static async void OpenedLauncher(int launchCount)
		{
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

			MethodBase m = MethodBase.GetCurrentMethod();
			Log.Output($"Delivered WebHook with telemetry '{m?.ReflectedType?.Name}' [1].");
		}

		public static async void SendLogFiles()
		{
			Console.ForegroundColor = ConsoleColor.Blue;

			try
			{
				DiscordWebhookClient client = new DiscordWebhookClient(Config.WebHookLogFiles);

				// Content
				DiscordMessage message = new DiscordMessage($"\\💻 {Os.AllInfos}\n\\🏠 {Os.Region}\n\\📆 {DateTime.Now:HH:mm:ss, dd.MM.yyyy} • {Os.TimeZone}");

				// Files
				string content1 = "File launcher.output.log is empty.";
				if (File.Exists(Log.OutputFile)) content1 = File.ReadAllText(Log.OutputFile);
				DiscordFile launcherOutput = new DiscordFile("launcher.output.log", Encoding.UTF8.GetBytes(content1));

				string content2 = "File launcher.error.log is empty.";
				if (File.Exists(Log.ErrorFile)) content2 = File.ReadAllText(Log.ErrorFile);
				DiscordFile launcherErrors = new DiscordFile("launcher.error.log", Encoding.UTF8.GetBytes(content2));

				string content3 = "File installer.output.log is empty.";
				if (File.Exists(Log.InstOutputFile)) content3 = File.ReadAllText(Log.InstOutputFile);
				DiscordFile installerOutput = new DiscordFile("installer.output.log", Encoding.UTF8.GetBytes(content3));

				// Send
				await client.SendToDiscord(message, new[] { launcherOutput, launcherErrors, installerOutput });

				// Last logs
				Log.Output("Log files was sent to developer.");
				MethodBase m = MethodBase.GetCurrentMethod();
				Log.Output($"Delivered WebHook with telemetry '{m?.ReflectedType?.Name}' [2].");
			}
			catch (Exception ex)
			{
				Log.Error(ex);
			}
		}

		public static async void SupportMe_AnswYes()
		{
			DiscordWebhookClient client = new DiscordWebhookClient(Config.WebHookUrlDefault);
			DiscordMessage message = new DiscordMessage(embeds: new[]
			{
				new DiscordMessageEmbed(color: 14544489, author: new DiscordMessageEmbedAuthor($"💸 {Os.Region}: Form Support me - Selected Yes - v{Program.AppVersion}"),
					footer: new DiscordMessageEmbedFooter($"📅 {DateTime.Now:HH:mm:ss, dd.MM.yyyy} • {Os.TimeZone}"))
			});
			await client.SendToDiscord(message);

			MethodBase m = MethodBase.GetCurrentMethod();
			Log.Output($"Delivered WebHook with telemetry '{m?.ReflectedType?.Name}' [3].");
		}

		public static async void SupportMe_AnswNo()
		{
			DiscordWebhookClient client = new DiscordWebhookClient(Config.WebHookUrlDefault);
			DiscordMessage message = new DiscordMessage(embeds: new[]
			{
				new DiscordMessageEmbed(color: 9614021, author: new DiscordMessageEmbedAuthor($"🌨️ {Os.Region}: Form Support me - Selected No - v{Program.AppVersion}"),
					footer: new DiscordMessageEmbedFooter($"📅 {DateTime.Now:HH:mm:ss, dd.MM.yyyy} • {Os.TimeZone}"))
			});
			await client.SendToDiscord(message);

			MethodBase m = MethodBase.GetCurrentMethod();
			Log.Output($"Delivered WebHook with telemetry '{m?.ReflectedType?.Name}' [4].");
		}

		public static async void WebHook_Error(Exception ex)
		{
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

			MethodBase m = MethodBase.GetCurrentMethod();
			Log.Output($"Delivered WebHook with telemetry '{m?.ReflectedType?.Name}' [5].");
		}
	}
}