using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace Ilsrep.PollApplication.Model
{
    class TcpCommunicator
    {
        Socket m_client;

        public bool isConnected
        {
            get { return m_client.Connected; }
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

        /*
        private int SendData(string sData)
        {
            Byte[] data = new Byte[1024];
            data = Encoding.ASCII.GetBytes(sData);

            int total = 0;
            int size = data.Length;
            int dataleft = size;
            int sent;

            while (total < size)
            {
                sent = m_client.Send(data, total, dataleft, SocketFlags.None);
                total += sent;
                dataleft -= sent;
            }

            return total;
        }
        */
         
        private string ReceiveData(int size)
        {
            int total = 0;
            int dataleft = size;
            byte[] data = new byte[size];
            int recv;

            while (total < size)
            {
                recv = m_client.Receive(data, total, dataleft, 0);
                if (recv == 0)
                    break;
                
                total += recv;
                dataleft -= recv;
            }

            String sData = Encoding.ASCII.GetString(data);

            return sData;
        }


        public bool sendId(string id)
        {
            id = "<pollSessionId>" + id + "</pollSessionId>";
            int sentBytes = m_client.Send(Encoding.ASCII.GetBytes(id));
            return (sentBytes !=0);
            //string result = ReceiveData(1);

            //if ( result == "1" )
            //    return true;
            //else
            //    return false;
        }

        public string getXML()
        {
            int countReceived;
            String xmlData = String.Empty;

            if ( ! isConnected )
                return String.Empty;

            while (true)
            {
                byte[] receivedData = new byte[1024];
                countReceived = m_client.Receive(receivedData);

                if (countReceived == 0)
                    break;

                xmlData += Encoding.ASCII.GetString(receivedData, 0, countReceived);
            }

            return xmlData;
        }
    }
}