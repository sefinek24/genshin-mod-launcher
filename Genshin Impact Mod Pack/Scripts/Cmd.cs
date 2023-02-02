using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using CliWrap;
using CliWrap.Buffered;
using Genshin_Impact_Mod.Forms;

namespace Genshin_Impact_Mod.Scripts
{
	internal abstract class Cmd
	{
		public static async Task Execute(string app, string args, string workingDir, bool exit, bool bypassUpdates, bool downloadSetup)
		{
			try
			{
				Log.Output($"Execute command: {app} {args} {workingDir}");

				if (Default.UpdateIsAvailable && !bypassUpdates)
				{
					MessageBox.Show("Sorry, update is required.", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
					Log.Output("Failed. Update is required.");
					return;
				}

				// CliWrap
				Command action = Cli.Wrap(app).WithArguments(args).WithWorkingDirectory(workingDir).WithValidation(CommandResultValidation.None);
				BufferedCommandResult result = await action.ExecuteBufferedAsync();

				// Variables
				string stdout = result.StandardOutput;
				string stderr = result.StandardError;

				// StandardOutput
				string stdoutLine = !string.IsNullOrEmpty(stdout) ? $"\n✅ STDOUT: {stdout}" : "";
				string stderrLine = !string.IsNullOrEmpty(stderr) ? $"\n❌ STDERR: {stderr}" : "";
				Log.Output($"Successfully executed {app} command. Exit code: {result.ExitCode}, start time: {result.StartTime}, exit time: {result.ExitTime}{stdoutLine}{stderrLine}");

				// StandardError
				if (result.ExitCode != 0)
				{
					string showCommand = !string.IsNullOrEmpty(app) ? $"\n\n» Executed command:\n{app} {args}" : "";
					string showWorkingDir = !string.IsNullOrEmpty(workingDir) ? $"\n\n» Working directory: {workingDir}" : "";
					string showExitCode = !double.IsNaN(result.ExitCode) ? $"\n\n» Exit code: {result.ExitCode}" : "";
					string showError = !string.IsNullOrEmpty(stderr) ? $"\n\n» Error:\n{stderr}" : "";
					string info = $"{showCommand}{showWorkingDir}{showExitCode}{showError}";

					if (!downloadSetup)
						Log.ErrorAndExit(new Exception($"Command execution failed because the underlying process ({app}) returned a non-zero exit code - {result.ExitCode}.\n{info}"));
					else
						Log.ErrorAuditLog(new Exception(info));
				}

				// Exit
				if (exit) Application.Exit();
			}
			catch (Exception ex)
			{
				Log.Error(ex);
			}
		}
	}
}