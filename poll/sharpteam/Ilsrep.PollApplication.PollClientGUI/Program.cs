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
        /// host to which connect
        /// </summary>
        public const string HOST = "localhost";
        /// <summary>
        /// port to which connect
        /// </summary>
        public const int PORT = 3320;
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
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LoginForm());
            if (isAuthorized)
            {
                Application.Run(new MainForm());
            }
        }
    }
}