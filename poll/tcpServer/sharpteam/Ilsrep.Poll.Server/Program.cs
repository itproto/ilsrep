using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Xml;
using System.IO;
using Ilsrep.PollApplication.Model;

namespace Ilsrep.PollApplication.Server
{
    public class PollServer
    {
        private const int PORT = 3320;
        private const string PATH_TO_POLLS = "Polls.xml";
        private static byte[] data = new byte[PollServer.DATA_SIZE];
        public const string WELCOME = "Welcome to poll server.";
        public const Int32 DATA_SIZE = 65536;
        

        /*
        private static int GetId(string xmlStringId)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlStringId);
            XmlNodeList pollSessionId = xmlDoc.GetElementsByTagName("pollSessionId");
            return Convert.ToInt32(pollSessionId[0].InnerText);
        }
        */

        private static string ReadFileToString(string fileName)
        {
            try
            {
                StreamReader streamReader = new StreamReader(fileName);
                String fileData = streamReader.ReadToEnd();
                streamReader.Close();
                return fileData;
            }
            catch (Exception error)
            {
                return "An error occured: " + error;
            }
        }

        public static void RunClientSession(NetworkStream client)
        {
            string xmlData = ReadFileToString(PATH_TO_POLLS);
            data = Encoding.ASCII.GetBytes(xmlData);
            client.Write(data, 0, data.Length);
        }

        public static void Main()
        {
            // Get local ip
            string localHost = System.Net.Dns.GetHostName();
            string localIp = System.Net.Dns.GetHostByName(localHost).AddressList[0].ToString();

            // Start server
            Console.WriteLine("Server started on host: {0}:{1}", localIp, PORT);
            IPEndPoint clientAddress = new IPEndPoint(IPAddress.Any, PORT);
            TcpListener tcpListener = new TcpListener(3320);
            tcpListener.Start();
            Console.WriteLine("Waiting for clients...");
            while (true)
            {
                // Wait until client connect
                while (!tcpListener.Pending())
                {
                    Thread.Sleep(1000);
                }

                // Create a new thread for each client
                ConnectionThread newConnection = new ConnectionThread();
                newConnection.threadListener = tcpListener;
                Thread newThread = new Thread(new ThreadStart(newConnection.HandleConnection));
                newThread.Start();
            }
        }
    }
}

