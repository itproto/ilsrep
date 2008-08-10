using System;
using System.Text;
using System.Net.Sockets;

namespace Ilsrep.Common
{
    public class TcpServer
    {
        public const int DATA_SIZE = 64 * 1024;
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
                throw new Exception("Couldn't connect to " + address + ":" + port + " - timed out");
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
        
        public String Receive()
        {
            byte[] data = new byte[DATA_SIZE];
            int countReceived = 0;
            try
            {
                countReceived = m_client.Receive(data);
            }
            catch (Exception)
            {
                return String.Empty;
            }
            return Encoding.ASCII.GetString(data,0,countReceived);
        }
    }
}
