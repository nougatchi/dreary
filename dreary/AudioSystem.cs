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

        public static void Initialize()
        {
            Wasapi = new WasapiOut(true, CSCore.CoreAudioAPI.AudioClientShareMode.Shared, 100);
        }

        public static string[] GetPrecachedAudios()
        {
            return PrecachedAudios.Keys.ToArray();
        }

        public static void PrecacheAudios(params string[] audios)
        {
            string cwd = Directory.GetCurrentDirectory();
            Console.WriteLine("CWD " + cwd);
            Console.WriteLine("Precaching " + audios.Length + " audios");
            PrecachedAudios = new Dictionary<string, IWaveSource>();
            foreach (string i in audios)
            {
                Console.WriteLine("Caching " + cwd + "\\" + i);
                IWaveSource waveSource = CodecFactory.Instance.GetCodec(cwd + "\\" + i);
                PrecachedAudios.Add(i, waveSource);
                Console.WriteLine("Done caching " + cwd + "\\" + i + ", size of " + waveSource.Length + "B");
            }
            Console.WriteLine("Done");
        }

        public static void PlayAudio(string name)
        {
            PlayAudio(name, 1.0f);
        }

        public static void PlayAudio(string name, float volume)
        {
            IWaveSource wave = PrecachedAudios[name];
            Wasapi.Initialize(wave);
            Wasapi.Volume = volume;
            Wasapi.Play();
            Wasapi.Stopped += SFX_DieEV;
        }

        private static void SFX_DieEV(object sender, PlaybackStoppedEventArgs e)
        {
            Wasapi.Dispose();
            Wasapi = new WasapiOut(true, CSCore.CoreAudioAPI.AudioClientShareMode.Shared, 100);
        }
    }
}
