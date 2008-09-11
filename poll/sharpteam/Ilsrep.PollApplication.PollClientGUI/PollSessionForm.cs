using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ilsrep.PollApplication.Model;

namespace Ilsrep.PollApplication.PollClientGUI
{
    /// <summary>
    /// PollSession Form
    /// </summary>
    public partial class PollSessionForm : Form
    {
        /// <summary>
        /// Poll, selected in pollsListBox
        /// </summary>
        public static Poll activePoll = new Poll();
        /// <summary>
        /// List of Polls that will be filled
        /// </summary>
        public static List<Poll> pollsList = new List<Poll>();
        /// <summary>
        /// Shows enabled status of TestMode
        /// </summary>
        public static bool isTestModeEnabled = false;
        
        /// <summary>
        /// Initialize form
        /// </summary>
        public PollSessionForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Refresh list of Polls in pollsListBox
        /// </summary>
        private void RefreshPollsList()
        {
            pollsListBox.Items.Clear();
            int index = 0;
            foreach (Poll curPoll in pollsList)
            {
                index++;
                pollsListBox.Items.Add(index + ". " + curPoll.name);
            }

            if (pollsList.Count == 0)
            {
                activePoll = null;
                return;
            }

            pollsListBox.SelectedIndex = 0;
            activePoll = pollsList[0];
        }

        /// <summary>
        /// Function enables or disables minScoreField and sets isTestModeEnabled value according to isTestMode CheckBox
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        private void isTestMode_CheckedChanged(object sender, EventArgs e)
        {
            if (isTestMode.Checked)
            {
                isTestModeEnabled = true;
                minScoreField.Enabled = true;
            }
            else
            {
                isTestModeEnabled = false;
                minScoreField.Enabled = false;
            }
        }

        /// <summary>
        /// Change poll if SelectedIndexChanged
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        private void pollsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                activePoll = pollsList[pollsListBox.SelectedIndex];
            }
            catch (Exception)
            {
                activePoll = null;
            }
        }
        
        /// <summary>
        /// Function opens PollForm and then add new poll to polls list
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        private void addButton_Click(object sender, EventArgs e)
        {
            // Open PollForm to fill new poll
            activePoll = null;
            PollForm pollForm = new PollForm();
            pollForm.ShowDialog();

            // Save changes
            if (activePoll != null)
                pollsList.Add(activePoll);

            RefreshPollsList();
        }

        /// <summary>
        /// Function opens PollForm and then save changed poll to polls list
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        private void editButton_Click(object sender, EventArgs e)
        {
            // Check if any poll selected
            if (activePoll == null)
            {
                MessageBox.Show("Please, choose poll to edit");
            }
            else
            {
                // Open PollForm to change poll
                PollForm pollForm = new PollForm();
                pollForm.ShowDialog();

                // Save changes
                pollsList[pollsListBox.SelectedIndex] = activePoll;

                RefreshPollsList();
            }
        }
        
        /// <summary>
        /// Function remove selected poll from polls list
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        private void removeButton_Click(object sender, EventArgs e)
        {
            // Check if any poll selected
            if (activePoll == null)
            {
                MessageBox.Show("Please, choose poll to remove");
            }
            else
            {
                // Ask user confirmation
                DialogResult userChoice = new DialogResult();
                userChoice = MessageBox.Show(null, "Do you really want to remove poll \"" + activePoll.name + "\"?", "Remove Poll?", MessageBoxButtons.YesNo);
                if (userChoice == DialogResult.Yes)
                {
                    pollsList.RemoveAt(pollsListBox.SelectedIndex);
                    RefreshPollsList();
                }
            }
        }

        /// <summary>
        /// Save all changes in PollEditorForm.pollSession
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void submitButton_Click(object sender, EventArgs e)
        {
            // Force poll adding if polls list is empty
            if (pollsList.Count == 0)
            {
                MessageBox.Show("No polls in PollSession...", "Info");
                return;
            }

            // Check if name field is empty
            if (nameField.Text == String.Empty)
            {
                MessageBox.Show("Error: Name can't be empty");
            }
            else
            {
                if (MainForm.pollSession == null)
                    MainForm.pollSession = new PollSession();

                // Fill name and testMode fields
                MainForm.pollSession.name = nameField.Text;
                MainForm.pollSession.testMode = isTestMode.Checked;

                // Fill minScore field
                if (isTestMode.Checked)
                {
                    try
                    {
                        Double minScore = Convert.ToDouble(minScoreField.Text);
                        if (minScore < 0 || minScore > 1)
                            throw new Exception("minScore must be in interval [0,1]");
                        MainForm.pollSession.minScore = minScore;
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message, "Error[MinScore field]");
                        return;
                    }
                }

                // Fill polls list
                MainForm.pollSession.polls.Clear();
                foreach (Poll curPoll in pollsList)
                {
                    MainForm.pollSession.polls.Add(curPoll);
                }
                Close();
            }
        }

        private void PollSessionForm_Load(object sender, EventArgs e)
        {
            int width = Screen.PrimaryScreen.WorkingArea.Width / 2 - this.Width / 2;
            int height = Screen.PrimaryScreen.WorkingArea.Height / 2 - this.Height / 2;
            this.Location = new Point(width, height);

            isTestModeEnabled = false;

            // Fill fields
            if (MainForm.pollSession != null)
            {
                // Copy polls list
                foreach (Poll curPoll in MainForm.pollSession.polls)
                {
                    pollsList.Add(curPoll);
                }

                nameField.Text = MainForm.pollSession.name;
                isTestMode.Checked = MainForm.pollSession.testMode;
                if (isTestMode.Checked)
                    minScoreField.Text = MainForm.pollSession.minScore.ToString();

                int pollIndex = 0;
                foreach (Poll curPoll in MainForm.pollSession.polls)
                {
                    // Save correct choice order number in correctChoiceId
                    int choiceIndex = 1;
                    foreach (Choice curChoice in curPoll.choices)
                    {
                        if (curChoice.id == curPoll.correctChoiceID)
                        {
                            MainForm.pollSession.polls[pollIndex].correctChoiceID = choiceIndex;
                            break;
                        }
                        choiceIndex++;
                    }
                    pollIndex++;
                }
            }

            RefreshPollsList();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void PollSessionForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            pollsList.Clear();
        }
    }
}