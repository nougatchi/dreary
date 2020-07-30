using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpGL.Dreary;

namespace dreary
{
    public static class ProgTest
    {
        private static void PrintByteArray(byte[] bytes)
        {
            var sb = new StringBuilder("byte["+bytes.Length+"] { ");
            foreach (var b in bytes)
            {
                sb.Append(b + ", ");
            }
            sb.Append("}");
            Console.WriteLine(sb.ToString());
            Console.WriteLine("ASCII: " + Encoding.ASCII.GetString(bytes));
        }
        private static string _ByteArrayToString(byte[] array, bool hidezeros)
        {
            string bstr = array.Length + " - ";
            if(hidezeros)
            {
                foreach(byte i in array)
                {
                    if(i != 0)
                    {
                        bstr += i.ToString("x") + " ";
                    }
                }
            } else
            {
                foreach(byte i in array)
                {
                    bstr += i.ToString("x") + " ";
                }
            }
            return bstr;
        }
        public static void Run()
        {
            Console.Title = "Dreary Program test";
            Console.WriteLine("Stack trace\n" + Environment.StackTrace);
            Console.WriteLine("DREARY PROGTEST");
            Console.WriteLine("Select mode");
            Console.WriteLine("1 test stack, 2 continue with program, 0 exit");

            bool continueprog = false;
            bool testing = true;
            while(testing)
            {
                Console.Write('.');
                switch(Console.ReadLine())
                {
                    case "0":
                        testing = false;
                        break;
                    case "1":
                        ByteStack stack = new ByteStack(16);
                        stack.Push((byte)0xff);
                        PrintByteArray(stack.DumpStack());
                        Console.WriteLine(stack.PopByte() + " should be " + 0xff);
                        stack.Push((ushort)0xffff);
                        PrintByteArray(stack.DumpStack());
                        ushort outputs = 0; stack.PopShort(ref outputs);
                        Console.WriteLine(outputs + " should be " + 0xffff);
                        stack.Push(0xffffffff);
                        PrintByteArray(stack.DumpStack());
                        int outputi = 0; stack.PopInt(ref outputi);
                        Console.WriteLine(outputi + " should be " + 0xffffffff);
                        break;
                    case "2":
                        continueprog = true;
                        testing = false;
                        break;
                    default:
                        Console.WriteLine("?");
                        break;
                }
            }
            if(!continueprog)
            {
                Environment.Exit(0);
            }
        }
    }
}
