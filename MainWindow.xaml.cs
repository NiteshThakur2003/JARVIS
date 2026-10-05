
using System;
using System.Speech.Synthesis;
using System.Windows;
using JarvisAI.Services;

namespace JarvisAI
{
    public partial class MainWindow : Window
    {
        private readonly CommandProcessor _commandProcessor;
        private readonly SpeechService _speechService;
        private readonly SpeechSynthesizer _voice;

        public MainWindow()
        {
            InitializeComponent();

            _commandProcessor = new CommandProcessor();
            _speechService = new SpeechService();
            _voice = new SpeechSynthesizer();

            // Select an English voice if available
            try
            {
                _voice.SelectVoiceByHints(
                    VoiceGender.Male,
                    VoiceAge.Adult,
                    0,
                    new System.Globalization.CultureInfo("en-US"));
            }
            catch
            {
                // Use the default installed voice
            }

            // Connect speech events
            _speechService.SpeechRecognized +=
                SpeechService_SpeechRecognized;

            _speechService.StatusChanged +=
                SpeechService_StatusChanged;

            // Start listening automatically when the window loads
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(
            object sender, RoutedEventArgs e)
        {
            _speechService.StartListening();
            Speak("Hello Nitesh. I am listening.");
        }


        private async void Speak(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            try
            {
                // Stop the current speech before speaking again.
                _voice.SpeakAsyncCancelAll();

                // Speak without blocking the UI.
                await _voice.SpeakAsync(message);
            }
            catch (Exception ex)
            {
                StatusText.Text = "Speech error: " + ex.Message;
            }
        }


        private void StartButton_Click(
            object sender, RoutedEventArgs e)
        {
            // Safe to call again; SpeechService checks
            // whether listening is already active.
            _speechService.StartListening();
        }

        private void SpeechService_SpeechRecognized(
            object? sender, string text)
        {
            Dispatcher.Invoke(() =>
            {
                StatusText.Text = "Command: " + text;

                string response =
                    _commandProcessor.ProcessCommand(text);

                StatusText.Text = response;
                Speak(response);
            });
        }

        private void SpeechService_StatusChanged(
            object? sender, string message)
        {
            Dispatcher.Invoke(() =>
            {
                StatusText.Text = message;
            });
        }

        private void ChromeButton_Click(
            object sender, RoutedEventArgs e)
        {
            RunCommand("open chrome");
        }

        private void OutlookButton_Click(
            object sender, RoutedEventArgs e)
        {
            RunCommand("open outlook");
        }

        private void CalculatorButton_Click(
            object sender, RoutedEventArgs e)
        {
            RunCommand("open calculator");
        }

        private void ExplorerButton_Click(
            object sender, RoutedEventArgs e)
        {
            RunCommand("open file explorer");
        }

        private void RunCommand(string command)
        {
            string response =
                _commandProcessor.ProcessCommand(command);

            StatusText.Text = response;
            Speak(response);
        }

        protected override void OnClosed(EventArgs e)
        {
            _speechService.StopListening();
            _voice.SpeakAsyncCancelAll();
            _voice.Dispose();

            base.OnClosed(e);
        }
    }
}
