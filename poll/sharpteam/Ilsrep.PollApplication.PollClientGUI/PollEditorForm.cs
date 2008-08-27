using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Ilsrep.PollApplication.PollClientGUI
{
    public partial class PollEditorForm : Form
    {
        public PollEditorForm()
        {
            InitializeComponent();
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            PollSessionForm pollSessionForm = new PollSessionForm();
            pollSessionForm.ShowDialog();
        }
    }
}
