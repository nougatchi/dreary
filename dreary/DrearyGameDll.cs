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
        /// <summary>
        /// Creates the DrearyGameDll class, which allows running functions from game.dll files
        /// </summary>
        /// <param name="assembly">The name of the dll</param>
        public DrearyGameDll(string assembly)
        {
            asm = Assembly.LoadFrom(Application.StartupPath + "\\" + assembly);
            instances = new Dictionary<string, object>();
        }

        /// <summary>
        /// Gets a created class from the class list
        /// </summary>
        /// <param name="name">The class name</param>
        /// <returns>The class</returns>
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

        /// <summary>
        /// Creates a new class and returns it
        /// </summary>
        /// <param name="className">The name of the class</param>
        /// <returns>The class</returns>
        public object DrearyCreateInstance(string className)
        {
            return asm.CreateInstance(className, true);
        }

        /// <summary>
        /// Creates a new class and adds it to the internal class list
        /// </summary>
        /// <param name="className">The name of the class</param>
        /// <param name="dictName">The name as it will be stored in the list</param>
        /// <returns>The class</returns>
        public object DrearyCreateInstance(string className, string dictName)
        {
            object newinstance = asm.CreateInstance(className, true);
            instances.Add(dictName, newinstance);
            return newinstance;
        }

        /// <summary>
        /// Calls a function from a created obj specified in obj.
        /// </summary>
        /// <param name="obj">The object</param>
        /// <param name="function">The function name</param>
        /// <returns>The returned object (can be null)</returns>
        public object DrearyCall(object obj, string function)
        {
            return DrearyCall(obj, function, null);
        }

        /// <summary>
        /// Calls a function from a created obj specified in objname.
        /// </summary>
        /// <param name="objname">The name of the object</param>
        /// <param name="function">The function name</param>
        /// <returns>The returned object (can be null)</returns>
        public object DrearyCall(string objname, string function)
        {
            return DrearyCall(objname, function, null);
        }

        /// <summary>
        /// Calls a function from a created obj specified in obj with parameters.
        /// </summary>
        /// <param name="obj">The object</param>
        /// <param name="function">The function name</param>
        /// <param name="parameters">The parameters</param>
        /// <returns>The returned object (can be null)</returns>
        public object DrearyCall(object obj, string function, params object[] parameters)
        {
            return (string)obj.GetType().GetMethod(function).Invoke(this, parameters);
        }

        /// <summary>
        /// Calls a function from a created obj specified in objname with parameters.
        /// </summary>
        /// <param name="objname">The name of the object</param>
        /// <param name="function">The function name</param>
        /// <param name="parameters">The parameters</param>
        /// <returns>The returned object (can be null)</returns>
        public object DrearyCall(string objname, string function, params object[] parameters)
        {
            object obj = GetCreatedInstance(objname);
            return obj.GetType().GetMethod(function).Invoke(this, parameters);
        }
    }
}
