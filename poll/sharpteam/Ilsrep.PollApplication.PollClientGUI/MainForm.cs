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
        private int currentPoll = 0;
        private int clientIndexSelected = 0;
        private ResultsList resultsList = new ResultsList();
        private const string POLLSESSION = "PollSession";
        private const string POLL = "Poll";
        private const string RESULTS = "Results";
        private string process = POLLSESSION;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            int width = Screen.PrimaryScreen.WorkingArea.Width / 2 - this.Width / 2;
            int height = Screen.PrimaryScreen.WorkingArea.Height / 2 - this.Height / 2;
            this.Location = new Point(width, height);
            this.Text += " [" + PollClientGUI.userName + "]";
            RefreshPollSessionsList();
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
            receivedPacket = PollClientGUI.ReceivePollPacket(sendPacket);
            if (receivedPacket == null)
                return;
            pollSessionsList = receivedPacket.pollSessionList.items;

            // Add PollSessions from list to pollSessionsListBox in Editor and in Client
            pollSessionsListBox.Items.Clear();
            clientListBox.Items.Clear();
            int index = 0;
            foreach (Item item in pollSessionsList)
            {
                index++;
                pollSessionsListBox.Items.Add(index + ". " + item.name);
                clientListBox.Items.Add(index + ". " + item.name);
            }

            if (pollSessionsList.Count() == 0)
            {
                MessageBox.Show("No pollsessions in DB...", "Info");
                selectedPollSession = null;
                return;
            }

            pollSessionsListBox.SelectedIndex = 0;
            clientListBox.SelectedIndex = 0;
            clientInfoBox.Text = "Please, select PollSession to pass test:";
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

            if (pollSession == null)
                return;

            // Save changes
            PollPacket sendPacket = new PollPacket();
            PollPacket receivedPacket = new PollPacket();
            sendPacket.request = new Request();
            sendPacket.request.type = Request.CREATE_POLLSESSION;
            sendPacket.pollSession = pollSession;
            receivedPacket = PollClientGUI.ReceivePollPacket(sendPacket);

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
                receivedPacket = PollClientGUI.ReceivePollPacket(sendPacket);
                if (receivedPacket == null)
                    return;

                pollSession = receivedPacket.pollSession;

                PollSessionForm pollSessionForm = new PollSessionForm();
                pollSessionForm.ShowDialog();

                // Save changes
                sendPacket.request.type = Request.EDIT_POLLSESSION;
                sendPacket.pollSession = pollSession;
                receivedPacket = PollClientGUI.ReceivePollPacket(sendPacket);

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
                    receivedPacket = PollClientGUI.ReceivePollPacket(sendPacket);

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

        private void ProcessPollSession()
        {
            if (clientIndexSelected != -1)
            {
                PollPacket pollPacket = new PollPacket();
                pollPacket.request = new Request();
                pollPacket.request.type = Request.GET_POLLSESSION;
                pollPacket.request.id = pollSessionsList[clientIndexSelected].id.ToString();
                pollPacket = PollClientGUI.ReceivePollPacket(pollPacket);
                pollSession = pollPacket.pollSession;

                this.Text = "PollClientGUI [" + PollClientGUI.userName + "] - " + pollSession.name;
            }
            else
            {
                MessageBox.Show("Please, select PollSession", "Error");
            }
        }

        private void ProcessPoll()
        {
            if (clientIndexSelected != -1)
            {
                if (process == POLL)
                {
                    PollResult result = new PollResult();
                    result.questionId = pollSession.polls[currentPoll - 1].id;
                    result.answerId = pollSession.polls[currentPoll - 1].choices[clientIndexSelected].id;
                    result.userName = PollClientGUI.userName;
                    resultsList.results.Add(result);
                }

                clientInfoBox.Clear();
                clientListBox.Items.Clear();

                if (currentPoll < pollSession.polls.Count)
                {
                    clientInfoBox.Text = "Poll #" + (currentPoll + 1);
                    clientInfoBox.AppendText(Environment.NewLine);
                    clientInfoBox.AppendText("Name: " + pollSession.polls[currentPoll].name);
                    clientInfoBox.AppendText(Environment.NewLine);
                    clientInfoBox.AppendText("Description: " + pollSession.polls[currentPoll].description);

                    int choiceIndex = 0;
                    foreach (Choice curChoice in pollSession.polls[currentPoll].choices)
                    {
                        choiceIndex++;
                        clientListBox.Items.Add(choiceIndex + ". " + curChoice.choice);
                    }

                    currentPoll++;
                    clientIndexSelected = -1;
                }
                else
                {
                    ProcessResults();
                    clientSubmitButton.Text = "OK";
                }
            }
            else
            {
                MessageBox.Show("Please, select your answer", "Error");
            }
        }

        private void ProcessResults()
        {
            PollPacket sendPacket = new PollPacket();
            sendPacket.request = new Request();
            sendPacket.request.type = Request.SAVE_RESULT;
            sendPacket.resultsList = resultsList;
            sendPacket.resultsList.userName = PollClientGUI.userName;
            sendPacket.resultsList.pollsessionId = pollSession.id;

            PollPacket receivedPacket = new PollPacket();
            receivedPacket = PollClientGUI.ReceivePollPacket(sendPacket);

            clientInfoBox.Text = "PollSession: " + pollSession.name + Environment.NewLine + "Here is your results:" + Environment.NewLine;
            float correctAnswers = 0;
            int index = 0;
            foreach (PollResult curResult in resultsList.results)
            {
                index++;

                int pollIndex = 0;
                foreach (Poll curPoll in pollSession.polls)
                {
                    if (curPoll.id == curResult.questionId)
                        break;
                    pollIndex++;
                }

                int choiceIndex = 0;
                foreach (Choice curChoice in pollSession.polls[pollIndex].choices)
                {
                    if (curChoice.id == curResult.answerId)
                        break;
                    choiceIndex++;
                }
                
                clientInfoBox.AppendText(index + ". " + pollSession.polls[pollIndex].name + ": " + pollSession.polls[pollIndex].choices[choiceIndex].choice + Environment.NewLine);
                
                if (pollSession.testMode)
                {
                    if (pollSession.polls[pollIndex].choices[choiceIndex].id == pollSession.polls[pollIndex].correctChoiceID)
                        correctAnswers++;
                }
            }

            if (pollSession.testMode)
            {
                double userScore = correctAnswers / pollSession.polls.Count;
                clientInfoBox.AppendText(Environment.NewLine + "Your scores: " + userScore);

                if (userScore >= pollSession.minScore)
                {
                    clientInfoBox.AppendText(Environment.NewLine + "Congratulations!!! You PASSED the test");
                }
                else
                {
                    clientInfoBox.AppendText(Environment.NewLine + "Sorry, try again... you FAILED");
                }
            }

            resultsList.results.Clear();
            currentPoll = 0;
            process = RESULTS;
        }

        private void clientSubmitButton_Click(object sender, EventArgs e)
        {
            switch (process)
            {
                case POLLSESSION:
                    ProcessPollSession();
                    ProcessPoll();
                    process = POLL;
                    clientSubmitButton.Text = "Next>";
                    break;
                case POLL:
                    ProcessPoll();
                    break;
                case RESULTS:
                    RefreshPollSessionsList();
                    this.Text = "PollClientGUI [" + PollClientGUI.userName + "]";
                    process = POLLSESSION;
                    clientSubmitButton.Text = "Submit";
                    break;
            }
        }

        private void clientListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                clientIndexSelected = clientListBox.SelectedIndex;
            }
            catch (Exception)
            {
                clientIndexSelected = -1;
            }
        }
    }
}