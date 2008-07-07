using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace Ilsrep.Common
{
    public class TcpServer
    {
        Socket m_client;

        public bool isConnected
        {
            get 
            { 
                return m_client.Connected; 
            }
        }

        public TcpServer()
        {
            m_client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Connect(string address, int port)
        {
            try
            {
                m_client.Connect(address, port);
            }
            catch (SocketException)
            {
                Console.WriteLine("Couldn't connect to {0}:{1} - timed out", address, port);
                Environment.Exit(-1);
            }
        }

        public void Disconnect()
        {
            m_client.Disconnect(false);
        }

        public void Send(string data)
        {
            m_client.Send(Encoding.ASCII.GetBytes(data));
        }

        public string Receive()
        {
            Byte[] receivedData = new Byte[1024];

            m_client.Receive(receivedData);

            return Encoding.ASCII.GetString(receivedData);
        }
    }
}
