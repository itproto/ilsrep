namespace Ilsrep.PollApplication.PollClientGUI
{
    partial class PollForm
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
            this.nameLabel = new System.Windows.Forms.Label();
            this.descriptionLabel = new System.Windows.Forms.Label();
            this.nameField = new System.Windows.Forms.TextBox();
            this.descriptionField = new System.Windows.Forms.TextBox();
            this.isCustomChoiceEnabled = new System.Windows.Forms.CheckBox();
            this.choicesListBox = new System.Windows.Forms.ListBox();
            this.addButton = new System.Windows.Forms.Button();
            this.editButton = new System.Windows.Forms.Button();
            this.removeButton = new System.Windows.Forms.Button();
            this.submitButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Location = new System.Drawing.Point(36, 15);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(35, 13);
            this.nameLabel.TabIndex = 0;
            this.nameLabel.Text = "Name";
            // 
            // descriptionLabel
            // 
            this.descriptionLabel.AutoSize = true;
            this.descriptionLabel.Location = new System.Drawing.Point(11, 40);
            this.descriptionLabel.Name = "descriptionLabel";
            this.descriptionLabel.Size = new System.Drawing.Size(60, 13);
            this.descriptionLabel.TabIndex = 1;
            this.descriptionLabel.Text = "Description";
            // 
            // nameField
            // 
            this.nameField.Location = new System.Drawing.Point(77, 12);
            this.nameField.Name = "nameField";
            this.nameField.Size = new System.Drawing.Size(197, 20);
            this.nameField.TabIndex = 2;
            // 
            // descriptionField
            // 
            this.descriptionField.Location = new System.Drawing.Point(77, 37);
            this.descriptionField.Name = "descriptionField";
            this.descriptionField.Size = new System.Drawing.Size(197, 20);
            this.descriptionField.TabIndex = 3;
            // 
            // isCustomChoiceEnabled
            // 
            this.isCustomChoiceEnabled.AutoSize = true;
            this.isCustomChoiceEnabled.Location = new System.Drawing.Point(77, 63);
            this.isCustomChoiceEnabled.Name = "isCustomChoiceEnabled";
            this.isCustomChoiceEnabled.Size = new System.Drawing.Size(131, 17);
            this.isCustomChoiceEnabled.TabIndex = 4;
            this.isCustomChoiceEnabled.Text = "Enable custom choice";
            this.isCustomChoiceEnabled.UseVisualStyleBackColor = true;
            // 
            // choicesListBox
            // 
            this.choicesListBox.FormattingEnabled = true;
            this.choicesListBox.Location = new System.Drawing.Point(12, 86);
            this.choicesListBox.Name = "choicesListBox";
            this.choicesListBox.Size = new System.Drawing.Size(262, 173);
            this.choicesListBox.TabIndex = 5;
            this.choicesListBox.SelectedIndexChanged += new System.EventHandler(this.choicesListBox_SelectedIndexChanged);
            // 
            // addButton
            // 
            this.addButton.Location = new System.Drawing.Point(12, 263);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(262, 23);
            this.addButton.TabIndex = 6;
            this.addButton.Text = "Add new choice";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // editButton
            // 
            this.editButton.Location = new System.Drawing.Point(12, 292);
            this.editButton.Name = "editButton";
            this.editButton.Size = new System.Drawing.Size(262, 23);
            this.editButton.TabIndex = 7;
            this.editButton.Text = "Edit choice";
            this.editButton.UseVisualStyleBackColor = true;
            this.editButton.Click += new System.EventHandler(this.editButton_Click);
            // 
            // removeButton
            // 
            this.removeButton.Location = new System.Drawing.Point(12, 321);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(181, 23);
            this.removeButton.TabIndex = 8;
            this.removeButton.Text = "Remove choice";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // submitButton
            // 
            this.submitButton.Location = new System.Drawing.Point(199, 321);
            this.submitButton.Name = "submitButton";
            this.submitButton.Size = new System.Drawing.Size(75, 23);
            this.submitButton.TabIndex = 9;
            this.submitButton.Text = "Submit";
            this.submitButton.UseVisualStyleBackColor = true;
            this.submitButton.Click += new System.EventHandler(this.submitButton_Click);
            // 
            // PollForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(286, 359);
            this.Controls.Add(this.submitButton);
            this.Controls.Add(this.removeButton);
            this.Controls.Add(this.editButton);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.choicesListBox);
            this.Controls.Add(this.isCustomChoiceEnabled);
            this.Controls.Add(this.descriptionField);
            this.Controls.Add(this.nameField);
            this.Controls.Add(this.descriptionLabel);
            this.Controls.Add(this.nameLabel);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(294, 393);
            this.MinimumSize = new System.Drawing.Size(294, 393);
            this.Name = "PollForm";
            this.Text = "Poll";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.Label descriptionLabel;
        private System.Windows.Forms.TextBox nameField;
        private System.Windows.Forms.TextBox descriptionField;
        private System.Windows.Forms.CheckBox isCustomChoiceEnabled;
        private System.Windows.Forms.ListBox choicesListBox;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button editButton;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.Button submitButton;
    }
}