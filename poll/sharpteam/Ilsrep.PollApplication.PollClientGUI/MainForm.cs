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
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            infoLabel.Text += PollClientGUI.userName;
        }

        private void pollClientButton_Click(object sender, EventArgs e)
        {
            PollClientForm pollClientForm = new PollClientForm();
            pollClientForm.ShowDialog( this );
        }

        private void pollEditorButton_Click(object sender, EventArgs e)
        {
            PollEditorForm pollEditorForm = new PollEditorForm();
            pollEditorForm.ShowDialog();
        }

        private void pollStatisticsButton_Click(object sender, EventArgs e)
        {
            StatisticsForm_PollSessions pollStatisticsForm = new StatisticsForm_PollSessions();
            pollStatisticsForm.ShowDialog();
        }
    }
}
