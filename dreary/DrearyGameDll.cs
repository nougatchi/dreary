using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace dreary
{
    public class DrearyGameDll
    {
        Assembly asm;
        public Dictionary<string, object> instances;
        public DrearyGameDll(string assembly)
        {
            Assembly asm = Assembly.LoadFrom(Application.StartupPath + "\\" + assembly);
            instances = new Dictionary<string, object>();
        }

        public object GetCreatedInstance(string name)
        {
            foreach(KeyValuePair<string,object> i in instances)
            {
                if(i.Key == name)
                {
                    return i.Value;
                }
            }
            return null;
        }

        public object DrearyCreateInstance(string className)
        {
            return asm.CreateInstance(className, true);
        }

        public object DrearyCreateInstance(string className, string dictName)
        {
            object newinstance = asm.CreateInstance(className, true);
            instances.Add(dictName, newinstance);
            return newinstance;
        }

        public object DrearyCall(object obj, string function, params object[] parameters)
        {
            return (string)obj.GetType().GetMethod(function).Invoke(this, parameters);
        }

        public object DrearyCall(string objname, string function, params object[] parameters)
        {
            object obj = GetCreatedInstance(objname);
            return (string)obj.GetType().GetMethod(function).Invoke(this, parameters);
        }
    }
}
