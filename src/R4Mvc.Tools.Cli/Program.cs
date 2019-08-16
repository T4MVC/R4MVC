using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace R4Mvc.Tools.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
            {
                Console.WriteLine("Sorry, this tol is only available on Windows based platforms at this time.");
                return;
            }
            var currentPath = Path.GetDirectoryName(typeof(Program).Assembly.Location);
            var toolPath = Path.Combine(currentPath, "..", "..", "R4Mvc.Tools.exe");

            var startInfo = new ProcessStartInfo
            {
                FileName = toolPath,
                Arguments = string.Join(" ", args.Select(a => a.Contains(' ') ? $"\"{a}\"" : a)),
                WorkingDirectory = Environment.CurrentDirectory,
                ErrorDialog = false,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
            };

            using (var process = new Process { StartInfo = startInfo, EnableRaisingEvents = true })
            using (var mreOut = new ManualResetEvent(false))
            using (var mreErr = new ManualResetEvent(false))
            {
                process.OutputDataReceived += (o, e) =>
                {
                    if (e.Data == null) mreOut.Set();
                    Console.Out.WriteLine(e.Data);
                };
                process.ErrorDataReceived += (o, e) =>
                {
                    if (e.Data == null) mreErr.Set();
                    Console.Error.WriteLine(e.Data);
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
                mreOut.WaitOne();
                mreErr.WaitOne();
            }
        }
    }
}
