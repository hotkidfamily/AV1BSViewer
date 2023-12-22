namespace AV1bitsAnalyzer
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose (bool disposing)
        {
            if ( disposing && (components != null) )
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
        private void InitializeComponent ()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            tableLayoutPanel1 = new TableLayoutPanel();
            LVHexInfo = new ListView();
            HexBoxDetail = new Be.Windows.Forms.HexBox();
            splitContainer1 = new SplitContainer();
            VVVlc = new LibVLCSharp.WinForms.VideoView();
            FlowLayoutPlayerButtons = new FlowLayoutPanel();
            BtnPlay = new Button();
            BtnNextFrame = new Button();
            BtnPreviousFrame = new Button();
            BtnLoop = new Button();
            RBoxObu = new RichTextBox();
            BtnAbout = new Button();
            flowLayoutPanel1 = new FlowLayoutPanel();
            BtnOpen = new Button();
            BtnClose = new Button();
            BtnReopen = new Button();
            button4 = new Button();
            button5 = new Button();
            PBarLoadding = new ProgressBar();
            flowLayoutPanel2 = new FlowLayoutPanel();
            BtnFormat = new Button();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) VVVlc).BeginInit();
            FlowLayoutPlayerButtons.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            flowLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 71.6F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28.4F));
            tableLayoutPanel1.Controls.Add(LVHexInfo, 0, 0);
            tableLayoutPanel1.Controls.Add(HexBoxDetail, 0, 1);
            tableLayoutPanel1.Controls.Add(splitContainer1, 1, 1);
            tableLayoutPanel1.Controls.Add(RBoxObu, 1, 0);
            tableLayoutPanel1.Location = new Point(12, 69);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 73.55769F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 26.4423084F));
            tableLayoutPanel1.Size = new Size(1163, 605);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // LVHexInfo
            // 
            LVHexInfo.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            LVHexInfo.FullRowSelect = true;
            LVHexInfo.GridLines = true;
            LVHexInfo.Location = new Point(3, 3);
            LVHexInfo.MultiSelect = false;
            LVHexInfo.Name = "LVHexInfo";
            LVHexInfo.Size = new Size(826, 439);
            LVHexInfo.TabIndex = 0;
            LVHexInfo.UseCompatibleStateImageBehavior = false;
            LVHexInfo.View = View.Details;
            LVHexInfo.SelectedIndexChanged += LVHexInfo_SelectedIndexChanged;
            // 
            // HexBoxDetail
            // 
            HexBoxDetail.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            HexBoxDetail.ColumnInfoVisible = true;
            HexBoxDetail.Font = new Font("Microsoft YaHei UI", 10F);
            HexBoxDetail.GroupSeparatorVisible = true;
            HexBoxDetail.LineInfoVisible = true;
            HexBoxDetail.Location = new Point(3, 448);
            HexBoxDetail.Name = "HexBoxDetail";
            HexBoxDetail.ReadOnly = true;
            HexBoxDetail.ShadowSelectionColor = Color.FromArgb(  100,   60,   188,   255);
            HexBoxDetail.Size = new Size(826, 154);
            HexBoxDetail.StringViewVisible = true;
            HexBoxDetail.TabIndex = 1;
            HexBoxDetail.UseFixedBytesPerLine = true;
            HexBoxDetail.VScrollBarVisible = true;
            // 
            // splitContainer1
            // 
            splitContainer1.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            splitContainer1.FixedPanel = FixedPanel.Panel2;
            splitContainer1.IsSplitterFixed = true;
            splitContainer1.Location = new Point(835, 448);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(VVVlc);
            splitContainer1.Panel1MinSize = 220;
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(FlowLayoutPlayerButtons);
            splitContainer1.Panel2MinSize = 68;
            splitContainer1.Size = new Size(325, 154);
            splitContainer1.SplitterDistance = 220;
            splitContainer1.TabIndex = 5;
            // 
            // VVVlc
            // 
            VVVlc.BackColor = Color.DarkGray;
            VVVlc.Location = new Point(1, 6);
            VVVlc.MediaPlayer = null;
            VVVlc.Name = "VVVlc";
            VVVlc.Size = new Size(217, 145);
            VVVlc.TabIndex = 5;
            VVVlc.Text = "This content will be rendered over the video";
            // 
            // FlowLayoutPlayerButtons
            // 
            FlowLayoutPlayerButtons.Controls.Add(BtnPlay);
            FlowLayoutPlayerButtons.Controls.Add(BtnNextFrame);
            FlowLayoutPlayerButtons.Controls.Add(BtnPreviousFrame);
            FlowLayoutPlayerButtons.Controls.Add(BtnLoop);
            FlowLayoutPlayerButtons.FlowDirection = FlowDirection.TopDown;
            FlowLayoutPlayerButtons.Location = new Point(0, 0);
            FlowLayoutPlayerButtons.Name = "FlowLayoutPlayerButtons";
            FlowLayoutPlayerButtons.Size = new Size(101, 154);
            FlowLayoutPlayerButtons.TabIndex = 6;
            // 
            // BtnPlay
            // 
            BtnPlay.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            BtnPlay.AutoSize = true;
            BtnPlay.Location = new Point(2, 2);
            BtnPlay.Margin = new Padding(2);
            BtnPlay.Name = "BtnPlay";
            BtnPlay.Size = new Size(69, 30);
            BtnPlay.TabIndex = 0;
            BtnPlay.Text = "▶️";
            BtnPlay.UseVisualStyleBackColor = true;
            BtnPlay.Click += BtnPlay_Click;
            // 
            // BtnNextFrame
            // 
            BtnNextFrame.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            BtnNextFrame.AutoSize = true;
            BtnNextFrame.Location = new Point(2, 36);
            BtnNextFrame.Margin = new Padding(2);
            BtnNextFrame.Name = "BtnNextFrame";
            BtnNextFrame.Size = new Size(69, 30);
            BtnNextFrame.TabIndex = 1;
            BtnNextFrame.Text = "⏩";
            BtnNextFrame.UseVisualStyleBackColor = true;
            BtnNextFrame.Click += BtnNextFrame_Click;
            // 
            // BtnPreviousFrame
            // 
            BtnPreviousFrame.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            BtnPreviousFrame.AutoSize = true;
            BtnPreviousFrame.Location = new Point(2, 70);
            BtnPreviousFrame.Margin = new Padding(2);
            BtnPreviousFrame.Name = "BtnPreviousFrame";
            BtnPreviousFrame.Size = new Size(69, 30);
            BtnPreviousFrame.TabIndex = 2;
            BtnPreviousFrame.Text = "⏪";
            BtnPreviousFrame.UseVisualStyleBackColor = true;
            BtnPreviousFrame.Click += BtnPreviousFrame_Click;
            // 
            // BtnLoop
            // 
            BtnLoop.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            BtnLoop.AutoSize = true;
            BtnLoop.Location = new Point(2, 104);
            BtnLoop.Margin = new Padding(2);
            BtnLoop.Name = "BtnLoop";
            BtnLoop.Size = new Size(69, 30);
            BtnLoop.TabIndex = 3;
            BtnLoop.Text = "🔁";
            BtnLoop.UseVisualStyleBackColor = true;
            BtnLoop.Click += BtnLoop_Click;
            // 
            // RBoxObu
            // 
            RBoxObu.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            RBoxObu.Location = new Point(835, 3);
            RBoxObu.Name = "RBoxObu";
            RBoxObu.ReadOnly = true;
            RBoxObu.Size = new Size(325, 439);
            RBoxObu.TabIndex = 4;
            RBoxObu.Text = "";
            RBoxObu.WordWrap = false;
            // 
            // BtnAbout
            // 
            BtnAbout.Anchor =  AnchorStyles.Top | AnchorStyles.Right;
            BtnAbout.Image = Properties.Resources.av1_logo;
            BtnAbout.Location = new Point(1096, 20);
            BtnAbout.Name = "BtnAbout";
            BtnAbout.Size = new Size(79, 33);
            BtnAbout.TabIndex = 5;
            BtnAbout.UseVisualStyleBackColor = true;
            BtnAbout.Click += BtnAbout_Click;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(BtnOpen);
            flowLayoutPanel1.Controls.Add(BtnClose);
            flowLayoutPanel1.Controls.Add(BtnReopen);
            flowLayoutPanel1.Controls.Add(button4);
            flowLayoutPanel1.Controls.Add(button5);
            flowLayoutPanel1.Location = new Point(12, 3);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(282, 54);
            flowLayoutPanel1.TabIndex = 2;
            // 
            // BtnOpen
            // 
            BtnOpen.Image = Properties.Resources.add;
            BtnOpen.Location = new Point(3, 3);
            BtnOpen.Name = "BtnOpen";
            BtnOpen.Size = new Size(48, 48);
            BtnOpen.TabIndex = 0;
            BtnOpen.UseVisualStyleBackColor = true;
            BtnOpen.Click += BtnOpen_Click;
            // 
            // BtnClose
            // 
            BtnClose.Image = Properties.Resources.close;
            BtnClose.Location = new Point(57, 3);
            BtnClose.Name = "BtnClose";
            BtnClose.Size = new Size(48, 48);
            BtnClose.TabIndex = 1;
            BtnClose.UseVisualStyleBackColor = true;
            BtnClose.Click += BtnClose_Click;
            // 
            // BtnReopen
            // 
            BtnReopen.Image = Properties.Resources.refresh;
            BtnReopen.Location = new Point(111, 3);
            BtnReopen.Name = "BtnReopen";
            BtnReopen.Size = new Size(48, 48);
            BtnReopen.TabIndex = 2;
            BtnReopen.UseVisualStyleBackColor = true;
            BtnReopen.Click += BtnReopen_Click;
            // 
            // button4
            // 
            button4.Image = Properties.Resources.dialpad;
            button4.Location = new Point(165, 3);
            button4.Name = "button4";
            button4.Size = new Size(48, 48);
            button4.TabIndex = 3;
            button4.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            button5.Image = Properties.Resources.dialpad;
            button5.Location = new Point(219, 3);
            button5.Name = "button5";
            button5.Size = new Size(48, 48);
            button5.TabIndex = 4;
            button5.UseVisualStyleBackColor = true;
            // 
            // PBarLoadding
            // 
            PBarLoadding.Location = new Point(15, 62);
            PBarLoadding.Maximum = 1000;
            PBarLoadding.Name = "PBarLoadding";
            PBarLoadding.Size = new Size(824, 2);
            PBarLoadding.Step = 1;
            PBarLoadding.TabIndex = 6;
            // 
            // flowLayoutPanel2
            // 
            flowLayoutPanel2.Controls.Add(BtnFormat);
            flowLayoutPanel2.Location = new Point(300, 20);
            flowLayoutPanel2.Name = "flowLayoutPanel2";
            flowLayoutPanel2.Size = new Size(158, 36);
            flowLayoutPanel2.TabIndex = 7;
            // 
            // BtnFormat
            // 
            BtnFormat.Enabled = false;
            BtnFormat.Image = Properties.Resources.add;
            BtnFormat.Location = new Point(3, 3);
            BtnFormat.Name = "BtnFormat";
            BtnFormat.Size = new Size(32, 32);
            BtnFormat.TabIndex = 0;
            BtnFormat.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1187, 686);
            Controls.Add(flowLayoutPanel2);
            Controls.Add(PBarLoadding);
            Controls.Add(flowLayoutPanel1);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(BtnAbout);
            Icon = (Icon) resources.GetObject("$this.Icon");
            MinimumSize = new Size(1035, 725);
            Name = "MainForm";
            Text = "MainForm";
            DragDrop += MainForm_DragDrop;
            DragEnter += MainForm_DragEnter;
            tableLayoutPanel1.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize) splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize) VVVlc).EndInit();
            FlowLayoutPlayerButtons.ResumeLayout(false);
            FlowLayoutPlayerButtons.PerformLayout();
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private TableLayoutPanel tableLayoutPanel1;
        private ListView LVHexInfo;
        private Be.Windows.Forms.HexBox HexBoxDetail;
        private FlowLayoutPanel flowLayoutPanel1;
        private Button BtnOpen;
        private Button BtnClose;
        private Button BtnReopen;
        private Button button4;
        private Button button5;
        private Button BtnAbout;
        private RichTextBox RBoxObu;
        private LibVLCSharp.WinForms.VideoView VVVlc;
        private Button BtnPlay;
        private Button BtnNextFrame;
        private Button BtnPreviousFrame;
        private Button BtnLoop;
        private SplitContainer splitContainer1;
        private FlowLayoutPanel FlowLayoutPlayerButtons;
        private ProgressBar PBarLoadding;
        private FlowLayoutPanel flowLayoutPanel2;
        private Button BtnFormat;
    }
}