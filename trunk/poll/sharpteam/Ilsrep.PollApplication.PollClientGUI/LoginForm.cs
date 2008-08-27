using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ilsrep.PollApplication.Model;
using Ilsrep.PollApplication.Communication;
using System.Xml;
using System.Xml.Serialization;
using Ilsrep.PollApplication.Helpers;

namespace Ilsrep.PollApplication.PollClientGUI
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Connect to server
        /// </summary>
        private void ConnectToServer()
        {
            try
            {
                infoBox.Items.Add("Please wait, connecting to server...");
                PollClientGUI.client.Connect(PollClientGUI.HOST, PollClientGUI.PORT);
                if (PollClientGUI.client.isConnected)
                {
                    infoBox.Items.Add("Connection established");
                }
            }
            catch (Exception exception)
            {
                infoBox.Items.Add("Error: " + exception.Message);
            }
        }

        /// <summary>
        /// Disconnect from server
        /// </summary>
        private void DisconnectFromServer()
        {
            PollClientGUI.client.Disconnect();
            infoBox.Items.Add("Disconnected from server");
        }

        /// <summary>
        /// Function sends request, receive PollPacket and check if receivedPacket == null. If true, user can retry to receive Packet, else function returns receivedPacket
        /// </summary>
        /// <param name="sendPacket">PollPacket with request to send</param>
        /// <returns>PollPacket receivedPacket</returns>
        private PollPacket ReceivePollPacket(PollPacket sendPacket)
        {
            try
            {
                string sendString = PollSerializator.SerializePacket(sendPacket);
                PollClientGUI.client.Send(sendString);
            }
            catch (Exception exception)
            {
                infoBox.Items.Add("Error: " + exception.Message);
            }

            string receivedString = PollClientGUI.client.Receive();
            PollPacket receivedPacket = new PollPacket();
            receivedPacket = PollSerializator.DeserializePacket(receivedString);

            // Check if received data is correct
            if (receivedPacket == null)
            {
                infoBox.Items.Add("Error: Wrong data received");
            }
            return receivedPacket;
        }

        private void AuthorizeUser()
        {
            PollPacket pollPacket = new PollPacket();
            pollPacket.user = new User();
            pollPacket.user.username = nameField.Text;
            pollPacket.user.password = passwordField.Text;

            if (confirmField.Enabled)
            {
                if (passwordField.Text == confirmField.Text)
                {
                    pollPacket.user.password = passwordField.Text;
                    pollPacket.user.isNew = true;
                    confirmField.Enabled = false;
                }
                else
                {
                    infoBox.Items.Add("Error: \"password\" and \"confirm password\" fields aren't identicals");
                    return;
                }
            }

            pollPacket = ReceivePollPacket(pollPacket);
            if (pollPacket == null)
                return;

            if (pollPacket.user.auth)
            {
                PollClientGUI.isAuthorized = true;
                PollClientGUI.userName = nameField.Text;
                Close();
            }
            else if (!pollPacket.user.isNew)
            {
                infoBox.Items.Add("Error: Wrong password");
            }
            else
            {
                infoBox.Items.Add("User not found in DB, program will create a new user");
                infoBox.Items.Add("Please, set your password");
                confirmField.Enabled = true;
            }
        }

        private void submitButton_Click(object sender, EventArgs e)
        {
            if (nameField.Text != String.Empty)
            {
                if (!PollClientGUI.client.isConnected)
                {
                    ConnectToServer();
                }
                AuthorizeUser();
            }
            else
            {
                infoBox.Items.Add("Error: name field is empty");
            }
        }
    }
}
