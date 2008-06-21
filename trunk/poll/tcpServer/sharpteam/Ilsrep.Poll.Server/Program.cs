using System;
using System.Collections.Generic;
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
        public const string WELCOME = "Welcome to poll server.";
        public const Int32 DATA_SIZE = 65536;

        private static int GetPollSessionId(string xmlStringId)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlStringId);
                XmlNodeList pollSessionId = xmlDoc.GetElementsByTagName("pollSessionId");
                return Convert.ToInt32(pollSessionId[0].InnerText);
            }
            catch (Exception)
            {
                return -1;
            }
        }

        private static string GetPollSessionById(int pollSessionId)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(PATH_TO_POLLS);
                XmlNodeList xmlPollSessionList = xmlDoc.GetElementsByTagName("pollsession");
                foreach (XmlNode xmlPollSession in xmlPollSessionList)
                {
                    bool isRightPollSession = (Convert.ToInt32(xmlPollSession.Attributes["id"].Value) == pollSessionId);
                    if (isRightPollSession)
                    {
                        return xmlPollSession.OuterXml;
                    }

                }
            }
            catch (Exception)
            {
                return "! An error occured in xml file reading";
            }
            return "! Wrong id";
        }

        public static void RunClientSession(NetworkStream client)
        {
            // Receive from client pollSessionId
            byte[] data = new byte[DATA_SIZE];
            int recvBytesCount = client.Read(data, 0, DATA_SIZE);
            string recvString = Encoding.ASCII.GetString(data, 0, recvBytesCount);
            int pollSessionId = GetPollSessionId(recvString);

            // Check if exist such id and send answer
            string sendString;
            bool correctId = (pollSessionId != -1);
            if (correctId)
            {
                // Send PollSession which id == pollSessionId
                sendString = GetPollSessionById(pollSessionId);
            }
            else
            {
                sendString = "! Wrond id";
            }
            data = Encoding.ASCII.GetBytes(sendString);
            client.Write(data, 0, sendString.Length);
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

