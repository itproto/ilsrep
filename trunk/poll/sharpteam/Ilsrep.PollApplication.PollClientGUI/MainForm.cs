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

        private int selectPollSession = -1;
        private int currentPoll = 0;
        private Choice selectedChoice;
        //private PollSession pollSession;
        private ResultsList resultsList = new ResultsList();


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
                if (receivedPacket == null)
                    return;

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

        private void clientPage_Enter( object sender, EventArgs e )
        {
            SelectPollSession();
        }

        private void SelectPollSession()
        {
            PollPacket pollPacket = new PollPacket();
            pollPacket.request = new Request();
            pollPacket.request.type = Request.GET_LIST;
            pollPacket = PollClientGUI.ReceivePollPacket( pollPacket );

            Label lblQuestion = new Label();
            lblQuestion.AutoSize = true;
            lblQuestion.Location = new System.Drawing.Point( 10, 10 );
            lblQuestion.Text = "Please choose poll session:";
            this.Controls.Add( lblQuestion );

            int index = 0;
            foreach ( Item item in pollPacket.pollSessionList.items )
            {
                RadioButton radioButton = new RadioButton();
                radioButton.Name = item.id;
                radioButton.Text = item.name;
                radioButton.Location = new System.Drawing.Point( 15, 30+10*index );
                radioButton.AutoSize = true;
                radioButton.CheckedChanged += delegate( Object sender2, EventArgs e2 )
                {
                    selectPollSession = Convert.ToInt32( ((RadioButton)sender2).Name );
                };

                this.clientPage.Controls.Add( radioButton );
                ++index;
            }

            Button button = new Button();
            button.Name = "submit";
            button.Text = "Select";
            button.Location = new System.Drawing.Point( 10, 50+10*pollPacket.pollSessionList.items.Count );
            button.AutoSize = true;
            button.Click += delegate
            {
                ((Button)button).Enabled = false;
                ProcessPollSession();
            };
            this.clientPage.Controls.Add( button );
        }

        private void ProcessPollSession()
        {
            PollPacket pollPacket = new PollPacket();
            pollPacket.request = new Request();
            pollPacket.request.type = Request.GET_POLLSESSION;
            pollPacket.request.id = selectPollSession.ToString();
            pollPacket = PollClientGUI.ReceivePollPacket( pollPacket );
            pollSession = pollPacket.pollSession;

            AskQuestion();
        }

        private void AskQuestion()
        {
            this.clientPage.Controls.Clear();

            Label lblQuestion = new Label();
            lblQuestion.AutoSize = true;
            lblQuestion.Location = new System.Drawing.Point( 10, 10 );
            lblQuestion.Text = pollSession.polls[currentPoll].description;
            this.clientPage.Controls.Add( lblQuestion );

            int index = 0;
            foreach ( Choice curChoice in pollSession.polls[currentPoll].choices )
            {
                RadioButton radioButton = new RadioButton();
                radioButton.Name = index.ToString();
                radioButton.Text = curChoice.choice;
                radioButton.Location = new System.Drawing.Point( 15, 30+20*index );
                radioButton.AutoSize = true;
                radioButton.CheckedChanged += delegate( Object sender2, EventArgs e2 )
                {
                    selectedChoice = pollSession.polls[currentPoll].choices[Convert.ToInt32( ((RadioButton)sender2).Name )];
                };

                this.clientPage.Controls.Add( radioButton );
                ++index;
            }

            Button button = new Button();
            button.Name = "submit";
            button.Text = "Select";
            button.Location = new System.Drawing.Point( 10, 50+20*pollSession.polls[currentPoll].choices.Count );
            button.AutoSize = true;
            button.Click += delegate
            {
                ((Button)button).Enabled = false;

                PollResult pollResult = new PollResult();
                pollResult.userName = PollClientGUI.userName;
                pollResult.answerId = selectedChoice.parent.id;
                pollResult.questionId = selectedChoice.id;
                resultsList.results.Add( pollResult );

                if ( currentPoll < pollSession.polls.Count - 1 )
                {
                    ++currentPoll;
                    AskQuestion();
                }
                else
                {
                    ProcessResults();
                }
            };
            this.clientPage.Controls.Add( button );
        }

        private void ProcessResults()
        {
            PollPacket pollPacket = new PollPacket();
            pollPacket.request = new Request();
            pollPacket.request.type = Request.SAVE_RESULT;
            pollPacket.resultsList = resultsList;
            pollPacket.resultsList.userName = PollClientGUI.userName;
            pollPacket.resultsList.pollsessionId = pollSession.id;

            PollClientGUI.client.Send( PollSerializator.SerializePacket( pollPacket ) );

            this.clientPage.Controls.Clear();
            SelectPollSession();
        }
    }
}
