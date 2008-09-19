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
    /// Poll Form
    /// </summary>
    public partial class PollForm : Form
    {
        /// <summary>
        /// choice, selected in choicesListBox
        /// </summary>
        public static Choice activeChoice = new Choice();
        /// <summary>
        /// list of choices, that will be filled
        /// </summary>
        public static List<Choice> choicesList = new List<Choice>();

        /// <summary>
        /// Initialize Form
        /// </summary>
        public PollForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Refresh list of choices in choicesListBox
        /// </summary>
        private void RefreshChoicesList()
        {
            choicesListBox.Items.Clear();
            int index = 0;
            foreach (Choice curChoice in choicesList)
            {
                index++;
                choicesListBox.Items.Add(index + ". " + curChoice.choice);
            }

            activeChoice = null;
        }

        /// <summary>
        /// Function opens ChoiceForm and then add new choice to choices list
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        private void addButton_Click(object sender, EventArgs e)
        {
            // Open ChoiceForm to fill new choice
            activeChoice = null;
            ChoiceForm choiceForm = new ChoiceForm();
            choiceForm.ShowDialog();

            // Save changes
            if (activeChoice != null)
                choicesList.Add(activeChoice);

            RefreshChoicesList();
        }

        /// <summary>
        /// Function opens ChoiceForm and then save changed choice to choices list
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        private void editButton_Click(object sender, EventArgs e)
        {
            // Check if any choice selected
            if (activeChoice == null)
            {
                MessageBox.Show("Please, choose choice to edit", "Error");
            }
            else
            {
                // Open ChoiceForm to change choice
                ChoiceForm choiceForm = new ChoiceForm();
                choiceForm.ShowDialog();

                // Save changes
                choicesList[choicesListBox.SelectedIndex] = activeChoice;

                RefreshChoicesList();
            }
        }

        /// <summary>
        /// Function remove selected choice from choices list
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        private void removeButton_Click(object sender, EventArgs e)
        {
            // Check if any choice selected
            if (activeChoice == null)
            {
                MessageBox.Show("Please, choose choice to remove", "Error");
            }
            else
            {
                // Ask user confirmation
                DialogResult userChoice = new DialogResult();
                userChoice = MessageBox.Show(null, "Do you really want to remove choice \"" + activeChoice.choice + "\"?", "Remove choice?", MessageBoxButtons.YesNo);
                if (userChoice == DialogResult.Yes)
                {
                    choicesList.RemoveAt(choicesListBox.SelectedIndex);
                    RefreshChoicesList();
                }
            }
        }

        /// <summary>
        /// Save all changes in PollSessionForm.poll
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        private void submitButton_Click(object sender, EventArgs e)
        {
            // Force choice adding if choices list is empty
            if (choicesList.Count == 0)
            {
                MessageBox.Show("No choices in Poll...", "Info");
                return;
            }

            // Check if name field is empty
            if (nameField.Text == String.Empty)
            {
                MessageBox.Show("Error: Name can't be empty");
            }
            else
            {
                if (PollSessionForm.activePoll == null)
                    PollSessionForm.activePoll = new Poll();

                // Save selected choice to correctChoiceId
                if (PollSessionForm.isTestModeEnabled)
                {
                    if (activeChoice == null)
                    {
                        MessageBox.Show("Please, select correct choice", "Error");
                        return;
                    }
                    else
                    {
                        PollSessionForm.activePoll.correctChoiceID = choicesListBox.SelectedIndex + 1;
                    }
                }

                // Fill name, description, customChoiceEnabled fields
                PollSessionForm.activePoll.name = nameField.Text;
                PollSessionForm.activePoll.description = descriptionField.Text;
                PollSessionForm.activePoll.customChoiceEnabled = isCustomChoiceEnabled.Checked;

                // Fill choices list
                PollSessionForm.activePoll.choices.Clear();
                foreach (Choice curChoice in choicesList)
                {
                    PollSessionForm.activePoll.choices.Add(curChoice);
                }
                Close();
            }
        }

        /// <summary>
        /// Change choice if SelectedIndexChanged
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        private void choicesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                activeChoice = choicesList[choicesListBox.SelectedIndex];
            }
            catch (Exception)
            {
                activeChoice = null;
            }
        }

        private void PollForm_Load(object sender, EventArgs e)
        {
            // Fill fields
            if (PollSessionForm.activePoll != null)
            {
                foreach (Choice curChoice in PollSessionForm.activePoll.choices)
                {
                    choicesList.Add(curChoice);
                }

                nameField.Text = PollSessionForm.activePoll.name;
                descriptionField.Text = PollSessionForm.activePoll.description;
                isCustomChoiceEnabled.Checked = PollSessionForm.activePoll.customChoiceEnabled;

                RefreshChoicesList();
            }

            if (PollSessionForm.isTestModeEnabled)
            {
                // Select correct choice in choicesListBox
                if (PollSessionForm.activePoll != null)
                {
                    try
                    {
                        choicesListBox.SelectedIndex = PollSessionForm.activePoll.correctChoiceID - 1;
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            else
            {
                isCustomChoiceEnabled.Enabled = true;
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void PollForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            choicesList.Clear();
        }
    }
}
