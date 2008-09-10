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
                PollClientGUI.client.Connect(PollClientGUI.HOST, PollClientGUI.PORT);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error");
            }
        }

        /// <summary>
        /// Disconnect from server
        /// </summary>
        private void DisconnectFromServer()
        {
            PollClientGUI.client.Disconnect();
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
                MessageBox.Show(exception.Message, "Error");
            }

            string receivedString = PollClientGUI.client.Receive();
            PollPacket receivedPacket = new PollPacket();
            receivedPacket = PollSerializator.DeserializePacket(receivedString);

            // Check if received data is correct
            if (receivedPacket == null)
            {
                MessageBox.Show("Wrong data received", "Error");
            }
            return receivedPacket;
        }

        private void AuthorizeUser()
        {
            PollPacket pollPacket = new PollPacket();
            pollPacket.user = new User();
            pollPacket.user.username = nameField.Text;
            pollPacket.user.password = passwordField.Text;

            if (confirmField.Visible)
            {
                if (passwordField.Text == confirmField.Text)
                {
                    pollPacket.user.password = passwordField.Text;
                    pollPacket.user.isNew = true;
                }
                else
                {
                    MessageBox.Show("\"password\" and \"confirm password\" fields aren't identicals", "Error");
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
                MessageBox.Show("Wrong password", "Error");
            }
            else
            {
                MessageBox.Show("User not found in DB, program will create a new user", "Info");
                confirmField.Visible = true;
                confirmLabel.Visible = true;
                //submitButton.Location.Y += 30;
                //Size.Height += 30;
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
                MessageBox.Show("Name field is empty", "Error");
            }
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            int width = Screen.PrimaryScreen.WorkingArea.Width/2 - this.Width/2;
            int height = Screen.PrimaryScreen.WorkingArea.Height/2 - this.Height/2;
            this.Location = new Point(width, height);
        }
    }
}
