using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ilsrep.PollApplication.Communication;
using Ilsrep.PollApplication.Model;
using Ilsrep.PollApplication.Helpers;

namespace Ilsrep.PollApplication.PollClientGUI
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// PollSession name, selected in pollSessionsListBox
        /// </summary>
        public static Item selectedPollSession = new Item();
        /// <summary>
        /// PollSession that will be filled
        /// </summary>
        public static PollSession pollSession = new PollSession();
        /// <summary>
        /// List of PollSessions names
        /// </summary>
        public static List<Item> pollSessionsList = new List<Item>();

        public MainForm()
        {
            InitializeComponent();
            RefreshPollSessionsList();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            int width = Screen.PrimaryScreen.WorkingArea.Width / 2 - this.Width / 2;
            int height = Screen.PrimaryScreen.WorkingArea.Height / 2 - this.Height / 2;
            this.Location = new Point(width, height);
            this.Text += " [" + PollClientGUI.userName + "]";
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
                return null;
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

        /// <summary>
        /// Get PollSessionsList from server and show them in pollSessionsListBox
        /// </summary>
        private void RefreshPollSessionsList()
        {
            // Get list of pollsessions
            PollPacket sendPacket = new PollPacket();
            sendPacket.request = new Request();
            sendPacket.request.type = Request.GET_LIST;

            PollPacket receivedPacket = new PollPacket();
            receivedPacket = ReceivePollPacket(sendPacket);
            if (receivedPacket == null)
                return;
            pollSessionsList = receivedPacket.pollSessionList.items;

            // Add PollSessions from list to pollSessionsListBox
            pollSessionsListBox.Items.Clear();
            int index = 0;
            foreach (Item item in pollSessionsList)
            {
                index++;
                pollSessionsListBox.Items.Add(index + ". " + item.name);
            }

            if (pollSessionsList.Count() == 0)
            {
                MessageBox.Show("No pollsessions in DB...", "Info");
                selectedPollSession = null;
                return;
            }

            pollSessionsListBox.SelectedIndex = 0;
            selectedPollSession = pollSessionsList[0];
        }
        
        /// <summary>
        /// Function opens PollSessionForm and then send new PollSession to server
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        private void createButton_Click(object sender, EventArgs e)
        {
            // Open PollSessionForm to fill new PollSession
            pollSession = null;
            PollSessionForm pollSessionForm = new PollSessionForm();
            pollSessionForm.ShowDialog();

            // Save changes
            PollPacket sendPacket = new PollPacket();
            PollPacket receivedPacket = new PollPacket();
            sendPacket.request = new Request();
            sendPacket.request.type = Request.CREATE_POLLSESSION;
            sendPacket.pollSession = pollSession;
            receivedPacket = ReceivePollPacket(sendPacket);

            RefreshPollSessionsList();
        }        
                
        /// <summary>
        /// Function opens PollSessionForm and then send changed PollSession to server
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        private void editButton_Click(object sender, EventArgs e)
        {
            // Check if any PollSession selected
            if (selectedPollSession != null)
            {
                // Get PollSession and open PollSessionForm to edit it
                PollPacket sendPacket = new PollPacket();
                sendPacket.request = new Request();
                sendPacket.request.type = Request.GET_POLLSESSION;
                sendPacket.request.id = selectedPollSession.id;
                PollPacket receivedPacket = new PollPacket();
                receivedPacket = ReceivePollPacket(sendPacket);
                pollSession = receivedPacket.pollSession;

                PollSessionForm pollSessionForm = new PollSessionForm();
                pollSessionForm.ShowDialog();

                // Save changes
                sendPacket.request.type = Request.EDIT_POLLSESSION;
                sendPacket.pollSession = pollSession;
                receivedPacket = ReceivePollPacket(sendPacket);

                RefreshPollSessionsList();
            }
            else
            {
                MessageBox.Show("Please, select PollSession to edit", "Error");
            }
        }

        /// <summary>
        /// Function send request to server to remove selected PollSession
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        private void removeButton_Click(object sender, EventArgs e)
        {
            // Check if any PollSession selected
            if (selectedPollSession != null)
            {
                // Ask user confirmation
                DialogResult userChoice = new DialogResult();
                userChoice = MessageBox.Show(null, "Do you really want to remove pollsession \"" + selectedPollSession.name + "\"?", "Remove PollSession?", MessageBoxButtons.YesNo);
                if (userChoice == DialogResult.Yes)
                {
                    PollPacket sendPacket = new PollPacket();
                    PollPacket receivedPacket = new PollPacket();
                    sendPacket.request = new Request();
                    sendPacket.request.type = Request.REMOVE_POLLSESSION;
                    sendPacket.request.id = selectedPollSession.id;
                    receivedPacket = ReceivePollPacket(sendPacket);

                    RefreshPollSessionsList();
                }
            }
            else
            {
                MessageBox.Show("Please, select PollSession to remove", "Error");
            }
        }

        /// <summary>
        /// Change selectedPollSession if SelectedIndexChanged
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        private void pollSessionsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                selectedPollSession = pollSessionsList[pollSessionsListBox.SelectedIndex];
            }
            catch (Exception)
            {
                selectedPollSession = null;
            }
        }

        private void editorPage_Enter(object sender, EventArgs e)
        {
            RefreshPollSessionsList();
        }
    }
}
