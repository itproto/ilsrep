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
using Ilsrep.PollApplication.PollWebService;

namespace Ilsrep.PollApplication.PollClientGUI
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void AuthorizeUser()
        {
            PollPacket pollPacket = new PollPacket();
            pollPacket.request = new Request();
            pollPacket.request.type = Request.USER;
            pollPacket.user = new User();
            pollPacket.user.username = nameField.Text;
            pollPacket.user.password = passwordField.Text;
            
            if (confirmField.Visible)
            {
                if (passwordField.Text == confirmField.Text)
                {
                    pollPacket.user.action = User.NEW_USER;
                    pollPacket = PollClientGUI.ReceivePollPacket(pollPacket);

                    switch (pollPacket.user.action)
                    {
                        case User.AUTH:
                            PollClientGUI.isAuthorized = true;
                            PollClientGUI.userName = nameField.Text;
                            Close();
                            return;
                        case User.EXIST:
                            MessageBox.Show("Error occured during registation", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                    }
                }
                else
                {
                    MessageBox.Show("\"password\" and \"confirm password\" fields aren't identicals", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                pollPacket.user.action = User.EXIST;
            }
            
            pollPacket = PollClientGUI.ReceivePollPacket(pollPacket);

            if (pollPacket == null)
                return;
            else if (pollPacket.user.action == User.EXIST)
            {
                pollPacket.user.action = User.AUTH;
                pollPacket.request = new Request();
                pollPacket.request.type = Request.USER;
                pollPacket = PollClientGUI.ReceivePollPacket(pollPacket);

                switch (pollPacket.user.action)
                {
                    case User.AUTH:
                        PollClientGUI.isAuthorized = true;
                        PollClientGUI.userName = nameField.Text;
                        Close();
                        break;
                    case User.EXIST:
                        MessageBox.Show("Wrong password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            else
            {
                MessageBox.Show("User not found in DB, program will create a new user", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                confirmField.Visible = true;
                confirmLabel.Visible = true;
                nameField.Enabled = false;
                this.Size = new Size(234, 150);
                submitButton.Top = 85;
                settingsButton.Top = 85;
            }

        }

        private void submitButton_Click(object sender, EventArgs e)
        {
            if (nameField.Text != String.Empty)
            {
                if (!PollClientGUI.client.isConnected)
                {
                    PollClientGUI.ConnectToServer();
                }
                AuthorizeUser();
            }
            else
            {
                MessageBox.Show("Name field is empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            this.Size = new Size(234, 122);
            submitButton.Top = 59;
            settingsButton.Top = 59;

            //--------------------------Test----------------------------------
            List<Item> surveys = PollWebService.PollWebService.GetSurveys();
            foreach (Item item in surveys)
            {
                MessageBox.Show(item.id + ". " + item.name);
            }
            //--------------------------End Of Test---------------------------
        }

        private void settingsButton_Click(object sender, EventArgs e)
        {
            SettingsForm settingsForm = new SettingsForm();
            settingsForm.ShowDialog();
        }
    }
}
