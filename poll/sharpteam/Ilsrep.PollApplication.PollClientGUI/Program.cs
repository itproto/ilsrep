using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Ilsrep.PollApplication.Model;
using Ilsrep.PollApplication.Communication;
using Ilsrep.PollApplication.Helpers;
using Ilsrep.Common;

namespace Ilsrep.PollApplication.PollClientGUI
{
    static class PollClientGUI
    {
        /// <summary>
        /// String that states yes
        /// </summary>
        public const string CONSOLE_YES = "y";
        /// <summary>
        /// String that states no
        /// </summary>
        public const string CONSOLE_NO = "n";
        /// <summary>
        /// holds current user's username
        /// </summary>
        public static string userName = "";
        /// <summary>
        /// handles connection to server
        /// </summary>
        public static TcpClient client = new TcpClient();
        /// <summary>
        /// holds pollsession that is read from server
        /// </summary>
        public static PollSession pollSession = new PollSession();
        /// <summary>
        /// holds user choices that he selects during poll session
        /// </summary>
        public static List<Choice> userChoices = new List<Choice>();
        /// <summary>
        /// Show is user authorized
        /// </summary>
        public static bool isAuthorized = false;
        /// <summary>
        /// Show is user logged out
        /// </summary>
        public static bool isLogOut = false;
        /// <summary>
        /// Settings
        /// </summary>
        public static SettingsHelper settings = new SettingsHelper();
        public const string PATH_TO_CONFIG_FILE = "Settings.xml";

        /// <summary>
        /// Connect to server
        /// </summary>
        public static void ConnectToServer()
        {
            client = new TcpClient();
            try
            {
                client.Connect(settings.host, settings.port);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Disconnect from server
        /// </summary>
        public static void DisconnectFromServer()
        {
            client.Disconnect();
        }

        /// <summary>
        /// Function sends request, receive PollPacket and check if receivedPacket == null. If true, user can retry to receive Packet, else function returns receivedPacket
        /// </summary>
        /// <param name="sendPacket">PollPacket with request to send</param>
        /// <returns>PollPacket receivedPacket</returns>
        public static PollPacket ReceivePollPacket( PollPacket sendPacket )
        {
            try
            {
                string sendString = PollSerializator.SerializePacket( sendPacket );
                PollClientGUI.client.Send( sendString );
            }
            catch ( Exception )
            {
                return null;
            }

            string receivedString = PollClientGUI.client.Receive();
            PollPacket receivedPacket = new PollPacket();
            receivedPacket = PollSerializator.DeserializePacket( receivedString );

            // Check if received data is correct
            if ( receivedPacket == null )
            {
                return null;
            }
            return receivedPacket;
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Load settings
            try
            {
                settings.LoadSettings(PATH_TO_CONFIG_FILE);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            while (true)
            {
                Application.Run(new LoginForm());
                if (isAuthorized)
                {
                    Application.Run(new MainForm());
                    if (isLogOut)
                    {
                        DisconnectFromServer();
                        isLogOut = false;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }
}