using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml;

namespace Ilsrep.Poll.Server
{
    public class Program
    {
        private const int PORT = 3320;
        private const int maxPendingConnCount = 10;

        public static int GetId(string xmlStringId)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlStringId);
            XmlNodeList pollSessionId = xmlDoc.GetElementsByTagName("pollSessionId");
            return Convert.ToInt32(pollSessionId[0].InnerText);
        }

        public static void Main()
        {
            byte[] data = new byte[1024];
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

            int c = client.Receive(data);
            string receicedData = Encoding.ASCII.GetString(data);
            Console.WriteLine(c + "\t" + receicedData);
            //int id = GetId(receicedData);
            //Console.WriteLine("Received id: " + id);

            Console.ReadKey(true);
            Console.WriteLine("Disconnected from {0}", clientInfo.Address);
            client.Close();
            Console.ReadKey(true);
        }
    }
}