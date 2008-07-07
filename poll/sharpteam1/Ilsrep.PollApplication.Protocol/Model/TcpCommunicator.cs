using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace Ilsrep.PollApplication.Model
{
    public class TcpCommunicator
    {
        Socket m_client;
        private const int DATA_SIZE = 65536;

        public bool isConnected
        {
            get 
            { 
                return m_client.Connected; 
            }
        }

        public TcpCommunicator()
        {
            m_client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Connect(string address, int port)
        {
            m_client.Connect(address, port);
        }

        public void Disconnect()
        {
            m_client.Disconnect(false);
        }

        public void Send(string data)
        {
            m_client.Send(Encoding.ASCII.GetBytes(data));
        }

        public bool sendID(string id)
        {
            int sentBytes = m_client.Send(Encoding.ASCII.GetBytes(id));

            Byte[] buffer = new Byte[1024];
            int countReceivedData = m_client.Receive(buffer);
            string result = Encoding.ASCII.GetString(buffer, 0, countReceivedData);

            if ( result == "1" )
                return true;
            else
                return false;
        }

        public string ReceiveXMLData()
        {
            int countReceivedBytes;
            String xmlData = String.Empty;

            byte[] receivedData = new byte[DATA_SIZE];
            countReceivedBytes = m_client.Receive(receivedData);
            xmlData = Encoding.ASCII.GetString(receivedData, 0, countReceivedBytes);

            return xmlData;
        }
    }
}