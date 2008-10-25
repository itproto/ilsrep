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
using Ilsrep.Common;

namespace Ilsrep.PollApplication.PollClientGUI
{
    public partial class MainForm : Form
    {
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
        private List<PollSession> pollSessionsList = new List<PollSession>();
        /// <summary>
        /// Where results are stored
        /// </summary>
        private ResultsList resultsList = new ResultsList();
        /// <summary>
        /// Users choice
        /// </summary>
        private String customChoice = String.Empty;
        /// <summary>
        /// History of changes
        /// </summary>
        private HistoryHelper history = new HistoryHelper();
        /// <summary>
        /// Generate new negative PollSession id
        /// </summary>
        private IDGenerator idGenerator = new IDGenerator();

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Text += " [" + PollClientGUI.userName + "]";
            GetPollSessions();
            history.Clear();
        }

        /// <summary>
        /// Get pollsession list and show them in the list box
        /// </summary>
        private void GetPollSessions()
        {
            // Get list of pollsessions
            PollPacket pollPacket = new PollPacket();
            pollPacket.request = new Request();
            pollPacket.request.type = Request.GET_LIST;
            pollPacket = PollClientGUI.ReceivePollPacket( pollPacket );

            if ( pollPacket == null )
                return;

            // Get pollSessionsList
            pollSessionsList.Clear();
            foreach (Item item in pollPacket.pollSessionList.items)
            {
                pollPacket.request = new Request();
                pollPacket.request.type = Request.GET_POLLSESSION;
                pollPacket.request.id = item.id;
                pollPacket = PollClientGUI.ReceivePollPacket(pollPacket);

                // Modify correctChoices
                foreach (Poll curPoll in pollPacket.pollSession.Polls)
                {
                    int choiceIndex = 0;
                    foreach (Choice curChoice in curPoll.Choices)
                    {
                        if (curChoice.Id == curPoll.CorrectChoiceID)
                            break;
                        choiceIndex++;
                    }
                    curPoll.CorrectChoiceID = choiceIndex;
                }

                pollSessionsList.Add(pollPacket.pollSession);
            }
            RefreshEditorList();
        }

