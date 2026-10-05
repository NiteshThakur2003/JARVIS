
using System;
using System.Globalization;

namespace JarvisAI.Services
{
    public class CommandProcessor
    {
        private readonly ApplicationService _applicationService;

        public CommandProcessor()
        {
            _applicationService = new ApplicationService();
        }

        public string ProcessCommand(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
                return "I didn't hear a command.";

            string text = command.Trim()
                .ToLowerInvariant()
                .TrimEnd('.', '!', '?', ',');

            // Wake word
            if (text == "hello jarvis" || text == "hey jarvis")
                return "Hello Nitesh. I am ready.";

            // Time and date
            if (IsAny(text,
                "what time is it",
                "tell me the time",
                "current time",
                "time",
                "samay kya hai",
                "kitne baje hain"))
            {
                return "The time is " +
                    DateTime.Now.ToString("h:mm tt", CultureInfo.InvariantCulture);
            }

            if (IsAny(text,
                "what is the date",
                "what's the date",
                "tell me the date",
                "today's date",
                "today date",
                "date",
                "aaj ki date",
                "aaj kya date hai"))
            {
                return "Today's date is " +
                    DateTime.Now.ToString("dddd, dd MMMM yyyy",
                        CultureInfo.InvariantCulture);
            }


            // Open commands with natural language aliases
            string[] openPrefixes =
            {
                "please open ",
                "can you open ",
                "could you open ",
                "i want to open ",
                "i need ",
                "open ",
                "start ",
                "launch ",
                "run ",
                "kholo ",
                "chalao "
            };

            foreach (string prefix in openPrefixes)
            {
                if (text.StartsWith(prefix))
                {
                    string app = text.Substring(prefix.Length).Trim();

                    if (app == "internet" ||
                        app == "the internet" ||
                        app == "my browser" ||
                        app == "web browser" ||
                        app == "google" ||
                        app == "google chrome")
                    {
                        app = "chrome";
                    }
                    else if (app == "my email" ||
                             app == "mail" ||
                             app == "email")
                    {
                        app = "outlook";
                    }
                    else if (app == "my files" ||
                             app == "file manager" ||
                             app == "files")
                    {
                        app = "file explorer";
                    }

                    return OpenAlias(app);
                }
            }

            // Natural-language intent
            if (text.Contains("browse the internet") ||
                text.Contains("surf the internet"))
            {
                return OpenAlias("chrome");
            }

            if (text.Contains("write a note") ||
                text.Contains("write something"))
            {
                return OpenAlias("notepad");
            }


            // Close commands
            string[] closePrefixes =
            {
                "please close ",
                "close ",
                "exit ",
                "quit ",
                "stop "
            };

            foreach (string prefix in closePrefixes)
            {
                if (text.StartsWith(prefix))
                {
                    string app = text.Substring(prefix.Length).Trim();
                    return CloseAlias(app);
                }
            }

            // Hindi close commands
            string[] hindiCloseSuffixes =
            {
                " band karo",
                " band kar do",
                " band kro"
            };

            foreach (string suffix in hindiCloseSuffixes)
            {
                if (text.EndsWith(suffix))
                {
                    string app = text.Substring(
                        0, text.Length - suffix.Length).Trim();

                    return CloseAlias(app);
                }
            }

            return $"I heard: {command}, but I don't know that command yet.";
        }

        private string OpenAlias(string app)
        {
            switch (app)
            {
                case "google":
                case "google chrome":
                case "my browser":
                case "internet":
                case "web browser":
                    app = "chrome";
                    break;

                case "mail":
                case "email":
                case "my email":
                    app = "outlook";
                    break;

                case "files":
                case "my files":
                case "file manager":
                    app = "file explorer";
                    break;

                case "text editor":
                case "write something":
                    app = "notepad";
                    break;

                case "calc":
                    app = "calculator";
                    break;
            }

            return _applicationService.OpenApplication(app);
        }

        private string CloseAlias(string app)
        {
            switch (app)
            {
                case "google":
                case "google chrome":
                case "my browser":
                case "web browser":
                    app = "chrome";
                    break;

                case "mail":
                case "my email":
                    app = "outlook";
                    break;

                case "text editor":
                    app = "notepad";
                    break;

                case "calc":
                    app = "calculator";
                    break;
            }

            return _applicationService.CloseApplication(app);
        }

        private static bool IsAny(string text, params string[] phrases)
        {
            foreach (string phrase in phrases)
            {
                if (text == phrase)
                    return true;
            }

            return false;
        }
    }
}
