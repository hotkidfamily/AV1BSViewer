using AV1bitsAnalyzer.Library;
using AV1bitsAnalyzer.Properties;
using Be.Windows.Forms;
using LibVLCSharp.Shared;
using System.Diagnostics;
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
            new (220, "OBU Type"),
            new (50, "Offset"),
            new (100, "Size"),
            new (50, "T,S id"),
            new (120, "I/P/B"),
            new (80, "Dec/Ord/Dsp"),
        ];

        private AV1Demuxer? _parser;
        private OBPAnalyzerContext _parserCtx;
        private readonly List<FramesInfo> _frames = [];
        private string? _parseFilePath;
        private string _MainFrameTextDefault = $"MainForm {Resources.AppVersion} ";
        private bool _expand = true;
        DynamicFileByteProvider? _dynamicFileByteProvider;

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

            _parserCtx = new();
            Core.Initialize();
            Text = _MainFrameTextDefault;
            ConsChart();
        }

        void ConsChart ()
        {
            chart1.Series[0].ChartType = SeriesChartType.StackedColumn;
            chart1.Series[1].ChartType = SeriesChartType.StackedColumn;

            var sizeSeries = chart1.Series["Size"];
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
            area.AxisY.IntervalType = DateTimeIntervalType.Number;
            area.AxisY.Interval = 6;
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

            var fs = new FileStream(_parseFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            _dynamicFileByteProvider = new DynamicFileByteProvider(fs);
            HexBoxDetail.ByteProvider = _dynamicFileByteProvider;
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
                            string v3 = obuRes.decode_frame_id == uint.MaxValue ? "-" : obuRes.decode_frame_id.ToString();
                            string v4 = obuRes.orderHint == -1 ? "-" : obuRes.orderHint.ToString();
                            string v5 = obuRes.display_frame_id == uint.MaxValue ? "-" : obuRes.display_frame_id.ToString();
                            List<string> ss = [$"    {obu.Type}", $"{obu.ObuOffset}", $"{obu.Size}", $"{obu.Tid}, {obu.Sid}"];
                            if ( obuRes.obuType == OBUType.OBU_FRAME || obuRes.obuType == OBUType.OBU_FRAME_HEADER )
                            {
                                ss.Add($"{obuRes.frameType}");
                                ss.Add($"{v3}, {v4}, {v5}");
                            }

                            var lvi = new ListViewItem(ss.ToArray(), group)
                            {
                                Tag = obuRes
                            };
                            LVHexInfo.Items.Add(lvi);
                            frame_Length += obu.Size + obu.ObuDataOffset;
                        }

                        chart1.Series["Intra"].Points.AddXY(f.FrameIdx, keyframe ? 1000 : 0);
                        chart1.Series["Inter"].Points.AddXY(f.FrameIdx, keyframe ? 0 : 100);
                        f.Frametype = keyframe ? "IDR       " : "Inter(P/B)";
                        DataPoint v = new ()
                        {
                            XValue = f.FrameIdx,
                            YValues = [frame_Length],
                            Label = frame_Length.ToString(),
                        };
                        chart1.Series["Size"].Points.Add(v);
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
            
            TVSpec.Nodes.Clear();
            TVSpec.Tag = null;

            _parser = null;
            VVVlc.MediaPlayer?.Stop();
            VVVlc.MediaPlayer?.Dispose();
            VVVlc.MediaPlayer = null;

            HexBoxDetail.ByteProvider = null;
            _parserCtx = new();
            PBarLoadding.Value = 0;
            foreach ( var s in chart1.Series )
            {
                s.Points.Clear();
            }

            LVHexInfo.Items.Clear();
            LVHexInfo.Groups.Clear();
            _expand = true;
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
                    frametype = "inter(show_existing)";
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

        private static void ParseFrameID (ref ObuParseRet ret, OBUFrameHeader fh, OBPAnalyzerContext ctx)
        {
            if ( fh.show_frame )
            {
                ret.display_frame_id = fh.order_hint;
            }
            else if ( fh.show_existing_frame )
            {
                ret.display_frame_id = ctx.RefOrderHint[fh.frame_to_show_map_idx];
                ret.orderHint = -1;
            }
            if ( !fh.show_existing_frame )
            {
                ret.decode_frame_id = ctx.seqheader!.frame_id_numbers_present_flag ? fh.current_frame_id : uint.MaxValue;
            }
        }

        internal struct ObuParseRet
        {
            //public string headerAnalysisRes = string.Empty;
            public Queue<STItem> st = new();
            public string frameType = "-";
            public int obuOffset = 0;
            public int obuDataOffset = 0;
            public int size = 0;
            public OBUType obuType = 0;
            public int orderHint = -1;
            public uint decode_frame_id = uint.MaxValue;
            public uint display_frame_id = uint.MaxValue;

            public ObuParseRet ()
            {
            }
        };
        private bool ParsePacket (OBU v, Span<byte> obuData, ref ObuParseRet ret)
        {
            OBPError err = new();
            var f = obuData;
            switch ( v.Type )
            {
                case OBUType.OBU_SEQUENCE_HEADER:
                    {
                        OBUSequenceHeader header = new();
                        ObuOperator.ObpParseSequenceHeader(f, ref header, ref err);
                        _parserCtx.seqheader = header;
                        //ret.headerAnalysisRes = header.ToString();// SpecString.ToSeqHeaderString(header);
                        ret.st = header.ToSpecTree();
                    }
                    break;
                case OBUType.OBU_TEMPORAL_DELIMITER:
                    {
                        _parserCtx.SeenFrameHeader = false;
                    }
                    break;
                case OBUType.OBU_TILE_GROUP:
                    {
                        ObuOperator.ObpParseTileGroup(f, ref _parserCtx, ref err);
                        if ( _parserCtx.tileGroup != null )
                            ret.st = _parserCtx.tileGroup.ToSpecTree();
                        //if ( _parserCtx.tileGroup != null )
                        //ret.headerAnalysisRes = _parserCtx.tileGroup.ToString();
                    }
                    break;
                case OBUType.OBU_METADATA:
                    {
                        OBUMetadata meta = new();
                        ObuOperator.ObpParseMetadata(f, ref meta, ref err);
                        //ret.headerAnalysisRes = meta.ToString();
                        ret.st = meta.ToSpecTree();
                    }
                    break;
                case OBUType.OBU_FRAME:
                    {
                        if ( _parserCtx.seqheader == null ) return false;

                        int tid = v.Tid, sid = v.Sid;
                        ObuOperator.ObpParseFrame(f, ref _parserCtx, tid, sid, ref err);
                        var fh = _parserCtx.curFrameHeader;
                        ret.frameType = ParseFrameType(fh!);
                        /*var ssHeader = fh!.ToString();// SpecString.ToFrameHeaderString(fh, _parserCtx.seqheader);
                        var tileGroupStrs = _parserCtx.tileGroup?.ToString();
                        var sb = new StringBuilder();
                        sb.Append(ssHeader);
                        sb.Append(tileGroupStrs);
                        ret.headerAnalysisRes = sb.ToString();*/
                        ret.st = fh.ToSpecTree();
                        foreach ( var v4 in _parserCtx.tileGroup.ToSpecTree() )
                        {
                            ret.st.Enqueue(v4);
                        }
                        ret.orderHint = fh.order_hint;
                        ret.orderHint = fh.order_hint;
                        ParseFrameID(ref ret, fh, _parserCtx);
                    }
                    break;
                case OBUType.OBU_FRAME_HEADER:
                case OBUType.OBU_REDUNDANT_FRAME_HEADER:
                    {
                        if ( _parserCtx.seqheader == null ) return false;
                        int tid = v.Tid, sid = v.Sid;
                        ObuOperator.ObpParseFrameHeader(f, ref _parserCtx, tid, sid, ref err);
                        var fh = _parserCtx.curFrameHeader;
                        ret.frameType = ParseFrameType(fh!);
                        //ret.headerAnalysisRes = fh.ToString();
                        // SpecString.ToFrameHeaderString(fh, _parserCtx.seqheader);
                        ret.st = fh!.ToSpecTree();
                        ret.orderHint = fh.order_hint;
                        ParseFrameID(ref ret, fh, _parserCtx);
                    }
                    break;
                case OBUType.OBU_TILE_LIST:
                    {
                        OBUTileList lsit = new();
                        ObuOperator.ObpParseTileList(f, ref lsit, ref err);
                        //ret.headerAnalysisRes = lsit.ToString();
                        ret.st = lsit.ToSpecTree();
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
                ObuParseRet pret = (ObuParseRet)item.Tag;
                HexBoxDetail.SelectionStart = v.Address + pret.obuOffset;
                HexBoxDetail.ScrollByteIntoView(HexBoxDetail.SelectionStart);
                HexBoxDetail.HighligedRegions.Clear();

                HexBox.HighlightedRegion region2 = new ()
                {
                    Start = (int)(v.Address + pret.obuOffset),
                    Length = pret.obuDataOffset,
                    Color = Color.IndianRed
                };
                HexBox.HighlightedRegion region = new ()
                {
                    Start = (int)(v.Address + pret.obuOffset + pret.obuDataOffset),
                    Length = pret.size,
                    Color = Color.CadetBlue
                };

                HexBoxDetail.HighligedRegions.Add(region);
                HexBoxDetail.HighligedRegions.Add(region2);

                if ( pret.obuType != OBUType.OBU_TEMPORAL_DELIMITER && pret.st.Count > 0 )
                {
                    Stack<TreeNode> roots = new();

                    var v2 = pret.st.Peek();
                    TreeNode main_root = new()
                    {
                        Text = "View",
                        Tag = -1,
                    };

                    roots.Push(main_root);

                    TreeNode root = roots.Peek();
                    foreach ( var v4 in pret.st )
                    {
                        var require_level = v4.level - 1;
                        if ( (int) root.Tag < require_level )
                        {
                            if ( root.Nodes.Count > 0 )
                            {
                                var previous = root.Nodes[root.Nodes.Count - 1];
                                roots.Push(previous);
                            }
                            root = roots.Peek();
                        }
                        else if ( (int) root.Tag > require_level )
                        {
                            while ( roots.Count > 0 )
                            {
                                roots.Pop();
                                root = roots.Peek();
                                if ( (int) root.Tag == require_level )
                                    break;
                            }
                        }

                        Debug.Assert((int) root.Tag == require_level);
                        TreeNode v5 = new ()
                        {
                            Text = v4.value,
                            Tag = v4.level
                        };
                        root.Nodes.Add(v5);

                    }

                    TVSpec.Invoke(() =>
                    {
                        TVSpec.BeginUpdate();
                        TVSpec.Nodes.Clear();
                        TVSpec.Nodes.Add(main_root);
                        TVSpec.TopNode = main_root;
                        TVSpec.ExpandAll();
                        main_root.EnsureVisible();
                        TVSpec.EndUpdate();
                    });
                }

                var idx = (item.Index + 1) * 1.0f;
                var total = LVHexInfo.Items.Count;
                ReportProgress(idx / total);

                chart1.ChartAreas[0].AxisX.ScaleView.Position = v.FrameIdx - 1;
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

        private void TVSpec_MouseMove (object sender, MouseEventArgs e)
        {
            TreeView _TreeView = (TreeView)sender;
            TreeNode _Node = _TreeView.GetNodeAt(e.X, e.Y);
            if ( _Node != null && _Node != _TreeView.Tag )
            {
                _TreeView.Refresh();
                Graphics _Graphics = Graphics.FromHwnd(_TreeView.Handle);
                _Graphics.FillRectangle(new SolidBrush(Color.FromArgb(100, 0, 0, 255)), new Rectangle(new Point(0, _Node.Bounds.Y), new Size(TVSpec.Width, _Node.Bounds.Height)));
                _Graphics.Dispose();
                _TreeView.Tag = _Node;
            }
        }
    }
}