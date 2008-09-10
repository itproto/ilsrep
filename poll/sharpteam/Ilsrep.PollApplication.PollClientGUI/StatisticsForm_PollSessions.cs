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
    /// <summary>
    /// PollStatistics Form
    /// </summary>
    public partial class StatisticsForm_PollSessions : Form
    {
        /// <summary>
        /// PollSession name, selected in pollSessionsListBox
        /// </summary>
        public static Item selectedPollSession = new Item();

        /// <summary>
        /// List of PollSessions names
        /// </summary>
        private List<Item> pollSessions = new List<Item>();

        /// <summary>
        /// Initialize form
        /// </summary>
        public StatisticsForm_PollSessions()
        {
            InitializeComponent();
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
            pollSessions = receivedPacket.pollSessionList.items;

            // Add PollSessions from list to pollSessionsListBox
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
                Close();
            }

            selectedPollSession = null;
        }

        /// <summary>
        /// Open StatisticForm
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        private void viewStatisticsButton_Click(object sender, EventArgs e)
        {
            if (selectedPollSession == null)
            {
                MessageBox.Show("Please, select PollSession to view statistics", "Error");
            }
            else
            {
                StatisticsForm statisticForm = new StatisticsForm();
                statisticForm.ShowDialog();
            }
        }

        /// <summary>
        /// Refresh pollSessionsList on PollStatisticsForm_Load
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        private void PollStatisticsForm_Load(object sender, EventArgs e)
        {
            int width = Screen.PrimaryScreen.WorkingArea.Width / 2 - this.Width / 2;
            int height = Screen.PrimaryScreen.WorkingArea.Height / 2 - this.Height / 2;
            this.Location = new Point(width, height);
            RefreshPollSessionsList();
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
                selectedPollSession = pollSessions[pollSessionsListBox.SelectedIndex];
            }
            catch (Exception)
            {
                selectedPollSession = null;
            }
        }
    }
}
