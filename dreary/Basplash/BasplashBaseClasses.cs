using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dreary.Basplash
{
    public class BasplashGame {
        public void Print(string text)
        {
            Console.WriteLine(text);
        }
        public void Test()
        {
            Console.WriteLine("Test finished");
        }
    }
    public class BasplashObj
    {
        Dictionary<string, int> integers;
        public BasplashObj()
        {
            integers = new Dictionary<string, int>();
        }
        public void RvspInt(string name, string initvar)
        {
            foreach(KeyValuePair<string,int> i in integers)
            {
                if(i.Key == name)
                {
                    return;
                }
            }
            integers.Add(name, int.Parse(initvar));
        }
        public int GetInt(string name)
        {
            foreach(KeyValuePair<string,int> i in integers)
            {
                if(name == i.Key)
                {
                    return i.Value;
                }
            }
            return int.MaxValue;
        }
        public object GetVal(string name)
        {
            int gi = GetInt(name);
            if (gi != int.MaxValue)
            {
                return gi;
            }
            return null;
        }
    }
}
