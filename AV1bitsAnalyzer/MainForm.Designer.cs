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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            VVVlc = new LibVLCSharp.WinForms.VideoView();
            BtnAbout = new Button();
            flowLayoutPanel1 = new FlowLayoutPanel();
            BtnOpen = new Button();
            BtnClose = new Button();
            BtnReopen = new Button();
            BtnExpand = new Button();
            button5 = new Button();
            PBarLoadding = new ProgressBar();
            flowLayoutPanel2 = new FlowLayoutPanel();
            BtnFormat = new Button();
            chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            splitContainer1 = new SplitContainer();
            FlowLayoutPlayerButtons = new FlowLayoutPanel();
            BtnPlay = new Button();
            BtnNextFrame = new Button();
            BtnPreviousFrame = new Button();
            BtnLoop = new Button();
            HexBoxDetail = new Be.Windows.Forms.HexBox();
            LVHexInfo = new ListView();
            tableLayoutPanel1 = new TableLayoutPanel();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            TVHeader = new TreeView();
            tabPage2 = new TabPage();
            TVFrame = new TreeView();
            ((System.ComponentModel.ISupportInitialize) VVVlc).BeginInit();
            flowLayoutPanel1.SuspendLayout();
            flowLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) chart1).BeginInit();
            ((System.ComponentModel.ISupportInitialize) splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            FlowLayoutPlayerButtons.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            SuspendLayout();
            // 
            // VVVlc
            // 
            VVVlc.BackColor = Color.DarkGray;
            VVVlc.Location = new Point(3, 3);
            VVVlc.MediaPlayer = null;
            VVVlc.Name = "VVVlc";
            VVVlc.Size = new Size(258, 179);
            VVVlc.TabIndex = 5;
            VVVlc.Text = "This content will be rendered over the video";
            // 
            // BtnAbout
            // 
            BtnAbout.Anchor =  AnchorStyles.Top | AnchorStyles.Right;
            BtnAbout.Image = Properties.Resources.av1_logo;
            BtnAbout.Location = new Point(1240, 20);
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
            flowLayoutPanel1.Controls.Add(BtnExpand);
            flowLayoutPanel1.Controls.Add(button5);
            flowLayoutPanel1.Location = new Point(5, 2);
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
            // BtnExpand
            // 
            BtnExpand.Image = Properties.Resources.compress;
            BtnExpand.Location = new Point(165, 3);
            BtnExpand.Name = "BtnExpand";
            BtnExpand.Size = new Size(48, 48);
            BtnExpand.TabIndex = 3;
            BtnExpand.UseVisualStyleBackColor = true;
            BtnExpand.Click += BtnExpand_Click;
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
            PBarLoadding.Location = new Point(4, 231);
            PBarLoadding.Maximum = 1000;
            PBarLoadding.Name = "PBarLoadding";
            PBarLoadding.Size = new Size(940, 2);
            PBarLoadding.Step = 1;
            PBarLoadding.TabIndex = 6;
            // 
            // flowLayoutPanel2
            // 
            flowLayoutPanel2.Controls.Add(BtnFormat);
            flowLayoutPanel2.Location = new Point(293, 20);
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
            // chart1
            // 
            chart1.Anchor =  AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            chartArea1.Name = "ChartArea1";
            chart1.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            chart1.Legends.Add(legend1);
            chart1.Location = new Point(5, 62);
            chart1.Name = "chart1";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StackedColumn;
            series1.Legend = "Legend1";
            series1.Name = "Intra";
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StackedColumn;
            series2.Legend = "Legend1";
            series2.Name = "Inter";
            series3.ChartArea = "ChartArea1";
            series3.Legend = "Legend1";
            series3.Name = "Size";
            chart1.Series.Add(series1);
            chart1.Series.Add(series2);
            chart1.Series.Add(series3);
            chart1.Size = new Size(1314, 163);
            chart1.TabIndex = 8;
            chart1.Text = "chart1";
            chart1.MouseDown += chart1_MouseDown;
            chart1.MouseWheel += Chart1_MouseWheel;
            // 
            // splitContainer1
            // 
            splitContainer1.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            splitContainer1.BorderStyle = BorderStyle.FixedSingle;
            splitContainer1.FixedPanel = FixedPanel.Panel2;
            splitContainer1.IsSplitterFixed = true;
            splitContainer1.Location = new Point(948, 538);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(VVVlc);
            splitContainer1.Panel1MinSize = 240;
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(FlowLayoutPlayerButtons);
            splitContainer1.Panel2MinSize = 68;
            splitContainer1.Size = new Size(369, 187);
            splitContainer1.SplitterDistance = 264;
            splitContainer1.TabIndex = 5;
            // 
            // FlowLayoutPlayerButtons
            // 
            FlowLayoutPlayerButtons.Controls.Add(BtnPlay);
            FlowLayoutPlayerButtons.Controls.Add(BtnNextFrame);
            FlowLayoutPlayerButtons.Controls.Add(BtnPreviousFrame);
            FlowLayoutPlayerButtons.Controls.Add(BtnLoop);
            FlowLayoutPlayerButtons.FlowDirection = FlowDirection.TopDown;
            FlowLayoutPlayerButtons.Location = new Point(13, 22);
            FlowLayoutPlayerButtons.Name = "FlowLayoutPlayerButtons";
            FlowLayoutPlayerButtons.Size = new Size(80, 140);
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
            // HexBoxDetail
            // 
            HexBoxDetail.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            HexBoxDetail.ColumnInfoVisible = true;
            HexBoxDetail.Font = new Font("Microsoft YaHei UI", 10F);
            HexBoxDetail.GroupSeparatorVisible = true;
            HexBoxDetail.LineInfoVisible = true;
            HexBoxDetail.Location = new Point(3, 538);
            HexBoxDetail.Name = "HexBoxDetail";
            HexBoxDetail.ReadOnly = true;
            HexBoxDetail.ShadowSelectionColor = Color.FromArgb(  100,   60,   188,   255);
            HexBoxDetail.Size = new Size(939, 187);
            HexBoxDetail.StringViewVisible = true;
            HexBoxDetail.TabIndex = 1;
            HexBoxDetail.UseFixedBytesPerLine = true;
            HexBoxDetail.VScrollBarVisible = true;
            // 
            // LVHexInfo
            // 
            LVHexInfo.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            LVHexInfo.FullRowSelect = true;
            LVHexInfo.GridLines = true;
            LVHexInfo.Location = new Point(3, 3);
            LVHexInfo.MultiSelect = false;
            LVHexInfo.Name = "LVHexInfo";
            LVHexInfo.Size = new Size(939, 529);
            LVHexInfo.TabIndex = 0;
            LVHexInfo.UseCompatibleStateImageBehavior = false;
            LVHexInfo.View = View.Details;
            LVHexInfo.SelectedIndexChanged += LVHexInfo_SelectedIndexChanged;
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
            tableLayoutPanel1.Controls.Add(tabControl1, 1, 0);
            tableLayoutPanel1.Location = new Point(2, 239);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 73.55769F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 26.4423084F));
            tableLayoutPanel1.Size = new Size(1320, 728);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Location = new Point(948, 3);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(369, 529);
            tabControl1.TabIndex = 6;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(TVHeader);
            tabPage1.Location = new Point(4, 26);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(361, 499);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Header";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // TVHeader
            // 
            TVHeader.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            TVHeader.Location = new Point(2, 3);
            TVHeader.Name = "TVHeader";
            TVHeader.Size = new Size(359, 490);
            TVHeader.TabIndex = 10;
            TVHeader.MouseMove += TVSpec_MouseMove;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(TVFrame);
            tabPage2.Location = new Point(4, 26);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(361, 499);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Frame";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // TVFrame
            // 
            TVFrame.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            TVFrame.Location = new Point(1, 4);
            TVFrame.Name = "TVFrame";
            TVFrame.Size = new Size(359, 490);
            TVFrame.TabIndex = 11;
            TVFrame.MouseMove += TVSpec_MouseMove;
            // 
            // MainForm
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1323, 971);
            Controls.Add(chart1);
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
            ((System.ComponentModel.ISupportInitialize) VVVlc).EndInit();
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize) chart1).EndInit();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize) splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            FlowLayoutPlayerButtons.ResumeLayout(false);
            FlowLayoutPlayerButtons.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private FlowLayoutPanel flowLayoutPanel1;
        private Button BtnOpen;
        private Button BtnClose;
        private Button BtnReopen;
        private Button BtnExpand;
        private Button button5;
        private Button BtnAbout;
        private LibVLCSharp.WinForms.VideoView VVVlc;
        private ProgressBar PBarLoadding;
        private FlowLayoutPanel flowLayoutPanel2;
        private Button BtnFormat;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private SplitContainer splitContainer1;
        private FlowLayoutPanel FlowLayoutPlayerButtons;
        private Button BtnPlay;
        private Button BtnNextFrame;
        private Button BtnPreviousFrame;
        private Button BtnLoop;
        private Be.Windows.Forms.HexBox HexBoxDetail;
        private ListView LVHexInfo;
        private TableLayoutPanel tableLayoutPanel1;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TreeView TVHeader;
        private TreeView TVFrame;
    }
}