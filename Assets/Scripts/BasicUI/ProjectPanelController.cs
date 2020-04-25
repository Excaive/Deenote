using UnityEngine;

namespace Deenote
{
    public sealed class ProjectPanelController : MonoBehaviour
    {
        public static ProjectPanelController Instance { get; private set; }

        private void Awake()
        {
#if DEBUG
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(this);
                Debug.LogError("Error: Unexpected multiple instances of ProjectPanelController");
            }
#else
        Instance = this;
#endif
        }

        public void NewProject()
        {
            FileExplorerController.Instance.Activate(FileExplorerController.Mode.CreateFile,
                "CreateNewProjectTitle",
                str => Debug.Log($"Got file name {str}"),
                ".dnt");
        }
    }
}
