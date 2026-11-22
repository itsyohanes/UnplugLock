// UnplugLock - lock the PC when the charger gets pulled.
// Sticks to C# 5 / .NET 4.x so it builds with the csc.exe that comes with Windows.

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;

namespace UnplugLock
{
    internal static class Program
    {
        [STAThread]
        private static int Main(string[] args)
        {
            // used by build.cmd to generate the icon
            if (args.Length >= 2 && args[0] == "--make-icon")
            {
                Logo.WriteIco(args[1]);
                if (args.Length >= 3)
                    using (var bmp = Logo.Render(256, true))
                        bmp.Save(args[2], System.Drawing.Imaging.ImageFormat.Png);
                return 0;
            }

            // running as UnplugLockHelper.exe
            if (args.Length >= 1 && args[0] == "--guard")
                return Guard.RunHelper(args);

            Ui.Init();

            bool isFirst;
            using (var mutex = new Mutex(true, @"Local\UnplugLock.SingleInstance", out isFirst))
            {
                if (!isFirst)
                {
                    // already running - tell it to show the tray icon in case it's hidden
                    try
                    {
                        using (var signal = EventWaitHandle.OpenExisting(TrayApp.ShowSignalName))
                            signal.Set();
                    }
                    catch
                    {
                        MessageBox.Show("UnplugLock is already running.", "UnplugLock",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    return 0;
                }

                Application.Run(new TrayApp());
            }
            return 0;
        }
    }

    // plug-shaped padlock logo
    internal static class Logo
    {
        private static readonly int[] IcoSizes = { 16, 24, 32, 48, 64, 128, 256 };

        public static Bitmap Render(int size, bool enabled)
        {
            var bmp = new Bitmap(size, size, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.Clear(Color.Transparent);
                g.ScaleTransform(size / 256f, size / 256f);
                Draw(g, enabled, size <= 24);
            }
            return bmp;
        }

        // a plug whose head is a padlock: two prongs on top, keyhole in the body, cord running off the bottom.
        // coords are for 256x256, chunkier prongs and keyhole for tiny sizes
        private static void Draw(Graphics g, bool enabled, bool small)
        {
            Color tileColor = enabled ? Color.FromArgb(44, 107, 224) : Color.FromArgb(128, 128, 128);

            // background
            using (var path = RoundedRect(new RectangleF(8, 8, 240, 240), 56))
            using (var brush = new SolidBrush(tileColor))
                g.FillPath(brush, path);

            using (var white = new SolidBrush(Color.White))
            {
                // prongs
                float prong = small ? 32 : 26;
                float gap = small ? 26 : 32;
                float left = 128 - gap / 2 - prong;
                foreach (float x in new[] { left, 128 + gap / 2 })
                    using (var path = RoundedRect(new RectangleF(x, 40, prong, 88), prong / 2))
                        g.FillPath(white, path);

                // body
                using (var path = RoundedRect(new RectangleF(50, 104, 156, 112), 30))
                    g.FillPath(white, path);

                // cord, down to the bottom edge of the tile
                g.FillRectangle(white, 114, 200, 28, 48);
            }

            // keyhole
            float r = small ? 22 : 17;
            float stem = small ? 20 : 16;
            using (var brush = new SolidBrush(tileColor))
            {
                g.FillEllipse(brush, 128 - r, 150 - r, r * 2, r * 2);
                using (var path = RoundedRect(new RectangleF(128 - stem / 2, 154, stem, 32), stem / 2))
                    g.FillPath(brush, path);
            }
        }

        private static GraphicsPath RoundedRect(RectangleF r, float radius)
        {
            float d = radius * 2;
            var p = new GraphicsPath();
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }

        // bitmaps for small sizes, png for 64+ (.NET can't read small png frames)
        public static void WriteIco(string path)
        {
            var frames = new byte[IcoSizes.Length][];
            for (int i = 0; i < IcoSizes.Length; i++)
                using (var bmp = Render(IcoSizes[i], true))
                {
                    if (IcoSizes[i] >= 64)
                    {
                        using (var ms = new System.IO.MemoryStream())
                        {
                            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                            frames[i] = ms.ToArray();
                        }
                    }
                    else
                    {
                        frames[i] = ToDib(bmp);
                    }
                }

            using (var fs = System.IO.File.Create(path))
            using (var w = new System.IO.BinaryWriter(fs))
            {
                w.Write((short)0);                 // reserved
                w.Write((short)1);                 // type: icon
                w.Write((short)IcoSizes.Length);   // image count

                int offset = 6 + 16 * IcoSizes.Length;
                for (int i = 0; i < IcoSizes.Length; i++)
                {
                    int s = IcoSizes[i];
                    w.Write((byte)(s >= 256 ? 0 : s)); // width (0 means 256)
                    w.Write((byte)(s >= 256 ? 0 : s)); // height
                    w.Write((byte)0);                  // palette colours
                    w.Write((byte)0);                  // reserved
                    w.Write((short)1);                 // colour planes
                    w.Write((short)32);                // bits per pixel
                    w.Write(frames[i].Length);         // data size
                    w.Write(offset);                   // data offset
                    offset += frames[i].Length;
                }
                foreach (var f in frames)
                    w.Write(f);
            }
        }

        // raw 32bpp DIB frame for the .ico
        private static byte[] ToDib(Bitmap bmp)
        {
            int s = bmp.Width;
            var data = bmp.LockBits(new Rectangle(0, 0, s, s),
                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var pixels = new byte[s * s * 4];
            for (int y = 0; y < s; y++)
                Marshal.Copy(IntPtr.Add(data.Scan0, y * data.Stride), pixels, y * s * 4, s * 4);
            bmp.UnlockBits(data);

            int maskStride = ((s + 31) / 32) * 4;
            using (var ms = new System.IO.MemoryStream())
            using (var w = new System.IO.BinaryWriter(ms))
            {
                w.Write(40);          // BITMAPINFOHEADER size
                w.Write(s);           // width
                w.Write(s * 2);       // height (colour + mask)
                w.Write((short)1);    // planes
                w.Write((short)32);   // bpp
                w.Write(0);           // BI_RGB
                w.Write(0); w.Write(0); w.Write(0); w.Write(0); w.Write(0);

                for (int y = s - 1; y >= 0; y--)         // bottom-up rows
                    w.Write(pixels, y * s * 4, s * 4);
                w.Write(new byte[maskStride * s]);       // AND mask, unused (we have alpha)
                return ms.ToArray();
            }
        }
    }

    // process guard: UnplugLock and UnplugLockHelper.exe (a copy of this exe) restart each other,
    // and both deny terminate/suspend to normal processes. an elevated admin can still kill them.
    // shared look for every window: colours, fonts, spacing (scaled for the screen's DPI), buttons and the tray menu
    internal static class Ui
    {
        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        public static readonly Color Brand = Color.FromArgb(44, 107, 224);
        public static readonly Color BrandHover = Color.FromArgb(36, 92, 201);
        public static readonly Color BrandPressed = Color.FromArgb(29, 79, 179);
        public static readonly Color BrandBand = Color.FromArgb(31, 87, 195);
        public static readonly Color Ink = Color.FromArgb(27, 27, 27);
        public static readonly Color Muted = Color.FromArgb(96, 96, 96);
        public static readonly Color Line = Color.FromArgb(229, 229, 229);
        public static readonly Color FooterBack = Color.FromArgb(243, 243, 243);
        public static readonly Color Error = Color.FromArgb(196, 43, 28);

        private static float _scale;
        private static Font _body, _small, _title, _strong;

        // call first thing, before any window or icon is made, so text and icons are sharp on scaled screens
        public static void Init()
        {
            try { SetProcessDPIAware(); }
            catch { /* old Windows, stays blurry */ }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
        }

        public static float Scale
        {
            get
            {
                if (_scale == 0)
                    using (var g = Graphics.FromHwnd(IntPtr.Zero))
                        _scale = g.DpiX / 96f;
                return _scale;
            }
        }

        public static int S(float px) { return (int)Math.Round(px * Scale); }
        public static Padding S(float l, float t, float r, float b) { return new Padding(S(l), S(t), S(r), S(b)); }

        public static Font Body { get { return _body ?? (_body = new Font("Segoe UI", 9.75f)); } }
        public static Font Small { get { return _small ?? (_small = new Font("Segoe UI", 9f)); } }
        public static Font Strong { get { return _strong ?? (_strong = new Font("Segoe UI Semibold", 9.75f)); } }
        public static Font Title { get { return _title ?? (_title = new Font("Segoe UI Semibold", 14.25f)); } }

        public static Label Text(string text, int width, Color color, Font font, float spaceAfter)
        {
            return new TextLabel
            {
                Text = text,
                AutoSize = true,
                MaximumSize = new Size(width, 0),
                ForeColor = color,
                Font = font,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 0, 0, S(spaceAfter))
            };
        }

        public static GraphicsPath Rounded(RectangleF r, float radius)
        {
            float d = Math.Min(radius * 2, Math.Min(r.Width, r.Height));
            var p = new GraphicsPath();
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }

        public static void StyleMenu(ToolStripDropDown menu)
        {
            menu.Renderer = new MenuRenderer();
            menu.Font = Body;
            menu.Padding = S(4, 4, 4, 4);
            foreach (ToolStripItem item in menu.Items)
            {
                if (item is ToolStripSeparator)
                    continue;
                item.Padding = S(4, 4, 8, 4);
                var sub = item as ToolStripMenuItem;
                if (sub != null && sub.HasDropDownItems)
                    StyleMenu(sub.DropDown);
            }
        }

        private sealed class MenuColors : ProfessionalColorTable
        {
            public override Color ToolStripDropDownBackground { get { return Color.White; } }
            public override Color ImageMarginGradientBegin { get { return Color.White; } }
            public override Color ImageMarginGradientMiddle { get { return Color.White; } }
            public override Color ImageMarginGradientEnd { get { return Color.White; } }
            public override Color MenuBorder { get { return Color.FromArgb(214, 214, 214); } }
            public override Color MenuItemBorder { get { return Color.Transparent; } }
            public override Color MenuItemSelected { get { return Color.FromArgb(240, 240, 240); } }
            public override Color SeparatorDark { get { return Line; } }
            public override Color SeparatorLight { get { return Color.White; } }
        }

        // flat, light menu with rounded hover rows, like the Windows 11 ones
        private sealed class MenuRenderer : ToolStripProfessionalRenderer
        {
            public MenuRenderer() : base(new MenuColors()) { RoundedEdges = false; }

            protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
            {
                if (!e.Item.Selected || !e.Item.Enabled)
                    return;
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                var r = new RectangleF(S(2), 0, e.Item.Width - S(4), e.Item.Height - 1);
                using (var path = Rounded(r, S(4)))
                using (var brush = new SolidBrush(Color.FromArgb(240, 240, 240)))
                    e.Graphics.FillPath(brush, path);
            }

            protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
            {
                int y = e.Item.Height / 2;
                using (var pen = new Pen(Line))
                    e.Graphics.DrawLine(pen, S(8), y, e.Item.Width - S(8), y);
            }

            protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
            {
                // just a tick, no box behind it
                var r = e.ImageRectangle;
                float x = r.X + r.Width * 0.2f, y = r.Y + r.Height * 0.52f, w = r.Width * 0.6f;
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var pen = new Pen(e.Item.Enabled ? Ink : Color.FromArgb(160, 160, 160), Math.Max(1.5f, 1.6f * Scale)))
                {
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;
                    pen.LineJoin = LineJoin.Round;
                    e.Graphics.DrawLines(pen, new[]
                    {
                        new PointF(x, y), new PointF(x + w * 0.35f, y + w * 0.32f), new PointF(x + w, y - w * 0.38f)
                    });
                }
            }

            protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
            {
                e.TextColor = e.Item.Enabled ? Ink : Color.FromArgb(128, 128, 128);
                base.OnRenderItemText(e);
            }

            protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
            {
                e.ArrowColor = Muted;
                base.OnRenderArrow(e);
            }
        }
    }

