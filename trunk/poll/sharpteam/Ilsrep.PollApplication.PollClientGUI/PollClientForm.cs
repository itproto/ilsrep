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
using Ilsrep.PollApplication.Helpers;

namespace Ilsrep.PollApplication.PollClientGUI
{
    public partial class PollClientForm : Form
    {
        private int selectedPollSession = -1;
        private int currentPoll = 0;
        private Choice selectedChoice;
        private PollSession pollSession;
        private ResultsList resultsList = new ResultsList();

        public PollClientForm()
        {
            InitializeComponent();
        }

        private void PollClientForm_Load( object sender, EventArgs e )
        {
            if ( !PollClientGUI.client.isConnected )
            {
                //PollClientGUI.client.Connect( PollClientGUI.HOST, PollClientGUI.PORT );
                Close();
            }
            else
            {
                SelectPollSession();
            }
        }

        private void SelectPollSession()
        {
            PollPacket pollPacket = new PollPacket();
            pollPacket.request = new Request();
            pollPacket.request.type = Request.GET_LIST;
            pollPacket = PollClientGUI.ReceivePollPacket( pollPacket );

            Label lblQuestion = new Label();
            lblQuestion.AutoSize = true;
            lblQuestion.Location = new System.Drawing.Point( 10, 10 );
            lblQuestion.Text = "Please choose poll session:";
            this.Controls.Add( lblQuestion );

            int index = 0;
            foreach ( Item item in pollPacket.pollSessionList.items )
            {
                RadioButton radioButton = new RadioButton();
                radioButton.Name = item.id;
                radioButton.Text = item.name;
                radioButton.Location = new System.Drawing.Point( 15, 30+10*index );
                radioButton.AutoSize = true;
                radioButton.CheckedChanged += delegate( Object sender2, EventArgs e2 )
                {
                    selectedPollSession = Convert.ToInt32(((RadioButton)sender2).Name);
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
            pollSession = pollPacket.pollSession;

            AskQuestion();

/*            foreach ( Poll poll in pollPacket.pollSession.polls )
            {
                resultsList.results.Add( AskQuestion(poll, pollPacket.pollSession) );
            }*/

        }

        private void AskQuestion()
        {
            this.Controls.Clear();

            Label lblQuestion = new Label();
            lblQuestion.AutoSize = true;
            lblQuestion.Location = new System.Drawing.Point( 10, 10 );
            lblQuestion.Text = pollSession.polls[currentPoll].description;
            this.Controls.Add( lblQuestion );

            int index = 0;
            foreach ( Choice curChoice in pollSession.polls[currentPoll].choices )
            {
                RadioButton radioButton = new RadioButton();
                radioButton.Name = index.ToString();
                radioButton.Text = curChoice.choice;
                radioButton.Location = new System.Drawing.Point( 15, 30+20*index );
                radioButton.AutoSize = true;
                radioButton.CheckedChanged += delegate( Object sender2, EventArgs e2 )
                {
                    selectedChoice = pollSession.polls[currentPoll].choices[Convert.ToInt32(((RadioButton)sender2).Name)];
                };

                this.Controls.Add( radioButton );
                ++index;
            }

            Button button = new Button();
            button.Name = "submit";
            button.Text = "Select";
            button.Location = new System.Drawing.Point( 10, 50+20*pollSession.polls[currentPoll].choices.Count );
            button.AutoSize = true;
            button.Click += delegate
            {
                ((Button)button).Enabled = false;

                PollResult pollResult = new PollResult();
                pollResult.userName = PollClientGUI.userName;
                pollResult.answerId = selectedChoice.parent.id;
                pollResult.questionId = selectedChoice.id;
                resultsList.results.Add( pollResult );

                if ( currentPoll < pollSession.polls.Count - 1 )
                {
                    ++currentPoll;
                    AskQuestion();
                }
                else
                {
                    ProcessResults();
                }
            };
            this.Controls.Add( button );
        }

        private void ProcessResults()
        {
            PollPacket pollPacket = new PollPacket();
            pollPacket.request = new Request();
            pollPacket.request.type = Request.SAVE_RESULT;
            pollPacket.resultsList = resultsList;
            pollPacket.resultsList.userName = PollClientGUI.userName;
            pollPacket.resultsList.pollsessionId = pollSession.id;

            PollClientGUI.client.Send( PollSerializator.SerializePacket( pollPacket ) );

            this.Controls.Clear();
            Close();
        }
    }
}
