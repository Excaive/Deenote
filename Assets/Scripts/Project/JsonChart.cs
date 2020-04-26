using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Deenote
{
    public sealed class JsonChart
    {
        public float speed;
        [CanBeNull] public JsonNote[] notes;
        [CanBeNull] public List<JsonLink> links;
    }

    [JsonObject(IsReference = true)]
    public sealed class JsonNote
    {
        public int type;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [CanBeNull] public JsonPianoSound[] sounds;
        public float pos;
        public float size;
        // ReSharper disable once InconsistentNaming
        public float _time;
        public float shift;
        public float time;
    }

    public sealed class JsonLink
    {
        [CanBeNull] public List<JsonNote> notes;
    }

    public sealed class JsonPianoSound
    {
        public float w;
        public float d;
        public short p;
        public short v;
    }
}