    // a label with no built-in side padding, so text in different sizes lines up on the same left edge
    internal sealed class TextLabel : Label
    {
        private const TextFormatFlags Flags =
            TextFormatFlags.NoPadding | TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl;

        public TextLabel()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            int max = MaximumSize.Width > 0 ? MaximumSize.Width - Padding.Horizontal : int.MaxValue;
            var s = TextRenderer.MeasureText(Text.Length == 0 ? " " : Text, Font, new Size(max, int.MaxValue), Flags);
            return new Size(Math.Min(s.Width, max) + Padding.Horizontal, s.Height + Padding.Vertical);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var r = new Rectangle(Padding.Left, Padding.Top, Width - Padding.Horizontal, Height - Padding.Vertical);
            TextRenderer.DrawText(e.Graphics, Text, Font, r, ForeColor, Flags);
        }
    }

    // rounded push button, blue when it's the main action
    internal sealed class RoundButton : Button
    {
        private readonly bool _primary;
        private bool _hover, _down;

        public RoundButton(string text, bool primary)
        {
            _primary = primary;
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            Font = Ui.Body;
            Margin = new Padding(Ui.S(8), 0, 0, 0);
            Text = text;
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            int w = TextRenderer.MeasureText(Text, Font).Width + Ui.S(40);
            Size = new Size(Math.Max(Ui.S(96), w), Ui.S(32));
        }

        protected override void OnMouseEnter(EventArgs e) { _hover = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _hover = false; _down = false; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnMouseDown(MouseEventArgs e) { _down = true; Invalidate(); base.OnMouseDown(e); }
        protected override void OnMouseUp(MouseEventArgs e) { _down = false; Invalidate(); base.OnMouseUp(e); }
        protected override void OnEnabledChanged(EventArgs e) { Invalidate(); base.OnEnabledChanged(e); }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            // corners show whatever is behind; the button rows are transparent, so use the first solid colour up the tree
            Color behind = SystemColors.Control;
            for (Control c = Parent; c != null; c = c.Parent)
                if (c.BackColor.A == 255)
                {
                    behind = c.BackColor;
                    break;
                }
            g.Clear(behind);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Color fill, border, text;
            if (_primary)
            {
                fill = !Enabled ? Color.FromArgb(160, 187, 238) : _down ? Ui.BrandPressed : _hover ? Ui.BrandHover : Ui.Brand;
                border = fill;
                text = Color.White;
            }
            else
            {
                fill = _down ? Color.FromArgb(235, 235, 235) : _hover ? Color.FromArgb(246, 246, 246) : Color.White;
                border = Color.FromArgb(208, 208, 208);
                text = Enabled ? Ui.Ink : Color.FromArgb(160, 160, 160);
            }

            var r = new RectangleF(0.5f, 0.5f, Width - 1.5f, Height - 1.5f);
            using (var path = Ui.Rounded(r, Ui.S(5)))
            {
                using (var brush = new SolidBrush(fill))
                    g.FillPath(brush, path);
                using (var pen = new Pen(border))
                    g.DrawPath(pen, path);
            }

            // keyboard focus ring
            if (Focused && ShowFocusCues)
            {
                var f = new RectangleF(1.5f, 1.5f, Width - 3.5f, Height - 3.5f);
                using (var path = Ui.Rounded(f, Ui.S(4)))
                using (var pen = new Pen(_primary ? Color.White : Ui.Brand, Math.Max(1f, Ui.Scale)))
                    g.DrawPath(pen, path);
            }

            TextRenderer.DrawText(g, Text, Font, ClientRectangle, text,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine);
        }
    }

    // every UnplugLock window: a header with the logo and a title, a white body, and a grey footer with the buttons
    internal class Dialog : Form
    {
        protected readonly int ContentWidth;
        protected readonly TableLayoutPanel Body;
        protected readonly TableLayoutPanel Footer;
        protected readonly FlowLayoutPanel Buttons;
        protected readonly Label Heading;
        protected readonly Label Subheading;

