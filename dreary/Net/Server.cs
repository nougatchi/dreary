using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpGL;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace dreary.Net
{
    public class Server
    {
        Form1 form;
        TcpListener listen;

        public Server(int port)
        {
            listen = new TcpListener(port);
        }

        public void Start()
        {
            listen.Start();
            while(true)
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
