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
        public static void Run()
        {
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
                        ByteStack stack = new ByteStack(64);
                        stack.Push((byte)0xff);
                        Console.WriteLine(stack.PopByte() + " should be " + 0xff);
                        stack.Push((ushort)0xffff);
                        ushort outputs = 0; stack.PopShort(ref outputs);
                        Console.WriteLine(outputs + " should be " + 0xffff);
                        stack.Push(0xffffffff);
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
