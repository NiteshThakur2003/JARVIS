
using System;
using System.Speech.Recognition;

namespace JarvisAI.Services
{
    public class SpeechService
    {
        private SpeechRecognitionEngine? _engine;
        private bool _isListening;

        public event EventHandler<string>? SpeechRecognized;
        public event EventHandler<string>? StatusChanged;

        public void StartListening()
        {
            try
            {
                if (_isListening && _engine != null)
                    return;

                StopEngine();

                var recognizers =
                    SpeechRecognitionEngine.InstalledRecognizers();

                if (recognizers.Count == 0)
                {
                    StatusChanged?.Invoke(
                        this, "No speech recognizer installed.");
                    return;
                }

                RecognizerInfo? recognizerInfo = null;

                foreach (var recognizer in recognizers)
                {
                    if (recognizer.Culture.Name.Equals(
                        "en-US",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        recognizerInfo = recognizer;
                        break;
                    }
                }

                recognizerInfo ??= recognizers[0];

                _engine = new SpeechRecognitionEngine(recognizerInfo);
                _engine.SetInputToDefaultAudioDevice();

               
var choices = new Choices(
    // Wake words
    "hello jarvis",
    "hey jarvis",

    // Open commands
    "open notepad",
    "open chrome",
    "open google chrome",
    "open browser",
    "open outlook",
    "open email",
    "open calculator",
    "open calc",
    "open file explorer",
    "open explorer",
    "open files",
    "open youtube",
    "start chrome",
    "start notepad",
    "launch chrome",
    "launch calculator",
    "run notepad",

"open google",
"open my browser",
"open the internet",
"open my files",
"open my email",
"please open chrome",
"please open notepad",
"launch outlook",
"browse the internet",
"surf the internet",
"write a note",
"i want to open chrome",
"i need calculator",


    // Hindi open commands
    "youtube kholo",
    "notepad kholo",
    "chrome kholo",
    "calculator kholo",
    "outlook kholo",
    "chrome chalao",
    "notepad chalao",

    // Close commands
    "close notepad",
    "close chrome",
    "close browser",
    "close outlook",
    "close calculator",
    "exit chrome",
    "quit chrome",
    "stop notepad",

    // Hindi close commands
    "chrome band karo",
    "notepad band karo",
    "calculator band karo",
    "outlook band karo",

    // Time and date
    "what time is it",
    "tell me the time",
    "current time",
    "time",
    "samay kya hai",
    "kitne baje hain",
    "what is the date",
    "what's the date",
    "tell me the date",
    "today's date",
    "today date",
    "date",
    "aaj ki date",
    "aaj kya date hai"

);

                var grammarBuilder = new GrammarBuilder();
                grammarBuilder.Culture = recognizerInfo.Culture;
                grammarBuilder.Append(choices);

                _engine.LoadGrammar(new Grammar(grammarBuilder));

                _engine.SpeechRecognized += Engine_SpeechRecognized;
                _engine.RecognizeCompleted += Engine_RecognizeCompleted;

                _isListening = true;
                _engine.RecognizeAsync(RecognizeMode.Multiple);

                StatusChanged?.Invoke(
                    this, "Listening... Say another command anytime.");
            }
            catch (Exception ex)
            {
                _isListening = false;
                StatusChanged?.Invoke(this, "Speech Error: " + ex.Message);
                StopEngine();
            }
        }

        public void StopListening()
        {
            StopEngine();
            StatusChanged?.Invoke(this, "Stopped listening.");
        }

        private void StopEngine()
        {
            _isListening = false;

            if (_engine == null)
                return;

            try
            {
                _engine.SpeechRecognized -= Engine_SpeechRecognized;
                _engine.RecognizeCompleted -= Engine_RecognizeCompleted;
                _engine.RecognizeAsyncCancel();
                _engine.Dispose();
            }
            catch
            {
                // Engine may already have stopped.
            }

            _engine = null;
        }

        private void Engine_SpeechRecognized(
     object? sender,
     SpeechRecognizedEventArgs e)
        {
            string command = e.Result.Text;
            float confidence = e.Result.Confidence;

            StatusChanged?.Invoke(
                this,
                $"Heard: {command} | Confidence: {confidence:P0}");

            SpeechRecognized?.Invoke(this, command);
        }

        private void Engine_RecognizeCompleted(
            object? sender, RecognizeCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                _isListening = false;
                StatusChanged?.Invoke(
                    this, "Recognition Error: " + e.Error.Message);
            }
            else if (e.Cancelled)
            {
                _isListening = false;
                StatusChanged?.Invoke(this, "Listening stopped.");
            }
        }
    }
}
