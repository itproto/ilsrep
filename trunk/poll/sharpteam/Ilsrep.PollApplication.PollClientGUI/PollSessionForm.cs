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
    public partial class PollSessionForm : Form
    {
        public static Poll poll = new Poll();
        public static List<Poll> polls = new List<Poll>();
        public static bool isTestModeEnabled = false;
        
        public PollSessionForm()
        {
            InitializeComponent();

            if (PollEditorForm.pollSession != null)
            {
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

        private void addButton_Click(object sender, EventArgs e)
        {
            poll = null;
            PollForm pollForm = new PollForm();
            pollForm.ShowDialog();

            // Save changes
            if (poll != null)
                polls.Add(poll);

            RefreshPollsList();
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            if (poll == null)
            {
                MessageBox.Show("Please, choose poll to edit");
            }
            else
            {
                PollForm pollForm = new PollForm();
                pollForm.ShowDialog();

                // Save changes
                polls[pollsListBox.SelectedIndex] = poll;

                RefreshPollsList();
            }
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            if (poll == null)
            {
                MessageBox.Show("Please, choose poll to remove");
            }
            else
            {
                DialogResult userChoice = new DialogResult();
                userChoice = MessageBox.Show(null, "Do you really want to remove poll \"" + poll.name + "\"?", "Remove Poll?", MessageBoxButtons.YesNo);
                if (userChoice == DialogResult.Yes)
                {
                    polls.RemoveAt(pollsListBox.SelectedIndex);
                    RefreshPollsList();
                }
            }
        }

        private void submitButton_Click(object sender, EventArgs e)
        {
            if (polls.Count == 0)
            {
                MessageBox.Show("No polls in PollSession...", "Info");
                addButton_Click(null, EventArgs.Empty);
                return;
            }

            if (nameField.Text == String.Empty)
            {
                MessageBox.Show("Error: Name can't be empty");
            }
            else
            {
                if (PollEditorForm.pollSession == null)
                    PollEditorForm.pollSession = new PollSession();

                PollEditorForm.pollSession.name = nameField.Text;
                PollEditorForm.pollSession.testMode = isTestMode.Checked;
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
                        MessageBox.Show("Error: " + exception.Message);
                        return;
                    }
                }

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