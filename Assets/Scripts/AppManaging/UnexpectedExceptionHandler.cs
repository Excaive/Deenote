using System;
using System.IO;
using UnityEngine;

namespace Deenote
{
    public sealed class UnexpectedExceptionHandler : MonoBehaviour
    {
        public static UnexpectedExceptionHandler Instance { get; private set; }

        private void Awake()
        {
#if DEBUG
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(this);
                Debug.LogError("Error: Unexpected multiple instances of UnexpectedExceptionHandler");
            }
#else
            Instance = this;
#endif

            AppDomain.CurrentDomain.UnhandledException += OnUnexpectedException;
            Application.logMessageReceived += OnUnityLogReceived;
            if (File.Exists("exceptions.log")) File.Delete("exceptions.log");
        }

        private string ExceptionString(Exception exception)
        {
            string fullMessage = exception.Message;
            Exception inner = exception.InnerException;
            while (inner != null)
            {
                fullMessage += ", " + inner.Message;
                inner = inner.InnerException;
            }
            return $"Unhandled exception: \"{fullMessage}\", in {exception.StackTrace}";
        }

        private void OnUnityLogReceived(string condition, string stackTrace, LogType type)
        {
            if (type != LogType.Exception) return;
            RecordMessage($"Unhandled exception: \"{condition}\", in {stackTrace}");
        }

        private void OnUnexpectedException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exception = e.ExceptionObject as Exception;
            RecordMessage(ExceptionString(exception));
        }

        private void RecordMessage(string message)
        {
            using (StreamWriter streamWriter = new StreamWriter("exceptions.log", true))
                streamWriter.WriteLine(message);
            MessageBox.Instance.Activate("Error", "",
                new MessageBox.ButtonInfo {key = "Back"});
            MessageBox.Instance.Text = message;
        }
    }
}
