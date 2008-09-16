namespace Ilsrep.PollApplication.PollClientGUI
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainTabControl = new System.Windows.Forms.TabControl();
            this.clientPage = new System.Windows.Forms.TabPage();
            this.editorPage = new System.Windows.Forms.TabPage();
            this.removeButton = new System.Windows.Forms.Button();
            this.editButton = new System.Windows.Forms.Button();
            this.createButton = new System.Windows.Forms.Button();
            this.pollSessionsListBox = new System.Windows.Forms.ListBox();
            this.statisticsPage = new System.Windows.Forms.TabPage();
            this.mainTabControl.SuspendLayout();
            this.editorPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainTabControl
            // 
            this.mainTabControl.Controls.Add( this.clientPage );
            this.mainTabControl.Controls.Add( this.editorPage );
            this.mainTabControl.Location = new System.Drawing.Point( 12, 12 );
            this.mainTabControl.Name = "mainTabControl";
            this.mainTabControl.SelectedIndex = 0;
            this.mainTabControl.Size = new System.Drawing.Size( 600, 426 );
            this.mainTabControl.TabIndex = 4;
            // 
            // clientPage
            // 
            this.clientPage.Location = new System.Drawing.Point( 4, 22 );
            this.clientPage.Name = "clientPage";
            this.clientPage.Padding = new System.Windows.Forms.Padding( 3 );
            this.clientPage.Size = new System.Drawing.Size( 592, 400 );
            this.clientPage.TabIndex = 0;
            this.clientPage.Text = "PollClient";
            this.clientPage.UseVisualStyleBackColor = true;
            this.clientPage.Enter += new System.EventHandler( this.clientPage_Enter );
            // 
            // editorPage
            // 
            this.editorPage.Controls.Add( this.removeButton );
            this.editorPage.Controls.Add( this.editButton );
            this.editorPage.Controls.Add( this.createButton );
            this.editorPage.Controls.Add( this.pollSessionsListBox );
            this.editorPage.Location = new System.Drawing.Point( 4, 22 );
            this.editorPage.Name = "editorPage";
            this.editorPage.Padding = new System.Windows.Forms.Padding( 3 );
            this.editorPage.Size = new System.Drawing.Size( 592, 400 );
            this.editorPage.TabIndex = 1;
            this.editorPage.Text = "PollEditor";
            this.editorPage.UseVisualStyleBackColor = true;
            // 
            // removeButton
            // 
            this.removeButton.Location = new System.Drawing.Point( 166, 356 );
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size( 269, 23 );
            this.removeButton.TabIndex = 7;
            this.removeButton.Text = "Remove pollsession";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler( this.removeButton_Click );
            // 
            // editButton
            // 
            this.editButton.Location = new System.Drawing.Point( 166, 327 );
            this.editButton.Name = "editButton";
            this.editButton.Size = new System.Drawing.Size( 269, 23 );
            this.editButton.TabIndex = 6;
            this.editButton.Text = "Edit pollsession";
            this.editButton.UseVisualStyleBackColor = true;
            this.editButton.Click += new System.EventHandler( this.editButton_Click );
            // 
            // createButton
            // 
            this.createButton.Location = new System.Drawing.Point( 166, 298 );
            this.createButton.Name = "createButton";
            this.createButton.Size = new System.Drawing.Size( 269, 23 );
            this.createButton.TabIndex = 5;
            this.createButton.Text = "Create new pollsession";
            this.createButton.UseVisualStyleBackColor = true;
            this.createButton.Click += new System.EventHandler( this.createButton_Click );
            // 
            // pollSessionsListBox
            // 
            this.pollSessionsListBox.FormattingEnabled = true;
            this.pollSessionsListBox.HorizontalScrollbar = true;
            this.pollSessionsListBox.Location = new System.Drawing.Point( 166, 18 );
            this.pollSessionsListBox.Name = "pollSessionsListBox";
            this.pollSessionsListBox.Size = new System.Drawing.Size( 269, 264 );
            this.pollSessionsListBox.TabIndex = 4;
            this.pollSessionsListBox.SelectedIndexChanged += new System.EventHandler( this.SelectedIndexChanged );
            // 
            // statisticsPage
            // 
            this.statisticsPage.Location = new System.Drawing.Point( 4, 22 );
            this.statisticsPage.Name = "statisticsPage";
            this.statisticsPage.Size = new System.Drawing.Size( 592, 400 );
            this.statisticsPage.TabIndex = 2;
            this.statisticsPage.Text = "PollStatistics";
            this.statisticsPage.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 613, 447 );
            this.Controls.Add( this.mainTabControl );
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size( 629, 483 );
            this.MinimumSize = new System.Drawing.Size( 629, 483 );
            this.Name = "MainForm";
            this.Text = "PollClientGUI";
            this.Load += new System.EventHandler( this.MainForm_Load );
            this.mainTabControl.ResumeLayout( false );
            this.editorPage.ResumeLayout( false );
            this.ResumeLayout( false );

        }

        #endregion

        private System.Windows.Forms.TabControl mainTabControl;
        private System.Windows.Forms.TabPage clientPage;
        private System.Windows.Forms.TabPage editorPage;
        private System.Windows.Forms.TabPage statisticsPage;
        private System.Windows.Forms.ListBox pollSessionsListBox;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.Button editButton;
        private System.Windows.Forms.Button createButton;
    }
}

