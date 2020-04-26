using UnityEngine;
using TMPro;

namespace Deenote
{
    [RequireComponent(typeof(TMP_Text))]
    public sealed class LocalizedText : MonoBehaviour
    {
        public enum FontType { Sans, Serif }

        [SerializeField] [HideInInspector] private TMP_Text text;
        [SerializeField] private FontType type;
        [SerializeField] private string textKeyField;
        private string[] _strings;
        public TMP_Text Text => text;
        public Color Color { set => Text.color = value; }

        public string Key
        {
            get => textKeyField;
            set
            {
                textKeyField = value;
                GetStrings();
                if (_strings != null) Text.text = _strings[(int) LanguageSelector.Language];
            }
        }

        private void OnValidate()
        {
            if (text is null)
                text = GetComponent<TMP_Text>();
        }

        private void Awake() => LanguageSelector.Texts.Add(this);
        private void Start() => SetLanguage(LanguageSelector.Language);
        private void OnDestroy() => LanguageSelector.Texts.Remove(this);

        private void GetStrings() => _strings = !string.IsNullOrEmpty(textKeyField)
            ? TextStrings.Instance[textKeyField]
            : null;

        public void SetLanguage(Language language)
        {
            if (_strings == null) GetStrings();
            int lang = (int) language;
            if (_strings != null) Text.text = _strings[lang];
            Text.font = type == FontType.Sans
                ? Parameters.Params.sansFonts[lang]
                : Parameters.Params.serifFonts[lang];
        }
    }
}
