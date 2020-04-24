// ReSharper disable InconsistentNaming

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Deenote
{
    public sealed class JsonChart
    {
        public float speed;
        public List<JsonNote> notes = new List<JsonNote>();
        public List<JsonLink> links = new List<JsonLink>();
    }

    [JsonObject(IsReference = true)]
    public sealed class JsonNote
    {
        public int type;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<JsonPianoSound> sounds;
        public float pos;
        public float size;
        public float _time;
        public float shift;
        public float time;
    }

    public sealed class JsonLink
    {
        public List<JsonNote> notes = new List<JsonNote>();
    }

    public sealed class JsonPianoSound
    {
        public float w;
        public float d;
        public short p;
        public short v;
    }
}
