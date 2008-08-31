﻿using System;
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
        public static Choice choice = new Choice();
        /// <summary>
        /// list of choices, that will be filled
        /// </summary>
        public static List<Choice> choices = new List<Choice>();

        /// <summary>
        /// Initialize Form
        /// </summary>
        public PollForm()
        {
            InitializeComponent();

            // Fill fields
            if (PollSessionForm.poll != null)
            {
                foreach (Choice curChoice in PollSessionForm.poll.choices)
                {
                    choices.Add(curChoice);
                }

                nameField.Text = PollSessionForm.poll.name;
                descriptionField.Text = PollSessionForm.poll.description;
                isCustomChoiceEnabled.Checked = PollSessionForm.poll.customChoiceEnabled;

                RefreshChoicesList();
            }

            if (PollSessionForm.isTestModeEnabled)
            {
                // Select correct choice in choicesListBox
                if (PollSessionForm.poll != null)
                    if (PollEditorForm.pollSession == null)
                    {
                        choicesListBox.SelectedIndex = PollSessionForm.poll.correctChoiceID - 1;
                    }
                    else
                    {
                        int selectedIndex = 0;
                        foreach (Choice curChoice in choices)
                        {
                            if (curChoice.id == PollSessionForm.poll.correctChoiceID)
                            {
                                choicesListBox.SelectedIndex = selectedIndex;
                                break;
                            }
                            selectedIndex++;
                        }
                    }
            }
            else
            {
                isCustomChoiceEnabled.Enabled = true;
            }
        }

        /// <summary>
        /// Refresh list of choices in choicesListBox
        /// </summary>
        private void RefreshChoicesList()
        {
            choicesListBox.Items.Clear();
            int index = 0;
            foreach (Choice curChoice in choices)
            {
                index++;
                choicesListBox.Items.Add(index + ". " + curChoice.choice);
            }

            choice = null;
        }

        /// <summary>
        /// Function opens ChoiceForm and then add new choice to choices list
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        private void addButton_Click(object sender, EventArgs e)
        {
            // Open ChoiceForm to fill new choice
            choice = null;
            ChoiceForm choiceForm = new ChoiceForm();
            choiceForm.ShowDialog();

            // Save changes
            if (choice != null)
                choices.Add(choice);

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
            if (choice == null)
            {
                MessageBox.Show("Please, choose choice to edit", "Error");
            }
            else
            {
                // Open ChoiceForm to change choice
                ChoiceForm choiceForm = new ChoiceForm();
                choiceForm.ShowDialog();

                // Save changes
                choices[choicesListBox.SelectedIndex] = choice;

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
            if (choice == null)
            {
                MessageBox.Show("Please, choose choice to remove", "Error");
            }
            else
            {
                // Ask user confirmation
                DialogResult userChoice = new DialogResult();
                userChoice = MessageBox.Show(null, "Do you really want to remove choice \"" + choice.choice + "\"?", "Remove choice?", MessageBoxButtons.YesNo);
                if (userChoice == DialogResult.Yes)
                {
                    choices.RemoveAt(choicesListBox.SelectedIndex);
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
            if (choices.Count == 0)
            {
                MessageBox.Show("No choices in Poll...", "Info");
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
                if (PollSessionForm.poll == null)
                    PollSessionForm.poll = new Poll();

                // Save selected choice to correctChoiceId
                if (PollSessionForm.isTestModeEnabled)
                {
                    if (choice == null)
                    {
                        MessageBox.Show("Please, select correct choice", "Error");
                        return;
                    }
                    else
                    {
                        PollSessionForm.poll.correctChoiceID = choicesListBox.SelectedIndex + 1;
                    }
                }

                // Fill name, description, customChoiceEnabled fields
                PollSessionForm.poll.name = nameField.Text;
                PollSessionForm.poll.description = descriptionField.Text;
                PollSessionForm.poll.customChoiceEnabled = isCustomChoiceEnabled.Checked;

                // Fill choices list
                PollSessionForm.poll.choices.Clear();
                foreach (Choice curChoice in choices)
                {
                    PollSessionForm.poll.choices.Add(curChoice);
                }
                choices.Clear();
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
                choice = choices[choicesListBox.SelectedIndex];
            }
            catch (Exception exception)
            {
                choice = null;
            }
        }
    }
}