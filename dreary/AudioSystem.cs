using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpGL;
using System.IO;
using System.Windows.Media;

namespace dreary
{
    public static class AudioSystem
    {
        public static void PlayAudio(string name)
        {
            MediaPlayer player = new MediaPlayer();
            player.Open(new Uri(Directory.GetCurrentDirectory() + "\\" + name));
            player.Play();
        }

        public static void PlayAudio(string name, float volume)
        {
            PlayAudio(name);
        }
    }
}