using UnityEngine;

namespace Deenote
{
    public sealed class QuitAppController : MonoBehaviour
    {
        public static QuitAppController Instance { get; private set; }

        private void Awake()
        {
#if DEBUG
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(this);
                Debug.LogError("Error: Unexpected multiple instances of QuitAppController");
            }
#else
            Instance = this;
#endif
        }

        private void ShowConfirmQuitMessage()
        {
            if (!EditTracker.Instance.unsavedEdit)
            {
                QuitApp();
                return;
            }
            MessageBox.Instance.Activate("QuitProgram", "QuitConfirm",
                new MessageBox.ButtonInfo
                {
                    key = "SaveAndQuit",
                    callback = QuitApp // TODO: also save
                },
                new MessageBox.ButtonInfo
                {
                    key = "NotSaveAndQuit",
                    callback = QuitApp
                },
                new MessageBox.ButtonInfo {key = "Back"});
        }

        private void QuitApp()
        {
            // TODO: Write app config
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void OnApplicationQuit() => ShowConfirmQuitMessage();
    }
}
