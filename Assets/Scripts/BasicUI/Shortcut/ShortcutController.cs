using UnityEngine;

namespace Deenote
{
    public sealed class ShortcutController : MonoBehaviour
    {
        public static ShortcutController Instance { get; private set; }

        private void Awake()
        {
#if DEBUG
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(this);
                Debug.LogError("Error: Unexpected multiple instances of ShortcutController");
            }
#else
            Instance = this;
#endif
        }
    }
}
