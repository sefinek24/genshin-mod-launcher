using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Genshin_Stella_Mod.Scripts
{
    internal class Utils
    {
        private static readonly string FileWithGamePath = $@"{Program.AppData}\game-path.sfn";

        public static string GetGame(string type)
        {
            if (!File.Exists(FileWithGamePath))
            {
                MessageBox.Show($"File with game path was not found in:\n{FileWithGamePath}", Program.AppName,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Log.Output($"File with game path was not found in: {FileWithGamePath}");
                return null;
            }

            var gameDir = File.ReadAllLines(FileWithGamePath).First();
            if (!Directory.Exists(gameDir))
            {
                MessageBox.Show($"Folder does not exists.\n{gameDir}", Program.AppName, MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                Log.Output($"Directory {gameDir} does not exists.");
                return null;
            }


            switch (type)
            {
                case "giGameDir":
                {
                    Log.Output($"Found Genshin Impact Game dir: {gameDir} [giGameDir]");
                    return gameDir;
                }

                case "giExe":
                {
                    var genshinImpactExeMain = $@"{gameDir}\Genshin Impact game\GenshinImpact.exe";
                    if (File.Exists(genshinImpactExeMain))
                    {
                        Log.Output($"Found GenshinImpact.exe in: {genshinImpactExeMain} [giExe]");
                        return genshinImpactExeMain;
                    }

                    MessageBox.Show($"File does not exists.\n{genshinImpactExeMain}", Program.AppName,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Log.Output($"File does not exists in: {genshinImpactExeMain} [giExe]");

                    var genshinImpactExeYuanShen = $@"{gameDir}\Genshin Impact game\YuanShen.exe";
                    if (File.Exists(genshinImpactExeYuanShen))
                    {
                        Log.Output($"Found GenshinImpact.exe in: {genshinImpactExeMain} [giExe]");
                        return genshinImpactExeYuanShen;
                    }

                    MessageBox.Show($"File {genshinImpactExeYuanShen} does not exists.", Program.AppName,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Log.Output($"File {genshinImpactExeYuanShen} does not exists. [giExe]");

                    return null;
                }

                case "giLauncher":
                {
                    var genshinImpactExe = $@"{gameDir}\launcher.exe";
                    if (!File.Exists(genshinImpactExe))
                    {
                        MessageBox.Show($"Launcher file does not exists.\n{genshinImpactExe}", Program.AppName,
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        Log.Output($"Launcher file does not exists in: {genshinImpactExe} [giLauncher]");
                        return null;
                    }

                    Log.Output($"Found Genshin Impact Launcher in: {genshinImpactExe} [giLauncher]");
                    return genshinImpactExe;
                }

                default:
                {
                    Log.Error(new Exception("Wrong parameter."));
                    return null;
                }
            }
        }

        public static void OpenUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                Log.Error(new Exception("URL is null or empty."));
                return;
            }

            try
            {
                Process.Start(url);
                Log.Output($"Opened '{url}' in default browser.");
            }
            catch (Exception ex)
            {
                Log.Error(new Exception($"Failed to open '{url}' in default browser.\n{ex}"));
            }
        }

        public static void RemoveClickEvent(Label button)
        {
            var eventClickField = typeof(Control).GetField("EventClick", BindingFlags.Static | BindingFlags.NonPublic);
            var eventHandler = eventClickField?.GetValue(button);
            if (eventHandler == null) return;

            var eventsProperty = button.GetType().GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Instance);
            var eventHandlerList = (EventHandlerList)eventsProperty?.GetValue(button, null);

            eventHandlerList?.RemoveHandler(eventHandler, eventHandlerList[eventHandler]);
        }
    }
}
