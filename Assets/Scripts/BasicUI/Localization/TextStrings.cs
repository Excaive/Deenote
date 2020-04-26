using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Deenote
{
    public sealed class TextStrings : MonoBehaviour
    {
        public static TextStrings Instance { get; private set; }
        private Dictionary<string, string[]> _strings;

        public TextAsset stringsFile;

        public string[] this[string textKey] => _strings[textKey];

        private void Awake()
        {
#if DEBUG
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(this);
                Debug.LogError("Error: Unexpected multiple instances of TextStrings");
            }
#else
            Instance = this;
#endif
            // Read the strings
            _strings = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(stringsFile.text);
        }
    }
}
