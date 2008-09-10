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
    /// Choice Form
    /// </summary>
    public partial class ChoiceForm : Form
    {
        /// <summary>
        /// Initialize Form
        /// </summary>
        public ChoiceForm()
        {
            InitializeComponent();

            // Fill choiceField
            if (PollForm.choice != null)
                choiceField.Text = PollForm.choice.choice;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">EventArgs e</param>
        private void submitButton_Click(object sender, EventArgs e)
        {
            // Check if choiceField is empty
            if (choiceField.Text == String.Empty)
            {
                MessageBox.Show("Choice can't be empty", "Error");
            }
            else
            {
                if (PollForm.choice == null)
                {
                    PollForm.choice = new Choice();
                    PollForm.choice.id = 0;
                }

                // Fill choiceField
                PollForm.choice.choice = choiceField.Text;
                Close();
            }
        }

        private void ChoiceForm_Load(object sender, EventArgs e)
        {
            int width = Screen.PrimaryScreen.WorkingArea.Width / 2 - this.Width / 2;
            int height = Screen.PrimaryScreen.WorkingArea.Height / 2 - this.Height / 2;
            this.Location = new Point(width, height);
        }
    }
}
