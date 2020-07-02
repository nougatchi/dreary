using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpGL;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;

namespace dreary.Net
{
    public class Server
    {
        Form1 form;
        TcpListener listen;

        public Server(int port, Form1 form)
        {
            listen = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
            this.form = form;
        }

        public void Start()
        {
            bool running = true;
            listen.Start();
            while(running)
            {
                TcpClient cli = listen.AcceptTcpClient();
                NetworkStream stream = cli.GetStream();
                BinaryFormatter fmt = new BinaryFormatter();
                fmt.Serialize(stream, form.scene);
                stream.Close();
                cli.Dispose();
            }
        }
    }
}
