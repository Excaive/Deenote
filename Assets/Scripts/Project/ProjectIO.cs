using System.IO;
using System.Linq;

namespace Deenote
{
    public static class ProjectIO
    {
        public static readonly byte[] FileHeader = {100, 110, 116};

        public static byte[] ReadByteArray(this BinaryReader reader)
            => reader.ReadBytes(reader.ReadInt32());

        public static void WriteByteArray(this BinaryWriter writer, byte[] bytes)
        {
            writer.Write(bytes.Length);
            writer.Write(bytes);
        }

        public static TempoEvent ReadTempoEvent(this BinaryReader reader) => new TempoEvent
        {
            time = reader.ReadSingle(),
            tempo = reader.ReadSingle()
        };

        public static void Write(this BinaryWriter writer, TempoEvent tempo)
        {
            writer.Write(tempo.time);
            writer.Write(tempo.tempo);
        }

        public static PianoSound ReadPianoSound(this BinaryReader reader) => new PianoSound
        {
            delay = reader.ReadSingle(),
            duration = reader.ReadSingle(),
            pitch = reader.ReadInt16(),
            volume = reader.ReadInt16()
        };

        public static void Write(this BinaryWriter writer, PianoSound sound)
        {
            writer.Write(sound.delay);
            writer.Write(sound.duration);
            writer.Write(sound.pitch);
            writer.Write(sound.volume);
        }

        public static Note ReadNote(this BinaryReader reader)
        {
            Note note = new Note
            {
                position = reader.ReadSingle(),
                size = reader.ReadSingle(),
                time = reader.ReadSingle(),
                shift = reader.ReadSingle(),
                type = reader.ReadInt32()
            };
            int soundCount = reader.ReadInt32();
            note.sounds.Capacity = soundCount;
            for (int i = 0; i < soundCount; i++)
                note.sounds.Add(reader.ReadPianoSound());
            note.isLink = reader.ReadBoolean();
            note.previous = reader.ReadInt32();
            note.next = reader.ReadInt32();
            return note;
        }

        public static void Write(this BinaryWriter writer, Note note)
        {
            writer.Write(note.position);
            writer.Write(note.size);
            writer.Write(note.time);
            writer.Write(note.shift);
            writer.Write(note.type);
            writer.Write(note.sounds.Count);
            foreach (PianoSound sound in note.sounds)
                writer.Write(sound);
            writer.Write(note.isLink);
            writer.Write(note.previous);
            writer.Write(note.next);
        }

        public static Chart ReadChart(this BinaryReader reader)
        {
            Chart chart = new Chart
            {
                speed = reader.ReadSingle(),
                level = reader.ReadString()
            };
            int noteCount = reader.ReadInt32();
            chart.notes.Capacity = noteCount;
            for (int i = 0; i < noteCount; i++)
                chart.notes.Add(reader.ReadNote());
            return chart;
        }

        public static void Write(this BinaryWriter writer, Chart chart)
        {
            writer.Write(chart.speed);
            writer.Write(chart.level);
            writer.Write(chart.notes.Count);
            foreach (Note note in chart.notes)
                writer.Write(note);
        }

        public static Project ReadProject(this BinaryReader reader)
        {
            if (!reader.ReadBytes(FileHeader.Length).SequenceEqual(FileHeader))
                throw new IOException("Incorrect header for Deenote project file");
            if (reader.ReadInt32() != 1) // Version number
                throw new IOException("Unknown version of Deenote project file");
            Project project = new Project
            {
                songName = reader.ReadString(),
                artist = reader.ReadString(),
                noter = reader.ReadString()
            };
            for (int i = 0; i < 4; i++)
                project.charts[i] = reader.ReadChart();
            int tempoEventCount = reader.ReadInt32();
            project.tempos.Capacity = tempoEventCount;
            for (int i = 0; i < tempoEventCount; i++)
                project.tempos.Add(reader.ReadTempoEvent());
            project.musicFileName = reader.ReadString();
            project.music = reader.ReadByteArray();
            return project;
        }

        public static void Write(this BinaryWriter writer, Project project)
        {
            writer.Write(FileHeader);
            writer.Write(1); // Version number
            writer.Write(project.songName);
            writer.Write(project.artist);
            writer.Write(project.noter);
            foreach (Chart chart in project.charts)
                writer.Write(chart);
            writer.Write(project.tempos.Count);
            foreach (TempoEvent tempo in project.tempos)
                writer.Write(tempo);
            writer.Write(project.musicFileName);
            writer.WriteByteArray(project.music);
        }
    }
}
