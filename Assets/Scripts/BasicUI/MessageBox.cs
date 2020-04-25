using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Deenote
{
    public sealed class MessageBox : MonoBehaviour
    {
        public delegate void Callback();

        public static MessageBox Instance { get; private set; }

        [SerializeField] private GameObject child;
        [SerializeField] private LocalizedText title;
        [SerializeField] private LocalizedText content;
        [SerializeField] private GameObject buttonParent;
        [SerializeField] private Button[] buttons;
        [SerializeField] private LocalizedText[] buttonTexts;

        public class ButtonInfo
        {
            public string key;
            public Callback callback = () => { };
        }

        private void OnValidate()
        {
            child = transform.GetChild(0).gameObject;
            if (buttonParent is null) return;
            buttons = GetComponentsInChildren<Button>(buttonParent);
            buttonTexts = (from button in buttons
                select button.GetComponentInChildren<LocalizedText>()).ToArray();
        }

        private void Awake()
        {
#if DEBUG
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(this);
                Debug.LogError("Error: Unexpected multiple instances of MessageBox");
            }
#else
        Instance = this;
#endif
        }

        private void Deactivate() => child.SetActive(false);

        public void Activate(string titleKey, string contentKey,
            params ButtonInfo[] buttonInfos)
        {
            child.SetActive(true);
            int buttonCount = buttonInfos.Length;
#if DEBUG
            Debug.Assert(buttonCount > 0 && buttonCount <= buttons.Length);
#endif
            title.Key = titleKey;
            content.Key = contentKey;
            {
                int i;
                for (i = 0; i < buttonCount; i++)
                {
                    buttons[i].gameObject.SetActive(true);
                    var onClick = buttons[i].onClick;
                    onClick.RemoveAllListeners();
                    onClick.AddListener(Deactivate);
                    onClick.AddListener(new UnityAction(buttonInfos[i].callback));
                    buttonTexts[i].Key = buttonInfos[i].key;
                }
                for (; i < buttons.Length; i++)
                    buttons[i].gameObject.SetActive(false);
            }
        }
    }
}
