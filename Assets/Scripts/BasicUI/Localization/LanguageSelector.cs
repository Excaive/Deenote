using System.Collections.Generic;

namespace Deenote
{
    public enum Language { English, Chinese }

    public static class LanguageSelector
    {
        public static readonly List<LocalizedText> Texts = new List<LocalizedText>();
        private static Language _language = Language.English;

        public static Language Language
        {
            get => _language;
            set
            {
                _language = value;
                for (int i = 0; i < Texts.Count; i++)
                {
                    var text = Texts[i];
                    text.SetLanguage(value);
                }
            }
        }
    }
}
