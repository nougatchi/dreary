using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
namespace dreary.Basplash
{
    public class Basplash
    {
        private int lnNum;
        private int lCall;
        List<BasplashHLObject> classes;
        BasplashObj obj;
        public Basplash()
        {
            classes = new List<BasplashHLObject>();
            classes.Add(new BasplashHLObject("game", new BasplashGame()));
            obj = new BasplashObj();
            classes.Add(new BasplashHLObject("object", obj));
        }
        public void Execute(string code)
        {
            string[] functions = code.Split(';'); // this is the EOF (end of function) character
            foreach(string function in functions)
            {
                if(string.IsNullOrWhiteSpace(function))
                {
                    continue;
                }
                // the first word should ALWAYS be a function
                string[] functype = functions[0].Split('.'); // this is the delim for children
                                                             // ex. game.print a b c
                BasplashHLObject cobject = null; // this is the current object it will be affecting
                string call = null;
                foreach (string i in functype)
                {
                    if(i != functype[functype.Length-1])
                    {
                        foreach (BasplashHLObject obj in classes)
                        {
                            if (i == obj.name)
                            {
                                cobject = obj;
                            }
                        }
                    } else
                    {
                        call = i.Split('|')[0];
                        break;
                    }
                }
                if (cobject == null)
                {
                    BasplashError("F0001: Attempt to access null var");
                }
                Collection<string> args = new Collection<string>(functype[functype.Length - 1].Split('|'));
                object[] vargs = new string[args.Count - 1];
                int vptr = 0;
                foreach (string value in args.Skip(1))
                {
                    if(value[0] == 'v')
                    {
                        obj.GetVal(value.Substring(1));
                        continue;
                    }
                    vargs[vptr] = value;
                    vptr++;
                }
                Type objtype = cobject.val.GetType();
                MethodInfo method = objtype.GetMethod(call);
                method.Invoke(cobject.val, vargs);
            }
        }
        private void BasplashError(string reason)
        {
            Console.WriteLine(reason);
            throw new BasplashException(reason);
        }
    }
    public class BasplashException : Exception
    {
        public BasplashException(string message) : base(message)
        {

        }
        public BasplashException() : base()
        {

        }
    }
    public class BasplashHLObject
    {
        public object val;
        public string name;
        public BasplashHLObject(string name, object val)
        {
            this.val = val;
            this.name = name;
        }
    }
}
