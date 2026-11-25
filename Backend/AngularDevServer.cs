using System.Diagnostics;
using Microsoft.Extensions.Logging;

internal sealed class AngularDevServer(ILogger logger)
{
    private Process? _process;

    public void Start()
    {
        var projectDirectory = FindProjectDirectory();
        var startInfo = new ProcessStartInfo
        {
            FileName = OperatingSystem.IsWindows()
                ? Environment.GetEnvironmentVariable("ComSpec") ?? "cmd.exe"
                : "npm",
            WorkingDirectory = projectDirectory,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        if (OperatingSystem.IsWindows())
        {
            startInfo.ArgumentList.Add("/d");
            startInfo.ArgumentList.Add("/s");
            startInfo.ArgumentList.Add("/c");
            startInfo.ArgumentList.Add("npm start");
        }
        else
        {
            startInfo.ArgumentList.Add("start");
        }

        var process = new Process { StartInfo = startInfo };
        process.OutputDataReceived += (_, eventArgs) =>
        {
            if (eventArgs.Data is not null)
                logger.LogInformation("Angular: {Output}", eventArgs.Data);
        };
        process.ErrorDataReceived += (_, eventArgs) =>
        {
            if (eventArgs.Data is not null)
                logger.LogError("Angular: {Error}", eventArgs.Data);
        };

        logger.LogInformation("Starting Angular development server in {ProjectDirectory}.", projectDirectory);
        try
        {
            if (!process.Start())
                throw new InvalidOperationException("Could not start the Angular development server.");
        }
        catch
        {
            process.Dispose();
            throw;
        }

        _process = process;
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
    }

    public void Stop()
    {
        var process = _process;
        _process = null;

        if (process is null)
            return;

        try
        {
            if (!process.HasExited)
            {
                logger.LogInformation("Stopping Angular development server.");
                process.Kill(entireProcessTree: true);
                if (!process.WaitForExit(5000))
                    logger.LogWarning("Angular development server did not stop within five seconds.");
            }
        }
        catch (InvalidOperationException exception)
        {
            logger.LogWarning(exception, "Could not stop the Angular development server cleanly.");
        }
        catch (System.ComponentModel.Win32Exception exception)
        {
            logger.LogWarning(exception, "Could not stop the Angular development server cleanly.");
        }
        finally
        {
            process.Dispose();
        }
    }

    private static string FindProjectDirectory()
    {
        foreach (var startingDirectory in new[] { AppContext.BaseDirectory, Directory.GetCurrentDirectory() })
        {
            for (var directory = new DirectoryInfo(startingDirectory); directory is not null; directory = directory.Parent)
            {
                var candidate = Path.Combine(directory.FullName, "uni-track");
                if (File.Exists(Path.Combine(candidate, "package.json")))
                    return candidate;
            }
        }

        throw new DirectoryNotFoundException("Could not find the uni-track Angular project containing package.json.");
    }
}