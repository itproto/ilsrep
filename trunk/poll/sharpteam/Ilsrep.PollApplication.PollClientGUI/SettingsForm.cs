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
    public partial class SettingsForm : Form
    {
        private string host;
        private int port;

        public SettingsForm()
        {
            InitializeComponent();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            hostTextBox.Text = PollClientGUI.settings.host;
            portTextBox.Text = PollClientGUI.settings.port.ToString();
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            // Check if correct settings
            try
            {
                host = hostTextBox.Text;
                port = Convert.ToInt32(portTextBox.Text);
                if (port < 0 || port > 65535)
                {
                    throw new Exception("Port number out of bounds");
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Eror", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Save settings
            PollClientGUI.settings.host = host;
            PollClientGUI.settings.port = port;
            PollClientGUI.settings.SaveSettings(PollClientGUI.PATH_TO_CONFIG_FILE);

            if (PollClientGUI.isAuthorized)
            {
                MessageBox.Show("New settings will be applied after user relogin", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            Close();
        }
    }
}
