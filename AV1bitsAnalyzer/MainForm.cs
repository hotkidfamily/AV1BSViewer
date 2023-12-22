using AV1bitsAnalyzer.Library;
using AV1bitsAnalyzer.Properties;
using Be.Windows.Forms;
using LibVLCSharp.Shared;
using System.Diagnostics;
using System.Text;

namespace AV1bitsAnalyzer
{
    struct ColInfos{
        int width = 0; 
        string header = string.Empty;
        public ColInfos (int width, string header) { Width = width; Header = header; }

        public int Width { get => width; set => width = value; }
        public string Header { get => header; set => header = value; }
    }

    public partial class MainForm : Form
    {
        private readonly ColInfos [] Cols = {
            new (100, "Address"),
            new (50, "Offset"),
            new (250, "OBU Type"),
            new (100, "Size"),
            new (100, "TID"),
            new (100, "SID"),
            new (50, "I/P/B"),
            };


        private AV1Demuxer? _parser;
        private OBPAnalyzerContext _decodeContext;
        private readonly List<FramesInfo> _frames = [];
        private string? _parseFilePath;
        private string _MainFrameTextDefault = "MainForm";

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
            this.Text = _MainFrameTextDefault;
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
            List<FramesInfo> list;
            while ( !IsDisposed )
            {
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
                        string target = string.Empty;
                        string type = "-";
                        ParsePacket(f, ref target, ref type);
                        f.PkgHeader = target;
                        f.Pkgtype = type;
                        var lvi = new ListViewItem(f.Info())
                        {
                            Tag = f
                        };
                        Debug.Assert(lvi.SubItems.Count == 7);
                        LVHexInfo.Items.Add(lvi);
                        if(f.Type == OBUType.OBU_TEMPORAL_DELIMITER )
                        {
                            lvi.BackColor = Color.GhostWhite;
                        }
                    }
                    LVHexInfo.EndUpdate();
                }

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
            this.Text = _MainFrameTextDefault;
        }

        private void BtnClose_Click (object sender, EventArgs e)
        {
            Clear();
        }

        private bool ParsePacket(FramesInfo v, ref string target, ref string frametype)
        {
            string targetString = string.Empty;
            OBPError err = new();
            var f = new Span<byte>(v.Data, (int)v.Offset, v.Size);
            switch ( v.Type )
            {
                case OBUType.OBU_SEQUENCE_HEADER:
                    {
                        OBUSequenceHeader header = new();
                        ObuOperator.ObpParseSequenceHeader(f, ref header, ref err);
                        _decodeContext.seqheader = header;
                        targetString = SpecString.ToSeqHeaderString(header);
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
                            targetString = _decodeContext.tileGroup.ToString();
                    }
                    break;
                case OBUType.OBU_METADATA:
                    {
                        OBUMetadata data = new();
                        ObuOperator.ObpParseMetadata(f, ref data, ref err);
                        var metas = SpecString.ToMetadataStrings(data);
                        var sb = new StringBuilder();
                        foreach ( string s in metas )
                        {
                            sb.Append(s);
                            sb.Append("\r\n");
                        }
                        targetString = sb.ToString();
                    }
                    break;
                case OBUType.OBU_FRAME:
                    {
                        if ( _decodeContext.seqheader == null ) return false;

                        int tid = v.Tid, sid = v.Sid;
                        ObuOperator.ObpParseFrame(f, ref _decodeContext, tid, sid, ref err);
                        var frameheader = _decodeContext.curFrameHeader;
                        if ( frameheader.frame_type == OBUFrameType.OBU_INTRA_ONLY_FRAME ||
                            frameheader.frame_type == OBUFrameType.OBU_KEY_FRAME )
                        {
                            frametype = "I";
                        }
                        else if ( frameheader.frame_type == OBUFrameType.OBU_INTER_FRAME )
                        {
                            if ( frameheader.reference_select == false )
                            {
                                frametype = "P";
                            }
                            else
                            {
                                frametype = "B";
                            }
                        }
                        var ssHeader = SpecString.ToFrameHeaderString(frameheader, _decodeContext.seqheader);
                        var tileGroupStrs = _decodeContext.tileGroup.ToString();
                        var sb = new StringBuilder();
                        sb.Append(ssHeader);
                        sb.Append(tileGroupStrs);
                        targetString = sb.ToString();
                    }
                    break;
                case OBUType.OBU_FRAME_HEADER:
                case OBUType.OBU_REDUNDANT_FRAME_HEADER:
                    {
                        if ( _decodeContext.seqheader == null ) return false;
                        int tid = v.Tid, sid = v.Sid;
                        ObuOperator.ObpParseFrameHeader(f, ref _decodeContext, tid, sid, ref err);
                        var frameheader = _decodeContext.curFrameHeader;

                        if ( frameheader.frame_type == OBUFrameType.OBU_INTRA_ONLY_FRAME ||
                            frameheader.frame_type == OBUFrameType.OBU_KEY_FRAME )
                        {
                            frametype = "I";
                        }
                        else if ( frameheader.frame_type == OBUFrameType.OBU_INTER_FRAME )
                        {
                            if ( frameheader.reference_select == false )
                            {
                                frametype = "P";
                            }
                            else
                            {
                                frametype = "B";
                            }
                        }
                        targetString = SpecString.ToFrameHeaderString(frameheader, _decodeContext.seqheader);
                    }
                    break;
                case OBUType.OBU_TILE_LIST:
                    {
                        OBUTileList lsit = new();
                        ObuOperator.ObpParseTileList(f, ref lsit, ref err);
                        targetString = lsit.ToString();
                    }
                    break;
                case OBUType.OBU_PADDING:
                    break;
            }
            target = targetString;

            return true;
        }

        private void LVHexInfo_SelectedIndexChanged (object sender, EventArgs e)
        {
            var item = LVHexInfo.FocusedItem;
            if ( item != null && item.Tag != null )
            {
                FramesInfo v = (FramesInfo)item.Tag;
                DynamicByteProvider provider = new(v.Data);
                HexBoxDetail.ByteProvider = provider;
                HexBoxDetail.LineInfoOffset = v.Address;
                HexBoxDetail.Select(0, v.Offset);

                if(v.Type != OBUType.OBU_TEMPORAL_DELIMITER )
                {
                    string targetString = v.PkgHeader;
                    RBoxObu.Invoke(() =>
                    {
                        RBoxObu.Text = targetString;
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
    }
}