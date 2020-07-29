using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSCore.Codecs.MP3;
using CSCore.Streams;
using CSCore.Codecs;
using CSCore;
using CSCore.Utils;
using CSCore.SoundOut;
using CSCore.XAudio2;
using CSharpGL;
using CSCore.XAudio2.X3DAudio;
using System.IO;

namespace dreary
{
    public static class AudioSystem
    {
        //private static WasapiOut Wasapi;
        private static XAudio2 XAudio2System;
        private static XAudio2MasteringVoice XMasteringVoice;
        private static StreamingSourceVoice XSourceVoice;
        private static X3DAudioCore X3DSystemCore;
        private static Dictionary<string, IWaveSource> PrecachedAudios;
        private static string[] AudiosPrecached;
        private static int _sourceChannels, _destinationChannels;
        private static List<string> AudiosToBePrecached;

        public static void Initialize()
        {
            //Wasapi = new WasapiOut(true, CSCore.CoreAudioAPI.AudioClientShareMode.Shared, 100);
            XAudio2System = XAudio2.CreateXAudio2();

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
            PlayAudio(name, volume, new Vector3(0), new Listener());
        }

        public static void PlayAudio(string name, float volume, Vector3 origin, Listener listener)
        {
            IWaveSource wave = PrecachedAudios[name];
            IWaveSource mwave = wave.ToMono();
            Console.WriteLine($"PLAYING A {mwave.WaveFormat.WaveFormatTag}, sR: {mwave.WaveFormat.SampleRate}Hz bPS: {mwave.WaveFormat.BitsPerSample} byPS: {mwave.WaveFormat.BytesPerSecond}");
            XMasteringVoice = XAudio2System.CreateMasteringVoice(XAudio2.DefaultChannels, XAudio2.DefaultSampleRate);
            XSourceVoice = new StreamingSourceVoice(XAudio2System, mwave);
            object defaultDevice = XAudio2System.DefaultDevice;
            ChannelMask mask;
            if (XAudio2System.Version == XAudio2Version.XAudio2_7)
            {
                var xaudio27 = (XAudio2_7)XAudio2System;
                var deviceDetails = xaudio27.GetDeviceDetails((int)defaultDevice);
                mask = deviceDetails.OutputFormat.ChannelMask;
                _destinationChannels = deviceDetails.OutputFormat.Channels;
            }
            else
            {
                mask = XMasteringVoice.ChannelMask;
                _destinationChannels = XMasteringVoice.VoiceDetails.InputChannels;
            }

        }

        //private static void SFX_DieEV(object sender, PlaybackStoppedEventArgs e)
        //{
        //    Wasapi.Dispose();
        //    Wasapi = new WasapiOut(true, CSCore.CoreAudioAPI.AudioClientShareMode.Shared, 100);
        //    string cwd = Directory.GetCurrentDirectory();
        //    List<int> pcR = new List<int>();
        //    foreach (string i in AudiosToBePrecached)
        //    {
        //        Console.WriteLine("Recaching " + i);
        //        PrecachedAudios[i].Dispose();
        //        PrecachedAudios[i] = CodecFactory.Instance.GetCodec(cwd + "\\" + i);
        //        Console.WriteLine("Recached " + i);
        //        pcR.Add(AudiosToBePrecached.IndexOf(i));
        //    }
        //    foreach(int i in pcR)
        //    {
        //        AudiosToBePrecached.RemoveAt(i);
        //    }
        //}
    }
}
