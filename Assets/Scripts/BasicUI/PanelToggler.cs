using UnityEngine;

namespace Deenote
{
    public sealed class PanelToggler : MonoBehaviour
    {
        public static PanelToggler Instance { get; private set; }

        public void TogglePanel(GameObject panel) => panel.SetActive(!panel.activeSelf);

        private void Awake()
        {
#if DEBUG
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(this);
                Debug.LogError("Error: Unexpected multiple instances of PanelToggler");
            }
#else
            Instance = this;
#endif
        }
    }
}
