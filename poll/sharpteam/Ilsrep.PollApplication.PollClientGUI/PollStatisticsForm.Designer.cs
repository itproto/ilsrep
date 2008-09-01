namespace Ilsrep.PollApplication.PollClientGUI
{
    partial class PollStatisticsForm
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
            this.pollSessionsListBox = new System.Windows.Forms.ListBox();
            this.viewStatisticsButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // pollSessionsListBox
            // 
            this.pollSessionsListBox.FormattingEnabled = true;
            this.pollSessionsListBox.HorizontalScrollbar = true;
            this.pollSessionsListBox.Location = new System.Drawing.Point(12, 12);
            this.pollSessionsListBox.Name = "pollSessionsListBox";
            this.pollSessionsListBox.Size = new System.Drawing.Size(268, 108);
            this.pollSessionsListBox.TabIndex = 0;
            this.pollSessionsListBox.SelectedIndexChanged += new System.EventHandler(this.pollSessionsListBox_SelectedIndexChanged);
            // 
            // viewStatisticsButton
            // 
            this.viewStatisticsButton.Location = new System.Drawing.Point(12, 126);
            this.viewStatisticsButton.Name = "viewStatisticsButton";
            this.viewStatisticsButton.Size = new System.Drawing.Size(268, 23);
            this.viewStatisticsButton.TabIndex = 1;
            this.viewStatisticsButton.Text = "View statistics";
            this.viewStatisticsButton.UseVisualStyleBackColor = true;
            this.viewStatisticsButton.Click += new System.EventHandler(this.viewStatisticsButton_Click);
            // 
            // PollStatisticsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 161);
            this.Controls.Add(this.viewStatisticsButton);
            this.Controls.Add(this.pollSessionsListBox);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(300, 195);
            this.MinimumSize = new System.Drawing.Size(300, 195);
            this.Name = "PollStatisticsForm";
            this.Text = "PollStatistics";
            this.Load += new System.EventHandler(this.PollStatisticsForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox pollSessionsListBox;
        private System.Windows.Forms.Button viewStatisticsButton;
    }
}