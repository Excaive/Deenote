using System;
using System.Collections.Generic;
using System.Linq;

namespace Deenote
{
    public struct TempoEvent
    {
        public float time;
        public float tempo;
    }

    public sealed class PianoSound
    {
        public float delay; // w
        public float duration; // d
        public short pitch; // p
        public short volume; // v

        public JsonPianoSound ToJson()
            => new JsonPianoSound {w = delay, d = duration, p = pitch, v = volume};

        public static PianoSound FromJson(JsonPianoSound sound)
            => new PianoSound {delay = sound.w, duration = sound.d, pitch = sound.p, volume = sound.v};
    }

    public sealed class Note
    {
        public float position; // pos
        public float size = 1.0f; // size
        public float time; // _time or time
        public float shift; // shift
        public int type; // type
        public List<PianoSound> sounds = new List<PianoSound>(); // sounds
        public bool isLink; // Whether the note is a link note
        public int previous = -1; // Index of previous link note in the same link
        public int next = -1; // Index of next link note in the same link

        public bool IsShown => position <= 2.0f && position >= -2.0f;

        public JsonNote ToJson() => new JsonNote
        {
            pos = position, size = size, shift = shift,
            type = type, _time = time, time = time,
            sounds = sounds.Select(sound => sound.ToJson()).ToArray()
        };

        public static Note FromJson(JsonNote note) => new Note
        {
            position = note.pos, size = note.size, shift = note.shift,
            type = note.type, time = note.time,
            sounds = note.sounds?.Select(PianoSound.FromJson).ToList() ?? new List<PianoSound>()
        };
    }

    public sealed class Chart
    {
        public float speed; // Speed value of the official charts
        public string level = ""; // Level of the chart
        public List<Note> notes = new List<Note>(); // All the notes info

        public bool IsEmpty => notes.Count == 0;

        public static Chart FromJsonChart(JsonChart chart)
        {
            Chart result = new Chart {speed = chart.speed};
            if (chart.notes == null) return result;
            result.notes = chart.notes.Select(Note.FromJson).ToList();
            if (chart.links == null) return result;
            foreach (JsonLink link in chart.links)
            {
                if ((link.notes?.Count ?? 0) == 0) continue;
                int previous = -1;
                foreach (int index
                    in link.notes.Select(note => Array.IndexOf(chart.notes, note)))
                {
                    result.notes[index].isLink = true;
                    result.notes[index].previous = previous;
                    if (previous != -1) result.notes[previous].next = index;
                    previous = index;
                }
            }
            return result;
        }

        public JsonChart ToJsonChart()
        {
            JsonChart result = new JsonChart
            {
                speed = speed,
                notes = notes.Select(note => note.ToJson()).ToArray(),
                links = new List<JsonLink>()
            };
            bool[] appeared = new bool[notes.Count];
            for (int i = 0; i < notes.Count; i++)
                if (notes[i].isLink && !appeared[i])
                {
                    int current = i;
                    JsonLink link = new JsonLink {notes = new List<JsonNote>()};
                    while (current != -1)
                    {
                        appeared[current] = true;
                        link.notes.Add(result.notes[current]);
                        current = notes[current].next;
                    }
                    result.links.Add(link);
                }
            return result;
        }
    }

    public sealed class Project
    {
        public string songName = "";
        public string artist = "";
        public string noter = "";
        public Chart[] charts = new Chart[4].WithValue(new Chart());
        public readonly List<TempoEvent> tempos = new List<TempoEvent>();
        public string musicFileName = "";
        public byte[] music;
    }
}
