using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Xml;
using System.IO;
using Ilsrep.PollApplication.Server;

namespace Ilsrep.PollApplication.Model
{
    class ConnectionThread
    {
        public TcpListener threadListener;
        private static int activeConnCount = 0;
        private static byte[] data = new byte[PollServer.DATA_SIZE];

        public void HandleConnection()
        {
            // Establish connection with new client
            TcpClient currentClient = threadListener.AcceptTcpClient();
            NetworkStream currentStream = currentClient.GetStream();
            activeConnCount++;
            string clientAdderss = currentClient.Client.RemoteEndPoint.ToString();
            Console.WriteLine("New client accepted: {0} ({1} active connections)", clientAdderss, activeConnCount);
            data = Encoding.ASCII.GetBytes(PollServer.WELCOME);
            currentStream.Write(data, 0, data.Length);

            // Run dialog with client
            PollServer.RunClientSession(currentStream);

            // Close connection with client
            currentStream.Close();
            currentClient.Close();
            activeConnCount--;
            Console.WriteLine("Client disconnected: {0} ({1} active connections left)", clientAdderss, activeConnCount);
        }
    }
}