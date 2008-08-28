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
using Ilsrep.PollApplication.Helpers;

namespace Ilsrep.PollApplication.PollClientGUI
{
    public partial class PollEditorForm : Form
    {
        public static Item selectedPollSession = new Item();
        public static PollSession pollSession = new PollSession();
        public static List<Item> pollSessions = new List<Item>();

        public PollEditorForm()
        {
            InitializeComponent();
            RefreshPollSessionsList();
            selectedPollSession = null;
            pollSession = null;
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
                MessageBox.Show("Error: " + exception.Message);
            }

            string receivedString = PollClientGUI.client.Receive();
            PollPacket receivedPacket = new PollPacket();
            receivedPacket = PollSerializator.DeserializePacket(receivedString);

            // Check if received data is correct
            if (receivedPacket == null)
            {
                MessageBox.Show("Error: Wrong data received");
            }
            return receivedPacket;
        }

        private void RefreshPollSessionsList()
        {
            selectedPollSession = null;

            // Get list of pollsessions from server and write they in console
            PollPacket sendPacket = new PollPacket();
            sendPacket.request = new Request();
            sendPacket.request.type = Request.GET_LIST;

            PollPacket receivedPacket = new PollPacket();
            receivedPacket = ReceivePollPacket(sendPacket);
            pollSessions = receivedPacket.pollSessionList.items;

            pollSessionsListBox.Items.Clear();
            int index = 0;
            foreach (Item item in pollSessions)
            {
                index++;
                pollSessionsListBox.Items.Add(index + ". " + item.name);
            }

            if (pollSessions.Count() == 0)
            {
                MessageBox.Show("No pollsessions in DB...", "Info");
                createButton_Click(null, EventArgs.Empty);
            }
        }

        private void pollSessionsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                selectedPollSession = pollSessions[pollSessionsListBox.SelectedIndex];
            }
            catch (Exception exception)
            {
                selectedPollSession = null;
            }
        }

        private void createButton_Click(object sender, EventArgs e)
        {
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

        private void editButton_Click(object sender, EventArgs e)
        {
            if (selectedPollSession != null)
            {
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

        private void removeButton_Click(object sender, EventArgs e)
        {
            if (selectedPollSession != null)
            {
                DialogResult userChoice = new DialogResult();
                userChoice = MessageBox.Show(null, "Do you really want to remove pollsession \"" + selectedPollSession.name + "\"?", "Remove PollSession?", MessageBoxButtons.YesNo);
                if (userChoice == DialogResult.Yes)
                {
                    PollPacket sendPacket = new PollPacket();
                    sendPacket.request = new Request();
                    sendPacket.request.type = Request.REMOVE_POLLSESSION;
                    sendPacket.request.id = selectedPollSession.id;

                    PollPacket receivedPacket = new PollPacket();
                    receivedPacket = ReceivePollPacket(sendPacket);
                    RefreshPollSessionsList();
                }
            }
            else
            {
                MessageBox.Show("Please, select PollSession to remove", "Error");
            }
        }
    }
}
