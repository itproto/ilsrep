namespace Ilsrep.PollApplication.PollClientGUI
{
    partial class PollSessionForm
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
            this.nameField = new System.Windows.Forms.TextBox();
            this.nameLabel = new System.Windows.Forms.Label();
            this.minScoreLabel = new System.Windows.Forms.Label();
            this.minScoreField = new System.Windows.Forms.TextBox();
            this.isTestMode = new System.Windows.Forms.CheckBox();
            this.pollsListBox = new System.Windows.Forms.ListBox();
            this.addButton = new System.Windows.Forms.Button();
            this.editButton = new System.Windows.Forms.Button();
            this.removeButton = new System.Windows.Forms.Button();
            this.submitButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // nameField
            // 
            this.nameField.Location = new System.Drawing.Point(62, 12);
            this.nameField.Name = "nameField";
            this.nameField.Size = new System.Drawing.Size(197, 20);
            this.nameField.TabIndex = 0;
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Location = new System.Drawing.Point(18, 15);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(35, 13);
            this.nameLabel.TabIndex = 1;
            this.nameLabel.Text = "Name";
            // 
            // minScoreLabel
            // 
            this.minScoreLabel.AutoSize = true;
            this.minScoreLabel.Location = new System.Drawing.Point(163, 39);
            this.minScoreLabel.Name = "minScoreLabel";
            this.minScoreLabel.Size = new System.Drawing.Size(53, 13);
            this.minScoreLabel.TabIndex = 2;
            this.minScoreLabel.Text = "Min score";
            // 
            // minScoreField
            // 
            this.minScoreField.Enabled = false;
            this.minScoreField.Location = new System.Drawing.Point(222, 36);
            this.minScoreField.Name = "minScoreField";
            this.minScoreField.Size = new System.Drawing.Size(37, 20);
            this.minScoreField.TabIndex = 3;
            // 
            // isTestMode
            // 
            this.isTestMode.AutoSize = true;
            this.isTestMode.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.isTestMode.Location = new System.Drawing.Point(62, 38);
            this.isTestMode.Name = "isTestMode";
            this.isTestMode.Size = new System.Drawing.Size(76, 17);
            this.isTestMode.TabIndex = 6;
            this.isTestMode.Text = "Test mode";
            this.isTestMode.UseVisualStyleBackColor = true;
            this.isTestMode.CheckedChanged += new System.EventHandler(this.isTestMode_CheckedChanged);
            // 
            // pollsListBox
            // 
            this.pollsListBox.FormattingEnabled = true;
            this.pollsListBox.Location = new System.Drawing.Point(12, 62);
            this.pollsListBox.Name = "pollsListBox";
            this.pollsListBox.Size = new System.Drawing.Size(247, 173);
            this.pollsListBox.TabIndex = 7;
            this.pollsListBox.SelectedIndexChanged += new System.EventHandler(this.pollsListBox_SelectedIndexChanged);
            // 
            // addButton
            // 
            this.addButton.Location = new System.Drawing.Point(12, 241);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(247, 23);
            this.addButton.TabIndex = 8;
            this.addButton.Text = "Add new poll";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // editButton
            // 
            this.editButton.Location = new System.Drawing.Point(12, 270);
            this.editButton.Name = "editButton";
            this.editButton.Size = new System.Drawing.Size(247, 23);
            this.editButton.TabIndex = 9;
            this.editButton.Text = "Edit poll";
            this.editButton.UseVisualStyleBackColor = true;
            this.editButton.Click += new System.EventHandler(this.editButton_Click);
            // 
            // removeButton
            // 
            this.removeButton.Location = new System.Drawing.Point(12, 300);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(247, 23);
            this.removeButton.TabIndex = 10;
            this.removeButton.Text = "Remove poll";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // submitButton
            // 
            this.submitButton.Location = new System.Drawing.Point(184, 329);
            this.submitButton.Name = "submitButton";
            this.submitButton.Size = new System.Drawing.Size(75, 23);
            this.submitButton.TabIndex = 11;
            this.submitButton.Text = "OK";
            this.submitButton.UseVisualStyleBackColor = true;
            this.submitButton.Click += new System.EventHandler(this.submitButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(103, 329);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 12;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // PollSessionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(272, 360);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.submitButton);
            this.Controls.Add(this.removeButton);
            this.Controls.Add(this.editButton);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.pollsListBox);
            this.Controls.Add(this.isTestMode);
            this.Controls.Add(this.minScoreField);
            this.Controls.Add(this.minScoreLabel);
            this.Controls.Add(this.nameLabel);
            this.Controls.Add(this.nameField);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(280, 394);
            this.MinimumSize = new System.Drawing.Size(280, 394);
            this.Name = "PollSessionForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PollSession";
            this.Load += new System.EventHandler(this.PollSessionForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PollSessionForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox nameField;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.Label minScoreLabel;
        private System.Windows.Forms.TextBox minScoreField;
        private System.Windows.Forms.CheckBox isTestMode;
        private System.Windows.Forms.ListBox pollsListBox;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button editButton;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.Button submitButton;
        private System.Windows.Forms.Button cancelButton;
    }
}