using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dreary.Script
{
    public static class DrearyEForms
    {
        public static string Title;
        public static double GetNumber(string prompt)
        {
            TextInputForm tif = new TextInputForm(Title + " " + prompt + " (number)", "OK");
            tif.ShowDialog();
            return double.Parse(tif.output);
        }
        public static string GetString(string prompt)
        {
            TextInputForm tif = new TextInputForm(Title + " " + prompt + " (string)", "OK");
            tif.ShowDialog();
            return tif.output;
        }
        public static void ShowNotice(string prompt, string notice)
        {
            NoticeForm tif = new NoticeForm(Title + " " + prompt + " (string)", notice);
            tif.Show();
        }
        public static float GetNumberF(string prompt)
        {
            TextInputForm tif = new TextInputForm(Title + " " + prompt + " (float)", "OK");
            tif.ShowDialog();
            return float.Parse(tif.output);
        }
        public static int GetNumberI(string prompt)
        {
            TextInputForm tif = new TextInputForm(Title + " " + prompt + " (integer)", "OK");
            tif.ShowDialog();
            return int.Parse(tif.output);
        }
    }
}
