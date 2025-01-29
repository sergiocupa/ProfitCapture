namespace ProfitCapture
{
    partial class CaptureConfigurator
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
            button1 = new Button();
            textBox1 = new TextBox();
            label1 = new Label();
            richTextBox1 = new RichTextBox();
            label2 = new Label();
            button2 = new Button();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(379, 50);
            button1.Name = "button1";
            button1.Size = new Size(41, 23);
            button1.TabIndex = 0;
            button1.Text = ">>";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(18, 50);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(355, 23);
            textBox1.TabIndex = 1;
            // 
            // label1
            // 
            label1.Location = new Point(18, 29);
            label1.Name = "label1";
            label1.Size = new Size(100, 18);
            label1.TabIndex = 2;
            label1.Text = "Pasta destino:";
            label1.TextAlign = ContentAlignment.BottomLeft;
            // 
            // richTextBox1
            // 
            richTextBox1.BackColor = Color.FromArgb(80, 80, 80);
            richTextBox1.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            richTextBox1.ForeColor = Color.White;
            richTextBox1.Location = new Point(17, 118);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(414, 138);
            richTextBox1.TabIndex = 3;
            richTextBox1.Text = "";
            // 
            // label2
            // 
            label2.Location = new Point(18, 98);
            label2.Name = "label2";
            label2.Size = new Size(451, 18);
            label2.TabIndex = 4;
            label2.Text = "DDE (copie neste campo o conteúdo a ser inserido no Excel)";
            label2.TextAlign = ContentAlignment.BottomLeft;
            // 
            // button2
            // 
            button2.Location = new Point(352, 280);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 5;
            button2.Text = "Salvar";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // CaptureConfigurator
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(449, 328);
            Controls.Add(button2);
            Controls.Add(label2);
            Controls.Add(richTextBox1);
            Controls.Add(label1);
            Controls.Add(textBox1);
            Controls.Add(button1);
            Name = "CaptureConfigurator";
            Text = "CaptureConfigurator";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private TextBox textBox1;
        private Label label1;
        private RichTextBox richTextBox1;
        private Label label2;
        private Button button2;
    }
}