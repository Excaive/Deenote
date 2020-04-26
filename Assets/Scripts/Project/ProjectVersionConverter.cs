using System.IO;
using System.Linq;
using NAudio.Wave;

namespace Deenote
{
    public static class ProjectVersionConverter
    {
        private static Project FromLegacyProject(LegacyFormats.Project project) => new Project
        {
            songName = project.name,
            noter = project.chartMaker,
            musicFileName = project.songName,
            charts = project.charts.Select(chart => new Chart
            {
                speed = chart.speed,
                level = chart.level,
                notes = chart.notes.Select(note => new Note
                {
                    position = note.position,
                    size = note.size,
                    time = note.time,
                    shift = note.shift,
                    sounds = note.sounds.Select(sound => new PianoSound
                    {
                        delay = sound.delay,
                        duration = sound.duration,
                        pitch = (short) sound.pitch,
                        volume = (short) sound.volume
                    }).ToList(),
                    isLink = note.isLink,
                    previous = note.prevLink,
                    next = note.nextLink
                }).ToList()
            }).ToArray()
        };

        private static byte[] EncodeToWav(float[] samples, int sampleRate, int channels)
        {
            WaveFormat format = new WaveFormat(sampleRate, 16, channels);
            MemoryStream stream = new MemoryStream();
            using (WaveFileWriter writer = new WaveFileWriter(stream, format))
                writer.WriteSamples(samples, 0, samples.Length);
            return stream.ToArray();
        }

        public static Project FromDsprojV1(LegacyFormats.DsprojV1 dsproj)
        {
            Project result = FromLegacyProject(dsproj.project);
            result.music = EncodeToWav(dsproj.sampleData, dsproj.frequency, dsproj.channel);
            result.musicFileName = "generated.wav";
            return result;
        }

        public static Project FromDsprojV2(LegacyFormats.DsprojV2 dsproj)
        {
            Project result = FromLegacyProject(dsproj.project);
            result.music = dsproj.audio;
            return result;
        }
    }
}
