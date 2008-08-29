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
        public static Poll poll = new Poll();
        /// <summary>
        /// List of Polls that will be filled
        /// </summary>
        public static List<Poll> polls = new List<Poll>();
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

            // Fill fields
            if (PollEditorForm.pollSession != null)
            {
                // Copy polls list
                foreach (Poll curPoll in PollEditorForm.pollSession.polls)
                {
                    polls.Add(curPoll);
                }

                nameField.Text = PollEditorForm.pollSession.name;
                isTestMode.Checked = PollEditorForm.pollSession.testMode;
                if (isTestMode.Checked)
                    minScoreField.Text = PollEditorForm.pollSession.minScore.ToString();
            }

            RefreshPollsList();
        }

        /// <summary>
        /// Refresh list of Polls in pollsListBox
        /// </summary>
        private void RefreshPollsList()
        {
            pollsListBox.Items.Clear();
            int index = 0;
            foreach (Poll curPoll in polls)
            {
                index++;
                pollsListBox.Items.Add(index + ". " + curPoll.name);
            }

            poll = null;
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
                poll = polls[pollsListBox.SelectedIndex];
            }
            catch (Exception exception)
            {
                poll = null;
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
            poll = null;
            PollForm pollForm = new PollForm();
            pollForm.ShowDialog();

            // Save changes
            if (poll != null)
                polls.Add(poll);

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
            if (poll == null)
            {
                MessageBox.Show("Please, choose poll to edit");
            }
            else
            {
                // Open PollForm to change poll
                PollForm pollForm = new PollForm();
                pollForm.ShowDialog();

                // Save changes
                polls[pollsListBox.SelectedIndex] = poll;

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
            if (poll == null)
            {
                MessageBox.Show("Please, choose poll to remove");
            }
            else
            {
                // Ask user confirmation
                DialogResult userChoice = new DialogResult();
                userChoice = MessageBox.Show(null, "Do you really want to remove poll \"" + poll.name + "\"?", "Remove Poll?", MessageBoxButtons.YesNo);
                if (userChoice == DialogResult.Yes)
                {
                    polls.RemoveAt(pollsListBox.SelectedIndex);
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
            if (polls.Count == 0)
            {
                MessageBox.Show("No polls in PollSession...", "Info");
                addButton_Click(null, EventArgs.Empty);
                return;
            }

            // Check if name field is empty
            if (nameField.Text == String.Empty)
            {
                MessageBox.Show("Error: Name can't be empty");
            }
            else
            {
                if (PollEditorForm.pollSession == null)
                    PollEditorForm.pollSession = new PollSession();

                // Fill name and testMode fields
                PollEditorForm.pollSession.name = nameField.Text;
                PollEditorForm.pollSession.testMode = isTestMode.Checked;

                // Fill minScore field
                if (isTestMode.Checked)
                {
                    try
                    {
                        Double minScore = Convert.ToDouble(minScoreField.Text);
                        if (minScore < 0 || minScore > 1)
                            throw new Exception("minScore must be in interval [0,1]");
                        PollEditorForm.pollSession.minScore = minScore;
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message, "Error");
                        return;
                    }
                }

                // Fill polls list
                PollEditorForm.pollSession.polls.Clear();
                foreach (Poll curPoll in polls)
                {
                    PollEditorForm.pollSession.polls.Add(curPoll);
                }
                polls.Clear();
                Close();
            }
        }
    }
}