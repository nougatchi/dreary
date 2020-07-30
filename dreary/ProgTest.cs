using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using CSharpGL.Dreary;
using System.IO;
using System.Threading;

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
        public static void RunAutomated()
        {
            Console.Title = "Dreary Program test";
            Console.WriteLine("Stack trace\n" + Environment.StackTrace);
            Console.WriteLine("DREARY PROGTEST");
            
            try
            {
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
            }
            catch (Exception e)
            {
                Console.WriteLine("test failed: " + e);
                Environment.Exit(1);
            }

            Console.Write(Directory.GetCurrentDirectory() + "\\");
            string audiotoplay = "GameSounds/rwd.mp3";
            Console.WriteLine("Playing " + audiotoplay);
            try
            {
                AudioSystem.PlayAudio(audiotoplay);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error playing sound: " + e);
                Environment.Exit(2);
            }

            Console.WriteLine("Testing form");
            try
            {
                Form1 form = new Form1();
                form.Show();
                Console.WriteLine("Sleeping for 5s");
                Thread.Sleep(5000);
                form.Close();
                form.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Environment.Exit(3);
            }
            Console.WriteLine("Test done");

            Console.WriteLine("TEST DONE");
            Environment.Exit(0);
        }
        public static void Run()
        {
            Console.Title = "Dreary Program test";
            Console.WriteLine("Stack trace\n" + Environment.StackTrace);
            Console.WriteLine("DREARY PROGTEST");
            Console.WriteLine("Select mode");
            Console.WriteLine("1 test stack, 2 continue program, 3 net test, 4 audio test, 5 form test, 6 auto test, 0 exit dreary");
            
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
                        try
                        {
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
                        } catch(Exception e)
                        {
                            Console.WriteLine("test failed: " + e);
                        }
                        break;
                    case "2":
                        continueprog = true;
                        testing = false;
                        break;
                    case "3":
                        try
                        {
                            Console.WriteLine("Starting server");
                            Task serverformthread = new Task(() =>
                            {
                                try
                                {
                                    Form1 serverform = new Form1();
                                    serverform.Show();
                                    serverform.server = new Net.Server(21212, serverform);
                                    serverform.server.Start();
                                } catch(Exception e)
                                {
                                    Console.WriteLine("Server failed " + e);
                                }
                            });
                            Console.WriteLine("Starting client");
                            Task clientformthread = new Task(() => {
                                try
                                {
                                    Form1 clientform = new Form1();
                                    clientform.Show();
                                    clientform.client = new Net.Client(clientform);
                                    clientform.client.Connect("127.0.0.1", 21212);
                                } catch(Exception e)
                                {
                                    Console.WriteLine("Client failed " + e);
                                }
                            });
                            serverformthread.Start();
                            clientformthread.Start();
                        } catch(Exception e)
                        {
                            Console.WriteLine("test failed: " + e);
                        }
                        break;
                    case "4":
                        Console.Write(Directory.GetCurrentDirectory() + "\\");
                        string audiotoplay = Console.ReadLine();
                        Console.WriteLine("Playing " + audiotoplay);
                        try
                        {
                            AudioSystem.PlayAudio(audiotoplay);
                        } catch(Exception e)
                        {
                            Console.WriteLine("Error playing sound: " + e);
                        }
                        break;
                    case "5":
                        Console.WriteLine("Testing form");
                        try
                        {
                            Form1 form = new Form1();
                            form.Show();
                            Console.WriteLine("Sleeping for 5s");
                            Thread.Sleep(5000);
                            form.Close();
                            form.Dispose();
                        } catch(Exception e)
                        {
                            Console.WriteLine(e);
                        }
                        Console.WriteLine("Test done");
                        break;
                    case "6":
                        Console.WriteLine("Autotesting (will exit upon finish)");
                        try
                        {
                            RunAutomated();
                        } catch(Exception e)
                        {
                            Console.WriteLine(e);
                        }
                        break;
                    default:
                        Console.WriteLine("?");
                        break;
                }
            }
            Console.WriteLine("TEST DONE");
            if (!continueprog)
            {
                Environment.Exit(0);
            }
        }
    }
}
