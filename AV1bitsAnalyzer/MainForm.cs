using AV1bitsAnalyzer.Library;
using AV1bitsAnalyzer.Properties;
using Be.Windows.Forms;
using LibVLCSharp.Shared;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms.DataVisualization.Charting;

namespace AV1bitsAnalyzer
{
    struct ColInfos{
        int width = 0; 
        string header = string.Empty;
        public ColInfos (int width, string header) { Width = width; Header = header; }

        public int Width { readonly get => width; set => width = value; }
        public string Header { readonly get => header; set => header = value; }
    }

    public partial class MainForm : Form
    {
        private readonly ColInfos [] Cols = [
            new (250, "OBU Type"),
            new (50, "Offset"),
            new (100, "Size"),
            new (50, "TID"),
            new (50, "SID"),
            new (120, "I/P/B"),
        ];

        private AV1Demuxer? _parser;
        private OBPAnalyzerContext _decodeContext;
        private readonly List<FramesInfo> _frames = [];
        private string? _parseFilePath;
        private string _MainFrameTextDefault = $"MainForm {Resources.AppVersion} ";
        private bool _expand = true;

        public MainForm ()
        {
            InitializeComponent();
            CenterToParent();

            LVHexInfo.Columns.Clear();

            for ( int i = 0; i < Cols.Length; i++ )
            {
                ColumnHeader ch = new()
                {
                    Width = Cols[i].Width,
                    TextAlign = HorizontalAlignment.Left,
                    Text = Cols[i].Header,
                };
                LVHexInfo.Columns.Add(ch);
            }
            _ = UpdateListView();

            _decodeContext = new();
            Core.Initialize();
            Text = _MainFrameTextDefault;
            ConsChart();
        }

        void ConsChart ()
        {
            chart1.Series[0].ChartType = SeriesChartType.StackedColumn;
            chart1.Series[1].ChartType = SeriesChartType.StackedColumn;

            var sizeSeries = chart1.Series[2];
            sizeSeries.ChartType = SeriesChartType.Line;
            sizeSeries.MarkerStyle = MarkerStyle.Circle;
            sizeSeries.MarkerSize = 5;
            sizeSeries["LabelStyle"] = "Top";
            sizeSeries.LabelAngle = 45;
            sizeSeries.SmartLabelStyle.Enabled = false;

            //chart1.Legends[0].InsideChartArea = chart1.ChartAreas[0].Name;
            chart1.Legends[0].Enabled = false;

            var area = chart1.ChartAreas[0];

            area.Position.Auto = false;
            area.Position.X = 1;
            area.Position.Y = 1;
            area.Position.Width = 98;
            area.Position.Height = 100;

            area.AxisX.IsMarginVisible = false;
            area.AxisX.LineWidth = 1;
            area.AxisX.ScrollBar.IsPositionedInside = false;
            area.AxisX.ScrollBar.LineColor = Color.FromKnownColor(KnownColor.HighlightText);
            area.AxisX.ScaleView.Size = chart1.Width / 30;
            area.AxisX.ScaleView.Position = 0;
            area.AxisX.MajorGrid.Enabled = false;
            area.AxisX.MinorGrid.Enabled = false;
            area.AxisX.MinorTickMark.Enabled = true;
            area.AxisX.MinorTickMark.Interval = 2;
            area.AxisX.LabelStyle.Interval = 2;

            area.Area3DStyle.Enable3D = false;
            area.AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.SmallScroll;
            /*            area.AxisY.ScaleBreakStyle.Enabled = true;
                        area.AxisY.ScaleBreakStyle.BreakLineStyle = BreakLineStyle.Wave;
                        area.AxisY.ScaleBreakStyle.Spacing = 2;
                        area.AxisY.ScaleBreakStyle.LineColor = Color.Aqua;*/
            area.AxisY.LogarithmBase = 10;
            area.AxisY.IsLogarithmic = true;
            area.AxisY.MajorGrid.Enabled = false;
        }

