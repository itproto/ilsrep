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
        /// Survey that will be filled
        /// </summary>
        public static Survey survey = new Survey();
        /// List of Surveys names
        /// </summary>
        private List<Survey> surveysList = new List<Survey>();
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
        /// Generate new negative Survey id
        /// </summary>
        private IDGenerator idGenerator = new IDGenerator();

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Text += " [" + PollClientGUI.userName + "]";
            GetSurveys();
            history.Clear();
        }

        /// <summary>
        /// Get survey list and show them in the list box
        /// </summary>
        private void GetSurveys()
        {
            // Get list of surveys
            List<Item> surveyList = PollClientGUI.pollService.GetSurveys();

            // Get surveysList
            surveysList.Clear();
            foreach (Item item in surveyList)
            {
                Survey survey = PollClientGUI.pollService.GetSurvey(Convert.ToInt32(item.id));

                // Modify correctChoices
                foreach (Poll curPoll in survey.Polls)
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

                surveysList.Add(survey);
            }
            RefreshEditorList();
        }

        /*
        private void GetSurveys_OLD()
        {
            // Get list of surveys
            PollPacket pollPacket = new PollPacket();
            pollPacket.request = new Request();
            pollPacket.request.type = Request.GET_LIST;
            pollPacket = PollClientGUI.ReceivePollPacket(pollPacket);

            if (pollPacket == null)
                return;

            // Get surveysList
            surveysList.Clear();
            foreach (Item item in pollPacket.surveyList.items)
            {
                pollPacket = new PollPacket();
                pollPacket.request = new Request();
                pollPacket.request.type = Request.GET_SURVEY;
                pollPacket.request.id = item.id;
                pollPacket = PollClientGUI.ReceivePollPacket(pollPacket);

                // Modify correctChoices
                foreach (Poll curPoll in pollPacket.survey.Polls)
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

                surveysList.Add(pollPacket.survey);
            }
            RefreshEditorList();
        }
        */

        private void RefreshEditorList()
        {
            // Add Surveys from list to surveysListBox in Editor
            surveysListBox.Items.Clear();

            int index = 0;
            foreach (Survey curSurvey in surveysList)
            {
                index++;
                surveysListBox.Items.Add(index + ". " + curSurvey.Name);
            }

            if (surveysListBox.Items.Count == 0)
            {
                MessageBox.Show("No surveys in DB...", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                surveysListBox.SelectedIndex = -1;
                removeButton.Enabled = false;
                propertyGrid.SelectedObject = null;
            }
            else
            {
                surveysListBox.SelectedIndex = 0;
                removeButton.Enabled = true;
            }
        }

        /// <summary>
        /// Function creates new Survey
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        private void createButton_Click( object sender, EventArgs e )
        {
            Survey survey = new Survey();
            int newId = idGenerator.id;
            survey.Name = "newSurvey" + Math.Abs(newId).ToString();
            survey.Id = newId;
            surveysList.Add(survey);
            propertyGrid.SelectedObject = surveysList[surveysList.Count - 1];
            RefreshEditorList();
            surveysListBox.SelectedIndex = surveysList.Count - 1;
        }

        /// <summary>
        /// Function send request to server to remove selected Survey
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        private void removeButton_Click(object sender, EventArgs e)
        {
            // Ask user confirmation
            DialogResult userChoice = new DialogResult();
            userChoice = MessageBox.Show("Do you really want to remove survey \"" + surveysList[surveysListBox.SelectedIndex].Name + "\"?", "Remove Survey?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (userChoice == DialogResult.Yes)
            {
                if (surveysList[surveysListBox.SelectedIndex].Id != -1)
                {
                    history.AddToDeleted(surveysList[surveysListBox.SelectedIndex].Id);
                }
                surveysList.Remove(surveysList[surveysListBox.SelectedIndex]);
                RefreshEditorList();
            }
        }

        /// <summary>
        /// Change selectedSurvey if SelectedIndexChanged
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        private void SelectedIndexChanged(object sender, EventArgs e)
        {
            propertyGrid.SelectedObject = surveysList[surveysListBox.SelectedIndex];
            if (surveysList[surveysListBox.SelectedIndex].Id >= 0)
            {
                history.AddToEdited(surveysList[surveysListBox.SelectedIndex].Id);
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
            surveysListBox1.Items.Clear();
            pollGroupBox.Controls.Clear();
            foreach (Survey curSurvey in surveysList)
            {
                surveysListBox1.Items.Add(curSurvey.Name);
            }

            if (surveysList.Count == 0)
            {
                submitButton.Enabled = false;
            }
            else
            {
                surveysListBox1.SelectedIndex = 0;
                submitButton.Enabled = true;
                surveysListBox1.Enabled = true;
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
            lblTopText2.Text = "Name: " + survey.Polls[currentPoll].Name;
            lblTopText2.Location = new System.Drawing.Point( 10, lblTopText.Top + lblTopText.Height );
            lblTopText2.Width = pollGroupBox.Width;
            lblTopText2.TextAlign = ContentAlignment.MiddleLeft;
            lblTopText2.ForeColor = Color.Green;
            lblTopText2.Font = new Font("Times New Roman", 11);
            lblTopText2.Height = 17;
            pollGroupBox.Controls.Add(lblTopText2);
            lblTopText3.Name = "lblTopText3";
            lblTopText3.Text = StringWorkHelper.WrapString("Description: " + survey.Polls[currentPoll].Description,65);
            lblTopText3.Location = new System.Drawing.Point( 10, lblTopText2.Top + lblTopText2.Height );
            lblTopText3.Width = pollGroupBox.Width;
            lblTopText3.TextAlign = ContentAlignment.MiddleLeft;
            lblTopText3.ForeColor = Color.Green;
            lblTopText3.Font = new Font("Times New Roman", 11);
            lblTopText3.AutoSize = true;
            pollGroupBox.Controls.Add(lblTopText3);

            int index = 0;
            foreach (Choice curChoice in survey.Polls[currentPoll].Choices)
            {
                index++;

                RadioButton radioButton = new RadioButton();
                radioButton.Name = index.ToString();
                radioButton.Text = index.ToString() + ". " + curChoice.choice;
                radioButton.Width = 300;
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
            if (!(survey.TestMode) && (survey.Polls[currentPoll].CustomChoiceEnabled))
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
                
                if (!(survey.TestMode) && (survey.Polls[currentPoll-1].CustomChoiceEnabled))
                {
                    customChoice = customChoiceBox.Text;
                }

                ProcessResult();
                if (currentPoll < survey.Polls.Count)
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
            result.questionId = survey.Polls[currentPoll - 1].Id;
            
            if (selectedChoice == survey.Polls[currentPoll - 1].Choices.Count())
            {
                result.answerId = -1;
                result.customChoice = customChoice;
            }
            else
            {
                result.answerId = survey.Polls[currentPoll - 1].Choices[selectedChoice].Id;
            }
            
            result.userName = PollClientGUI.userName;
            resultsList.results.Add(result);
        }
        
        private void ProcessResults()
        {
            //  OLD   <---
            /*PollPacket sendPacket = new PollPacket();
            sendPacket.request = new Request();
            sendPacket.request.type = Request.SAVE_RESULT;
            sendPacket.resultsList = resultsList;
            sendPacket.resultsList.userName = PollClientGUI.userName;
            sendPacket.resultsList.surveyId = survey.Id;
            PollClientGUI.ReceivePollPacket(sendPacket);*/

            resultsList.userName = PollClientGUI.userName;
            resultsList.surveyId = survey.Id;
            PollClientGUI.pollService.SaveSurveyResult(resultsList);

            pollGroupBox.Controls.Clear();
            Label lblTopText = new Label();
            lblTopText.AutoSize = true;
            lblTopText.Name = "lblTopText";
            lblTopText.Location = new System.Drawing.Point(10, 15);
            lblTopText.ForeColor = Color.Brown;
            lblTopText.Font = new Font("Times New Roman", 11, FontStyle.Bold);
            lblTopText.Text = "Survey: " + survey.Name;
            pollGroupBox.Controls.Add(lblTopText);

            Label lblTopTextResults = new Label();
            lblTopTextResults.AutoSize = true;
            lblTopTextResults.Name = "lblTopTextResults";
            lblTopTextResults.Location = new System.Drawing.Point(10, lblTopText.Top + lblTopText.Height + 10);
            lblTopTextResults.Text = "Here are your Survey results: " + Environment.NewLine;
            pollGroupBox.Controls.Add(lblTopTextResults);

            Label lblTestResults = new Label();
            lblTestResults.Text = String.Empty;
            float correctAnswers = 0;
            int index = 0;
            foreach (PollResult curResult in resultsList.results)
            {
                index++;

                int pollIndex = 0;
                foreach (Poll curPoll in survey.Polls)
                {
                    if (curPoll.Id == curResult.questionId)
                        break;
                    pollIndex++;
                }

                int choiceIndex = 0;
                foreach (Choice curChoice in survey.Polls[pollIndex].Choices)
                {
                    if (curChoice.Id == curResult.answerId)
                        break;
                    choiceIndex++;
                }

                if (curResult.answerId == -1)
                {
                    lblTopTextResults.Text += index + ". " + survey.Polls[pollIndex].Name + ": " + curResult.customChoice + Environment.NewLine;
                }
                else
                {
                    lblTopTextResults.Text += index + ". " + survey.Polls[pollIndex].Name + ": " + survey.Polls[pollIndex].Choices[choiceIndex].choice + Environment.NewLine;
                }

                if (survey.TestMode)
                {
                    if (choiceIndex == survey.Polls[pollIndex].CorrectChoiceID)
                        correctAnswers++;
                }
            }

            if (survey.TestMode)
            {
                double userScore = correctAnswers / survey.Polls.Count;
                lblTestResults.Text += "Your score: " + Convert.ToInt32(userScore * 100) + "%";

                if (userScore >= survey.MinScore)
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
            int top = ((survey.TestMode) ? lblTestResults.Top : lblTopTextResults.Top) + ((survey.TestMode) ? lblTestResults.Height : lblTopTextResults.Height);
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
            // Verification of filling of survey's fields
            foreach (Survey curSurvey in surveysList)
            {
                try
                {
                    if (curSurvey.Polls.Count == 0)
                        throw new Exception("Survey \"" + curSurvey.Name + "\" must have polls");

                    foreach (Poll curPoll in curSurvey.Polls)
                    {
                        if (curPoll.Choices.Count == 0)
                            throw new Exception("Survey \"" + curSurvey.Name + "\" -> Poll \"" + curPoll.Name + "\": poll must have choices");
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // ----- for synchronize with BD -----
            foreach (Survey curSurvey in surveysList)
            {
                foreach (Poll curPoll in curSurvey.Polls)
                {
                    curPoll.CorrectChoiceID += 1;
                }
            }

            history.Exclude();

            // Save all new
            foreach (Survey curSurvey in surveysList)
            {
                if (curSurvey.Id < 0)
                {
                    PollClientGUI.pollService.CreateSurvey(curSurvey);
                }
            }

            /*  OLD   <---
            foreach (Survey curSurvey in surveysList)
            {
                if (curSurvey.Id < 0)
                {
                    PollPacket pollPacket = new PollPacket();
                    pollPacket.request = new Request();
                    pollPacket.request.type = Request.CREATE_SURVEY;
                    pollPacket.survey = curSurvey;
                    PollClientGUI.client.Send(PollSerializator.SerializePacket(pollPacket));
                }
            }
            */

            // Save all edited
            foreach (int idEdited in history.edited)
            {
                Survey survey = new Survey();

                // Find needed Survey in list
                foreach (Survey curSurvey in surveysList)
                {
                    if (curSurvey.Id == idEdited)
                    {
                        survey = curSurvey;
                        break;
                    }
                }

                PollClientGUI.pollService.EditSurvey(survey);
            }

            /*   OLD  <---
            foreach (int idEdited in history.edited)
            {
                PollPacket pollPacket = new PollPacket();
                pollPacket.request = new Request();
                pollPacket.request.id = idEdited.ToString();

                // Find needed Survey in list
                foreach (Survey curSurvey in surveysList)
                {
                    if (curSurvey.Id == idEdited)
                    {
                        pollPacket.survey = curSurvey;
                        break;
                    }
                }

                pollPacket.request.type = Request.EDIT_SURVEY;
                PollClientGUI.client.Send(PollSerializator.SerializePacket(pollPacket));
            } 
            */

            // Remove all deleted
            foreach (int idDeleted in history.deleted)
            {
                PollClientGUI.pollService.RemoveSurvey(idDeleted);
            }

            /*   OLD  <---
            foreach (int idDeleted in history.deleted)
            {
                PollPacket pollPacket = new PollPacket();
                pollPacket.request = new Request();
                pollPacket.request.id = idDeleted.ToString();
                pollPacket.request.type = Request.REMOVE_SURVEY;
                PollClientGUI.client.Send(PollSerializator.SerializePacket(pollPacket));
            }
            */

            MessageBox.Show("Changes successfully sent to server", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            GetSurveys();
            history.Clear();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Changes has been canceled", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            GetSurveys();
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

        private void surveysListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            survey = surveysList[surveysListBox1.SelectedIndex];
        }

        private void submitButton_Click(object sender, EventArgs e)
        {
            GetPoll();
            submitButton.Enabled = false;
            surveysListBox1.Enabled = false;
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
            GetSurveys();
            history.Clear();
        }
    }
}
