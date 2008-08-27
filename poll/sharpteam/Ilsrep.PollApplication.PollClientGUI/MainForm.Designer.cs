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
            this.pollClientButton = new System.Windows.Forms.Button();
            this.pollEditorButton = new System.Windows.Forms.Button();
            this.infoLabel = new System.Windows.Forms.Label();
            this.pollStatisticsButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // pollClientButton
            // 
            this.pollClientButton.Location = new System.Drawing.Point(58, 53);
            this.pollClientButton.Name = "pollClientButton";
            this.pollClientButton.Size = new System.Drawing.Size(108, 23);
            this.pollClientButton.TabIndex = 0;
            this.pollClientButton.Text = "Start PollClient";
            this.pollClientButton.UseVisualStyleBackColor = true;
            this.pollClientButton.Click += new System.EventHandler(this.pollClientButton_Click);
            // 
            // pollEditorButton
            // 
            this.pollEditorButton.Location = new System.Drawing.Point(58, 82);
            this.pollEditorButton.Name = "pollEditorButton";
            this.pollEditorButton.Size = new System.Drawing.Size(108, 23);
            this.pollEditorButton.TabIndex = 1;
            this.pollEditorButton.Text = "Start PollEditor";
            this.pollEditorButton.UseVisualStyleBackColor = true;
            this.pollEditorButton.Click += new System.EventHandler(this.pollEditorButton_Click);
            // 
            // infoLabel
            // 
            this.infoLabel.AutoSize = true;
            this.infoLabel.Location = new System.Drawing.Point(27, 20);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(93, 13);
            this.infoLabel.TabIndex = 2;
            this.infoLabel.Text = "Glad to meet you, ";
            // 
            // pollStatisticsButton
            // 
            this.pollStatisticsButton.Location = new System.Drawing.Point(58, 111);
            this.pollStatisticsButton.Name = "pollStatisticsButton";
            this.pollStatisticsButton.Size = new System.Drawing.Size(108, 23);
            this.pollStatisticsButton.TabIndex = 3;
            this.pollStatisticsButton.Text = "View Statistics";
            this.pollStatisticsButton.UseVisualStyleBackColor = true;
            this.pollStatisticsButton.Click += new System.EventHandler(this.pollStatisticsButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(220, 160);
            this.Controls.Add(this.pollStatisticsButton);
            this.Controls.Add(this.infoLabel);
            this.Controls.Add(this.pollEditorButton);
            this.Controls.Add(this.pollClientButton);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(228, 194);
            this.MinimumSize = new System.Drawing.Size(228, 194);
            this.Name = "MainForm";
            this.Text = "PollClientGUI";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button pollClientButton;
        private System.Windows.Forms.Button pollEditorButton;
        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.Button pollStatisticsButton;
    }
}

