using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Deenote
{
    public sealed class ProjectController : MonoBehaviour
    {
        public static ProjectController Instance { get; private set; }

        private string _path = ""; // When path is empty, no project is loaded

        public Project Project { get; private set; }

        private void Awake()
        {
#if DEBUG
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(this);
                Debug.LogError("Error: Unexpected multiple instances of ProjectPanelController");
            }
#else
            Instance = this;
#endif
        }

        private Project LoadLegacyProject(string path)
        {
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                try
                {
                    BinaryFormatter formatter = new BinaryFormatter
                        {Binder = new LegacyFormats.LegacyDsprojBinder()};
                    object obj = formatter.Deserialize(stream);
                    switch (obj)
                    {
                        case LegacyFormats.DsprojV1 v1:
                            return ProjectVersionConverter.FromDsprojV1(v1);
                        case LegacyFormats.DsprojV2 v2:
                            return ProjectVersionConverter.FromDsprojV2(v2);
                        default:
                            throw new IOException("Incorrect format");
                    }
                }
                catch (IOException)
                {
                    MessageBox.Instance.Activate("ProjectLoadFailTitle", "CorruptedFile",
                        new MessageBox.ButtonInfo {key = "Back"});
                    return null;
                }
            }
        }

        private void UpdateProjectFormat(string path)
        {
            Project project = LoadLegacyProject(path);
            if (project == null) return; // Failed to load

            // If loaded successfully, prompt the user for saving in new format
            void SavePrompt()
            {
                FileExplorerController.Instance.Activate(FileExplorerController.Mode.CreateFile,
                    "CreateNewProjectTitle", newPath =>
                    {
                        Project = project;
                        SaveAsNoPrompt(newPath);
                        _path = newPath;
                    }, ".dnt");
            }

            MessageBox.Instance.Activate("UpdateProjectTitle", "UpdateProjectMsg",
                new MessageBox.ButtonInfo {key = "OK", callback = SavePrompt},
                new MessageBox.ButtonInfo {key = "Cancel"});
        }

        private void LoadProject(string path)
        {
            // TODO: Ask the user whether to save current changes
            if (new FileInfo(path).Extension != ".dnt")
            {
                UpdateProjectFormat(path);
                return;
            }
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                try
                {
                    using (BinaryReader reader = new BinaryReader(stream))
                        Project = reader.ReadProject();
                }
                catch (IOException)
                {
                    MessageBox.Instance.Activate("ProjectLoadFailTitle", "CorruptedFile",
                        new MessageBox.ButtonInfo {key = "Back"});
                    return;
                }
            }
            _path = path;
        }

        private void SaveAsNoPrompt(string path)
        {
            using (FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            using (BinaryWriter writer = new BinaryWriter(stream))
                writer.Write(Project);
            EditTracker.Instance.unsavedEdit = false;
        }

        public void SaveProject() => SaveAsNoPrompt(_path);

        public void NewProject()
        {
            //FileExplorerController.Instance.Activate(FileExplorerController.Mode.CreateFile,
            //    "CreateNewProjectTitle",
            //    str => Debug.Log($"Got file name {str}"),
            //    ".dnt");
        }

        public void OpenProject()
        {
            FileExplorerController.Instance.Activate(FileExplorerController.Mode.SelectFile,
                "SelectFile", LoadProject, ".dnt", ".dsproj");
        }
    }
}