        protected Dialog(string windowTitle, string heading, string subheading, int width, bool banner)
        {
            ContentWidth = Ui.S(width);
            int outer = ContentWidth + Ui.S(48);

            Text = windowTitle;
            Icon = TrayApp.AppIcon;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            AutoScaleMode = AutoScaleMode.None; // sizes below are already scaled
            Font = Ui.Body;
            BackColor = Color.White;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;

            var root = new TableLayoutPanel
            {
                ColumnCount = 1,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, outer));

            // header
            var header = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 2,
                AutoSize = true,
                Dock = DockStyle.Fill,
                Margin = Padding.Empty,
                Padding = banner ? Ui.S(24, 22, 24, 22) : Ui.S(24, 24, 24, 4),
                BackColor = banner ? Ui.BrandBand : Color.White
            };
            int logoSize = Ui.S(banner ? 52 : 40);
            var logo = new PictureBox
            {
                Image = Logo.Render(logoSize, true),
                Size = new Size(logoSize, logoSize),
                Margin = new Padding(0, 0, Ui.S(16), 0),
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Left // centred on the title and subtitle
            };
            header.Controls.Add(logo, 0, 0);
            header.SetRowSpan(logo, 2);

            int textWidth = ContentWidth - logoSize - Ui.S(16);
            Heading = Ui.Text(heading, textWidth, banner ? Color.White : Ui.Ink, Ui.Title, 2);
            Heading.Margin = new Padding(0, Ui.S(banner ? 3 : 0), 0, Ui.S(2));
            Subheading = Ui.Text(subheading, textWidth, banner ? Color.FromArgb(214, 228, 255) : Ui.Muted, Ui.Body, 0);
            header.Controls.Add(Heading, 1, 0);
            header.Controls.Add(Subheading, 1, 1);
            root.Controls.Add(header);

            Body = new TableLayoutPanel
            {
                ColumnCount = 1,
                AutoSize = true,
                Dock = DockStyle.Fill,
                Margin = Padding.Empty,
                Padding = Ui.S(24, 20, 24, 24),
                BackColor = Color.White
            };
            root.Controls.Add(Body);

