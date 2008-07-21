﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace Ilsrep.Common
{
    public class TcpServer
    {
        public const int DATA_SIZE = 65536;
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
            countReceived = m_client.Receive(data);
            return Encoding.ASCII.GetString(data,0,countReceived);
        }

        public String Receive(int countReceive)
        {
            Byte[] receivedData = new Byte[countReceive];
            int countReceived = m_client.Receive(receivedData);
            return Encoding.ASCII.GetString(receivedData, 0, countReceived);
        }
    }
}