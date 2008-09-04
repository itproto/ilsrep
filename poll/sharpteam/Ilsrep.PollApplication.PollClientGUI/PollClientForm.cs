using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ilsrep.Common;
using Ilsrep.PollApplication.Communication;
using Ilsrep.PollApplication.Model;

namespace Ilsrep.PollApplication.PollClientGUI
{
    public partial class PollClientForm : Form
    {
        private int selectedPollSession = -1;

        public PollClientForm()
        {
            InitializeComponent();
        }

        private void PollClientForm_Load( object sender, EventArgs e )
        {
            if ( !PollClientGUI.client.isConnected )
            {
                Close();
            }

            SelectPollSession();
        }

        private void SelectPollSession()
        {
            PollPacket pollPacket = new PollPacket();
            pollPacket.request = new Request();
            pollPacket.request.type = Request.GET_LIST;
            pollPacket = PollClientGUI.ReceivePollPacket( pollPacket );

            lblQuestion = new Label();
            lblQuestion.AutoSize = true;
            lblQuestion.Location = new System.Drawing.Point( 10, 10 );
            lblQuestion.Text = "Please choose poll session:";
            this.Controls.Add( lblQuestion );

            int index = 0;
            foreach ( Item item in pollPacket.pollSessionList.items )
            {
                RadioButton radioButton = new RadioButton();

                radioButton = new RadioButton();
                radioButton.Name = item.id;
                radioButton.Text = item.name;
                radioButton.Location = new System.Drawing.Point( 15, 30+10*index );
                radioButton.AutoSize = true;
                radioButton.CheckedChanged += delegate( Object sender2, EventArgs e2 )
                {
                    selectedPollSession = ((RadioButton)sender2).Name;
                };

                this.Controls.Add( radioButton );
                ++index;
            }

            Button button = new Button();
            button.Name = "submit";
            button.Text = "Select";
            button.Location = new System.Drawing.Point( 10, 50+10*pollPacket.pollSessionList.items.Count );
            button.AutoSize = true;
            button.Click += delegate
            {
                ((Button)button).Enabled = false;
                ProcessPollSession();
            };
            this.Controls.Add( button );
        }

        private void ProcessPollSession()
        {
            PollPacket pollPacket = new PollPacket();
            pollPacket.request = new Request();
            pollPacket.request.type = Request.GET_POLLSESSION;
            pollPacket.request.id = selectedPollSession.ToString();
            pollPacket = PollClientGUI.ReceivePollPacket( pollPacket );

            this.Controls.Clear();

            ResultsList resultsList = new ResultsList();
/*            foreach ( Poll poll in pollPacket.pollSession.polls )
            {
                resultsList.results.Add( AskQuestion(poll, pollPacket.pollSession) );
            }*/

        }

        private PollResult AskQuestion( Poll poll, PollSession pollSession )
        {

        }
    }
}
