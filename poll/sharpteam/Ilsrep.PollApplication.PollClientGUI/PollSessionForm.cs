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
    public partial class PollSessionForm : Form
    {
        public PollSessionForm()
        {
            InitializeComponent();
        }

        private void isTestMode_CheckedChanged(object sender, EventArgs e)
        {
            if (isTestMode.Checked)
            {
                minScoreField.Enabled = true;
            }
            else
            {
                minScoreField.Enabled = false;
            }
        }
    }
}
