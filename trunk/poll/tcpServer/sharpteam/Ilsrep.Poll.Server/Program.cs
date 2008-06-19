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
        private const int maxPendingConnCount = 10;
        private const string POLL_FILE = "Polls.xml";

        public static int GetId(string xmlStringId)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlStringId);
            XmlNodeList pollSessionId = xmlDoc.GetElementsByTagName("pollSessionId");
            return Convert.ToInt32(pollSessionId[0].InnerText);
        }

        public static string readFile(string filename)
        {
            StreamReader sr = new StreamReader(filename);
            String fileData = sr.ReadToEnd();
            sr.Close();

            return fileData;
        }

        private static int SendData(Socket s, byte[] data)
        {
            int total = 0;
            int size = data.Length;
            int dataleft = size;
            int sent;

            while (total < size)
            {
                sent = s.Send(data, total, dataleft, SocketFlags.None);
                total += sent;
                dataleft -= sent;
            }
            return total;
        }

        public static void Main()
        {
            Byte[] data = new Byte[1024];
            IPEndPoint clientAddress = new IPEndPoint(IPAddress.Any, PORT);

            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            client.Bind(clientAddress);
            client.Listen(maxPendingConnCount);
            client = client.Accept();

            IPEndPoint clientInfo = (IPEndPoint)client.RemoteEndPoint;
            Console.WriteLine("New client {0}", clientInfo.Address);

            // ------------------------------------------------------------ //
            //data = Encoding.ASCII.GetBytes("Welcome to Ilsrep.Poll.Server");
            //client.Send(data, data.Length, SocketFlags.None);
            // ------------------------------------------------------------ //

            //int c = client.Receive(data);
            //string receicedData = Encoding.ASCII.GetString(data);
            //Console.WriteLine(c + "\t" + receicedData);
            //int id = GetId(receicedData);
            //Console.WriteLine("Received id: " + id);

            //Console.ReadKey(true);
            String fileData = readFile(POLL_FILE);
            data = Encoding.ASCII.GetBytes(fileData);

            int countSent = SendData(client, data);
            Console.WriteLine("Sent to {0} {1} bytes", clientInfo.Address, countSent);

            client.Close();
            Console.WriteLine("Disconnected from {0}", clientInfo.Address);
            Console.ReadKey(true);

        }
    }
}