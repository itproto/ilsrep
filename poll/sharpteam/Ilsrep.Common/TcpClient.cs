using System;
using System.Text;
using System.Net.Sockets;

namespace Ilsrep.Common
{
    /// <summary>
    /// Tcp Client that is used to connect to server
    /// </summary>
    public class TcpClient
    {
        /// <summary>
        /// Max size of the data sent
        /// </summary>
        public const int DATA_SIZE = 64 * 1024;
        /// <summary>
        /// Socket that will do all transfers
        /// </summary>
        Socket m_client;

        /// <summary>
        /// checks if client is connected
        /// </summary>
        public bool isConnected
        {
            get 
            { 
                return m_client.Connected; 
            }
        }

        /// <summary>
        /// Contructor
        /// </summary>
        public TcpClient()
        {
            m_client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        /// <summary>
        /// Connects to server using address and port
        /// </summary>
        /// <param name="address">address of server</param>
        /// <param name="port">port to which to connect</param>
        public void Connect(string address, int port)
        {
            try
            {
                m_client.Connect(address, port);
            }
            catch (SocketException)
            {
                throw new Exception("Error: Couldn't connect to " + address + ":" + port);
            }
        }

        /// <summary>
        /// Disconnects from server
        /// </summary>
        public void Disconnect()
        {
            m_client.Disconnect(true);
        }

        /// <summary>
        /// Sends data to server
        /// </summary>
        /// <param name="data">data that is to be sent</param>
        public void Send(string data)
        {
            m_client.Send(Encoding.ASCII.GetBytes(data));
        }
        
        /// <summary>
        /// Receives data from server
        /// </summary>
        /// <returns>data that was received</returns>
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
