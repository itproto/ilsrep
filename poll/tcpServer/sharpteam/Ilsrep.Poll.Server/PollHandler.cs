using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Xml;
using System.IO;

namespace Ilsrep.PollApplication.Server
{
    class PollHandler
    {
        public TcpClient currentClient = new TcpClient();
        private string clientAddress;
        private static int activeConnCount = 0;
        private static byte[] data = new byte[PollServer.DATA_SIZE];

        private static bool idExists(int pollSessionID)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(PollServer.PATH_TO_POLLS);
                XmlNodeList xmlPollSessionList = xmlDoc.GetElementsByTagName("pollsession");

                foreach (XmlNode xmlPollSession in xmlPollSessionList)
                {
                    bool isRightPollSession = (Convert.ToInt32(xmlPollSession.Attributes["id"].Value) == pollSessionID);
                    if (isRightPollSession)
                    {
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }

        private static string GetPollSessionById(int pollSessionID)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(PollServer.PATH_TO_POLLS);
                XmlNodeList xmlPollSessionList = xmlDoc.GetElementsByTagName("pollsession");
                foreach (XmlNode xmlPollSession in xmlPollSessionList)
                {
                    bool isRightPollSession = (Convert.ToInt32(xmlPollSession.Attributes["id"].Value) == pollSessionID);
                    if (isRightPollSession)
                    {
                        return xmlPollSession.OuterXml;
                    }
                }
            }
            catch (Exception)
            {
                return String.Empty;
            }

            return String.Empty;
        }

        public void RunClientSession(NetworkStream client)
        {
            string sendString = String.Empty;
            int pollSessionID = 0;
            byte[] data = new byte[PollServer.DATA_SIZE];

            while (true)
            {
                // Receive from client pollSessionId
                int receivedBytesCount = client.Read(data, 0, PollServer.DATA_SIZE);
                string recievedString = Encoding.ASCII.GetString(data, 0, receivedBytesCount);

                // Get poll sesssion id sent by server
                try
                {
                    pollSessionID = Convert.ToInt32(recievedString);

                    // Check if exists such id
                    if (idExists(pollSessionID))
                    {
                        Console.WriteLine("{0}: ID validated: {1}", clientAddress, pollSessionID);
                        sendString = "1";
                        client.Write(Encoding.ASCII.GetBytes(sendString), 0, sendString.Length);
                        break;
                    }
                    else
                    {
                        Console.WriteLine("{0}: Client asked for non-existant ID: {1}", clientAddress, pollSessionID);
                        sendString = "-1";
                        client.Write(Encoding.ASCII.GetBytes(sendString), 0, sendString.Length);
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("{0}: Client asked for non-existant ID: {1}", clientAddress, recievedString);
                    sendString = "-1";
                    client.Write(Encoding.ASCII.GetBytes(sendString), 0, sendString.Length);
                }
            }

            // Send PollSession
            sendString = GetPollSessionById(pollSessionID);

            if (sendString == String.Empty)
            {
                Console.WriteLine("Unrecovable error: couldn't get poll by id");
                return;
            }

            data = Encoding.ASCII.GetBytes(sendString);
            client.Write(data, 0, sendString.Length);
        }

        public void HandleConnection()
        {
            // Establish connection with new client
            NetworkStream currentStream = currentClient.GetStream();
            activeConnCount++;
            clientAddress = currentClient.Client.RemoteEndPoint.ToString();
            Console.WriteLine("New client accepted - {0} ({1} active connections)", clientAddress, activeConnCount);
                         
            // Run dialog with client
            RunClientSession(currentStream);

            // Close clients connection
            currentStream.Close();
            currentClient.Close();
            activeConnCount--;
            Console.WriteLine("Client disconnected: {0} ({1} active connections left)", clientAddress, activeConnCount);
        }
    }
}