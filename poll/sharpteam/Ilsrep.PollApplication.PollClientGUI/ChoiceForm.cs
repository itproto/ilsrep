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
    public partial class ChoiceForm : Form
    {
        public ChoiceForm()
        {
            InitializeComponent();

            if (PollForm.choice != null)
                choiceField.Text = PollForm.choice.choice;
        }

        private void submitButton_Click(object sender, EventArgs e)
        {
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

                PollForm.choice.choice = choiceField.Text;
                Close();
            }
        }
    }
}
