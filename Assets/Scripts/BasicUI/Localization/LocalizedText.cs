using UnityEngine;
using TMPro;

namespace Deenote
{
    [RequireComponent(typeof(TMP_Text))]
    public sealed class LocalizedText : MonoBehaviour
    {
        public enum FontType { Sans, Serif }

        public TMP_Text Text { get; private set; }
        public Color Color { set => Text.color = value; }
        [SerializeField] private FontType type;
        [SerializeField] private string textKeyField;
        private string[] strings;

        public string TextKey
        {
            get => textKeyField;
            set
            {
                textKeyField = value;
                GetStrings();
                if (strings != null) Text.text = strings[(int) LanguageSelector.Language];
            }
        }

        private void Awake() => LanguageSelector.Texts.Add(this);
        private void Start() => TextKey = textKeyField;
        private void OnDestroy() => LanguageSelector.Texts.Remove(this);

        private void GetStrings() => strings = !string.IsNullOrEmpty(textKeyField)
            ? TextStrings.Instance[textKeyField]
            : null;

        public void SetLanguage(Language language)
        {
            if (strings == null) GetStrings();
            int lang = (int) language;
            if (strings != null) Text.text = strings[lang];
            Text.font = type == FontType.Sans
                ? Parameters.Params.sansFonts[lang]
                : Parameters.Params.serifFonts[lang];
        }

        private void OnValidate()
        {
            if (Text is null)
                Text = GetComponent<TMP_Text>();
        }
    }
}
