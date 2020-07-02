using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using CSharpGL;
using System.Threading;

namespace dreary.Net
{
    public class Client
    {
        TcpClient cli;
        Form1 form;
        public Client(Form1 form)
        {
            form.ConnectModeEnable();
            form.StatusmessageEnabled = true;
            form.Statusmessage = "Please wait.";
            cli = new TcpClient();
        }
        public void Connect(string ip, int port)
        {
            try
            {
                form.Statusmessage = $"Connecting to {ip}:{port}...";
                cli.Connect(ip, port);
                form.Statusmessage = $"Connected. Waiting for instance...";
                while (cli.Available == 0)
                {

                }
                NetworkStream stream = cli.GetStream();
                BinaryFormatter fmt = new BinaryFormatter();
                object rcv = fmt.Deserialize(stream);
                try
                {
                    form.Statusmessage = $"OK";
                    form.scene.RootNode = (GroupNode)rcv;
                    form.Statusmessage = $"Disconnecting...";
                    stream.Close();
                    cli.Dispose();
                }
                catch (Exception e)
                {
                    form.Statusmessage = $"Failure receiving instance. " + e.Message;
                }
            } catch(Exception e)
            {
                form.Statusmessage = $"Failure in connection. " + e.Message;
            }
            form.Statusmessage = $"Done.";
            Thread.Sleep(5000);
            form.Reenter();
        }
    }
}
