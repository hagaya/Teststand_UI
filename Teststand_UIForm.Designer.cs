namespace Teststand_UI
{
    partial class Teststand_UIForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Teststand_UIForm));
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            loginToolStripMenuItem = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            statusStrip1 = new StatusStrip();
            btnPlay = new Button();
            btnStop = new Button();
            btnStartOver = new Button();
            checkedListBox1 = new CheckedListBox();
            EditLoops = new TextBox();
            ProgressBarTest = new ProgressBar();
            ProgressBarTotal = new ProgressBar();
            label1 = new Label();
            pictureBox1 = new PictureBox();
            menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(24, 24);
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, helpToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1357, 33);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { loginToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(54, 29);
            fileToolStripMenuItem.Text = "File";
            // 
            // loginToolStripMenuItem
            // 
            loginToolStripMenuItem.Name = "loginToolStripMenuItem";
            loginToolStripMenuItem.Size = new Size(158, 34);
            loginToolStripMenuItem.Text = "Login";
            loginToolStripMenuItem.Click += loginToolStripMenuItem_Click;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { aboutToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(65, 29);
            helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(164, 34);
            aboutToolStripMenuItem.Text = "About";
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new Size(24, 24);
            statusStrip1.Location = new Point(0, 965);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new Padding(1, 0, 20, 0);
            statusStrip1.Size = new Size(1357, 22);
            statusStrip1.TabIndex = 2;
            statusStrip1.Text = "statusStrip1";
            // 
            // btnPlay
            // 
            btnPlay.BackColor = Color.White;
            btnPlay.FlatAppearance.BorderSize = 0;
            btnPlay.FlatStyle = FlatStyle.Flat;
            btnPlay.Image = (Image)resources.GetObject("btnPlay.Image");
            btnPlay.Location = new Point(36, 65);
            btnPlay.Margin = new Padding(4, 5, 4, 5);
            btnPlay.Name = "btnPlay";
            btnPlay.Size = new Size(143, 167);
            btnPlay.TabIndex = 3;
            btnPlay.UseVisualStyleBackColor = false;
            btnPlay.Click += btnPlay_Click;
            // 
            // btnStop
            // 
            btnStop.BackColor = Color.White;
            btnStop.FlatAppearance.BorderSize = 0;
            btnStop.FlatStyle = FlatStyle.Flat;
            btnStop.Image = (Image)resources.GetObject("btnStop.Image");
            btnStop.Location = new Point(187, 65);
            btnStop.Margin = new Padding(4, 5, 4, 5);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(143, 167);
            btnStop.TabIndex = 4;
            btnStop.UseVisualStyleBackColor = false;
            btnStop.Click += btnStop_Click;
            // 
            // btnStartOver
            // 
            btnStartOver.BackColor = Color.White;
            btnStartOver.FlatAppearance.BorderSize = 0;
            btnStartOver.FlatStyle = FlatStyle.Flat;
            btnStartOver.Image = (Image)resources.GetObject("btnStartOver.Image");
            btnStartOver.Location = new Point(339, 65);
            btnStartOver.Margin = new Padding(4, 5, 4, 5);
            btnStartOver.Name = "btnStartOver";
            btnStartOver.Size = new Size(143, 167);
            btnStartOver.TabIndex = 5;
            btnStartOver.UseVisualStyleBackColor = false;
            btnStartOver.Click += btnStartOver_Click;
            // 
            // checkedListBox1
            // 
            checkedListBox1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            checkedListBox1.FormattingEnabled = true;
            checkedListBox1.Location = new Point(33, 240);
            checkedListBox1.Margin = new Padding(4, 5, 4, 5);
            checkedListBox1.Name = "checkedListBox1";
            checkedListBox1.Size = new Size(1290, 480);
            checkedListBox1.TabIndex = 6;
            // 
            // EditLoops
            // 
            EditLoops.Location = new Point(587, 132);
            EditLoops.Margin = new Padding(4, 5, 4, 5);
            EditLoops.Name = "EditLoops";
            EditLoops.Size = new Size(43, 31);
            EditLoops.TabIndex = 7;
            EditLoops.Text = "1";
            // 
            // ProgressBarTest
            // 
            ProgressBarTest.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ProgressBarTest.Location = new Point(33, 783);
            ProgressBarTest.Margin = new Padding(4, 5, 4, 5);
            ProgressBarTest.Name = "ProgressBarTest";
            ProgressBarTest.Size = new Size(1294, 38);
            ProgressBarTest.TabIndex = 8;
            // 
            // ProgressBarTotal
            // 
            ProgressBarTotal.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ProgressBarTotal.Location = new Point(36, 848);
            ProgressBarTotal.Margin = new Padding(4, 5, 4, 5);
            ProgressBarTotal.Name = "ProgressBarTotal";
            ProgressBarTotal.Size = new Size(1294, 38);
            ProgressBarTotal.TabIndex = 9;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(519, 137);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(65, 25);
            label1.TabIndex = 10;
            label1.Text = "Loops:";
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(758, 86);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(334, 117);
            pictureBox1.TabIndex = 11;
            pictureBox1.TabStop = false;
            // 
            // Teststand_UIForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            BackgroundImageLayout = ImageLayout.None;
            ClientSize = new Size(1357, 987);
            Controls.Add(pictureBox1);
            Controls.Add(label1);
            Controls.Add(ProgressBarTotal);
            Controls.Add(ProgressBarTest);
            Controls.Add(EditLoops);
            Controls.Add(checkedListBox1);
            Controls.Add(btnStartOver);
            Controls.Add(btnStop);
            Controls.Add(btnPlay);
            Controls.Add(statusStrip1);
            Controls.Add(menuStrip1);
            DoubleBuffered = true;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            Name = "Teststand_UIForm";
            Text = "Teststand UI";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem loginToolStripMenuItem;
        private StatusStrip statusStrip1;
        private Button btnPlay;
        private Button btnStop;
        private Button btnStartOver;
        private CheckedListBox checkedListBox1;
        private TextBox EditLoops;
        private ProgressBar ProgressBarTest;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ProgressBar ProgressBarTotal;
        private Label label1;
        private PictureBox pictureBox1;
    }
}