﻿using UnityEngine;
using TMPro;

namespace Deenote
{
    public sealed class FileExplorerButton : MonoBehaviour
    {
        public delegate void Callback();

        [SerializeField] private TMP_Text buttonText;

        public Callback callback;

        public string Text { set => buttonText.text = value; }
        public Color TextColor { set => buttonText.color = value; }

        private void OnValidate()
        {
            if (buttonText is null)
                buttonText = GetComponentInChildren<TMP_Text>();
        }

        public void OnClick() => callback?.Invoke();
    }
}