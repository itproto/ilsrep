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
            this.submitButton = new System.Windows.Forms.Button();
            this.pollGroupBox = new System.Windows.Forms.GroupBox();
            this.pollSessionsListLabel1 = new System.Windows.Forms.Label();
            this.pollSessionsListBox1 = new System.Windows.Forms.ListBox();
            this.editorPage = new System.Windows.Forms.TabPage();
            this.cancelButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.pollSessionsListLabel = new System.Windows.Forms.Label();
            this.removeButton = new System.Windows.Forms.Button();
            this.createButton = new System.Windows.Forms.Button();
            this.pollSessionsListBox = new System.Windows.Forms.ListBox();
            this.statisticsPage = new System.Windows.Forms.TabPage();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainTabControl.SuspendLayout();
            this.clientPage.SuspendLayout();
            this.editorPage.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainTabControl
            // 
            this.mainTabControl.Controls.Add(this.clientPage);
            this.mainTabControl.Controls.Add(this.editorPage);
            this.mainTabControl.Location = new System.Drawing.Point(10, 33);
            this.mainTabControl.Name = "mainTabControl";
            this.mainTabControl.SelectedIndex = 0;
            this.mainTabControl.Size = new System.Drawing.Size(600, 410);
            this.mainTabControl.TabIndex = 4;
            // 
            // clientPage
            // 
            this.clientPage.Controls.Add(this.submitButton);
            this.clientPage.Controls.Add(this.pollGroupBox);
            this.clientPage.Controls.Add(this.pollSessionsListLabel1);
            this.clientPage.Controls.Add(this.pollSessionsListBox1);
            this.clientPage.Location = new System.Drawing.Point(4, 22);
            this.clientPage.Name = "clientPage";
            this.clientPage.Padding = new System.Windows.Forms.Padding(3);
            this.clientPage.Size = new System.Drawing.Size(592, 384);
            this.clientPage.TabIndex = 0;
            this.clientPage.Text = "Start Poll";
            this.clientPage.UseVisualStyleBackColor = true;
            this.clientPage.Enter += new System.EventHandler(this.clientPage_Enter);
            // 
            // submitButton
            // 
            this.submitButton.Location = new System.Drawing.Point(132, 346);
            this.submitButton.Name = "submitButton";
            this.submitButton.Size = new System.Drawing.Size(75, 23);
            this.submitButton.TabIndex = 1;
            this.submitButton.Text = "Select";
            this.submitButton.UseVisualStyleBackColor = true;
            this.submitButton.Click += new System.EventHandler(this.submitButton_Click);
            // 
            // pollGroupBox
            // 
            this.pollGroupBox.Location = new System.Drawing.Point(213, 18);
            this.pollGroupBox.Name = "pollGroupBox";
            this.pollGroupBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.pollGroupBox.Size = new System.Drawing.Size(373, 322);
            this.pollGroupBox.TabIndex = 4;
            this.pollGroupBox.TabStop = false;
            // 
            // pollSessionsListLabel1
            // 
            this.pollSessionsListLabel1.AutoSize = true;
            this.pollSessionsListLabel1.Location = new System.Drawing.Point(65, 8);
            this.pollSessionsListLabel1.Name = "pollSessionsListLabel1";
            this.pollSessionsListLabel1.Size = new System.Drawing.Size(85, 13);
            this.pollSessionsListLabel1.TabIndex = 2;
            this.pollSessionsListLabel1.Text = "PollSessionsList:";
            // 
            // pollSessionsListBox1
            // 
            this.pollSessionsListBox1.FormattingEnabled = true;
            this.pollSessionsListBox1.Location = new System.Drawing.Point(6, 24);
            this.pollSessionsListBox1.Name = "pollSessionsListBox1";
            this.pollSessionsListBox1.Size = new System.Drawing.Size(201, 316);
            this.pollSessionsListBox1.TabIndex = 0;
            this.pollSessionsListBox1.SelectedIndexChanged += new System.EventHandler(this.pollSessionsListBox1_SelectedIndexChanged);
            // 
            // editorPage
            // 
            this.editorPage.Controls.Add(this.cancelButton);
            this.editorPage.Controls.Add(this.saveButton);
            this.editorPage.Controls.Add(this.propertyGrid);
            this.editorPage.Controls.Add(this.pollSessionsListLabel);
            this.editorPage.Controls.Add(this.removeButton);
            this.editorPage.Controls.Add(this.createButton);
            this.editorPage.Controls.Add(this.pollSessionsListBox);
            this.editorPage.Location = new System.Drawing.Point(4, 22);
            this.editorPage.Name = "editorPage";
            this.editorPage.Padding = new System.Windows.Forms.Padding(3);
            this.editorPage.Size = new System.Drawing.Size(592, 384);
            this.editorPage.TabIndex = 1;
            this.editorPage.Text = "Poll Editor";
            this.editorPage.UseVisualStyleBackColor = true;
            this.editorPage.Enter += new System.EventHandler(this.editorPage_Enter);
            // 
            // cancelButton
            // 
            this.cancelButton.Image = global::Ilsrep.PollApplication.PollClientGUI.Properties.Resources.cross;
            this.cancelButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cancelButton.Location = new System.Drawing.Point(87, 349);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Padding = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 11;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Image = global::Ilsrep.PollApplication.PollClientGUI.Properties.Resources.tick;
            this.saveButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.saveButton.Location = new System.Drawing.Point(6, 349);
            this.saveButton.Name = "saveButton";
            this.saveButton.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 10;
            this.saveButton.Text = "Save";
            this.saveButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // propertyGrid
            // 
            this.propertyGrid.Location = new System.Drawing.Point(281, 27);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(305, 345);
            this.propertyGrid.TabIndex = 9;
            // 
            // pollSessionsListLabel
            // 
            this.pollSessionsListLabel.AutoSize = true;
            this.pollSessionsListLabel.Location = new System.Drawing.Point(88, 11);
            this.pollSessionsListLabel.Name = "pollSessionsListLabel";
            this.pollSessionsListLabel.Size = new System.Drawing.Size(84, 13);
            this.pollSessionsListLabel.TabIndex = 8;
            this.pollSessionsListLabel.Text = "PollSessions list:";
            // 
            // removeButton
            // 
            this.removeButton.Image = global::Ilsrep.PollApplication.PollClientGUI.Properties.Resources.delete;
            this.removeButton.Location = new System.Drawing.Point(250, 349);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(25, 23);
            this.removeButton.TabIndex = 7;
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // createButton
            // 
            this.createButton.Image = global::Ilsrep.PollApplication.PollClientGUI.Properties.Resources.add;
            this.createButton.Location = new System.Drawing.Point(219, 349);
            this.createButton.Name = "createButton";
            this.createButton.Size = new System.Drawing.Size(25, 23);
            this.createButton.TabIndex = 5;
            this.createButton.UseVisualStyleBackColor = true;
            this.createButton.Click += new System.EventHandler(this.createButton_Click);
            // 
            // pollSessionsListBox
            // 
            this.pollSessionsListBox.FormattingEnabled = true;
            this.pollSessionsListBox.HorizontalScrollbar = true;
            this.pollSessionsListBox.Location = new System.Drawing.Point(6, 27);
            this.pollSessionsListBox.Name = "pollSessionsListBox";
            this.pollSessionsListBox.Size = new System.Drawing.Size(269, 316);
            this.pollSessionsListBox.TabIndex = 4;
            this.pollSessionsListBox.SelectedIndexChanged += new System.EventHandler(this.SelectedIndexChanged);
            // 
            // statisticsPage
            // 
            this.statisticsPage.Location = new System.Drawing.Point(4, 22);
            this.statisticsPage.Name = "statisticsPage";
            this.statisticsPage.Size = new System.Drawing.Size(592, 400);
            this.statisticsPage.TabIndex = 2;
            this.statisticsPage.Text = "PollStatistics";
            this.statisticsPage.UseVisualStyleBackColor = true;
            // 
            // menuStrip
            // 
            this.menuStrip.BackColor = System.Drawing.Color.Transparent;
            this.menuStrip.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem,
            this.logoutToolStripMenuItem});
            this.menuStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(618, 24);
            this.menuStrip.TabIndex = 5;
            this.menuStrip.Text = "menuStrip";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Image = global::Ilsrep.PollApplication.PollClientGUI.Properties.Resources.wrench;
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(74, 20);
            this.settingsToolStripMenuItem.Text = "&Settings";
            this.settingsToolStripMenuItem.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // logoutToolStripMenuItem
            // 
            this.logoutToolStripMenuItem.Image = global::Ilsrep.PollApplication.PollClientGUI.Properties.Resources.door_out;
            this.logoutToolStripMenuItem.Name = "logoutToolStripMenuItem";
            this.logoutToolStripMenuItem.Size = new System.Drawing.Size(68, 20);
            this.logoutToolStripMenuItem.Text = "&Logout";
            this.logoutToolStripMenuItem.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.logoutToolStripMenuItem.Click += new System.EventHandler(this.logoutToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(618, 449);
            this.Controls.Add(this.mainTabControl);
            this.Controls.Add(this.menuStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PollClientGUI";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.mainTabControl.ResumeLayout(false);
            this.clientPage.ResumeLayout(false);
            this.clientPage.PerformLayout();
            this.editorPage.ResumeLayout(false);
            this.editorPage.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl mainTabControl;
        private System.Windows.Forms.TabPage clientPage;
        private System.Windows.Forms.TabPage editorPage;
        private System.Windows.Forms.TabPage statisticsPage;
        private System.Windows.Forms.ListBox pollSessionsListBox;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.Button createButton;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logoutToolStripMenuItem;
        private System.Windows.Forms.Label pollSessionsListLabel;
        private System.Windows.Forms.PropertyGrid propertyGrid;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label pollSessionsListLabel1;
        private System.Windows.Forms.Button submitButton;
        private System.Windows.Forms.ListBox pollSessionsListBox1;
        private System.Windows.Forms.GroupBox pollGroupBox;
    }
}

