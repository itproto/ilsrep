using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace Ilsrep.Common.TcpServer
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

        public string Receive()
        {
            Byte[] receivedData = new Byte[1024];

            m_client.Receive(receivedData);

            return Encoding.ASCII.GetString(receivedData);
        }
    }
}
