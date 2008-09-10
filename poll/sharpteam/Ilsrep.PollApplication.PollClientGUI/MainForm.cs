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
            int width = Screen.PrimaryScreen.WorkingArea.Width / 2 - this.Width / 2;
            int height = Screen.PrimaryScreen.WorkingArea.Height / 2 - this.Height / 2;
            this.Location = new Point(width, height);
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