        private void RefreshEditorList()
        {
            // Add PollSessions from list to pollSessionsListBox in Editor
            pollSessionsListBox.Items.Clear();

            int index = 0;
            foreach (PollSession curPollSession in pollSessionsList)
            {
                index++;
                pollSessionsListBox.Items.Add(index + ". " + curPollSession.Name);
            }

            if (pollSessionsListBox.Items.Count == 0)
            {
                MessageBox.Show("No pollsessions in DB...", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                pollSessionsListBox.SelectedIndex = -1;
                removeButton.Enabled = false;
                propertyGrid.SelectedObject = null;
            }
            else
            {
                pollSessionsListBox.SelectedIndex = 0;
                removeButton.Enabled = true;
            }
        }

        /// <summary>
        /// Function opens PollSessionForm and then send new PollSession to server
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        private void createButton_Click( object sender, EventArgs e )
        {
            PollSession pollSession = new PollSession();
            int newId = idGenerator.id;
            pollSession.Name = "newPollSession" + Math.Abs(newId).ToString();
            pollSession.Id = newId;
            pollSessionsList.Add(pollSession);
            propertyGrid.SelectedObject = pollSessionsList[pollSessionsList.Count - 1];
            RefreshEditorList();
            pollSessionsListBox.SelectedIndex = pollSessionsList.Count - 1;
        }

        /// <summary>
        /// Function send request to server to remove selected PollSession
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        private void removeButton_Click(object sender, EventArgs e)
        {
            // Ask user confirmation
            DialogResult userChoice = new DialogResult();
            userChoice = MessageBox.Show("Do you really want to remove pollsession \"" + pollSessionsList[pollSessionsListBox.SelectedIndex].Name + "\"?", "Remove PollSession?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (userChoice == DialogResult.Yes)
            {
                if (pollSessionsList[pollSessionsListBox.SelectedIndex].Id != -1)
                {
                    history.AddToDeleted(pollSessionsList[pollSessionsListBox.SelectedIndex].Id);
                }
                pollSessionsList.Remove(pollSessionsList[pollSessionsListBox.SelectedIndex]);
                RefreshEditorList();
            }
        }

        /// <summary>
        /// Change selectedPollSession if SelectedIndexChanged
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        private void SelectedIndexChanged(object sender, EventArgs e)
        {
            propertyGrid.SelectedObject = pollSessionsList[pollSessionsListBox.SelectedIndex];
            if (pollSessionsList[pollSessionsListBox.SelectedIndex].Id >= 0)
            {
                history.AddToEdited(pollSessionsList[pollSessionsListBox.SelectedIndex].Id);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clientPage_Enter(object sender, EventArgs e)
        {
            currentPoll = 0;
            pollSessionsListBox1.Items.Clear();
            pollGroupBox.Controls.Clear();
            foreach (PollSession curPollSession in pollSessionsList)
            {
                pollSessionsListBox1.Items.Add(curPollSession.Name);
            }

            if (pollSessionsList.Count == 0)
            {
                submitButton.Enabled = false;
            }
            else
            {
                pollSessionsListBox1.SelectedIndex = 0;
                submitButton.Enabled = true;
                pollSessionsListBox1.Enabled = true;
                this.AcceptButton = submitButton;
            }
        }



        private void GetPoll()
        {
            pollGroupBox.Controls.Clear();
            selectedChoice = -1;

            Label lblTopText = new Label();
            Label lblTopText2 = new Label();
            Label lblTopText3 = new Label();
            lblTopText.Name = "lblTopText";
            lblTopText.Text = "Poll #" + (currentPoll + 1);
            lblTopText.Location = new System.Drawing.Point(10, 15);
            lblTopText.Width = pollGroupBox.Width;
            lblTopText.TextAlign = ContentAlignment.MiddleLeft;
            lblTopText.ForeColor = Color.Brown;
            lblTopText.Font = new Font("Times New Roman", 11, FontStyle.Bold);
            lblTopText.Height = 17;
            pollGroupBox.Controls.Add(lblTopText);
            lblTopText2.Name = "lblTopText2";
            lblTopText2.Text = "Name: " + pollSession.Polls[currentPoll].Name;
            lblTopText2.Location = new System.Drawing.Point( 10, lblTopText.Top + lblTopText.Height );
            lblTopText2.Width = pollGroupBox.Width;
            lblTopText2.TextAlign = ContentAlignment.MiddleLeft;
            lblTopText2.ForeColor = Color.Green;
            lblTopText2.Font = new Font("Times New Roman", 11);
            lblTopText2.Height = 17;
            pollGroupBox.Controls.Add(lblTopText2);
            lblTopText3.Name = "lblTopText3";
            lblTopText3.Text = StringWorkHelper.WrapString("Description: " + pollSession.Polls[currentPoll].Description,65);
            lblTopText3.Location = new System.Drawing.Point( 10, lblTopText2.Top + lblTopText2.Height );
            lblTopText3.Width = pollGroupBox.Width;
            lblTopText3.TextAlign = ContentAlignment.MiddleLeft;
            lblTopText3.ForeColor = Color.Green;
            lblTopText3.Font = new Font("Times New Roman", 11);
            lblTopText3.AutoSize = true;
            pollGroupBox.Controls.Add(lblTopText3);

            int index = 0;
            foreach (Choice curChoice in pollSession.Polls[currentPoll].Choices)
            {
                index++;

                RadioButton radioButton = new RadioButton();
                radioButton.Name = index.ToString();
                radioButton.Text = index.ToString() + ". " + curChoice.choice;
                radioButton.Location = new System.Drawing.Point(10, lblTopText3.Top + lblTopText3.Height + 20 * (index - 1));
                radioButton.CheckedChanged += delegate(Object sender2, EventArgs e2)
                {
                    selectedChoice = Convert.ToInt32(((RadioButton)sender2).Name) - 1;
                };
                pollGroupBox.Controls.Add(radioButton);

                if ( index == 1 )
                    radioButton.Focus();
            }

            TextBox customChoiceBox = new TextBox();
            if (!(pollSession.TestMode) && (pollSession.Polls[currentPoll].CustomChoiceEnabled))
            {
                index++;
                RadioButton radioButton = new RadioButton();
                radioButton.Name = index.ToString();
                radioButton.Text = index.ToString() + ". " + "Other choice:";
                radioButton.Location = new System.Drawing.Point(10, lblTopText3.Top + lblTopText3.Height + 20 * (index - 1));
                radioButton.CheckedChanged += delegate(Object sender2, EventArgs e2)
                {
                    selectedChoice = Convert.ToInt32(((RadioButton)sender2).Name) - 1;
                };
                customChoiceBox.Name = "customChoiceBox";
                customChoiceBox.Text = String.Empty;
                customChoiceBox.Location = new System.Drawing.Point(radioButton.Left + radioButton.Width + 10, lblTopText3.Top + lblTopText3.Height + 20 * (index - 1));
                customChoiceBox.Width = 200;
                pollGroupBox.Controls.Add(radioButton);
                pollGroupBox.Controls.Add(customChoiceBox);
            }

            Button btnSubmit = new Button();
            btnSubmit.Name = "btnSubmit";
            btnSubmit.Text = "Next";
            btnSubmit.TextAlign = ContentAlignment.MiddleLeft;
            btnSubmit.Image = Ilsrep.PollApplication.PollClientGUI.Properties.Resources.bullet_go;
            btnSubmit.ImageAlign = ContentAlignment.MiddleRight;
            btnSubmit.Padding = new Padding(11, 0, 11, 0);
            btnSubmit.Location = new System.Drawing.Point(this.pollGroupBox.Width - btnSubmit.Width-10, lblTopText3.Top + lblTopText.Height + 20 * (index + 1));
            btnSubmit.Click += delegate
            {
                if (selectedChoice == -1)
                {
                    MessageBox.Show("Select a choice", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                
                if (!(pollSession.TestMode) && (pollSession.Polls[currentPoll-1].CustomChoiceEnabled))
                {
                    customChoice = customChoiceBox.Text;
                }

                ProcessResult();
                if (currentPoll < pollSession.Polls.Count)
                {
                    GetPoll();
                }
                else
                {
                    ProcessResults();
                }
            };
            pollGroupBox.Controls.Add(btnSubmit);
            this.AcceptButton = btnSubmit;

            currentPoll++;
        }

        private void ProcessResult()
        {
            PollResult result = new PollResult();
            result.questionId = pollSession.Polls[currentPoll - 1].Id;
            
            if (selectedChoice == pollSession.Polls[currentPoll - 1].Choices.Count())
            {
                result.answerId = -1;
                result.customChoice = customChoice;
            }
            else
            {
                result.answerId = pollSession.Polls[currentPoll - 1].Choices[selectedChoice].Id;
            }
            
            result.userName = PollClientGUI.userName;
            resultsList.results.Add(result);
        }

        private void ProcessResults()
        {
            PollPacket sendPacket = new PollPacket();
            sendPacket.request = new Request();
            sendPacket.request.type = Request.SAVE_RESULT;
            sendPacket.resultsList = resultsList;
            sendPacket.resultsList.userName = PollClientGUI.userName;
            sendPacket.resultsList.pollsessionId = pollSession.Id;

            PollClientGUI.ReceivePollPacket(sendPacket);

            pollGroupBox.Controls.Clear();
            Label lblTopText = new Label();
            lblTopText.AutoSize = true;
            lblTopText.Name = "lblTopText";
            lblTopText.Location = new System.Drawing.Point(10, 15);
            lblTopText.ForeColor = Color.Brown;
            lblTopText.Font = new Font("Times New Roman", 11, FontStyle.Bold);
            lblTopText.Text = "PollSession: " + pollSession.Name;
            pollGroupBox.Controls.Add(lblTopText);

            Label lblTopTextResults = new Label();
            lblTopTextResults.AutoSize = true;
            lblTopTextResults.Name = "lblTopTextResults";
            lblTopTextResults.Location = new System.Drawing.Point(10, lblTopText.Top + lblTopText.Height + 10);
            lblTopTextResults.Text = "Here are your PollSession results: " + Environment.NewLine;
            pollGroupBox.Controls.Add(lblTopTextResults);

            Label lblTestResults = new Label();
            lblTestResults.Text = String.Empty;
            float correctAnswers = 0;
            int index = 0;
            foreach (PollResult curResult in resultsList.results)
            {
                index++;

                int pollIndex = 0;
                foreach (Poll curPoll in pollSession.Polls)
                {
                    if (curPoll.Id == curResult.questionId)
                        break;
                    pollIndex++;
                }

                int choiceIndex = 0;
                foreach (Choice curChoice in pollSession.Polls[pollIndex].Choices)
                {
                    if (curChoice.Id == curResult.answerId)
                        break;
                    choiceIndex++;
                }

                if (curResult.answerId == -1)
                {
                    lblTopTextResults.Text += index + ". " + pollSession.Polls[pollIndex].Name + ": " + curResult.customChoice + Environment.NewLine;
                }
                else
                {
                    lblTopTextResults.Text += index + ". " + pollSession.Polls[pollIndex].Name + ": " + pollSession.Polls[pollIndex].Choices[choiceIndex].choice + Environment.NewLine;
                }

                if (pollSession.TestMode)
                {
                    if (choiceIndex == pollSession.Polls[pollIndex].CorrectChoiceID)
                        correctAnswers++;
                }
            }

            if (pollSession.TestMode)
            {
                double userScore = correctAnswers / pollSession.Polls.Count;
                lblTestResults.Text += "Your score: " + Convert.ToInt32(userScore * 100) + "%";

                if (userScore >= pollSession.MinScore)
                {
                    lblTestResults.Text += Environment.NewLine + "Congratulations!!! You PASSED the test";
                    lblTestResults.ForeColor = Color.Green;
                }
                else
                {
                    lblTestResults.Text += Environment.NewLine + "Sorry, try again... you FAILED";
                    lblTestResults.ForeColor = Color.Red;
                }

                lblTestResults.AutoSize = true;
                lblTestResults.Name = "lblTestResults";
                lblTestResults.Location = new System.Drawing.Point(10, lblTopTextResults.Top + lblTopTextResults.Height + 10);
                pollGroupBox.Controls.Add(lblTestResults);
            }

            Button btnSubmit = new Button();
            int top = ((pollSession.TestMode) ? lblTestResults.Top : lblTopTextResults.Top) + ((pollSession.TestMode) ? lblTestResults.Height : lblTopTextResults.Height);
            btnSubmit.Location = new System.Drawing.Point(10, top + 10);
            btnSubmit.Name = "btnSubmit";
            btnSubmit.Text = "OK";
            btnSubmit.TextAlign = ContentAlignment.MiddleRight;
            btnSubmit.Image = Ilsrep.PollApplication.PollClientGUI.Properties.Resources.tick;
            btnSubmit.ImageAlign = ContentAlignment.MiddleLeft;
            btnSubmit.Padding = new Padding(15, 0, 15, 0);
            btnSubmit.Click += new EventHandler(clientPage_Enter);
            pollGroupBox.Controls.Add(btnSubmit);
            this.AcceptButton = btnSubmit;

            resultsList.results.Clear();
            currentPoll = -1;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            // Verification of filling of pollsession's fields
            foreach (PollSession curPollSession in pollSessionsList)
            {
                try
                {
                    if (curPollSession.Polls.Count == 0)
                        throw new Exception("PollSession \"" + curPollSession.Name + "\" must have polls");

                    foreach (Poll curPoll in curPollSession.Polls)
                    {
                        if (curPoll.Choices.Count == 0)
                            throw new Exception("PollSession \"" + curPollSession.Name + "\" -> Poll \"" + curPoll.Name + "\": poll must have choices");
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // ----- for synchronize with server -----
            foreach (PollSession curPollSession in pollSessionsList)
            {
                foreach (Poll curPoll in curPollSession.Polls)
                {
                    curPoll.CorrectChoiceID += 1;
                }
            }

            history.Exclude();

            // Save all new
            foreach (PollSession curPollSession in pollSessionsList)
            {
                if (curPollSession.Id < 0)
                {
                    PollPacket pollPacket = new PollPacket();
                    pollPacket.request = new Request();
                    pollPacket.request.type = Request.CREATE_POLLSESSION;
                    pollPacket.pollSession = curPollSession;
                    PollClientGUI.ReceivePollPacket(pollPacket);
                }
            }

            // Save all edited
            foreach (int idEdited in history.edited)
            {
                PollPacket pollPacket = new PollPacket();
                pollPacket.request = new Request();
                pollPacket.request.id = idEdited.ToString();

                // Find needed PollSession in list
                foreach (PollSession curPollSession in pollSessionsList)
                {
                    if (curPollSession.Id == idEdited)
                    {
                        pollPacket.pollSession = curPollSession;
                        break;
                    }
                }

                pollPacket.request.type = Request.EDIT_POLLSESSION;
                PollClientGUI.ReceivePollPacket(pollPacket);
            }

            // Remove all deleted
            foreach (int idDeleted in history.deleted)
            {
                PollPacket pollPacket = new PollPacket();
                pollPacket.request = new Request();
                pollPacket.request.id = idDeleted.ToString();
                pollPacket.request.type = Request.REMOVE_POLLSESSION;
                PollClientGUI.ReceivePollPacket(pollPacket);
            }

            MessageBox.Show("Changes successfully sent to server", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            GetPollSessions();
            history.Clear();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Changes has been canceled", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            GetPollSessions();
            history.Clear();
        }

        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PollClientGUI.isAuthorized = false;
            PollClientGUI.isLogOut = true;
            Close();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsForm settingsForm = new SettingsForm();
            settingsForm.ShowDialog();
        }

        private void editorPage_Enter(object sender, EventArgs e)
        {
            RefreshEditorList();
        }

        private void pollSessionsListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            pollSession = pollSessionsList[pollSessionsListBox1.SelectedIndex];
        }

        private void submitButton_Click(object sender, EventArgs e)
        {
            GetPoll();
            submitButton.Enabled = false;
            pollSessionsListBox1.Enabled = false;
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            Pen pen = new Pen(ColorTranslator.FromHtml("#d8d2bd"), 1);
            graphics.DrawLine(pen, 0, 24, 618, 24);
            pen.Color = Color.White;
            graphics.DrawLine(pen, 0, 25, 618, 25);
        }

        private void editorPage_Leave(object sender, EventArgs e)
        {
            GetPollSessions();
            history.Clear();
        }
    }
}