
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace JarvisAI.Services
{
    public class ApplicationService
    {
        public string OpenApplication(string application)
        {
            if (string.IsNullOrWhiteSpace(application))
                return "Please tell me which application to open.";

            string app = Normalize(application);

            try
            {
                switch (app)
                {
                    case "notepad":
                    case "text editor":
                        StartApp("notepad.exe");
                        return "Opening Notepad.";

                    case "calculator":
                    case "calc":
                        StartApp("calc.exe");
                        return "Opening Calculator.";

                    case "explorer":
                    case "file explorer":
                    case "files":
                        StartApp("explorer.exe");
                        return "Opening File Explorer.";

                    case "chrome":
                    case "google chrome":
                    case "browser":
                    case "web browser":
                        string chrome = FindChrome();

                        if (chrome == null)
                            return "Chrome was not found on this computer.";

                        StartApp(chrome);
                        return "Opening Google Chrome.";

                    case "outlook":
                    case "microsoft outlook":
                    case "email":
                        StartApp("outlook.exe");
                        return "Opening Outlook.";

                    case "youtube":
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = "https://www.youtube.com/",
                            UseShellExecute = true
                        });

                        return "Opening YouTube.";

                    default:
                        return $"I don't know how to open {application} yet.";
                }
            }
            catch (Exception ex)
            {
                return $"Could not open {application}: {ex.Message}";
            }
        }

        public string CloseApplication(string application)
        {
            if (string.IsNullOrWhiteSpace(application))
                return "Please tell me which application to close.";

            string app = Normalize(application);
            string[] processNames;

            switch (app)
            {
                case "notepad":
                case "text editor":
                    processNames = new[] { "notepad" };
                    break;

                case "calculator":
                case "calc":
                    processNames = new[] { "CalculatorApp", "calc" };
                    break;

                case "chrome":
                case "google chrome":
                case "browser":
                case "web browser":
                    processNames = new[] { "chrome" };
                    break;

                case "outlook":
                case "microsoft outlook":
                case "email":
                    processNames = new[] { "OUTLOOK" };
                    break;

                default:
                    return $"Closing {application} is not supported yet.";
            }

            try
            {
                Process[] processes = processNames
                    .SelectMany(name => Process.GetProcessesByName(name))
                    .GroupBy(p => p.Id)
                    .Select(g => g.First())
                    .ToArray();

                if (processes.Length == 0)
                    return $"{application} is not running.";

                int requested = 0;

                foreach (Process process in processes)
                {
                    try
                    {
                        if (process.CloseMainWindow())
                            requested++;
                    }
                    catch
                    {
                        // This process may not have a closable window.
                    }
                    finally
                    {
                        process.Dispose();
                    }
                }

                if (requested > 0)
                    return $"Sent a close request to {application}.";

                return $"Could not close {application}. Close it manually if needed.";
            }
            catch (Exception ex)
            {
                return $"Could not close {application}: {ex.Message}";
            }
        }

        private static string Normalize(string value)
        {
            string app = value.Trim().ToLowerInvariant();

            string[] suffixes =
            {
                " application",
                " app",
                " program"
            };

            foreach (string suffix in suffixes)
            {
                if (app.EndsWith(suffix))
                {
                    app = app.Substring(
                        0, app.Length - suffix.Length).Trim();
                }
            }

            return app;
        }

        private static void StartApp(string fileName)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = fileName,
                UseShellExecute = true
            });
        }

        private static string FindChrome()
        {
            string[] paths =
            {
                Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.ProgramFiles),
                    "Google", "Chrome", "Application", "chrome.exe"),

                Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.ProgramFilesX86),
                    "Google", "Chrome", "Application", "chrome.exe"),

                Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.LocalApplicationData),
                    "Google", "Chrome", "Application", "chrome.exe")
            };

            foreach (string path in paths)
            {
                if (File.Exists(path))
                    return path;
            }

            return null;
        }
    }
}
