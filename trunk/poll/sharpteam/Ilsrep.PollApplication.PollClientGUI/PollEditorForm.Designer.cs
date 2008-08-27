namespace Ilsrep.PollApplication.PollClientGUI
{
    partial class PollEditorForm
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
            this.createButton = new System.Windows.Forms.Button();
            this.editButton = new System.Windows.Forms.Button();
            this.removeButton = new System.Windows.Forms.Button();
            this.pollSessionsListBox = new System.Windows.Forms.ListBox();
            this.infoBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // createButton
            // 
            this.createButton.Location = new System.Drawing.Point(229, 143);
            this.createButton.Name = "createButton";
            this.createButton.Size = new System.Drawing.Size(206, 23);
            this.createButton.TabIndex = 0;
            this.createButton.Text = "Create new pollsession";
            this.createButton.UseVisualStyleBackColor = true;
            // 
            // editButton
            // 
            this.editButton.Location = new System.Drawing.Point(229, 172);
            this.editButton.Name = "editButton";
            this.editButton.Size = new System.Drawing.Size(206, 23);
            this.editButton.TabIndex = 1;
            this.editButton.Text = "Edit pollsession";
            this.editButton.UseVisualStyleBackColor = true;
            this.editButton.Click += new System.EventHandler(this.editButton_Click);
            // 
            // removeButton
            // 
            this.removeButton.Location = new System.Drawing.Point(229, 201);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(206, 23);
            this.removeButton.TabIndex = 2;
            this.removeButton.Text = "Remove pollsession";
            this.removeButton.UseVisualStyleBackColor = true;
            // 
            // pollSessionsListBox
            // 
            this.pollSessionsListBox.FormattingEnabled = true;
            this.pollSessionsListBox.HorizontalScrollbar = true;
            this.pollSessionsListBox.Location = new System.Drawing.Point(229, 12);
            this.pollSessionsListBox.Name = "pollSessionsListBox";
            this.pollSessionsListBox.Size = new System.Drawing.Size(206, 121);
            this.pollSessionsListBox.TabIndex = 3;
            // 
            // infoBox
            // 
            this.infoBox.FormattingEnabled = true;
            this.infoBox.HorizontalScrollbar = true;
            this.infoBox.Items.AddRange(new object[] {
            "Welcome to PollEditor application"});
            this.infoBox.Location = new System.Drawing.Point(12, 12);
            this.infoBox.Name = "infoBox";
            this.infoBox.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.infoBox.Size = new System.Drawing.Size(211, 212);
            this.infoBox.TabIndex = 4;
            // 
            // PollEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(447, 233);
            this.Controls.Add(this.infoBox);
            this.Controls.Add(this.pollSessionsListBox);
            this.Controls.Add(this.removeButton);
            this.Controls.Add(this.editButton);
            this.Controls.Add(this.createButton);
            this.Name = "PollEditorForm";
            this.Text = "PollClientGUI.PollEditor";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button createButton;
        private System.Windows.Forms.Button editButton;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.ListBox pollSessionsListBox;
        private System.Windows.Forms.ListBox infoBox;
    }
}