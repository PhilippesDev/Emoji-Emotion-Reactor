using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms;
using System.Xml.Linq;

namespace reactor
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            pictureBox1 = new PictureBox();
            label1 = new Label();
            label2 = new Label();
            panel1 = new Panel();
            Streamcamera = new PictureBox();
            button1 = new Button();
            panel2 = new Panel();
            emojiBox = new PictureBox();
            Result = new Label();
            button3 = new Button();
            button2 = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)Streamcamera).BeginInit();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)emojiBox).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(437, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(170, 162);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Play Chickens", 23.9999962F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.White;
            label1.Location = new Point(349, 186);
            label1.Name = "label1";
            label1.Size = new Size(242, 33);
            label1.TabIndex = 1;
            label1.Text = "Emotion Emoji ";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Play Chickens", 23.9999962F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.ForeColor = Color.FromArgb(0, 174, 242);
            label2.Location = new Point(578, 186);
            label2.Name = "label2";
            label2.Size = new Size(136, 33);
            label2.TabIndex = 2;
            label2.Text = "Reactor";
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(21, 41, 73);
            panel1.Controls.Add(Streamcamera);
            panel1.Controls.Add(button1);
            panel1.Location = new Point(60, 245);
            panel1.Name = "panel1";
            panel1.Size = new Size(439, 425);
            panel1.TabIndex = 3;
            // 
            // Streamcamera
            // 
            Streamcamera.Location = new Point(27, 29);
            Streamcamera.Name = "Streamcamera";
            Streamcamera.Size = new Size(388, 316);
            Streamcamera.SizeMode = PictureBoxSizeMode.Zoom;
            Streamcamera.TabIndex = 1;
            Streamcamera.TabStop = false;
            // 
            // button1
            // 
            button1.BackColor = Color.FromArgb(0, 174, 242);
            button1.FlatAppearance.BorderSize = 0;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new Font("Play Chickens", 11.9999981F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button1.ForeColor = Color.White;
            button1.Location = new Point(27, 360);
            button1.Name = "button1";
            button1.Size = new Size(388, 40);
            button1.TabIndex = 0;
            button1.Text = "Allumer la camera";
            button1.UseVisualStyleBackColor = false;
            // 
            // panel2
            // 
            panel2.BackColor = Color.FromArgb(21, 41, 73);
            panel2.Controls.Add(emojiBox);
            panel2.Controls.Add(Result);
            panel2.Controls.Add(button3);
            panel2.Controls.Add(button2);
            panel2.Location = new Point(506, 245);
            panel2.Name = "panel2";
            panel2.Size = new Size(439, 425);
            panel2.TabIndex = 4;
            // 
            // emojiBox
            // 
            emojiBox.Location = new Point(22, 29);
            emojiBox.Name = "emojiBox";
            emojiBox.Size = new Size(388, 262);
            emojiBox.SizeMode = PictureBoxSizeMode.Zoom;
            emojiBox.TabIndex = 2;
            emojiBox.TabStop = false;
            // 
            // Result
            // 
            Result.AutoSize = true;
            Result.Font = new Font("Play Chickens", 11.9999981F);
            Result.ForeColor = Color.White;
            Result.Location = new Point(133, 314);
            Result.Name = "Result";
            Result.Size = new Size(170, 16);
            Result.TabIndex = 8;
            Result.Text = "Aucun visage detecte";
            // 
            // button3
            // 
            button3.BackColor = Color.FromArgb(0, 174, 242);
            button3.FlatAppearance.BorderSize = 0;
            button3.FlatStyle = FlatStyle.Flat;
            button3.Font = new Font("Play Chickens", 11.9999981F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button3.ForeColor = Color.White;
            button3.Location = new Point(322, 360);
            button3.Name = "button3";
            button3.Size = new Size(87, 40);
            button3.TabIndex = 7;
            button3.Text = "Genre";
            button3.UseVisualStyleBackColor = false;
            // 
            // button2
            // 
            button2.BackColor = Color.FromArgb(0, 174, 242);
            button2.FlatAppearance.BorderSize = 0;
            button2.FlatStyle = FlatStyle.Flat;
            button2.Font = new Font("Play Chickens", 11.9999981F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button2.ForeColor = Color.White;
            button2.Location = new Point(22, 360);
            button2.Name = "button2";
            button2.Size = new Size(87, 40);
            button2.TabIndex = 6;
            button2.Text = "Age";
            button2.UseVisualStyleBackColor = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(17, 26, 43);
            ClientSize = new Size(986, 700);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(pictureBox1);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)Streamcamera).EndInit();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)emojiBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private Label label1;
        private Label label2;
        private Panel panel1;
        private Panel panel2;
        private Button button1;
        private Button button3;
        private Button button2;
        private Label Result;
        private PictureBox Streamcamera;
        private PictureBox emojiBox;
    }
}
