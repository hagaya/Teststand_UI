namespace FabricTester
{
    partial class FabricTesterForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FabricTesterForm));
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
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(24, 24);
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, helpToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new Padding(4, 1, 0, 1);
            menuStrip1.Size = new Size(950, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { loginToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 22);
            fileToolStripMenuItem.Text = "File";
            // 
            // loginToolStripMenuItem
            // 
            loginToolStripMenuItem.Name = "loginToolStripMenuItem";
            loginToolStripMenuItem.Size = new Size(104, 22);
            loginToolStripMenuItem.Text = "Login";
            loginToolStripMenuItem.Click += loginToolStripMenuItem_Click;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { aboutToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(44, 22);
            helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(107, 22);
            aboutToolStripMenuItem.Text = "About";
            // 
            // statusStrip1
            // 
            statusStrip1.Location = new Point(0, 570);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(950, 22);
            statusStrip1.TabIndex = 2;
            statusStrip1.Text = "statusStrip1";
            // 
            // btnPlay
            // 
            btnPlay.Image = (Image)resources.GetObject("btnPlay.Image");
            btnPlay.Location = new Point(25, 39);
            btnPlay.Name = "btnPlay";
            btnPlay.Size = new Size(100, 100);
            btnPlay.TabIndex = 3;
            btnPlay.UseVisualStyleBackColor = true;
            btnPlay.Click += btnPlay_Click;
            // 
            // btnStop
            // 
            btnStop.Image = (Image)resources.GetObject("btnStop.Image");
            btnStop.Location = new Point(131, 39);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(100, 100);
            btnStop.TabIndex = 4;
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // btnStartOver
            // 
            btnStartOver.Image = (Image)resources.GetObject("btnStartOver.Image");
            btnStartOver.Location = new Point(237, 39);
            btnStartOver.Name = "btnStartOver";
            btnStartOver.Size = new Size(100, 100);
            btnStartOver.TabIndex = 5;
            btnStartOver.UseVisualStyleBackColor = true;
            btnStartOver.Click += btnStartOver_Click;
            // 
            // checkedListBox1
            // 
            checkedListBox1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            checkedListBox1.FormattingEnabled = true;
            checkedListBox1.Location = new Point(23, 161);
            checkedListBox1.Name = "checkedListBox1";
            checkedListBox1.Size = new Size(904, 238);
            checkedListBox1.TabIndex = 6;
            // 
            // EditLoops
            // 
            EditLoops.Location = new Point(411, 79);
            EditLoops.Name = "EditLoops";
            EditLoops.Size = new Size(31, 23);
            EditLoops.TabIndex = 7;
            EditLoops.Text = "1";
            // 
            // ProgressBarTest
            // 
            ProgressBarTest.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ProgressBarTest.Location = new Point(23, 470);
            ProgressBarTest.Name = "ProgressBarTest";
            ProgressBarTest.Size = new Size(906, 23);
            ProgressBarTest.TabIndex = 8;
            // 
            // ProgressBarTotal
            // 
            ProgressBarTotal.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ProgressBarTotal.Location = new Point(25, 509);
            ProgressBarTotal.Name = "ProgressBarTotal";
            ProgressBarTotal.Size = new Size(906, 23);
            ProgressBarTotal.TabIndex = 9;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(363, 82);
            label1.Name = "label1";
            label1.Size = new Size(42, 15);
            label1.TabIndex = 10;
            label1.Text = "Loops:";
            // 
            // FabricTesterForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(950, 592);
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
            MainMenuStrip = menuStrip1;
            Margin = new Padding(2);
            Name = "FabricTesterForm";
            Text = "Fabric Tester";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
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
    }
}