            // footer: a note or link on the left, buttons on the right
            Footer = new TableLayoutPanel
            {
                ColumnCount = 2,
                AutoSize = true,
                Dock = DockStyle.Fill,
                Margin = Padding.Empty,
                Padding = Ui.S(24, 14, 24, 14),
                BackColor = Ui.FooterBack
            };
            Footer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            Footer.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            Footer.Paint += delegate(object s, PaintEventArgs e)
            {
                using (var pen = new Pen(Ui.Line))
                    e.Graphics.DrawLine(pen, 0, 0, Footer.Width, 0);
            };
            Buttons = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true,
                WrapContents = false,
                Margin = Padding.Empty,
                Anchor = AnchorStyles.Right,
                BackColor = Color.Transparent
            };
            Footer.Controls.Add(Buttons, 1, 0);
            root.Controls.Add(Footer);

            Controls.Add(root);
        }

        protected Label AddText(string text, Color color, Font font, float spaceAfter)
        {
            var label = Ui.Text(text, ContentWidth, color, font, spaceAfter);
            Body.Controls.Add(label);
            return label;
        }

        protected void SetFooterNote(Control note)
        {
            note.Anchor = AnchorStyles.Left;
            note.Margin = Padding.Empty;
            Footer.Controls.Add(note, 0, 0);
        }

        // laid out right to left: add Cancel first so the main button sits to its left
        protected RoundButton AddButton(string text, bool primary)
        {
            var b = new RoundButton(text, primary);
            Buttons.Controls.Add(b);
            return b;
        }
    }

    internal static class Guard
    {
        public const string StopSignalName = @"Local\UnplugLock.GuardStop"; // main -> helper: stand down
        public const string QuitSignalName = @"Local\UnplugLock.Quit";      // setup -> main: exit cleanly
        public const string HelperName = "UnplugLockHelper";

        private const int DACL_SECURITY_INFORMATION = 4;
        private const int PROCESS_TERMINATE = 0x0001;
        private const int PROCESS_SUSPEND_RESUME = 0x0800;
        private const int SYNCHRONIZE = 0x00100000;

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetCurrentProcess();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern SafeWaitHandle OpenProcess(int access, bool inherit, int pid);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool GetKernelObjectSecurity(IntPtr handle, int info, byte[] sd, uint length, out uint needed);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetKernelObjectSecurity(IntPtr handle, int info, byte[] sd);

        private static byte[] _originalDacl;

        public static string HelperPath
        {
            get { return Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), HelperName + ".exe"); }
        }

        public static void Protect()
        {
            if (_originalDacl != null)
                return;
            try
            {
                IntPtr self = GetCurrentProcess();
                uint needed;
                GetKernelObjectSecurity(self, DACL_SECURITY_INFORMATION, null, 0, out needed);
                var buf = new byte[needed];
                if (!GetKernelObjectSecurity(self, DACL_SECURITY_INFORMATION, buf, needed, out needed))
                    return;

                var sd = new RawSecurityDescriptor(buf, 0);
                var everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
                var dacl = sd.DiscretionaryAcl;
                if (dacl == null)
                {
                    // null dacl = full access for everyone, keep that apart from the deny
                    dacl = new RawAcl(GenericAcl.AclRevision, 2);
                    dacl.InsertAce(0, new CommonAce(AceFlags.None, AceQualifier.AccessAllowed, 0x1FFFFF, everyone, false, null));
                }
                dacl.InsertAce(0, new CommonAce(AceFlags.None, AceQualifier.AccessDenied,
                    PROCESS_TERMINATE | PROCESS_SUSPEND_RESUME, everyone, false, null));
                sd.DiscretionaryAcl = dacl;

                var updated = new byte[sd.BinaryLength];
                sd.GetBinaryForm(updated, 0);
                if (SetKernelObjectSecurity(self, DACL_SECURITY_INFORMATION, updated))
                    _originalDacl = buf;
            }
            catch { /* run unprotected */ }
        }

        public static void Unprotect()
        {
            if (_originalDacl == null)
                return;
            if (SetKernelObjectSecurity(GetCurrentProcess(), DACL_SECURITY_INFORMATION, _originalDacl))
                _originalDacl = null;
        }

        public static Process StartHelper()
        {
            string self = Application.ExecutablePath;
            try
            {
                var src = new FileInfo(self);
                var dst = new FileInfo(HelperPath);
                if (!dst.Exists || dst.Length != src.Length || dst.LastWriteTimeUtc != src.LastWriteTimeUtc)
                    File.Copy(self, HelperPath, true);
            }
            catch { /* probably an old helper still running from it, use what's there */ }

            var psi = new ProcessStartInfo(HelperPath,
                "--guard " + Process.GetCurrentProcess().Id + " \"" + self + "\"")
            {
                UseShellExecute = false,
                WorkingDirectory = Path.GetDirectoryName(self)
            };
            return Process.Start(psi);
        }

        // helper: wait for the main app to die, restart it unless told to stand down
        public static int RunHelper(string[] args)
        {
            int pid;
            if (args.Length < 3 || !int.TryParse(args[1], out pid))
                return 1;
            string appPath = args[2];

            Protect();

            EventWaitHandle stop;
            try { stop = EventWaitHandle.OpenExisting(StopSignalName); }
            catch { return 0; } // main isn't running a guard

            using (stop)
            {
                using (var handle = OpenProcess(SYNCHRONIZE, false, pid))
                {
                    if (!handle.IsInvalid)
                    {
                        using (var mainExited = new ManualResetEvent(false))
                        {
                            mainExited.SafeWaitHandle = handle;
                            WaitHandle.WaitAny(new WaitHandle[] { stop, mainExited });
                            mainExited.SafeWaitHandle = new SafeWaitHandle(IntPtr.Zero, false); // handle is disposed by the using above
                        }
                    }
                }

                if (stop.WaitOne(0))
                    return 0;

                Thread.Sleep(300);
                if (stop.WaitOne(0) || !Settings.Load().Guard)
                    return 0;

                try
                {
                    Process.Start(new ProcessStartInfo(appPath)
                    {
                        UseShellExecute = false,
                        WorkingDirectory = Path.GetDirectoryName(appPath)
                    });
                }
                catch { /* nothing we can do */ }
            }
            return 0;
        }
    }

    // settings live in HKCU\Software\UnplugLock, plus a hidden backup in %LOCALAPPDATA%\UnplugLock.
    // once a password is set the key is locked against edits, and the running app puts back
    // anything that gets changed or deleted.
    internal sealed class Settings
    {
        private const string KeyPath = @"Software\UnplugLock";
        private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string RunValueName = "UnplugLock";
        private const int HashIterations = 200000;
        private const string CodeChars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // no 0/O or 1/I

        private const RegistryRights LockedRights =
            RegistryRights.SetValue | RegistryRights.CreateSubKey | RegistryRights.Delete;

        public bool Enabled = true;
        public int DelaySeconds = 0; // 0 = lock instantly
        public bool HideIcon = false;
        public bool Guard = true;

        // only PBKDF2 hashes + salts get stored
        private string _passwordHash;
        private string _passwordSalt;
        private string _recoveryHash;
        private string _recoverySalt;

        public bool HasPassword
        {
            get { return !string.IsNullOrEmpty(_passwordHash) && !string.IsNullOrEmpty(_passwordSalt); }
        }

        public bool HasRecoveryCode
        {
            get { return !string.IsNullOrEmpty(_recoveryHash) && !string.IsNullOrEmpty(_recoverySalt); }
        }

        public static string BackupDir
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "UnplugLock");
            }
        }

        private static string BackupPath { get { return Path.Combine(BackupDir, "settings.dat"); } }

        private static SecurityIdentifier Everyone
        {
            get { return new SecurityIdentifier(WellKnownSidType.WorldSid, null); }
        }

        // ---- load / save

        public static Settings Load()
        {
            Settings reg = ReadRegistry();
            Settings bak = ReadBackup();

            Settings s;
            if (reg != null && bak != null)
                s = Merge(reg, bak);
            else
                s = reg ?? bak ?? new Settings();

            // one copy missing or tampered with - repair both from the merged result
            if ((reg != null || bak != null) && (!s.SameAs(reg) || !s.SameAs(bak)))
                s.Save();
            return s;
        }

        // if the copies disagree, keep whichever is stricter
        private static Settings Merge(Settings reg, Settings bak)
        {
            var s = reg.Clone();
            if (!reg.HasPassword && bak.HasPassword)
            {
                s._passwordHash = bak._passwordHash;
                s._passwordSalt = bak._passwordSalt;
                s._recoveryHash = bak._recoveryHash;
                s._recoverySalt = bak._recoverySalt;
            }
            s.Enabled = reg.Enabled || bak.Enabled;
            s.Guard = reg.Guard || bak.Guard;
            s.DelaySeconds = Math.Min(reg.DelaySeconds, bak.DelaySeconds);
            return s;
        }

        public void Save()
        {
            try { WriteRegistry(); }
            catch { /* not fatal */ }
            try { WriteBackup(); }
            catch { /* not fatal */ }
        }

        // true if both stored copies still match what's in memory
        public bool IsInSync()
        {
            return SameAs(ReadRegistry()) && SameAs(ReadBackup()) && (!HasPassword || IsLocked());
        }

        public static void DeleteAll()
        {
            try { Unlock(); }
            catch { /* key may not exist */ }
            Registry.CurrentUser.DeleteSubKeyTree(KeyPath, false);
            try
            {
                if (Directory.Exists(BackupDir))
                {
                    foreach (var f in Directory.GetFiles(BackupDir))
                        File.SetAttributes(f, FileAttributes.Normal);
                    Directory.Delete(BackupDir, true);
                }
            }
            catch { /* best effort */ }
        }

        private Settings Clone()
        {
            return (Settings)MemberwiseClone();
        }

        private bool SameAs(Settings o)
        {
            return o != null &&
                   o.Enabled == Enabled && o.DelaySeconds == DelaySeconds &&
                   o.HideIcon == HideIcon && o.Guard == Guard &&
                   o._passwordHash == _passwordHash && o._passwordSalt == _passwordSalt &&
                   o._recoveryHash == _recoveryHash && o._recoverySalt == _recoverySalt;
        }

        private static Settings ReadRegistry()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(KeyPath))
                {
                    if (key == null)
                        return null;
                    var s = new Settings();
                    s.Enabled = Convert.ToInt32(key.GetValue("Enabled", 1)) != 0;
                    s.DelaySeconds = Math.Max(0, Convert.ToInt32(key.GetValue("DelaySeconds", 0)));
                    s.HideIcon = Convert.ToInt32(key.GetValue("HideIcon", 0)) != 0;
                    s.Guard = Convert.ToInt32(key.GetValue("Guard", 1)) != 0;
                    s._passwordHash = NullIfEmpty(key.GetValue("PasswordHash") as string);
                    s._passwordSalt = NullIfEmpty(key.GetValue("PasswordSalt") as string);
                    s._recoveryHash = NullIfEmpty(key.GetValue("RecoveryHash") as string);
                    s._recoverySalt = NullIfEmpty(key.GetValue("RecoverySalt") as string);
                    return s;
                }
            }
            catch
            {
                return null;
            }
        }

        private void WriteRegistry()
        {
            Unlock();
            using (var key = Registry.CurrentUser.CreateSubKey(KeyPath))
            {
                key.SetValue("Enabled", Enabled ? 1 : 0, RegistryValueKind.DWord);
                key.SetValue("DelaySeconds", DelaySeconds, RegistryValueKind.DWord);
                key.SetValue("HideIcon", HideIcon ? 1 : 0, RegistryValueKind.DWord);
                key.SetValue("Guard", Guard ? 1 : 0, RegistryValueKind.DWord);
                SetOrDelete(key, "PasswordHash", _passwordHash);
                SetOrDelete(key, "PasswordSalt", _passwordSalt);
                SetOrDelete(key, "RecoveryHash", _recoveryHash);
                SetOrDelete(key, "RecoverySalt", _recoverySalt);
            }
            if (HasPassword)
                Lock();
        }

        private static void SetOrDelete(RegistryKey key, string name, string value)
        {
            if (string.IsNullOrEmpty(value))
                key.DeleteValue(name, false);
            else
                key.SetValue(name, value, RegistryValueKind.String);
        }

        private static Settings ReadBackup()
        {
            try
            {
                if (!File.Exists(BackupPath))
                    return null;
                var s = new Settings();
                foreach (var line in File.ReadAllLines(BackupPath))
                {
                    int eq = line.IndexOf('=');
                    if (eq <= 0)
                        continue;
                    string name = line.Substring(0, eq);
                    string value = line.Substring(eq + 1);
                    switch (name)
                    {
                        case "Enabled": s.Enabled = value == "1"; break;
                        case "DelaySeconds": s.DelaySeconds = Math.Max(0, int.Parse(value)); break;
                        case "HideIcon": s.HideIcon = value == "1"; break;
                        case "Guard": s.Guard = value == "1"; break;
                        case "PasswordHash": s._passwordHash = NullIfEmpty(value); break;
                        case "PasswordSalt": s._passwordSalt = NullIfEmpty(value); break;
                        case "RecoveryHash": s._recoveryHash = NullIfEmpty(value); break;
                        case "RecoverySalt": s._recoverySalt = NullIfEmpty(value); break;
                    }
                }
                return s;
            }
            catch
            {
                return null;
            }
        }

        private void WriteBackup()
        {
            Directory.CreateDirectory(BackupDir);
            if (File.Exists(BackupPath))
                File.SetAttributes(BackupPath, FileAttributes.Normal);
            File.WriteAllLines(BackupPath, new[]
            {
                "Enabled=" + (Enabled ? "1" : "0"),
                "DelaySeconds=" + DelaySeconds,
                "HideIcon=" + (HideIcon ? "1" : "0"),
                "Guard=" + (Guard ? "1" : "0"),
                "PasswordHash=" + _passwordHash,
                "PasswordSalt=" + _passwordSalt,
                "RecoveryHash=" + _recoveryHash,
                "RecoverySalt=" + _recoverySalt
            });
            File.SetAttributes(BackupPath, FileAttributes.Hidden);
        }

        private static string NullIfEmpty(string s)
        {
            return string.IsNullOrEmpty(s) ? null : s;
        }

        // ---- registry key lock (deny writes to everyone; we're the owner so we can lift it)

        private static RegistryKey OpenForPermissions()
        {
            return Registry.CurrentUser.OpenSubKey(KeyPath, RegistryKeyPermissionCheck.ReadWriteSubTree,
                RegistryRights.ReadPermissions | RegistryRights.ChangePermissions);
        }

        private static RegistryAccessRule DenyRule()
        {
            return new RegistryAccessRule(Everyone, LockedRights, AccessControlType.Deny);
        }

        private static void Lock()
        {
            using (var key = OpenForPermissions())
            {
                if (key == null)
                    return;
                var sec = key.GetAccessControl();
                sec.AddAccessRule(DenyRule());
                key.SetAccessControl(sec);
            }
        }

        public static void Unlock()
        {
            using (var key = OpenForPermissions())
            {
                if (key == null)
                    return;
                var sec = key.GetAccessControl();
                sec.RemoveAccessRuleAll(DenyRule());
                key.SetAccessControl(sec);
            }
        }

        private static bool IsLocked()
        {
            try
            {
                using (var key = OpenForPermissions())
                {
                    if (key == null)
                        return false;
                    var rules = key.GetAccessControl().GetAccessRules(true, false, typeof(SecurityIdentifier));
                    foreach (RegistryAccessRule r in rules)
                        if (r.AccessControlType == AccessControlType.Deny && r.IdentityReference == Everyone &&
                            (r.RegistryRights & LockedRights) == LockedRights)
                            return true;
                    return false;
                }
            }
            catch
            {
                return true; // can't tell - don't thrash
            }
        }

        // ---- password + recovery code

        // null removes the password (and its recovery code)
        public void SetPassword(string password, string recoveryCode = null)
        {
            if (string.IsNullOrEmpty(password))
            {
                _passwordHash = _passwordSalt = _recoveryHash = _recoverySalt = null;
                return;
            }
            Hash(password, out _passwordHash, out _passwordSalt);
            if (!string.IsNullOrEmpty(recoveryCode))
                Hash(NormalizeCode(recoveryCode), out _recoveryHash, out _recoverySalt);
            else
                _recoveryHash = _recoverySalt = null;
        }

        public bool CheckPassword(string password)
        {
            if (!HasPassword)
                return true;
            return Matches(password, _passwordHash, _passwordSalt);
        }

        public bool CheckRecoveryCode(string code)
        {
            return HasRecoveryCode && Matches(NormalizeCode(code), _recoveryHash, _recoverySalt);
        }

        // 25 chars from a 32-char alphabet = 125 bits, shown as XXXXX-XXXXX-XXXXX-XXXXX-XXXXX
        public static string NewRecoveryCode()
        {
            var bytes = new byte[25];
            using (var rng = new RNGCryptoServiceProvider())
                rng.GetBytes(bytes);
            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                if (i > 0 && i % 5 == 0)
                    sb.Append('-');
                sb.Append(CodeChars[bytes[i] % 32]); // 256 % 32 == 0, no bias
            }
            return sb.ToString();
        }

        // ignore case, spaces and dashes
        private static string NormalizeCode(string code)
        {
            var sb = new System.Text.StringBuilder();
            foreach (char c in (code ?? "").ToUpperInvariant())
                if (char.IsLetterOrDigit(c))
                    sb.Append(c);
            return sb.ToString();
        }

        private static void Hash(string secret, out string hash, out string salt)
        {
            var saltBytes = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
                rng.GetBytes(saltBytes);
            salt = Convert.ToBase64String(saltBytes);
            hash = Convert.ToBase64String(Derive(secret, saltBytes));
        }

        private static bool Matches(string secret, string hash, string salt)
        {
            try
            {
                byte[] expected = Convert.FromBase64String(hash);
                byte[] actual = Derive(secret ?? "", Convert.FromBase64String(salt));
                if (expected.Length != actual.Length)
                    return false;
                int diff = 0;
                for (int i = 0; i < expected.Length; i++)
                    diff |= expected[i] ^ actual[i]; // constant time
                return diff == 0;
            }
            catch
            {
                return false;
            }
        }

        private static byte[] Derive(string secret, byte[] salt)
        {
            using (var kdf = new Rfc2898DeriveBytes(secret, salt, HashIterations, HashAlgorithmName.SHA256))
                return kdf.GetBytes(32);
        }

        public static bool StartsWithWindows
        {
            get
            {
                try
                {
                    using (var key = Registry.CurrentUser.OpenSubKey(RunKeyPath))
                    {
                        var v = key == null ? null : key.GetValue(RunValueName) as string;
                        return !string.IsNullOrEmpty(v);
                    }
                }
                catch { return false; }
            }
            set
            {
                using (var key = Registry.CurrentUser.CreateSubKey(RunKeyPath))
                {
                    if (value)
                        key.SetValue(RunValueName, "\"" + Application.ExecutablePath + "\"");
                    else
                        key.DeleteValue(RunValueName, false);
                }
            }
        }
    }

    internal enum AskResult { Cancelled, Entered, Recovered }

    internal sealed class PasswordDialog : Dialog
    {
        private enum Mode { Enter, ChooseNew, RecoveryCode }

        private const int MinLength = 4;

        private readonly Mode _mode;
        private readonly Settings _settings; // for "Forgot password?"
        private readonly TextBox _first;
        private readonly TextBox _second; // only when setting a new password
        private readonly Label _error;

        private static string TitleFor(Mode mode)
        {
            return mode == Mode.ChooseNew ? "Choose a password"
                 : mode == Mode.RecoveryCode ? "Enter your recovery code"
                 : "Enter your password";
        }

        // prompts are written as "...to do this:" - read better as a sentence under the title
        private static string Sentence(string prompt)
        {
            return prompt.EndsWith(":") ? prompt.Substring(0, prompt.Length - 1) + "." : prompt;
        }

        private PasswordDialog(string prompt, Mode mode, Settings settings)
            : base("UnplugLock", TitleFor(mode), Sentence(prompt), 340, false)
        {
            _mode = mode;
            _settings = settings;
            TopMost = true;

            AddText(mode == Mode.ChooseNew ? "New password" : mode == Mode.RecoveryCode ? "Recovery code" : "Password",
                Ui.Ink, Ui.Strong, 6);
            _first = new TextBox
            {
                UseSystemPasswordChar = mode != Mode.RecoveryCode,
                Width = ContentWidth,
                Margin = Padding.Empty
            };
            if (mode == Mode.RecoveryCode)
            {
                _first.CharacterCasing = CharacterCasing.Upper;
                _first.Font = new Font("Consolas", 11f);
            }
            Body.Controls.Add(_first);

            if (mode == Mode.ChooseNew)
            {
                AddText("At least " + MinLength + " characters.", Ui.Muted, Ui.Small, 0).Margin = Ui.S(0, 6, 0, 16);
                AddText("Confirm password", Ui.Ink, Ui.Strong, 6);
                _second = new TextBox { UseSystemPasswordChar = true, Width = ContentWidth, Margin = Padding.Empty };
                Body.Controls.Add(_second);
            }

            _error = AddText("", Ui.Error, Ui.Small, 0);
            _error.Margin = Ui.S(0, 10, 0, 0);
            _error.Visible = false;

            if (mode == Mode.Enter && settings != null && settings.HasRecoveryCode)
            {
                var forgot = new LinkLabel
                {
                    Text = "Forgot password?",
                    AutoSize = true,
                    LinkBehavior = LinkBehavior.HoverUnderline,
                    LinkColor = Ui.Brand,
                    ActiveLinkColor = Ui.BrandPressed,
                    BackColor = Color.Transparent
                };
                forgot.LinkClicked += OnForgot;
                SetFooterNote(forgot);
            }

            var cancel = AddButton("Cancel", false);
            cancel.DialogResult = DialogResult.Cancel;
            var ok = AddButton(mode == Mode.ChooseNew ? "Set password" : "Continue", true);
            ok.Click += OnOk;

            AcceptButton = ok;
            CancelButton = cancel;
            Shown += delegate { Activate(); _first.Focus(); };
        }

        private void ShowError(string message)
        {
            _error.Text = message;
            _error.Visible = true;
        }

        private void OnOk(object sender, EventArgs e)
        {
            if (_mode == Mode.ChooseNew)
            {
                if (_first.Text.Length < MinLength)
                {
                    ShowError("Use at least " + MinLength + " characters.");
                    _first.Focus();
                    return;
                }
                if (_first.Text != _second.Text)
                {
                    ShowError("The passwords don't match.");
                    _second.SelectAll();
                    _second.Focus();
                    return;
                }
            }
            else if (_mode == Mode.RecoveryCode && _first.Text.Trim().Length == 0)
            {
                return;
            }
            DialogResult = DialogResult.OK;
        }

        private void OnForgot(object sender, LinkLabelLinkClickedEventArgs e)
        {
            while (true)
            {
                string code;
                using (var d = new PasswordDialog(
                    "Enter your recovery code. It removes your UnplugLock password so you can set a new one.",
                    Mode.RecoveryCode, null))
                {
                    if (d.ShowDialog(this) != DialogResult.OK)
                        return;
                    code = d._first.Text;
                }

                if (_settings.CheckRecoveryCode(code))
                {
                    DialogResult = DialogResult.Retry; // = recovered
                    return;
                }

                Thread.Sleep(1000); // slow down guessing
                MessageBox.Show(this, "That recovery code isn't right.", "UnplugLock",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // asks for the current password; "Forgot password?" returns Recovered once a valid code is entered
        public static AskResult Ask(string prompt, Settings settings, out string password)
        {
            using (var d = new PasswordDialog(prompt, Mode.Enter, settings))
            {
                var r = d.ShowDialog();
                password = null;
                if (r == DialogResult.Retry)
                    return AskResult.Recovered;
                if (r != DialogResult.OK)
                    return AskResult.Cancelled;
                password = d._first.Text;
                return AskResult.Entered;
            }
        }

        public static string AskNew()
        {
            using (var d = new PasswordDialog(
                "You'll need it to turn off, change the settings of, or exit UnplugLock.",
                Mode.ChooseNew, null))
                return d.ShowDialog() == DialogResult.OK ? d._first.Text : null;
        }
    }

    // shown once, right after a password is set - the only time the recovery code is ever visible
    internal sealed class RecoveryCodeDialog : Dialog
    {
        private readonly string _code;
        private bool _confirmed;

        private RecoveryCodeDialog(string code)
            : base("UnplugLock", "Save your recovery code", "It's the only way back in if you forget your password.", 420, false)
        {
            _code = code;
            ControlBox = false;
            TopMost = true;

            AddText("If you forget your password, click \"Forgot password?\" on the password prompt " +
                    "and enter this code. It removes the password so you can set a new one.", Ui.Ink, Ui.Body, 10);
            AddText("Keep it somewhere other than this laptop, like your phone, a password manager or on paper. " +
                    "It won't be shown again.", Ui.Ink, Ui.Body, 18);

            var codeBox = new TextBox
            {
                Text = code,
                ReadOnly = true,
                Font = new Font("Consolas", 15f, FontStyle.Bold),
                TextAlign = HorizontalAlignment.Center,
                Width = ContentWidth,
                BackColor = Color.FromArgb(243, 247, 255),
                ForeColor = Ui.Ink,
                Margin = Ui.S(0, 0, 0, 12)
            };
            Body.Controls.Add(codeBox);

            var copy = new RoundButton("Copy", false) { Margin = Padding.Empty };
            var save = new RoundButton("Save to file...", false);
            copy.Click += delegate
            {
                try
                {
                    Clipboard.SetText(_code);
                    copy.Text = "Copied";
                }
                catch { /* clipboard busy */ }
            };
            save.Click += delegate { SaveToFile(); };
            var tools = new FlowLayoutPanel { AutoSize = true, WrapContents = false, Margin = Ui.S(0, 0, 0, 18) };
            tools.Controls.Add(copy);
            tools.Controls.Add(save);
            Body.Controls.Add(tools);

            var check = new CheckBox { Text = "I've saved my recovery code", AutoSize = true, Margin = Padding.Empty };
            Body.Controls.Add(check);

            var ok = AddButton("Done", true);
            ok.Enabled = false;
            check.CheckedChanged += delegate { ok.Enabled = check.Checked; };
            ok.Click += delegate
            {
                _confirmed = true;
                DialogResult = DialogResult.OK;
            };
            AcceptButton = ok;

            // no closing it (alt+f4 etc.) until they confirm
            FormClosing += delegate(object sender, FormClosingEventArgs e)
            {
                if (!_confirmed && e.CloseReason == CloseReason.UserClosing)
                    e.Cancel = true;
            };
            Shown += delegate { Activate(); codeBox.SelectionLength = 0; check.Focus(); };
        }

        private void SaveToFile()
        {
            using (var dlg = new SaveFileDialog
            {
                FileName = "UnplugLock recovery code.txt",
                Filter = "Text file (*.txt)|*.txt",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            })
            {
                if (dlg.ShowDialog(this) != DialogResult.OK)
                    return;
                try
                {
                    File.WriteAllText(dlg.FileName,
                        "UnplugLock recovery code\r\n\r\n" + _code + "\r\n\r\n" +
                        "If you forget your UnplugLock password, click \"Forgot password?\" on the password prompt\r\n" +
                        "and enter this code. Created " + DateTime.Now.ToString("d MMMM yyyy") + ".\r\n");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Couldn't save the file:\n" + ex.Message, "UnplugLock",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        public static void Present(string code)
        {
            using (var d = new RecoveryCodeDialog(code))
                d.ShowDialog();
        }
    }

    // Locking does nothing useful if the Windows account has no password - the lock screen
    // just has a "Sign in" button. Only local accounts can be blank; domain, Entra and
    // Microsoft accounts always have one.
    internal static class WindowsSignIn
    {
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool LogonUser(string user, string domain, string password,
            int logonType, int provider, out IntPtr token);

        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(IntPtr handle);

        private const int LogonNetwork = 3;
        private const int ProviderDefault = 0;
        // blank passwords are only allowed at the console, so a non-console logon with one fails like this
        private const int ErrorAccountRestriction = 1327;

        public const string SettingsPage = "ms-settings:signinoptions";

        // Tries an empty password for the current user. On an account that has a password this
        // is one failed logon, so only call it at startup or while we already think it's blank.
        public static bool HasNoPassword()
        {
            if (!string.Equals(Environment.UserDomainName, Environment.MachineName, StringComparison.OrdinalIgnoreCase))
                return false; // not a local account

            IntPtr token;
            if (LogonUser(Environment.UserName, ".", "", LogonNetwork, ProviderDefault, out token))
            {
                CloseHandle(token);
                return true;
            }
            return Marshal.GetLastWin32Error() == ErrorAccountRestriction;
        }

        public static void OpenSettings()
        {
            try { Process.Start(SettingsPage); }
            catch { /* very old builds without the settings page - nothing more we can do */ }
        }
    }

    internal sealed class TrayApp : ApplicationContext
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool LockWorkStation();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool DestroyIcon(IntPtr hIcon);

        // a second instance sets this to unhide the tray icon
        public const string ShowSignalName = @"Local\UnplugLock.ShowIcon";

        private static readonly int[] DelayChoices = { 0, 1, 3, 5, 10 };

        private static Icon _appIcon;
        public static Icon AppIcon
        {
            get
            {
                if (_appIcon == null)
                    _appIcon = MakeIcon(true, 32);
                return _appIcon;
            }
        }

        private readonly Settings _settings;
        private readonly NotifyIcon _tray;
        private readonly ToolStripMenuItem _enabledItem;
        private readonly ToolStripMenuItem _startupItem;
        private readonly ToolStripMenuItem _delayMenu;
        private readonly ToolStripMenuItem _statusItem;
        private readonly ToolStripMenuItem _setPasswordItem;
        private readonly ToolStripMenuItem _removePasswordItem;
        private readonly ToolStripMenuItem _guardItem;
        private readonly ToolStripMenuItem _signInItem;
        private readonly ToolStripSeparator _signInSeparator;

        // the Windows account has no password, so locking won't keep anyone out
        private bool _noSignIn;
        private bool _signInBalloon;

        // fallback in case we miss a power event
        private readonly System.Windows.Forms.Timer _pollTimer;
        // lock delay
        private readonly System.Windows.Forms.Timer _delayTimer;
        // short pause before restarting a killed helper
        private readonly System.Windows.Forms.Timer _helperRestart;

        private readonly EventWaitHandle _guardStop;
        private readonly EventWaitHandle _quitSignal;
        private readonly RegisteredWaitHandle _quitWait;
        private Process _helper;
        private bool _exiting;
        private int _ticks;

        // for getting back onto the UI thread
        private readonly Control _invoker;
        private readonly EventWaitHandle _showSignal;
        private readonly RegisteredWaitHandle _showWait;

        private PowerLineStatus _lastStatus;
        private readonly Icon _iconOn;
        private readonly Icon _iconOff;
        private bool _busy; // true while a dialog is open

        public TrayApp()
        {
            _settings = Settings.Load();
            _lastStatus = CurrentPowerStatus();

            _invoker = new Control();
            IntPtr forceHandle = _invoker.Handle;

            int traySize = SystemInformation.SmallIconSize.Width;
            _iconOn = MakeIcon(true, traySize);
            _iconOff = MakeIcon(false, traySize);

            _statusItem = new ToolStripMenuItem { Enabled = false };

            _enabledItem = new ToolStripMenuItem("Enabled", null, delegate { Guarded(ToggleEnabled); });

            _delayMenu = new ToolStripMenuItem("Lock delay");
            foreach (int d in DelayChoices)
            {
                int seconds = d;
                var item = new ToolStripMenuItem(seconds == 0 ? "Instant" : seconds + (seconds == 1 ? " second" : " seconds"));
                item.Tag = seconds;
                item.Click += delegate { Guarded(delegate { SetDelay(seconds); }); };
                _delayMenu.DropDownItems.Add(item);
            }

            _startupItem = new ToolStripMenuItem("Start with Windows", null, delegate { Guarded(ToggleStartup); });
            var hideItem = new ToolStripMenuItem("Hide tray icon", null, delegate { Guarded(HideTrayIcon); });
            _guardItem = new ToolStripMenuItem("Guard process (auto-restart)", null, delegate { Guarded(ToggleGuard); });

            _setPasswordItem = new ToolStripMenuItem("Set password...", null, delegate { Guarded(SetPassword); });
            _removePasswordItem = new ToolStripMenuItem("Remove password", null, delegate { Guarded(RemovePassword); });
            var passwordMenu = new ToolStripMenuItem("Password");
            passwordMenu.DropDownItems.Add(_setPasswordItem);
            passwordMenu.DropDownItems.Add(_removePasswordItem);

            var lockNowItem = new ToolStripMenuItem("Lock now (test)", null, delegate { LockWorkStation(); });
            var exitItem = new ToolStripMenuItem("Exit", null, delegate { Guarded(Exit); });

            _signInItem = new ToolStripMenuItem("Windows has no password - set one up...",
                SystemIcons.Warning.ToBitmap(), delegate { WindowsSignIn.OpenSettings(); }) { Visible = false };
            _signInSeparator = new ToolStripSeparator { Visible = false };

            var menu = new ContextMenuStrip();
            menu.Items.Add(_signInItem);
            menu.Items.Add(_signInSeparator);
            menu.Items.Add(_statusItem);
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(_enabledItem);
            menu.Items.Add(_delayMenu);
            menu.Items.Add(_startupItem);
            menu.Items.Add(_guardItem);
            menu.Items.Add(hideItem);
            menu.Items.Add(passwordMenu);
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(lockNowItem);
            menu.Items.Add(exitItem);
            Ui.StyleMenu(menu);
            menu.Opening += delegate
            {
                // once they've added a password, drop the warning (checking a blank account costs nothing)
                if (_noSignIn && !WindowsSignIn.HasNoPassword())
                    _noSignIn = false;
                RefreshMenu();
            };

            _tray = new NotifyIcon { ContextMenuStrip = menu };
            _tray.DoubleClick += delegate { Guarded(ToggleEnabled); };
            _tray.BalloonTipClicked += delegate
            {
                if (_signInBalloon)
                    WindowsSignIn.OpenSettings();
            };

            _delayTimer = new System.Windows.Forms.Timer();
            _delayTimer.Tick += delegate { OnDelayElapsed(); };

            _pollTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            _pollTimer.Tick += delegate { OnTick(); };
            _pollTimer.Start();

            _helperRestart = new System.Windows.Forms.Timer { Interval = 500 };
            _helperRestart.Tick += delegate
            {
                _helperRestart.Stop();
                if (_settings.Guard && !_exiting)
                    SpawnHelper();
            };

            SystemEvents.PowerModeChanged += OnPowerModeChanged;
            SystemEvents.SessionEnded += OnSessionEnded;

            _showSignal = new EventWaitHandle(false, EventResetMode.AutoReset, ShowSignalName);
            _showWait = ThreadPool.RegisterWaitForSingleObject(_showSignal,
                delegate { _invoker.BeginInvoke((MethodInvoker)ShowTrayIcon); },
                null, Timeout.Infinite, false);

            // setup uses this to close us cleanly (it already checked the password)
            _quitSignal = new EventWaitHandle(false, EventResetMode.AutoReset, Guard.QuitSignalName);
            _quitWait = ThreadPool.RegisterWaitForSingleObject(_quitSignal,
                delegate { _invoker.BeginInvoke((MethodInvoker)Quit); },
                null, Timeout.Infinite, false);

            _guardStop = new EventWaitHandle(false, EventResetMode.ManualReset, Guard.StopSignalName);
            if (_settings.Guard)
                StartGuard();

            RefreshMenu();
            _tray.Visible = !_settings.HideIcon;

            ThreadPool.QueueUserWorkItem(delegate
            {
                if (!WindowsSignIn.HasNoPassword())
                    return;
                try
                {
                    _invoker.BeginInvoke((MethodInvoker)delegate
                    {
                        if (_exiting)
                            return;
                        _noSignIn = true;
                        RefreshMenu();
                        if (_tray.Visible)
                            WarnNoSignIn();
                    });
                }
                catch (InvalidOperationException) { /* already shutting down */ }
            });
        }

        private void WarnNoSignIn()
        {
            _signInBalloon = true;
            _tray.ShowBalloonTip(10000, "Your Windows account has no password",
                "UnplugLock can lock the screen, but anyone can click straight back in. " +
                "Click here to set up a password or PIN.",
                ToolTipIcon.Warning);
        }

        private void OnTick()
        {
            CheckPower();

            // settings edited or deleted behind our back - put them back
            if (_settings.HasPassword && !_settings.IsInSync())
                _settings.Save();

            // put the startup entry back if someone removes it while guarded
            if (_settings.Guard && ++_ticks % 5 == 0 && !Settings.StartsWithWindows)
            {
                try { Settings.StartsWithWindows = true; }
                catch { /* try again next time */ }
            }
        }

        private void StartGuard()
        {
            Guard.Protect();
            _guardStop.Reset();
            try { Settings.StartsWithWindows = true; }
            catch { /* the tick retries */ }
            SpawnHelper();
        }

        private void StopGuard()
        {
            _guardStop.Set();
            _helperRestart.Stop();
            DropHelper();
            Guard.Unprotect();
        }

        private void SpawnHelper()
        {
            DropHelper();
            try
            {
                _helper = Guard.StartHelper();
                _helper.EnableRaisingEvents = true;
                _helper.Exited += OnHelperExited;
            }
            catch
            {
                _helperRestart.Start(); // try again shortly
            }
        }

        private void DropHelper()
        {
            if (_helper == null)
                return;
            _helper.Exited -= OnHelperExited;
            _helper.Dispose();
            _helper = null;
        }

        private void OnHelperExited(object sender, EventArgs e)
        {
            _invoker.BeginInvoke((MethodInvoker)delegate
            {
                if (_settings.Guard && !_exiting)
                    _helperRestart.Start();
            });
        }

        private void OnSessionEnded(object sender, SessionEndedEventArgs e)
        {
            // logging off / shutting down - don't let the helper fight it
            _exiting = true;
            _guardStop.Set();
        }


        private static PowerLineStatus CurrentPowerStatus()
        {
            return SystemInformation.PowerStatus.PowerLineStatus;
        }

        private void OnPowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            if (e.Mode == PowerModes.StatusChange || e.Mode == PowerModes.Resume)
                CheckPower();
        }

        private void CheckPower()
        {
            var now = CurrentPowerStatus();
            if (now == PowerLineStatus.Unknown)
                return;

            if (now != _lastStatus)
            {
                var previous = _lastStatus;
                _lastStatus = now;

                if (previous == PowerLineStatus.Online && now == PowerLineStatus.Offline)
                    OnUnplugged();
                else if (now == PowerLineStatus.Online)
                    _delayTimer.Stop(); // power came back during the delay

                RefreshMenu();
            }
        }

        private void OnUnplugged()
        {
            if (!_settings.Enabled)
                return;

            if (_settings.DelaySeconds <= 0)
            {
                LockWorkStation();
            }
            else
            {
                _delayTimer.Stop();
                _delayTimer.Interval = _settings.DelaySeconds * 1000;
                _delayTimer.Start();
            }
        }

        private void OnDelayElapsed()
        {
            _delayTimer.Stop();
            // still unplugged?
            if (_settings.Enabled && CurrentPowerStatus() == PowerLineStatus.Offline)
                LockWorkStation();
        }


        // stops menu clicks stacking up dialogs
        private void Guarded(Action action)
        {
            if (_busy)
                return;
            _busy = true;
            try { action(); }
            finally
            {
                _busy = false;
                RefreshMenu();
            }
        }

        private bool Authorize(string toDoWhat)
        {
            if (!_settings.HasPassword)
                return true;

            while (true)
            {
                string pw;
                var result = PasswordDialog.Ask("Enter your UnplugLock password to " + toDoWhat + ":", _settings, out pw);
                if (result == AskResult.Cancelled)
                    return false;
                if (result == AskResult.Recovered)
                {
                    _settings.SetPassword(null);
                    _settings.Save();
                    MessageBox.Show("Recovery code accepted. Your password has been removed. " +
                        "Set a new one from the Password menu.", "UnplugLock",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return true;
                }
                if (_settings.CheckPassword(pw))
                    return true;

                Thread.Sleep(1000); // slow down guessing
                MessageBox.Show("Wrong password.", "UnplugLock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ToggleEnabled()
        {
            if (_settings.Enabled && !Authorize("turn it off"))
                return;

            _settings.Enabled = !_settings.Enabled;
            if (!_settings.Enabled)
                _delayTimer.Stop();
            _settings.Save();
        }

        private void SetDelay(int seconds)
        {
            if (seconds == _settings.DelaySeconds || !Authorize("change the lock delay"))
                return;
            _settings.DelaySeconds = seconds;
            _settings.Save();
        }

        private void ToggleGuard()
        {
            if (_settings.Guard)
            {
                if (!Authorize("turn off the process guard"))
                    return;
                _settings.Guard = false;
                _settings.Save();
                StopGuard();
                return;
            }

            _settings.Guard = true;
            _settings.Save();
            StartGuard();
        }

        private void ToggleStartup()
        {
            if (_settings.Guard)
                return; // required while guarded
            bool on = Settings.StartsWithWindows;
            if (on && !Authorize("stop it starting with Windows"))
                return;
            try
            {
                Settings.StartsWithWindows = !on;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Couldn't change the startup setting:\n" + ex.Message, "UnplugLock",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void HideTrayIcon()
        {
            var answer = MessageBox.Show(
                "The tray icon will be hidden, but UnplugLock keeps running and protecting your laptop.\n\n" +
                "To show the icon again, open UnplugLock again (from the Start menu or UnplugLock.exe).",
                "UnplugLock", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (answer != DialogResult.OK)
                return;

            _settings.HideIcon = true;
            _settings.Save();
            _tray.Visible = false;
        }

        private void ShowTrayIcon()
        {
            _settings.HideIcon = false;
            _settings.Save();
            _tray.Visible = true;
            if (_noSignIn)
            {
                WarnNoSignIn();
                return;
            }
            _signInBalloon = false;
            _tray.ShowBalloonTip(4000, "UnplugLock is running",
                _settings.Enabled ? "Your laptop locks when unplugged. Right-click the icon for options."
                                  : "UnplugLock is turned off. Right-click the icon for options.",
                ToolTipIcon.Info);
        }

        private void SetPassword()
        {
            if (_settings.HasPassword && !Authorize("change the password"))
                return;

            string pw = PasswordDialog.AskNew();
            if (pw == null)
                return;
            string code = Settings.NewRecoveryCode();
            _settings.SetPassword(pw, code);
            RecoveryCodeDialog.Present(code);
            _settings.Save();
        }

        private void RemovePassword()
        {
            if (!_settings.HasPassword || !Authorize("remove the password"))
                return;
            _settings.SetPassword(null);
            _settings.Save();
            MessageBox.Show("Password removed.", "UnplugLock", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Exit()
        {
            if (Authorize("exit"))
                Quit();
        }

        // clean exit: tell the helper to stand down so it doesn't restart us
        private void Quit()
        {
            if (_exiting)
                return;
            _exiting = true;
            _guardStop.Set();
            ExitThread();
        }


        private void RefreshMenu()
        {
            string power;
            switch (_lastStatus)
            {
                case PowerLineStatus.Online: power = "Plugged in"; break;
                case PowerLineStatus.Offline: power = "On battery"; break;
                default: power = "Power status unknown"; break;
            }

            _statusItem.Text = power;
            _enabledItem.Checked = _settings.Enabled;
            _startupItem.Checked = Settings.StartsWithWindows;
            _startupItem.Enabled = !_settings.Guard;
            _startupItem.Text = _settings.Guard ? "Start with Windows (required by guard)" : "Start with Windows";
            _guardItem.Checked = _settings.Guard;
            foreach (ToolStripItem item in _delayMenu.DropDownItems)
            {
                var mi = item as ToolStripMenuItem;
                if (mi != null)
                    mi.Checked = (int)mi.Tag == _settings.DelaySeconds;
            }
            _setPasswordItem.Text = _settings.HasPassword ? "Change password..." : "Set password...";
            _removePasswordItem.Enabled = _settings.HasPassword;
            _signInItem.Visible = _noSignIn;
            _signInSeparator.Visible = _noSignIn;

            _tray.Icon = _settings.Enabled ? _iconOn : _iconOff;
            string tip = _noSignIn
                ? "UnplugLock - Windows has no password, locking won't help"
                : "UnplugLock - " + (_settings.Enabled ? "on" : "off") + " (" + power.ToLowerInvariant() + ")";
            _tray.Text = tip.Length > 63 ? tip.Substring(0, 63) : tip; // tooltip max is 63 chars
        }

        private static Icon MakeIcon(bool enabled, int size)
        {
            using (var bmp = Logo.Render(size, enabled))
            {
                IntPtr h = bmp.GetHicon();
                Icon icon = (Icon)Icon.FromHandle(h).Clone();
                DestroyIcon(h);
                return icon;
            }
        }

        protected override void ExitThreadCore()
        {
            _exiting = true;
            _guardStop.Set();
            _helperRestart.Stop();
            DropHelper();
            _showWait.Unregister(null);
            _showSignal.Dispose();
            _quitWait.Unregister(null);
            _quitSignal.Dispose();
            _guardStop.Dispose();
            SystemEvents.PowerModeChanged -= OnPowerModeChanged;
            SystemEvents.SessionEnded -= OnSessionEnded;
            _pollTimer.Stop();
            _delayTimer.Stop();
            _tray.Visible = false;
            _tray.Dispose();
            _iconOn.Dispose();
            _iconOff.Dispose();
            _invoker.Dispose();
            base.ExitThreadCore();
        }
    }
}
