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
        public TcpListener threadListener;
        private string clientAddress;
        private static int activeConnCount = 0;
        private static byte[] data = new byte[PollServer.DATA_SIZE];

        /*
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
        */

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
                //return "! An error occured in xml file reading";
                return String.Empty;
            }

            //return "! Wrong ";
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

                // get poll sesssion id sent by server
                //pollSessionID = GetPollSessionId(recievedString);
                try
                {
                    pollSessionID = Convert.ToInt32(recievedString);
                    // Check if exists such ID
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
            TcpClient currentClient = threadListener.AcceptTcpClient();
            NetworkStream currentStream = currentClient.GetStream();
            activeConnCount++;
            clientAddress = currentClient.Client.RemoteEndPoint.ToString();
            Console.WriteLine("New client accepted - {0} ({1} active connections)", clientAddress, activeConnCount);
            
            /*
            // Send WELCOME
            data = Encoding.ASCII.GetBytes(PollServer.WELCOME);
            currentStream.Write(data, 0, data.Length);
            */
             
            // Run dialog with client
            RunClientSession(currentStream);

            // Close connection with client
            currentStream.Close();
            currentClient.Close();
            activeConnCount--;
            Console.WriteLine("Client disconnected: {0} ({1} active connections left)", clientAddress, activeConnCount);
        }
    }
}