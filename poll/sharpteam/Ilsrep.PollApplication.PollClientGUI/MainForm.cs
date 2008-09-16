﻿using System;
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
        /// public static Item selectedPollSession = new Item();
        private int selectedPollSession = -1;

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
        //private int clientIndexSelected = -1;

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

            // Add PollSessions from list to pollSessionsListBox in Editor and in Client
            pollSessionsListBox.Items.Clear();
            //clientListBox.Items.Clear();
            //clientPage.Controls.Clear();

            if ( pollSessionsList.Count() == 0 )
            {
                MessageBox.Show( "No pollsessions in DB...", "Info" );
                selectedPollSession = -1;
                return;
            }

            int index = 0;
            foreach (Item item in pollSessionsList)
            {
                index++;
                pollSessionsListBox.Items.Add(index + ". " + item.name);
                //clientListBox.Items.Add(index + ". " + item.name);
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
            // Open PollSessionForm to fill new PollSession
            pollSession = null;
            PollSessionForm pollSessionForm = new PollSessionForm();
            pollSessionForm.ShowDialog();

            if ( pollSession == null )
                return;

            // Save changes
            PollPacket sendPacket = new PollPacket();
            //PollPacket receivedPacket = new PollPacket();
            sendPacket.request = new Request();
            sendPacket.request.type = Request.CREATE_POLLSESSION;
            sendPacket.pollSession = pollSession;
            PollClientGUI.client.Send( PollSerializator.SerializePacket( sendPacket ) );
            //receivedPacket = PollClientGUI.ReceivePollPacket(sendPacket);

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
            if (selectedPollSession != -1)
            {
                // Get PollSession and open PollSessionForm to edit it
                PollPacket sendPacket = new PollPacket();
                sendPacket.request = new Request();
                sendPacket.request.type = Request.GET_POLLSESSION;
                sendPacket.request.id = pollSessionsList[selectedPollSession].id;
                
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
                PollClientGUI.client.Send( PollSerializator.SerializePacket( sendPacket ) );

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
            if (selectedPollSession != -1)
            {
                // Ask user confirmation
                DialogResult userChoice = new DialogResult();
                userChoice = MessageBox.Show(null, "Do you really want to remove pollsession \"" + pollSessionsList[selectedPollSession].name + "\"?", "Remove PollSession?", MessageBoxButtons.YesNo);
                if (userChoice == DialogResult.Yes)
                {
                    PollPacket sendPacket = new PollPacket();
                    //PollPacket receivedPacket = new PollPacket();
                    sendPacket.request = new Request();
                    sendPacket.request.type = Request.REMOVE_POLLSESSION;
                    sendPacket.request.id = pollSessionsList[selectedPollSession].id;
                    PollClientGUI.client.Send( PollSerializator.SerializePacket( sendPacket ) );
                    //receivedPacket = PollClientGUI.ReceivePollPacket(sendPacket);

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
            selectedChoice = 0;
            currentPoll = 0;
            selectedPollSession = 0;

            Label lblTopText = new Label();
            lblTopText.AutoSize = true;
            lblTopText.Name = "lblTopText";
            lblTopText.Text = "Please, select PollSession to pass test:";
            lblTopText.Location = new System.Drawing.Point( 5, 5 );
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
                //clientListBox.Items.Add(index + ". " + item.name);
            }

            Button btnSubmit = new Button();
            btnSubmit.Name = "btnSubmit";
            btnSubmit.Text = "Submit";
            btnSubmit.Location = new System.Drawing.Point( 5, 5 + lblTopText.Height + 20*index + 10 );
            btnSubmit.Click += delegate
            {
                GetPollSession();
                GetPoll();
            };
            clientPage.Controls.Add( btnSubmit );
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
                MessageBox.Show("Please, select PollSession", "Error");
            }
        }

        private void GetPoll()
        {
            clientPage.Controls.Clear();
            selectedChoice = -1;

            Label lblTopText = new Label();
            lblTopText.AutoSize = true;
            lblTopText.Name = "lblTopText";
            lblTopText.Location = new System.Drawing.Point( 5, 5 );
            lblTopText.Text = "Poll #" + (currentPoll + 1)
                         + Environment.NewLine
                         + "Name: " + pollSession.polls[currentPoll].name
                         + Environment.NewLine
                         + "Description: " + pollSession.polls[currentPoll].description;
            clientPage.Controls.Add( lblTopText );

            int index = 0;
            foreach (Choice curChoice in pollSession.polls[currentPoll].choices)
            {
                index++;

                RadioButton radioButton = new RadioButton();
                radioButton.Name = index.ToString();
                radioButton.Text = index.ToString() + ". " + curChoice.choice;
                radioButton.Location = new System.Drawing.Point( 5, 5 + lblTopText.Height + 20*(index-1) );
                radioButton.CheckedChanged += delegate( Object sender2, EventArgs e2 )
                {
                    selectedChoice = Convert.ToInt32( ((RadioButton)sender2).Name ) - 1;
                };
                clientPage.Controls.Add( radioButton );

                //clientListBox.Items.Add(choiceIndex + ". " + curChoice.choice);
            }

            Button btnSubmit = new Button();
            btnSubmit.Name = "btnSubmit";
            btnSubmit.Text = "Submit";
            btnSubmit.Location = new System.Drawing.Point( 5, 5 + lblTopText.Height + 20*index + 10 );
            btnSubmit.Click += delegate
            {
                if ( selectedChoice == -1 )
                {
                    MessageBox.Show( "Select a choice" );
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

            PollClientGUI.client.Send( PollSerializator.SerializePacket( sendPacket ) );

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
                lblTopText.Text += Environment.NewLine + "Your scores: " + userScore;

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
            btnSubmit.Text = "OK";
            btnSubmit.Click += new EventHandler( clientPage_Enter );
            clientPage.Controls.Add( btnSubmit );

            resultsList.results.Clear();
            currentPoll = -1;
            selectedPollSession = -1;
        }
    }
}