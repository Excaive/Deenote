using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Deenote.LegacyFormats
{
    [Serializable]
    public sealed class Chart
    {
        public float speed;
        public int difficulty;
        public string level = "";
        public List<float> beats = new List<float>();
        public List<Note> notes = new List<Note>();
    }

    [Serializable]
    public sealed class Note
    {
        public float position;
        public float size;
        public float time;
        public float shift;
        public List<PianoSound> sounds = new List<PianoSound>();
        public bool isLink;
        public int prevLink = -1;
        public int nextLink = -1;
    }

    [Serializable]
    public sealed class PianoSound
    {
        public float delay; // w
        public float duration; // d
        public int pitch; // p
        public int volume; // v
    }

    [Serializable]
    public sealed class Project
    {
        public string name = "";
        public string chartMaker = "";
        public string songName = "";
        public Chart[] charts = { };
    }

    [Serializable]
    public sealed class DsprojV1
    {
        public Project project;

        public float[] sampleData = { };
        public int frequency;
        public int channel;
        public int length;
    }

    [Serializable]
    public sealed class DsprojV2
    {
        public Project project;
        public byte[] audio = { };
        public string audioType;
    }

    public sealed class LegacyDsprojBinder : SerializationBinder
    {
        private const string LegacyAssemblyName =
            "Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
        private const string LegacyAssemblyShortName = "Assembly-CSharp";
        private readonly string currentAssemblyName = typeof(LegacyDsprojBinder).Assembly.FullName;
        private readonly string currentNamespace = typeof(LegacyDsprojBinder).Namespace;
        private readonly Regex wordRegex = new Regex(@"^\w*$");
        private readonly Regex legacyRegex;

        public LegacyDsprojBinder() =>
            legacyRegex = new Regex($@"(\w*), {Regex.Escape(LegacyAssemblyName)}",
                RegexOptions.Compiled);

        private string ConvertTypeName(string name)
        {
            if (wordRegex.IsMatch(name))
                return $"{currentNamespace}.{name}";
            return legacyRegex.Replace(name,
                match => $"{currentNamespace}.{match.Groups[1].Value}, {currentAssemblyName}");
        }

        public override Type BindToType(string assemblyName, string typeName)
        {
            if (assemblyName == LegacyAssemblyName || assemblyName == LegacyAssemblyShortName)
                assemblyName = currentAssemblyName;
            switch (typeName)
            {
                case "SerializableProjectData": return typeof(DsprojV1);
                case "FullProjectDataV2": return typeof(DsprojV2);
                default:
                    string name = $"{ConvertTypeName(typeName)}, {assemblyName}";
                    Type type = Type.GetType(name);
                    return type;
            }
        }
    }
}
