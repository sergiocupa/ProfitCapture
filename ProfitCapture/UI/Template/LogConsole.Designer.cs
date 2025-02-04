namespace ProfitCapture.UI.Template
{
    partial class LogConsole
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
            Content = new RichTextBox();
            SuspendLayout();
            // 
            // Content
            // 
            Content.BackColor = Color.FromArgb(60, 60, 60);
            Content.BorderStyle = BorderStyle.None;
            Content.Dock = DockStyle.Fill;
            Content.Location = new Point(6, 6);
            Content.Margin = new Padding(0);
            Content.Name = "Content";
            Content.Size = new Size(845, 551);
            Content.TabIndex = 0;
            Content.Text = "656565";
            // 
            // LogConsole
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(60, 60, 60);
            ClientSize = new Size(857, 563);
            Controls.Add(Content);
            Name = "LogConsole";
            Padding = new Padding(6);
            Text = "LogConsole";
            FormClosing += LogConsole_FormClosing;
            Shown += LogConsole_Shown;
            ResumeLayout(false);
        }

        #endregion

        public RichTextBox Content;
    }
}