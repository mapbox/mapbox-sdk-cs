
public bool RunCommand(string cmd, bool showCmd = false)
{
    using (Process p = new Process())
    {
        p.StartInfo = new ProcessStartInfo("cmd", @"/c """ + cmd + "\"")
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            //WorkingDirectory = Environment.GetEnvironmentVariable("ROOTDIR"),
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8
        };

        p.OutputDataReceived += (sender, e) =>
        {
            string msg;
            if (e.Data != null)
            {
                Console.WriteLine(e.Data);
            }
        };
        p.ErrorDataReceived += (sender, e) =>
        {
            string msg;
            if (e.Data != null)
            {
                Console.Error.WriteLine(e.Data);
            }
        };

        p.Start();
        Console.WriteLine("[{0}] process id: {1}", showCmd ? cmd : "process started", p.Id);
        p.BeginOutputReadLine();
        p.BeginErrorReadLine();
        p.WaitForExit();

        Console.WriteLine("[{0}] exit code: {1}", showCmd ? cmd : "process exited", p.ExitCode);
        return 0 == p.ExitCode;
    }
}
