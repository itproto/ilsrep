using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ilsrep.PollApplication.Communication;
using Ilsrep.PollApplication.Helpers;
using Ilsrep.PollApplication.Model;

namespace Ilsrep.PollApplication.PollClientGUI
{
    /// <summary>
    /// Statistic Form
    /// </summary>
    public partial class StatisticsForm : Form
    {
        /// <summary>
        /// Initialize Form
        /// </summary>
        public StatisticsForm()
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
        /// Get top 10 results of selected PollSession
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        private void StatisticForm_Load(object sender, EventArgs e)
        {
            PollPacket sendPacket = new PollPacket();
            PollPacket receivedPacket = new PollPacket();
            sendPacket.request = new Request();

            sendPacket.request.type = Request.GET_RESULTS;
            sendPacket.request.id = StatisticsForm_PollSessions.selectedPollSession.id.ToString();
            receivedPacket = ReceivePollPacket(sendPacket);

            foreach (PollResult curResult in receivedPacket.resultsList.results)
            {
                resultsListBox.Items.Add(String.Format("name:{0} poll:{1} choice:{2} date:{3} custom:{4}", curResult.userName, curResult.questionId, curResult.answerId, curResult.date, curResult.customChoice));
            }
        }

        /// <summary>
        /// Close Form
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
