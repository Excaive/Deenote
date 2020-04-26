using UnityEngine;

namespace Deenote
{
    // This class exists since Unity only wants non-static methods for button callbacks,
    // and that it doesn't accept enum types for the dropdowns.
    public sealed class LanguageSelectorNonStatic : MonoBehaviour
    {
        public static LanguageSelectorNonStatic Instance { get; private set; }

        public void SetLanguage(int language) => LanguageSelector.Language = (Language) language;
        
        private void Awake()
        {
#if DEBUG
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(this);
                Debug.LogError("Error: Unexpected multiple instances of LanguageSelectorNonStatic");
            }
#else
            Instance = this;
#endif
        }
    }
}
