using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml;
using System.IO;

namespace Ilsrep.Poll.Server
{
    public class Program
    {
        private const int PORT = 3320;
        private const Int32 DATA_SIZE = 65536;
        private const int MAX_PENDING_CONN_COUNT = 10;
        private const string PATH_TO_POLLS = "Polls.xml";

        /*
        public static int GetId(string xmlStringId)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlStringId);
            XmlNodeList pollSessionId = xmlDoc.GetElementsByTagName("pollSessionId");
            return Convert.ToInt32(pollSessionId[0].InnerText);
        }
        */

        public static string ReadFile(string fileName)
        {
            StreamReader streamReader = new StreamReader(fileName);
            String fileData = streamReader.ReadToEnd();
            streamReader.Close();
            return fileData;
        }

        public static void Main()
        {
            byte[] data = new byte[DATA_SIZE];
            IPEndPoint clientAddress = new IPEndPoint(IPAddress.Any, PORT);
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Connect a client
            client.Bind(clientAddress);
            client.Listen(MAX_PENDING_CONN_COUNT);
            client = client.Accept();

            // Write client info
            IPEndPoint clientInfo = (IPEndPoint)client.RemoteEndPoint;
            Console.WriteLine("New client {0}", clientInfo.Address);

            // Send XmlFile
            String fileData = ReadFile(PATH_TO_POLLS);
            data = Encoding.ASCII.GetBytes(fileData);
            int countSentBytes = client.Send(data);
            Console.WriteLine("Sent to {0} {1} bytes", clientInfo.Address, countSentBytes);

            // Close connection and exit
            client.Close();
            Console.WriteLine("Disconnected from {0}", clientInfo.Address);
            Console.ReadKey(true);
        }
    }
}