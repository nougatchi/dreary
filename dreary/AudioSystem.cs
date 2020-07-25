using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSCore.Codecs.MP3;
using CSCore.Streams;
using CSCore.Codecs;
using CSCore;
using CSCore.SoundOut;
using System.IO;

namespace dreary
{
    public static class AudioSystem
    {
        private static WasapiOut Wasapi;
        private static Dictionary<string, IWaveSource> PrecachedAudios;
        private static string[] AudiosPrecached;
        private static List<string> AudiosToBePrecached;

        public static void Initialize()
        {
            Wasapi = new WasapiOut(true, CSCore.CoreAudioAPI.AudioClientShareMode.Shared, 100);
            AudiosToBePrecached = new List<string>();
        }

        public static string[] GetPrecachedAudios()
        {
            return PrecachedAudios.Keys.ToArray();
        }

        public static void PrecacheAudios(params string[] audios)
        {
            string cwd = Directory.GetCurrentDirectory();
            Console.WriteLine("Precaching " + audios.Length + " audios");
            PrecachedAudios = new Dictionary<string, IWaveSource>();
            foreach (string i in audios)
            {
                Console.WriteLine("Caching " + i);
                IWaveSource waveSource = CodecFactory.Instance.GetCodec(cwd + "\\" + i);
                PrecachedAudios.Add(i, waveSource);
                Console.WriteLine("Done caching " + i + ", size of " + waveSource.Length + "B");
            }
            Console.WriteLine("Done");
            AudiosPrecached = audios;
        }

        public static void PlayAudio(string name)
        {
            PlayAudio(name, 1.0f);
        }

        public static void PlayAudio(string name, float volume)
        {
            IWaveSource wave = PrecachedAudios[name];
            Console.WriteLine($"PLAYING A {wave.WaveFormat.WaveFormatTag}, sR: {wave.WaveFormat.SampleRate}Hz bPS: {wave.WaveFormat.BitsPerSample} byPS: {wave.WaveFormat.BytesPerSecond}");
            Wasapi.Initialize(wave);
            Wasapi.Volume = volume;
            AudiosToBePrecached.Add(name);
            Wasapi.Play();
            Wasapi.Stopped += SFX_DieEV;
        }

        private static void SFX_DieEV(object sender, PlaybackStoppedEventArgs e)
        {
            Wasapi.Dispose();
            Wasapi = new WasapiOut(true, CSCore.CoreAudioAPI.AudioClientShareMode.Shared, 100);
            string cwd = Directory.GetCurrentDirectory();
            List<int> pcR = new List<int>();
            foreach (string i in AudiosToBePrecached)
            {
                Console.WriteLine("Recaching " + i);
                PrecachedAudios[i].Dispose();
                PrecachedAudios[i] = CodecFactory.Instance.GetCodec(cwd + "\\" + i);
                Console.WriteLine("Recached " + i);
                pcR.Add(AudiosToBePrecached.IndexOf(i));
            }
            foreach(int i in pcR)
            {
                AudiosToBePrecached.RemoveAt(i);
            }
        }
    }
}