        private void BtnAbout_Click (object sender, EventArgs e)
        {
            MessageBox.Show($"AV1 OBU Parser {Properties.Resources.AppVersion}\n\n\n - Support IVF, OBU in Annex B and Section5 format.\n\n * Built on {Properties.Resources.BuildDate}", "About Me", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void CreateNewParse (string filename)
        {
            if ( _parser != null )
            {
                Clear();
            }

            _parseFilePath = filename;
            var type = ParseType.ParseType_IVF;
            if ( File.Exists(_parseFilePath) )
            {
                _parser = new AV1Demuxer();
                bool ivf = BinaryProbe.AV1IVF(_parseFilePath);
                bool annexb = false;

                if ( !ivf )
                {
                    annexb = BinaryProbe.ObuAnnexB(_parseFilePath);
                }

                type = ivf ? ParseType.ParseType_IVF :
                    annexb ? ParseType.ParseType_OBU_AnnexB :
                    ParseType.ParseType_OBU_Section5;

                _parser.Parse(_parseFilePath, type, new FrameCallbackHandle(OnGetFrame));
            }

            var bmp = Resources.ivf;
            switch ( type )
            {
                case ParseType.ParseType_IVF:
                    bmp = Resources.ivf;
                    break;
                case ParseType.ParseType_OBU_Section5:
                    bmp = Resources.se5;
                    break;
                case ParseType.ParseType_OBU_AnnexB:
                    bmp = Resources.axb;
                    break;
            }

            BtnFormat.Image = bmp;

            this.Text = $"{_MainFrameTextDefault} ( {_parseFilePath} )";
        }

        private void BtnOpen_Click (object sender, EventArgs e)
        {
            using var fsd = new OpenFileDialog()
            {
                Title = "Select AV1 bitstream file(s)...",
                Filter = "AV1 OBU Files (.obu)|*.obu;*.obu;*.av1;*.ivf|All Files (*.*)|*.*"
            };
            if ( fsd.ShowDialog(this) == DialogResult.OK )
            {
                CreateNewParse(fsd.FileName);
            }
        }

        private void CreatePlayer (string path)
        {
            VVVlc.MediaPlayer?.Stop();

            var libVLC = new LibVLC();
            var media = new Media(libVLC, new Uri(path));
            var player = new MediaPlayer(media);
            media.Dispose();
            VVVlc.MediaPlayer = player;
            player.EnableKeyInput = false;
            player.EnableMouseInput = false;

            player.EndReached += (sender, e) =>
            {
                Task.Run(() =>
                {
                    if ( BtnLoop.ForeColor == Color.Blue )
                        CreatePlayer(path);
                    else
                    {
                        VVVlc.MediaPlayer?.Stop();
                        VVVlc.MediaPlayer = null;
                    }
                });
            };
            player.Play();
        }

        private void ReportProgress (float p1)
        {
            var act = new Action<float>( (p) =>
            {
                PBarLoadding.Value = (int) (p * 1000);
            });

            PBarLoadding.Invoke(act, p1);
        }

        private void OnGetFrame (ref FramesInfo v)
        {
            ReportProgress(v.Progress);

            if ( v.Address != long.MaxValue )
            {
                lock ( _frames )
                {
                    _frames.Add(v);
                }
            }
        }

        private async Task UpdateListView ()
        {
            bool update ()
            {
                List<FramesInfo> list;

                lock ( _frames )
                {
                    list = [.. _frames];
                    _frames.Clear();
                };

                if ( list.Count > 0 )
                {
                    LVHexInfo.BeginUpdate();
                    foreach ( var f in list )
                    {
                        ListViewGroup group = new(f.ToString(), HorizontalAlignment.Left)
                        {
                            Tag = f,
                            CollapsedState = ListViewGroupCollapsedState.Collapsed,
                        };
                        LVHexInfo.Groups.Add(group);

                        bool keyframe = false;
                        int frame_Length = 0;

                        Span<byte> od = new (f.Data);
                        foreach ( var obu in f.Obus )
                        {
                            Span<byte> obuslice = od[(obu.ObuOffset + obu.ObuDataOffset)..];
                            ObuParseRet obuRes = new()
                            {
                                size = obu.Size,
                                obuOffset = obu.ObuOffset,
                                obuDataOffset = obu.ObuDataOffset,
                                obuType = obu.Type,
                            };
                            ParsePacket(obu, obuslice, ref obuRes);
                            if ( obuRes.frameType == "I" )
                            {
                                keyframe = true;
                            }
                            string[] ss = [$"    {obu.Type}", $"{obu.ObuOffset}", $"{obu.Size}", $"{obu.Tid}", $"{obu.Sid}", $"{obuRes.frameType}"];
                            var lvi = new ListViewItem(ss, group)
                            {
                                Tag = obuRes
                            };
                            LVHexInfo.Items.Add(lvi);
                            frame_Length += obu.Size + obu.ObuDataOffset;
                        }

                        if ( keyframe )
                        {
                            chart1.Series["Intra"].Points.AddXY(f.FrameIdx, 1000);
                            chart1.Series["Inter"].Points.AddXY(f.FrameIdx, 0);
                            f.Frametype = "IDR       ";
                        }
                        else
                        {
                            chart1.Series["Intra"].Points.AddXY(f.FrameIdx, 0);
                            chart1.Series["Inter"].Points.AddXY(f.FrameIdx, 100);
                            f.Frametype = "Inter(P/B)";
                        }
                        {
                            DataPoint v = new ()
                            {
                                XValue = f.FrameIdx,
                                YValues = [frame_Length],
                                Label = frame_Length.ToString(),
                                //LabelAngle = 60,
                            };
                            chart1.Series[2].Points.Add(v);
                        }
                        group.Header = f.ToString();
                    }
                    LVHexInfo.EndUpdate();
                }

                return true;
            }

            while ( !IsDisposed )
            {
                await Task.FromResult(update());
                await Task.Delay(TimeSpan.FromMilliseconds(1000));
            }
        }

        private void Clear ()
        {
            _parser?.Stop();
            _frames.Clear();
            RBoxObu.Text = string.Empty;
            _parser = null;
            VVVlc.MediaPlayer?.Stop();
            VVVlc.MediaPlayer?.Dispose();
            VVVlc.MediaPlayer = null;
            LVHexInfo.Items.Clear();
            HexBoxDetail.ByteProvider = null;
            _decodeContext = new();
            PBarLoadding.Value = 0;
            foreach ( var s in chart1.Series )
            {
                s.Points.Clear();
            }
            Text = _MainFrameTextDefault;
        }

        private void BtnClose_Click (object sender, EventArgs e)
        {
            Clear();
        }

        private static string ParseFrameType (OBUFrameHeader fh)
        {
            string frametype = "-";
            if ( fh.frame_type == OBUFrameType.OBU_INTRA_ONLY_FRAME ||
                            fh.frame_type == OBUFrameType.OBU_KEY_FRAME )
            {
                frametype = "I";
            }
            else if ( fh.frame_type == OBUFrameType.OBU_INTER_FRAME )
            {
                if ( fh.show_existing_frame )
                {
                    frametype = "inter(showd_existing_frame)";
                }
                else if ( !fh.showable_frame && !fh.show_frame && !fh.show_existing_frame )
                {
                    frametype = "ARF";
                }
                else if ( fh.reference_select == false )
                {
                    frametype = "P";
                }
                else
                {
                    frametype = "inter(P/B)";
                }
            }

            return frametype;
        }
        internal struct ObuParseRet
        {
            public string headerAnalysisRes = string.Empty;
            public string frameType = "-";
            public int obuOffset = 0;
            public int obuDataOffset = 0;
            public int size = 0;
            public OBUType obuType = 0;

            public ObuParseRet ()
            {
            }
        };
        private bool ParsePacket (OBU v, Span<byte> obuData, ref ObuParseRet ret)
        {
            string targetString = string.Empty;
            OBPError err = new();
            var f = obuData;
            switch ( v.Type )
            {
                case OBUType.OBU_SEQUENCE_HEADER:
                    {
                        OBUSequenceHeader header = new();
                        ObuOperator.ObpParseSequenceHeader(f, ref header, ref err);
                        _decodeContext.seqheader = header;
                        ret.headerAnalysisRes = SpecString.ToSeqHeaderString(header);
                    }
                    break;
                case OBUType.OBU_TEMPORAL_DELIMITER:
                    {
                        _decodeContext.SeenFrameHeader = false;
                    }
                    break;
                case OBUType.OBU_TILE_GROUP:
                    {
                        ObuOperator.ObpParseTileGroup(f, ref _decodeContext, ref err);
                        if ( _decodeContext.tileGroup != null )
                            ret.headerAnalysisRes = _decodeContext.tileGroup.ToString();
                    }
                    break;
                case OBUType.OBU_METADATA:
                    {
                        OBUMetadata meta = new();
                        ObuOperator.ObpParseMetadata(f, ref meta, ref err);
                        ret.headerAnalysisRes = meta.ToString();
                    }
                    break;
                case OBUType.OBU_FRAME:
                    {
                        if ( _decodeContext.seqheader == null ) return false;

                        int tid = v.Tid, sid = v.Sid;
                        ObuOperator.ObpParseFrame(f, ref _decodeContext, tid, sid, ref err);
                        var fh = _decodeContext.curFrameHeader;
                        ret.frameType = ParseFrameType(fh);
                        var ssHeader = SpecString.ToFrameHeaderString(fh, _decodeContext.seqheader);
                        var tileGroupStrs = _decodeContext.tileGroup?.ToString();
                        var sb = new StringBuilder();
                        sb.Append(ssHeader);
                        sb.Append(tileGroupStrs);
                        ret.headerAnalysisRes = sb.ToString();
                    }
                    break;
                case OBUType.OBU_FRAME_HEADER:
                case OBUType.OBU_REDUNDANT_FRAME_HEADER:
                    {
                        if ( _decodeContext.seqheader == null ) return false;
                        int tid = v.Tid, sid = v.Sid;
                        ObuOperator.ObpParseFrameHeader(f, ref _decodeContext, tid, sid, ref err);
                        var fh = _decodeContext.curFrameHeader;
                        ret.frameType = ParseFrameType(fh);
                        ret.headerAnalysisRes = SpecString.ToFrameHeaderString(fh, _decodeContext.seqheader);
                    }
                    break;
                case OBUType.OBU_TILE_LIST:
                    {
                        OBUTileList lsit = new();
                        ObuOperator.ObpParseTileList(f, ref lsit, ref err);
                        ret.headerAnalysisRes = lsit.ToString();
                    }
                    break;
                case OBUType.OBU_PADDING:
                    break;
            }

            return true;
        }

        private void LVHexInfo_SelectedIndexChanged (object sender, EventArgs e)
        {
            var item = LVHexInfo.FocusedItem;
            ListViewGroup? gp = null;

            if ( item != null )
            {
                foreach ( ListViewGroup group in LVHexInfo.Groups )
                {
                    // 检查 item 是否在当前 group 的 Items 集合中
                    if ( group.Items.Contains(item) )
                    {
                        gp = group; break;
                    }
                }
                Debug.Assert(gp != null);
            }

            if ( item?.Tag != null && gp?.Tag != null )
            {
                FramesInfo v = (FramesInfo)gp.Tag;
                DynamicByteProvider provider = new(v.Data);
                ObuParseRet v2 = (ObuParseRet)item.Tag;
                HexBoxDetail.ByteProvider = provider;
                HexBoxDetail.SelectionStart = v2.obuOffset;
                HexBoxDetail.HighligedRegions.Clear();

                HexBox.HighlightedRegion region2 = new ()
                {
                    Start = v2.obuOffset,
                    Length = v2.obuDataOffset,
                    Color = Color.IndianRed
                };
                HexBox.HighlightedRegion region = new ()
                {
                    Start = v2.obuOffset + v2.obuDataOffset,
                    Length = v2.size,
                    Color = Color.CadetBlue
                };

                HexBoxDetail.HighligedRegions.Add(region);
                HexBoxDetail.HighligedRegions.Add(region2);

                if ( v2.obuType != OBUType.OBU_TEMPORAL_DELIMITER )
                {
                    RBoxObu.Invoke(() =>
                    {
                        RBoxObu.Text = v2.headerAnalysisRes;
                    });
                }

                var idx = (item.Index + 1) * 1.0f;
                var total = LVHexInfo.Items.Count;
                ReportProgress(idx / total);
            }
        }

        private void BtnPlay_Click (object sender, EventArgs e)
        {
            if ( VVVlc.MediaPlayer == null && _parseFilePath != null )
            {
                CreatePlayer(_parseFilePath);
            }
            else if ( VVVlc.MediaPlayer != null )
            {
                var player = VVVlc.MediaPlayer;

                switch ( player.State )
                {
                    case VLCState.Stopped:
                    case VLCState.Ended:
                        CreatePlayer(_parseFilePath!);
                        break;

                    case VLCState.Paused:
                        player.SetPause(false);
                        break;

                    default:
                        player.SetPause(true);
                        break;
                }
            }
        }

        private void BtnNextFrame_Click (object sender, EventArgs e)
        {
            if ( VVVlc.MediaPlayer == null ) { return; }

            var player = VVVlc.MediaPlayer;
            player.NextFrame();
        }

        private void BtnPreviousFrame_Click (object sender, EventArgs e)
        {
            if ( VVVlc.MediaPlayer == null ) { return; }

            var player = VVVlc.MediaPlayer;

            var fps = player.Fps;
            var duration = player.Time;
        }

        private void BtnLoop_Click (object sender, EventArgs e)
        {
            if ( BtnLoop.ForeColor == Color.Blue )
            {
                BtnLoop.ForeColor = Color.Black;
            }
            else
            {
                BtnLoop.ForeColor = Color.Blue;
            }
        }

        private void MainForm_DragEnter (object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void MainForm_DragDrop (object sender, DragEventArgs e)
        {
            if ( e.Data != null )
            {
                if ( e.Data.GetData(DataFormats.FileDrop, false) is string[] files )
                {
                    CreateNewParse(files[0]);
                }
            }
        }

        private void BtnReopen_Click (object sender, EventArgs e)
        {
            if ( _parseFilePath != null )
            {
                CreateNewParse(_parseFilePath);
            }
        }


        private void BtnExpand_Click (object sender, EventArgs e)
        {
            LVHexInfo.BeginUpdate();
            foreach ( ListViewGroup g in LVHexInfo.Groups )
            {
                g.CollapsedState = _expand ? ListViewGroupCollapsedState.Expanded : ListViewGroupCollapsedState.Collapsed;
            }
            LVHexInfo.EndUpdate();
            _expand = !_expand;
        }

        private void chart1_MouseDown (object sender, MouseEventArgs e)
        {
            HitTestResult result = chart1.HitTest( e.X, e.Y );

            if ( result.ChartElementType == ChartElementType.DataPoint
                || result.ChartElementType == ChartElementType.DataPointLabel )
            {
                var idx = result.PointIndex;

                var g = LVHexInfo.Groups[idx];
                if ( g != null )
                {
                    LVHexInfo.TopItem = g.Items[0];
                    g.Items[0].Selected = true;
                    LVHexInfo.EnsureVisible(g.Items[0].Index);
                }
            }
        }

        private void Chart1_MouseWheel (object sender, MouseEventArgs e)
        {
            int numberOfTextLinesToMove = e.Delta * SystemInformation.MouseWheelScrollLines / 120;
            int numberOfPixelsToMove = numberOfTextLinesToMove * 2;

            var max = chart1.Series[0].Points.Count - (chart1.Width/30 - 2);
            var view = chart1.ChartAreas[0].AxisX.ScaleView;
            var siz =  Math.Min(Math.Max(view.Position - numberOfPixelsToMove, 0), max);
            chart1.ChartAreas[0].AxisX.ScaleView.Position = siz;
        }
    }
}