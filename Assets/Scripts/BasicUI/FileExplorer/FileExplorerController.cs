using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Deenote
{
    public sealed class FileExplorerController : MonoBehaviour
    {
        public delegate void Callback(string path);

        public enum Mode { SelectFile, CreateFile, SelectDirectory }

        public static FileExplorerController Instance { get; private set; }

        [SerializeField] private GameObject child;
        [SerializeField] private LocalizedText titleText;
        [SerializeField] private FileExplorerButton buttonPrefab;
        [SerializeField] private ScrollRect scrollView;
        [SerializeField] private Transform scrollViewContent;
        [SerializeField] private TMP_InputField directoryInputField;
        [SerializeField] private TMP_InputField fileNameInputField;
        [SerializeField] private LocalizedText fileNamePlaceholder;
        [SerializeField] private Button confirmButton;

        private ObjectPool<FileExplorerButton> _buttonPool;
        private Mode _mode;
        private string[] _extensions;
        private DirectoryInfo _currentDirectory;
        private Callback _callback;
        private List<FileExplorerButton> _buttons = new List<FileExplorerButton>();

        private void OnValidate()
        {
            child = transform.GetChild(0).gameObject;
            scrollViewContent = scrollView.transform.GetChild(0).GetChild(0);
        }

        private void InitButtonPool()
        {
            _buttonPool = new ObjectPool<FileExplorerButton>(buttonPrefab, scrollViewContent);
            _buttonPool.GetCallback += item => item.gameObject.SetActive(true);
            _buttonPool.ReturnCallback += item => item.gameObject.SetActive(false);
            _buttonPool.Size = 10;
        }

        private void Awake()
        {
#if DEBUG
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(this);
                Debug.LogError("Error: Unexpected multiple instances of FileExplorerController");
            }
#else
        Instance = this;
#endif
            InitButtonPool();
        }

        private string CurrentDirectory
        {
            get
            {
                string result = _currentDirectory.FullName;
                if (result.Last() != '\\') result += '\\';
                return result;
            }
        }

        public void Deactivate()
        {
            child.SetActive(false);
            fileNameInputField.text = "";
        }

        public void Activate(Mode mode, string titleKey,
            Callback callback, params string[] extensions)
        {
            _mode = mode;
            titleText.Key = titleKey;
            _callback = callback ?? (path => { });
            _extensions = extensions;
            if (_currentDirectory == null)
                _currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());

            void SetFileNameInputFieldInteractable(bool value)
            {
                fileNameInputField.interactable = value;
                fileNamePlaceholder.Key = value ? "InputFileName" : "";
            }

            switch (_mode)
            {
                case Mode.CreateFile:
                    SetFileNameInputFieldInteractable(true);
                    confirmButton.interactable = false;
                    break;
                case Mode.SelectFile:
                    SetFileNameInputFieldInteractable(false);
                    confirmButton.interactable = false;
                    break;
                case Mode.SelectDirectory:
                    SetFileNameInputFieldInteractable(false);
                    confirmButton.interactable = true;
                    break;
                default: throw new ArgumentOutOfRangeException();
            }

            DirectoryInfo[] dirs = GetDirectories(_currentDirectory);
            if (dirs == null) return; // Unauthorized
            child.SetActive(true);
            UpdateUI(GetDirectories(_currentDirectory));
        }

        private DirectoryInfo[] GetDirectories(DirectoryInfo dir)
        {
            DirectoryInfo[] dirs = null;
            try
            {
                dirs = dir.GetDirectories();
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Instance.Activate("Error", "PathAccessDenied",
                    new MessageBox.ButtonInfo {key = "Back"});
            }
            return dirs;
        }

        private void UpdateUI(DirectoryInfo[] dirs)
        {
            directoryInputField.text = CurrentDirectory;
            for (int i = 0; i < _buttons.Count; i++)
                _buttonPool.ReturnObject(_buttons[i]);
            _buttons.Clear();

            if (_currentDirectory.Parent != null)
            {
                FileExplorerButton button = _buttonPool.GetObject();
                _buttons.Add(button);
                button.Text = "..";
                button.TextColor = Color.white.WithAlpha(0.5f);
                button.callback = () => { GoToDirectory(_currentDirectory.Parent); };
            }

            for (int i = 0; i < dirs.Length; i++)
            {
                FileExplorerButton button = _buttonPool.GetObject();
                _buttons.Add(button);
                button.Text = dirs[i].Name;
                button.TextColor = Color.white.WithAlpha(0.5f);
                int temp = i;
                button.callback = () => { GoToDirectory(dirs[temp]); };
            }

            if (_mode != Mode.SelectDirectory)
            {
                FileInfo[] files = _currentDirectory.GetFiles();
                for (int i = 0; i < files.Length; i++)
                    if (Array.Exists(_extensions, extension =>
                        string.Equals(extension, files[i].Extension, StringComparison.OrdinalIgnoreCase)))
                    {
                        FileExplorerButton button = _buttonPool.GetObject();
                        _buttons.Add(button);
                        button.Text = files[i].Name;
                        button.TextColor = Color.white;
                        int temp = i;
                        if (_mode == Mode.SelectFile)
                            button.callback = () =>
                            {
                                fileNameInputField.text = files[temp].Name;
                                confirmButton.interactable = true;
                            };
                        else
                            button.callback = () =>
                            {
                                fileNameInputField.text = files[temp].Name;
                                FileNameInputCallback();
                            };
                    }
            }
            scrollView.verticalNormalizedPosition = 1.0f;
        }

        private void GoToDirectory(DirectoryInfo dir)
        {
            DirectoryInfo[] dirs = GetDirectories(dir);
            if (dirs == null) return; // Unauthorized
            _currentDirectory = dir;
            switch (_mode)
            {
                case Mode.CreateFile:
                    break;
                case Mode.SelectFile:
                    fileNameInputField.text = "";
                    confirmButton.interactable = false;
                    break;
                case Mode.SelectDirectory:
                    fileNameInputField.text = dir.Name;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            UpdateUI(dirs);
        }

        public void DirectoryInputCallback()
        {
            DirectoryInfo directory = new DirectoryInfo(directoryInputField.text);
            if (directory.Exists)
                GoToDirectory(directory);
            else
                directoryInputField.text = CurrentDirectory;
        }

        public void FileNameInputCallback()
        {
            if (string.IsNullOrWhiteSpace(fileNameInputField.text))
            {
                confirmButton.interactable = false;
                return;
            }
            if (!fileNameInputField.text.EndsWith(_extensions[0]))
                fileNameInputField.text += _extensions[0];
            bool valid = fileNameInputField.text.IndexOfAny(Path.GetInvalidFileNameChars()) < 0;
            if (valid)
                confirmButton.interactable = true;
            else
            {
                fileNameInputField.text = "";
                confirmButton.interactable = false;
                MessageBox.Instance.Activate("Error", "FileNameInvalid",
                    new MessageBox.ButtonInfo {key = "Back"});
            }
        }

        public void Confirm()
        {
            switch (_mode)
            {
                case Mode.CreateFile:
                {
                    string result = CurrentDirectory + fileNameInputField.text;
                    FileInfo file = new FileInfo(result);
                    if (file.Exists)
                        MessageBox.Instance.Activate(
                            "OverwriteTitle", "OverwriteMsg",
                            new MessageBox.ButtonInfo
                            {
                                callback = () =>
                                {
                                    Deactivate();
                                    _callback(result);
                                },
                                key = "Confirm"
                            },
                            new MessageBox.ButtonInfo {key = "Cancel"});
                    else
                    {
                        Deactivate();
                        _callback(result);
                    }
                    break;
                }
                case Mode.SelectFile:
                {
                    string result = CurrentDirectory + fileNameInputField.text;
                    Deactivate();
                    _callback(result);
                    break;
                }
                case Mode.SelectDirectory:
                {
                    string result = CurrentDirectory;
                    Deactivate();
                    _callback(result);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
