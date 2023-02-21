using System;
using System.IO;
using Genshin_Impact_Mod.Forms.Errors;
using Genshin_Impact_Mod.Properties;

namespace Genshin_Impact_Mod.Scripts
{
    internal abstract class Log
    {
        public static readonly string Folder = $@"{Program.AppData}\logs";
        public static readonly string ErrorFile = $@"{Folder}\launcher.error.log";
        public static readonly string OutputFile = $@"{Folder}\launcher.output.log";
        public static readonly string InstOutputFile = $@"{Folder}\installer.output.log";

        public static async void Output(string log)
        {
            if (!Directory.Exists(Program.AppData)) Directory.CreateDirectory(Program.AppData);
            if (!Directory.Exists(Folder)) Directory.CreateDirectory(Folder);

            using (StreamWriter sw = File.AppendText(OutputFile))
            {
                await sw.WriteLineAsync($"[{DateTime.Now}]: {log}");
            }
        }

        public static async void ErrorAuditLog(Exception log)
        {
            if (!Directory.Exists(Program.AppData)) Directory.CreateDirectory(Program.AppData);
            if (!Directory.Exists(Folder)) Directory.CreateDirectory(Folder);

            using (StreamWriter sw = File.AppendText(ErrorFile))
            {
                await sw.WriteLineAsync($"[{DateTime.Now}]: {Program.AppName} • v{Program.AppVersion}\n{log}\n\n");
            }
        }

        public static void Error(Exception log)
        {
            new ErrorOccurred { Icon = Resources.icon_52x52 }.ShowDialog();

            ErrorAuditLog(log);
        }

        public static void ErrorAndExit(Exception ex)
        {
            Error(ex);

            WebHook.WebHook_Error(ex);
            Environment.Exit(0);
        }
    }
}