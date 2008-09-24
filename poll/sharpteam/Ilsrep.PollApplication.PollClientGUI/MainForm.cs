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
        /// Selected pollsession
        /// </summary>
        private int selectedPollSession = -1;
        /// <summary>
        /// Selected choice
        /// </summary>
        private int selectedChoice = -1;
        /// <summary>
        /// Currently poll that is being processed
        /// </summary>
        private int currentPoll = 0;
        /// <summary>
        /// PollSession that will be filled
        /// </summary>
        public static PollSession pollSession = new PollSession();
        /// <summary>
        /// List of PollSessions names
        /// </summary>
        private List<Item> pollSessionsList = new List<Item>();
        /// <summary>
        /// Where results are stored
        /// </summary>
        private ResultsList resultsList = new ResultsList();
        /// <summary>
        /// Action that user do at the moment
        /// </summary>
        private String currentAction = null;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Text += " [" + PollClientGUI.userName + "]";
            RefreshPollSessionsList();
        }

        /// <summary>
        /// Get pollsession list and show them in the list box
        /// </summary>
        private void RefreshPollSessionsList()
        {
            // Get list of pollsessions
            PollPacket pollPacket = new PollPacket();
            pollPacket.request = new Request();
            pollPacket.request.type = Request.GET_LIST;
            pollPacket = PollClientGUI.ReceivePollPacket( pollPacket );
            
            if ( pollPacket == null )
                return;
            
            pollSessionsList = pollPacket.pollSessionList.items;

            // Add PollSessions from list to pollSessionsListBox in Editor
            pollSessionsListBox.Items.Clear();

            if ( pollSessionsList.Count() == 0 )
            {
                MessageBox.Show("No pollsessions in DB...", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                selectedPollSession = -1;
                return;
            }

            int index = 0;
            foreach (Item item in pollSessionsList)
            {
                index++;
                pollSessionsListBox.Items.Add(index + ". " + item.name);
            }

            //pollSessionsListBox.SelectedIndex = 0;
            //clientListBox.SelectedIndex = 0;

            selectedPollSession = -1;
            //selectedPollSession = pollSessionsList[0];
        }
        
        /// <summary>
        /// Function opens PollSessionForm and then send new PollSession to server
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        private void createButton_Click( object sender, EventArgs e )
        {
            pollSession = new PollSession();
            currentAction = Request.CREATE_POLLSESSION;
            propertyGrid.SelectedObject = pollSession;
            saveButton.Enabled = true;
            cancelButton.Enabled = true;
        }
                
        /// <summary>
        /// Function opens PollSessionForm and then send changed PollSession to server
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        private void editButton_Click(object sender, EventArgs e)
        {
            // Check if any PollSession selected
            if (selectedPollSession != -1)
            {
                // Get PollSession and open PollSessionForm to edit it
                PollPacket pollPacket = new PollPacket();
                pollPacket.request = new Request();
                pollPacket.request.type = Request.GET_POLLSESSION;
                pollPacket.request.id = pollSessionsList[selectedPollSession].id;

                pollPacket = PollClientGUI.ReceivePollPacket(pollPacket);
                if (pollPacket == null)
                    return;

                pollSession = pollPacket.pollSession;

                currentAction = Request.EDIT_POLLSESSION;
                propertyGrid.SelectedObject = pollSession;
                saveButton.Enabled = true;
                cancelButton.Enabled = true;
            }
            else
            {
                MessageBox.Show("Please, select PollSession to edit", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            if (selectedPollSession != -1)
            {
                // Ask user confirmation
                DialogResult userChoice = new DialogResult();
                userChoice = MessageBox.Show("Do you really want to remove pollsession \"" + pollSessionsList[selectedPollSession].name + "\"?", "Remove PollSession?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (userChoice == DialogResult.Yes)
                {
                    PollPacket sendPacket = new PollPacket();
                    //PollPacket receivedPacket = new PollPacket();
                    sendPacket.request = new Request();
                    sendPacket.request.type = Request.REMOVE_POLLSESSION;
                    sendPacket.request.id = pollSessionsList[selectedPollSession].id;
                    PollClientGUI.ReceivePollPacket(sendPacket);

                    RefreshPollSessionsList();
                }
            }
            else
            {
                MessageBox.Show("Please, select PollSession to remove", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Change selectedPollSession if SelectedIndexChanged
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        private void SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                selectedPollSession = ((ListBox)sender).SelectedIndex;
            }
            catch (Exception)
            {
                selectedPollSession = -1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clientPage_Enter( object sender, EventArgs e )
        {
            clientPage.Controls.Clear();
            selectedChoice = -1;
            currentPoll = 0;
            selectedPollSession = -1;

            Label lblTopText = new Label();
            lblTopText.Name = "lblTopText";
            lblTopText.Text = "Please, select PollSession to pass test:";
            lblTopText.Location = new System.Drawing.Point( 0, 5 );
            lblTopText.Width = clientPage.Width;
            lblTopText.TextAlign = ContentAlignment.MiddleCenter;
            clientPage.Controls.Add( lblTopText );

            int index = 0;
            foreach ( Item item in pollSessionsList )
            {
                index++;

                RadioButton radioButton = new RadioButton();
                radioButton.Name = index.ToString();
                radioButton.Text = index.ToString() + ". " + item.name;
                radioButton.Location = new System.Drawing.Point( 5, 5 + lblTopText.Height + 20*(index-1) );
                radioButton.CheckedChanged += delegate( Object sender2, EventArgs e2 )
                {
                    selectedPollSession = Convert.ToInt32( ((RadioButton)sender2).Name ) - 1;
                };
                clientPage.Controls.Add( radioButton );

                //if ( index == 1 )
                    //radioButton.Focus();
            }

            Button btnSubmit = new Button();
            btnSubmit.Name = "btnSubmit";
            btnSubmit.Text = "Select";
            btnSubmit.Location = new System.Drawing.Point( 5, 5 + lblTopText.Height + 20*index + 10 );
            btnSubmit.Click += delegate
            {
                try
                {
                    GetPollSession();
                    GetPoll();
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            clientPage.Controls.Add( btnSubmit );
            this.AcceptButton = btnSubmit;
        }

        /// <summary>
        /// Get PollSession by the selected poll session index
        /// </summary>
        private void GetPollSession()
        {
            if (selectedPollSession != -1)
            {
                PollPacket pollPacket = new PollPacket();
                pollPacket.request = new Request();
                pollPacket.request.type = Request.GET_POLLSESSION;
                pollPacket.request.id = pollSessionsList[selectedPollSession].id.ToString();
                pollPacket = PollClientGUI.ReceivePollPacket(pollPacket);
                pollSession = pollPacket.pollSession;
                //this.Text = "PollClientGUI [" + PollClientGUI.userName + "] - " + pollSession.name;
            }
            else
            {
                throw new Exception("Please, select PollSession");
            }
        }

        private void GetPoll()
        {
            clientPage.Controls.Clear();
            selectedChoice = -1;

            Label lblTopText = new Label();
            Label lblTopText2 = new Label();
            Label lblTopText3 = new Label();
            lblTopText.Name = "lblTopText";
            lblTopText.Text = "Poll #" + (currentPoll + 1);
            lblTopText.Location = new System.Drawing.Point( 0, 5 );
            lblTopText.Height = 14;
            lblTopText.Width = clientPage.Width;
            lblTopText.TextAlign = ContentAlignment.MiddleCenter;
            clientPage.Controls.Add(lblTopText);
            lblTopText2.Name = "lblTopText2";
            lblTopText2.Text = "Name: " + pollSession.polls[currentPoll].name;
            lblTopText2.Location = new System.Drawing.Point( 0, lblTopText.Top + lblTopText.Height );
            lblTopText2.Height = 14;
            lblTopText2.Width = clientPage.Width;
            lblTopText2.TextAlign = ContentAlignment.MiddleCenter;
            clientPage.Controls.Add(lblTopText2);
            lblTopText3.Name = "lblTopText3";
            lblTopText3.Text = "Description: " + pollSession.polls[currentPoll].description;
            lblTopText3.Location = new System.Drawing.Point( 0, lblTopText2.Top + lblTopText2.Height );
            lblTopText3.Height = 14;
            lblTopText3.Width = clientPage.Width;
            lblTopText3.TextAlign = ContentAlignment.MiddleCenter;
            clientPage.Controls.Add(lblTopText3);

            int index = 0;
            foreach (Choice curChoice in pollSession.polls[currentPoll].choices)
            {
                index++;

                RadioButton radioButton = new RadioButton();
                radioButton.Name = index.ToString();
                radioButton.Text = index.ToString() + ". " + curChoice.choice;
                radioButton.Location = new System.Drawing.Point( 5, lblTopText3.Top + lblTopText3.Height + 20*(index-1) );
                radioButton.CheckedChanged += delegate( Object sender2, EventArgs e2 )
                {
                    selectedChoice = Convert.ToInt32( ((RadioButton)sender2).Name ) - 1;
                };
                clientPage.Controls.Add( radioButton );

                if ( index == 1 )
                    radioButton.Focus();

                //clientListBox.Items.Add(choiceIndex + ". " + curChoice.choice);
            }

            Button btnSubmit = new Button();
            btnSubmit.Name = "btnSubmit";
            btnSubmit.Text = "Next";
            btnSubmit.Location = new System.Drawing.Point( this.clientPage.Width - btnSubmit.Width, lblTopText3.Top + lblTopText.Height + 20*(index+1) );
            btnSubmit.Click += delegate
            {
                if ( selectedChoice == -1 )
                {
                    MessageBox.Show("Select a choice", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                ProcessResult();
                if (currentPoll < pollSession.polls.Count)
                {
                    GetPoll();
                }
                else
                {
                    ProcessResults();
                }
            };
            clientPage.Controls.Add( btnSubmit );
            this.AcceptButton = btnSubmit;

            currentPoll++;
        }

        private void ProcessResult()
        {
            PollResult result = new PollResult();
            result.questionId = pollSession.polls[currentPoll - 1].id;
            result.answerId = pollSession.polls[currentPoll - 1].choices[selectedChoice].id;
            result.userName = PollClientGUI.userName;
            resultsList.results.Add( result );
        }

        private void ProcessResults()
        {
            PollPacket sendPacket = new PollPacket();
            sendPacket.request = new Request();
            sendPacket.request.type = Request.SAVE_RESULT;
            sendPacket.resultsList = resultsList;
            sendPacket.resultsList.userName = PollClientGUI.userName;
            sendPacket.resultsList.pollsessionId = pollSession.id;

            PollClientGUI.ReceivePollPacket(sendPacket);

            clientPage.Controls.Clear();
            Label lblTopText = new Label();
            lblTopText.AutoSize = true;
            lblTopText.Name = "lblTopText";
            lblTopText.Location = new System.Drawing.Point( 5, 5 );
            lblTopText.Text = "PollSession: " + pollSession.name + Environment.NewLine + "Here is your results:" + Environment.NewLine;
            clientPage.Controls.Add( lblTopText );

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
                
                lblTopText.Text += index + ". " + pollSession.polls[pollIndex].name + ": " + pollSession.polls[pollIndex].choices[choiceIndex].choice + Environment.NewLine;
                
                if (pollSession.testMode)
                {
                    if (pollSession.polls[pollIndex].choices[choiceIndex].id == pollSession.polls[pollIndex].correctChoiceID)
                        correctAnswers++;
                }
            }

            if (pollSession.testMode)
            {
                double userScore = correctAnswers / pollSession.polls.Count;
                lblTopText.Text += Environment.NewLine + "Your score: " + Convert.ToInt32(userScore*100) + "%";

                if (userScore >= pollSession.minScore)
                {
                    lblTopText.Text += Environment.NewLine + "Congratulations!!! You PASSED the test";
                }
                else
                {
                    lblTopText.Text += Environment.NewLine + "Sorry, try again... you FAILED";
                }
            }

            Button btnSubmit = new Button();
            btnSubmit.Location = new System.Drawing.Point( 5, 5 + lblTopText.Height + 30 );
            btnSubmit.Name = "btnSubmit";
            btnSubmit.Text = "OK";
            btnSubmit.Click += new EventHandler( clientPage_Enter );
            clientPage.Controls.Add( btnSubmit );
            this.AcceptButton = btnSubmit;

            resultsList.results.Clear();
            currentPoll = -1;
            selectedPollSession = -1;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            // Verification of filling of pollsession's fields
            try
            {
                if (pollSession.name == null)
                    throw new Exception("PollSession name can't be empty");
                if (pollSession.polls.Count == 0)
                    throw new Exception("PollSession must have polls");
                
                int pollIndex = 0;
                foreach (Poll curPoll in pollSession.polls)
                {
                    if (curPoll.name == null)
                        throw new Exception("Poll#" + pollIndex + " name can't be empty");
                    if (curPoll.description == null)
                        throw new Exception("Poll#" + pollIndex + " description can't be empty");
                    if (curPoll.choices.Count == 0)
                        throw new Exception("Poll#" + pollIndex + " must have choices");

                    int choiceIndex = 0;
                    foreach (Choice curChoice in curPoll.choices)
                    {
                        if (curChoice.choice == null)
                            throw new Exception("Choice#" + choiceIndex + " of Poll#" + pollIndex + " can't be empty");
                        choiceIndex++;
                    }

                    pollIndex++;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            PollPacket pollPacket = new PollPacket();
            pollPacket.request = new Request();

            // ----- for synchronize with server(will fixed on future) -----
            foreach (Poll curPoll in pollSession.polls)
            {
                curPoll.correctChoiceID += 1;
            }

            switch (currentAction)
            {
                case Request.CREATE_POLLSESSION:
                    pollPacket.request.type = Request.CREATE_POLLSESSION;
                    pollPacket.pollSession = pollSession;
                    PollClientGUI.ReceivePollPacket(pollPacket);
                    RefreshPollSessionsList();
                    break;
                case Request.EDIT_POLLSESSION:
                    pollPacket.request.type = Request.EDIT_POLLSESSION;
                    pollPacket.pollSession = pollSession;
                    PollClientGUI.ReceivePollPacket(pollPacket);
                    RefreshPollSessionsList();
                    break;
            }

            currentAction = null;
            propertyGrid.SelectedObject = null;
            saveButton.Enabled = false;
            cancelButton.Enabled = false;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            currentAction = null;
            propertyGrid.SelectedObject = null;
            saveButton.Enabled = false;
            cancelButton.Enabled = false;
        }

        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PollClientGUI.isAuthorized = false;
            PollClientGUI.isLogOut = true;
            Close();
        }
    }
}