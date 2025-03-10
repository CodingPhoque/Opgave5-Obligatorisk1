using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace Opgave5Json
{
    class Program
    {
        static void Main(string[] args)
        {
            Program program = new Program();
            program.Start();
        }

        public void Start()
        {
            int port = 42010;
            TcpListener listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            Console.WriteLine("JSON-Server lytter på port " + port);

            // Opret én instans af JsonTCPServer
            JsonTCPServer server = new JsonTCPServer();

            while (true)
            {
                // Vent på klient og accepter
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("Klient forbundet.");

                // Håndter klienten i en ny tråd
                Thread t = new Thread(() => server.HandleClient(client));
                t.Start();
            }
        }
    }
}
