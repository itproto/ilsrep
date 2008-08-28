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
    public partial class PollForm : Form
    {
        public static Choice choice = new Choice();
        public static List<Choice> choices = new List<Choice>();

        public PollForm()
        {
            InitializeComponent();

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

        private void addButton_Click(object sender, EventArgs e)
        {
            choice = null;
            ChoiceForm choiceForm = new ChoiceForm();
            choiceForm.ShowDialog();

            // Save changes
            if (choice != null)
            {
                choices.Add(choice);
            }

            RefreshChoicesList();
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            if (choice == null)
            {
                MessageBox.Show("Please, choose choice to edit", "Error");
            }
            else
            {
                ChoiceForm choiceForm = new ChoiceForm();
                choiceForm.ShowDialog();

                // Save changes
                choices[choicesListBox.SelectedIndex] = choice;

                RefreshChoicesList();
            }
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            if (choice == null)
            {
                MessageBox.Show("Please, choose choice to remove", "Error");
            }
            else
            {
                DialogResult userChoice = new DialogResult();
                userChoice = MessageBox.Show(null, "Do you really want to remove choice \"" + choice.choice + "\"?", "Remove choice?", MessageBoxButtons.YesNo);
                if (userChoice == DialogResult.Yes)
                {
                    choices.RemoveAt(choicesListBox.SelectedIndex);
                    RefreshChoicesList();
                }
            }
        }

        private void submitButton_Click(object sender, EventArgs e)
        {
            if (choices.Count == 0)
            {
                MessageBox.Show("No choices in Poll...", "Info");
                addButton_Click(null, EventArgs.Empty);
                return;
            }

            if (nameField.Text == String.Empty)
            {
                MessageBox.Show("Error: Name can't be empty");
            }
            else
            {
                if (PollSessionForm.isTestModeEnabled)
                {
                    if (choice == null)
                    {
                        MessageBox.Show("Please, select correct choice", "Error");
                        return;
                    }
                    else
                    {
                        if (PollEditorForm.pollSession == null)
                        {
                            PollSessionForm.poll = new Poll();
                            PollSessionForm.poll.correctChoiceID = choicesListBox.SelectedIndex + 1;
                        }
                        else
                        {
                            PollSessionForm.poll.correctChoiceID = choices[choicesListBox.SelectedIndex].id;
                        }
                    }
                }

                if (PollSessionForm.poll == null)
                    PollSessionForm.poll = new Poll();

                PollSessionForm.poll.name = nameField.Text;
                PollSessionForm.poll.description = descriptionField.Text;
                PollSessionForm.poll.customChoiceEnabled = isCustomChoiceEnabled.Checked;

                PollSessionForm.poll.choices.Clear();
                foreach (Choice curChoice in choices)
                {
                    PollSessionForm.poll.choices.Add(curChoice);
                }
                choices.Clear();
                Close();
            }
        }

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
