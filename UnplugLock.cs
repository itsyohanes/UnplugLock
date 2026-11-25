// UnplugLock - lock the PC when the charger gets pulled.
// Sticks to C# 5 / .NET 4.x so it builds with the csc.exe that comes with Windows.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.IO.Pipes;
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
        // shown in the About window and used by the installer. Must match VERSION in build.cmd.
        public const string Version = "1.4.1";

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
            Lang.Use(Settings.Load(false).Language);

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
                        MessageBox.Show(Lang.T("AlreadyRunning"), "UnplugLock",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    return 0;
                }

                // the helper passes --restarted when it brings us back
                var app = new TrayApp(Array.IndexOf(args, "--restarted") >= 0);
                Application.Run(app);
                // only a real quit tells the helper to stand down. Anything else (a crash, a posted
                // WM_QUIT, being ended) gets us restarted.
                return app.QuitRequested ? Guard.CleanExitCode : Guard.UnexpectedExitCode;
            }
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

        // runs slow work (password hashing, a pause after a wrong guess) off the UI thread and keeps
        // the message loop going meanwhile, so power and network checks don't stall. Open windows
        // are disabled until it's done, so nothing can be clicked twice.
        public static T Busy<T>(Func<T> work)
        {
            T result = default(T);
            Exception error = null;
            var forms = new List<Form>();
            foreach (Form f in Application.OpenForms)
                if (f.Enabled)
                    forms.Add(f);
            foreach (var f in forms)
                f.Enabled = false;
            try
            {
                using (var done = new ManualResetEvent(false))
                {
                    ThreadPool.QueueUserWorkItem(delegate
                    {
                        try { result = work(); }
                        catch (Exception ex) { error = ex; }
                        done.Set();
                    });
                    while (!done.WaitOne(15))
                        Application.DoEvents();
                }
            }
            finally
            {
                foreach (var f in forms)
                    if (!f.IsDisposed)
                        f.Enabled = true;
            }
            if (error != null)
                throw new InvalidOperationException(error.Message, error);
            return result;
        }

        public static void Pause(int milliseconds)
        {
            Busy(delegate { Thread.Sleep(milliseconds); return true; });
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
        // main -> helper: "check whether the guard is still on". Anyone can set it, so the helper
        // only stands down if the saved settings really say the guard is off.
        public const string StopSignalName = @"Local\UnplugLock.GuardStop";
        // what 1.3.0 and earlier listen on to quit. Setup still sets it when updating an old copy.
        // Newer copies don't listen on it: anyone could set it, so quitting goes through QuitListener.
        public const string LegacyQuitSignalName = @"Local\UnplugLock.Quit";
        public const string HelperName = "UnplugLockHelper";

        // the main app's exit code tells the helper whether to bring it back
        public const int CleanExitCode = 0;
        public const int UnexpectedExitCode = 3;

        private const int DACL_SECURITY_INFORMATION = 4;
        private const int SYNCHRONIZE = 0x00100000;
        private const int PROCESS_QUERY_LIMITED_INFORMATION = 0x1000;

        // everything that would let another process end, freeze or take over ours: terminate, create
        // a thread in it, read/write its memory, copy its handles, start children as it, change its
        // limits or priority, suspend it, and rewrite its permissions or owner. Waiting on it and
        // basic queries stay allowed (the helper and setup need those).
        internal const int DeniedRights =
            0x0001 | 0x0002 | 0x0008 | 0x0010 | 0x0020 | 0x0040 | 0x0080 | 0x0100 | 0x0200 | 0x0800 |
            0x00010000 | 0x00040000 | 0x00080000; // DELETE, WRITE_DAC, WRITE_OWNER

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetCurrentProcess();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern SafeWaitHandle OpenProcess(int access, bool inherit, int pid);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetExitCodeProcess(SafeWaitHandle process, out int exitCode);

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
                // without an OWNER RIGHTS entry, the process owner (us) always gets WRITE_DAC, so any
                // program running as us could just rewrite these permissions and then end the process
                var ownerRights = new SecurityIdentifier("S-1-3-4");
                var dacl = sd.DiscretionaryAcl;
                if (dacl == null)
                {
                    // null dacl = full access for everyone, keep that apart from the deny
                    dacl = new RawAcl(GenericAcl.AclRevision, 3);
                    dacl.InsertAce(0, new CommonAce(AceFlags.None, AceQualifier.AccessAllowed, 0x1FFFFF, everyone, false, null));
                }
                dacl.InsertAce(0, new CommonAce(AceFlags.None, AceQualifier.AccessDenied, DeniedRights, everyone, false, null));
                dacl.InsertAce(1, new CommonAce(AceFlags.None, AceQualifier.AccessDenied, DeniedRights, ownerRights, false, null));
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

        // whether the helper brings the main app back after it ends. exitCode is null if we couldn't read it.
        public static bool ShouldRestart(int? exitCode, bool guardOn)
        {
            return guardOn && exitCode != CleanExitCode;
        }

        // helper: wait for the main app to end, and restart it unless it quit properly or the guard is off
        public static int RunHelper(string[] args)
        {
            int pid;
            if (args.Length < 3 || !int.TryParse(args[1], out pid))
                return 1;
            string appPath = args[2];

            Protect();

            EventWaitHandle recheck;
            try { recheck = EventWaitHandle.OpenExisting(StopSignalName); }
            catch { return 0; } // main isn't running a guard

            int? exitCode = null;
            using (recheck)
            using (var handle = OpenProcess(SYNCHRONIZE | PROCESS_QUERY_LIMITED_INFORMATION, false, pid))
            {
                if (!handle.IsInvalid)
                {
                    using (var mainExited = new ManualResetEvent(false))
                    {
                        mainExited.SafeWaitHandle = handle;
                        try
                        {
                            while (WaitHandle.WaitAny(new WaitHandle[] { recheck, mainExited }) == 0)
                            {
                                // the main app turned the guard off - or someone else set the signal,
                                // in which case the settings still say on and we keep watching
                                if (!Settings.Load(false).Guard)
                                    return 0;
                                Thread.Sleep(200); // a flood of signals can't make us spin
                            }
                        }
                        finally
                        {
                            mainExited.SafeWaitHandle = new SafeWaitHandle(IntPtr.Zero, false); // handle is disposed by the using above
                        }
                    }
                    int code;
                    if (GetExitCodeProcess(handle, out code))
                        exitCode = code;
                }
            }

            if (!ShouldRestart(exitCode, Settings.Load(false).Guard))
                return 0;

            Thread.Sleep(300);
            try
            {
                Process.Start(new ProcessStartInfo(appPath, "--restarted")
                {
                    UseShellExecute = false,
                    WorkingDirectory = Path.GetDirectoryName(appPath)
                });
            }
            catch { /* nothing we can do */ }
            return 0;
        }
    }

    // How setup closes the running app. A named event used to do this, but any program could set
    // it and close UnplugLock without the password. Now it's a pipe that only the current user can
    // open (and not over the network), and with a password set the request has to carry the
    // password or the recovery code.
    internal sealed class QuitListener
    {
        private const int MaxSecretBytes = 1024;

        private readonly string _pipeName;
        private readonly Func<string, bool> _authorize;
        private readonly Action _onQuit;
        private volatile bool _stopped;

        public QuitListener(string pipeName, Func<string, bool> authorize, Action onQuit)
        {
            _pipeName = pipeName;
            _authorize = authorize;
            _onQuit = onQuit;
        }

        // one per user and Windows session
        public static string DefaultPipeName
        {
            get
            {
                using (var me = WindowsIdentity.GetCurrent())
                    return "UnplugLock.Quit." + me.User.Value + "." + Process.GetCurrentProcess().SessionId;
            }
        }

        public void Start()
        {
            new Thread(Listen) { IsBackground = true, Name = "UnplugLock quit pipe" }.Start();
        }

        public void Stop()
        {
            _stopped = true;
        }

        private PipeSecurity Security()
        {
            var ps = new PipeSecurity();
            using (var me = WindowsIdentity.GetCurrent())
                ps.AddAccessRule(new PipeAccessRule(me.User, PipeAccessRights.ReadWrite, AccessControlType.Allow));
            ps.AddAccessRule(new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.NetworkSid, null),
                PipeAccessRights.FullControl, AccessControlType.Deny));
            return ps;
        }

        private void Listen()
        {
            while (!_stopped)
            {
                NamedPipeServerStream server;
                try
                {
                    // one instance only, so nothing else can sit on the same name while we're running
                    server = new NamedPipeServerStream(_pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte,
                        PipeOptions.Asynchronous, 64, 64, Security());
                }
                catch
                {
                    Thread.Sleep(5000); // name taken, try again later
                    continue;
                }

                using (server)
                {
                    try
                    {
                        server.WaitForConnection();
                        if (_stopped)
                            return;
                        var length = new byte[2];
                        if (!ReadExactly(server, length, 2000))
                            continue;
                        int n = (length[0] << 8) | length[1];
                        if (n > MaxSecretBytes)
                            continue;
                        var bytes = new byte[n];
                        if (n > 0 && !ReadExactly(server, bytes, 2000))
                            continue;

                        bool ok = _authorize(System.Text.Encoding.UTF8.GetString(bytes));
                        if (!ok)
                            Thread.Sleep(1000); // slow down guessing
                        server.WriteByte(ok ? (byte)1 : (byte)0);
                        server.Flush();
                        server.WaitForPipeDrain();
                        if (ok && !_stopped)
                            _onQuit();
                    }
                    catch { /* client went away, wait for the next one */ }
                }
            }
        }

        private static bool ReadExactly(Stream s, byte[] buf, int timeoutMs)
        {
            int got = 0;
            int deadline = Environment.TickCount + timeoutMs;
            while (got < buf.Length)
            {
                int left = deadline - Environment.TickCount;
                if (left <= 0)
                    return false;
                var ar = s.BeginRead(buf, got, buf.Length - got, null, null);
                if (!ar.AsyncWaitHandle.WaitOne(left))
                    return false; // disposing the pipe cancels the read
                int read = s.EndRead(ar);
                if (read == 0)
                    return false;
                got += read;
            }
            return true;
        }

        // asks the running app to quit. null = nothing listening, true = it's quitting, false = refused.
        public static bool? RequestQuit(string pipeName, string secret, int connectTimeoutMs)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(secret ?? "");
            if (bytes.Length > MaxSecretBytes)
                return false;
            try
            {
                using (var client = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous))
                {
                    client.Connect(connectTimeoutMs);
                    var msg = new byte[2 + bytes.Length];
                    msg[0] = (byte)(bytes.Length >> 8);
                    msg[1] = (byte)bytes.Length;
                    Buffer.BlockCopy(bytes, 0, msg, 2, bytes.Length);
                    client.Write(msg, 0, msg.Length);
                    client.Flush();

                    // checking the password takes a moment, a wrong one a second more
                    var answer = new byte[1];
                    if (!ReadExactly(client, answer, 15000))
                        return false;
                    return answer[0] == 1;
                }
            }
            catch (TimeoutException) { return null; }
            catch (IOException) { return null; }
            catch (UnauthorizedAccessException) { return false; }
        }
    }

    // settings live in HKCU\Software\UnplugLock, plus a hidden backup in %LOCALAPPDATA%\UnplugLock.
    // once a password is set the key is locked against edits, and the running app puts back
    // anything that gets changed or deleted.
    internal sealed class Settings
    {
        // the tests point these somewhere else so they never touch the real settings
        internal static string KeyPath = @"Software\UnplugLock";
        internal static string RunValueName = "UnplugLock";
        internal static string BackupDirOverride = null;
        private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const int HashIterations = 200000;
        // the longest choice in the menu. Anything bigger from a stored copy is cut down to this,
        // so an edited value can't quietly turn locking off (or overflow the timer).
        public const int MaxDelaySeconds = 10;
        private const string CodeChars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // no 0/O or 1/I

        private const RegistryRights LockedRights =
            RegistryRights.SetValue | RegistryRights.CreateSubKey | RegistryRights.Delete;

        public bool Enabled = true;
        public int DelaySeconds = 0; // 0 = lock instantly
        public bool HideIcon = false;
        public bool Guard = true;
        public bool LockOnEthernet = false; // also lock when a wired network drops
        public bool LockOnWifi = false;     // also lock when Wi-Fi disconnects
        public string Language;             // picked in setup ("de", "en", ...); null follows Windows

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
                return BackupDirOverride ??
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "UnplugLock");
            }
        }

        private static string BackupPath { get { return Path.Combine(BackupDir, "settings.dat"); } }

        private static SecurityIdentifier Everyone
        {
            get { return new SecurityIdentifier(WellKnownSidType.WorldSid, null); }
        }

        // ---- load / save

        // repair: false only reads (the helper uses this, so it never writes behind the app's back)
        public static Settings Load(bool repair = true)
        {
            Settings reg = ReadRegistry();
            Settings bak = ReadBackup();

            Settings s;
            if (reg != null && bak != null)
                s = Merge(reg, bak);
            else
                s = reg ?? bak ?? new Settings();

            // one copy missing or tampered with - repair both from the merged result
            if (repair && (reg != null || bak != null) && (!s.SameAs(reg) || !s.SameAs(bak)))
                s.Save();
            return s;
        }

        private static int ClampDelay(int seconds)
        {
            return Math.Max(0, Math.Min(MaxDelaySeconds, seconds));
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
            s.LockOnEthernet = reg.LockOnEthernet || bak.LockOnEthernet;
            s.LockOnWifi = reg.LockOnWifi || bak.LockOnWifi;
            s.DelaySeconds = Math.Min(reg.DelaySeconds, bak.DelaySeconds);
            s.Language = reg.Language ?? bak.Language; // nothing to be strict about
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
                   o.LockOnEthernet == LockOnEthernet && o.LockOnWifi == LockOnWifi && o.Language == Language &&
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
                    s.DelaySeconds = ClampDelay(Convert.ToInt32(key.GetValue("DelaySeconds", 0)));
                    s.HideIcon = Convert.ToInt32(key.GetValue("HideIcon", 0)) != 0;
                    s.Guard = Convert.ToInt32(key.GetValue("Guard", 1)) != 0;
                    s.LockOnEthernet = Convert.ToInt32(key.GetValue("LockOnEthernet", 0)) != 0;
                    s.LockOnWifi = Convert.ToInt32(key.GetValue("LockOnWifi", 0)) != 0;
                    s.Language = NullIfEmpty(key.GetValue("Language") as string);
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
                key.SetValue("LockOnEthernet", LockOnEthernet ? 1 : 0, RegistryValueKind.DWord);
                key.SetValue("LockOnWifi", LockOnWifi ? 1 : 0, RegistryValueKind.DWord);
                SetOrDelete(key, "Language", Language);
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
                // a line that doesn't make sense is skipped, not the whole file. Otherwise one bad
                // line would make the backup count as missing, and an edited registry copy would win
                // outright instead of being merged with the stricter backup.
                var s = new Settings();
                foreach (var line in File.ReadAllLines(BackupPath))
                {
                    int eq = line.IndexOf('=');
                    if (eq <= 0)
                        continue;
                    string name = line.Substring(0, eq);
                    string value = line.Substring(eq + 1);
                    int seconds;
                    switch (name)
                    {
                        case "Enabled": s.Enabled = value == "1"; break;
                        case "DelaySeconds":
                            if (int.TryParse(value, out seconds))
                                s.DelaySeconds = ClampDelay(seconds);
                            break;
                        case "HideIcon": s.HideIcon = value == "1"; break;
                        case "Guard": s.Guard = value == "1"; break;
                        case "LockOnEthernet": s.LockOnEthernet = value == "1"; break;
                        case "LockOnWifi": s.LockOnWifi = value == "1"; break;
                        case "Language": s.Language = NullIfEmpty(value); break;
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
                "LockOnEthernet=" + (LockOnEthernet ? "1" : "0"),
                "LockOnWifi=" + (LockOnWifi ? "1" : "0"),
                "Language=" + Language,
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

        // the password or the recovery code (or anything, with no password set)
        public bool Unlocks(string secret)
        {
            return CheckPassword(secret) || CheckRecoveryCode(secret);
        }

        // takes the password and recovery code hashes from o (worked out off the UI thread)
        public void CopyPasswordFrom(Settings o)
        {
            _passwordHash = o._passwordHash;
            _passwordSalt = o._passwordSalt;
            _recoveryHash = o._recoveryHash;
            _recoverySalt = o._recoverySalt;
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
                    // it has to point at us: an entry changed to something else ("x", another
                    // program) counts as off, so the guard puts it back
                    using (var key = Registry.CurrentUser.OpenSubKey(RunKeyPath))
                    {
                        var v = key == null ? null : key.GetValue(RunValueName) as string;
                        if (string.IsNullOrEmpty(v))
                            return false;
                        return string.Equals(Path.GetFullPath(v.Trim().Trim('"')),
                            Path.GetFullPath(Application.ExecutablePath), StringComparison.OrdinalIgnoreCase);
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

        internal const int MinLength = 6;

        private readonly Mode _mode;
        private readonly Settings _settings; // for "Forgot password?"
        private string _recoveryCode;        // the code that worked, if "Forgot password?" was used
        private readonly TextBox _first;
        private readonly TextBox _second; // only when setting a new password
        private readonly Label _error;

        private static string TitleFor(Mode mode)
        {
            return Lang.T(mode == Mode.ChooseNew ? "Pw.TitleChoose"
                        : mode == Mode.RecoveryCode ? "Pw.TitleRecovery"
                        : "Pw.TitleEnter");
        }

        private PasswordDialog(string prompt, Mode mode, Settings settings)
            : base("UnplugLock", TitleFor(mode), prompt, 340, false)
        {
            _mode = mode;
            _settings = settings;
            TopMost = true;

            AddText(Lang.T(mode == Mode.ChooseNew ? "Pw.LabelNew" : mode == Mode.RecoveryCode ? "Pw.LabelRecovery" : "Pw.Label"),
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
                // the hashes can be read by anything running as you, so a reused password could be guessed offline
                AddText(Lang.T("Pw.Hint", MinLength), Ui.Muted, Ui.Small, 0).Margin = Ui.S(0, 6, 0, 16);
                AddText(Lang.T("Pw.LabelConfirm"), Ui.Ink, Ui.Strong, 6);
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
                    Text = Lang.T("Pw.Forgot"),
                    AutoSize = true,
                    LinkBehavior = LinkBehavior.HoverUnderline,
                    LinkColor = Ui.Brand,
                    ActiveLinkColor = Ui.BrandPressed,
                    BackColor = Color.Transparent
                };
                forgot.LinkClicked += OnForgot;
                SetFooterNote(forgot);
            }

            var cancel = AddButton(Lang.T("Button.Cancel"), false);
            cancel.DialogResult = DialogResult.Cancel;
            var ok = AddButton(Lang.T(mode == Mode.ChooseNew ? "Pw.Set" : "Button.Continue"), true);
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
                    ShowError(Lang.T("Pw.TooShort", MinLength));
                    _first.Focus();
                    return;
                }
                if (_first.Text != _second.Text)
                {
                    ShowError(Lang.T("Pw.Mismatch"));
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
                using (var d = new PasswordDialog(Lang.T("Pw.RecoveryPrompt"), Mode.RecoveryCode, null))
                {
                    if (d.ShowDialog(this) != DialogResult.OK)
                        return;
                    code = d._first.Text;
                }

                if (Ui.Busy(delegate { return _settings.CheckRecoveryCode(code); }))
                {
                    _recoveryCode = code;
                    DialogResult = DialogResult.Retry; // = recovered
                    return;
                }

                Ui.Pause(1000); // slow down guessing
                MessageBox.Show(this, Lang.T("Pw.RecoveryWrong"), "UnplugLock",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // asks for the current password; "Forgot password?" returns Recovered once a valid code is
        // entered, with the code in password
        public static AskResult Ask(string prompt, Settings settings, out string password)
        {
            using (var d = new PasswordDialog(prompt, Mode.Enter, settings))
            {
                var r = d.ShowDialog();
                password = null;
                if (r == DialogResult.Retry)
                {
                    password = d._recoveryCode;
                    return AskResult.Recovered;
                }
                if (r != DialogResult.OK)
                    return AskResult.Cancelled;
                password = d._first.Text;
                return AskResult.Entered;
            }
        }

        public static string AskNew()
        {
            using (var d = new PasswordDialog(Lang.T("Pw.NewPrompt"), Mode.ChooseNew, null))
                return d.ShowDialog() == DialogResult.OK ? d._first.Text : null;
        }
    }

    // shown once, right after a password is set - the only time the recovery code is ever visible
    internal sealed class RecoveryCodeDialog : Dialog
    {
        private readonly string _code;
        private bool _confirmed;

        private RecoveryCodeDialog(string code)
            : base("UnplugLock", Lang.T("Rc.Title"), Lang.T("Rc.Subtitle"), 420, false)
        {
            _code = code;
            ControlBox = false;
            TopMost = true;

            AddText(Lang.T("Rc.Explain", Lang.T("Pw.Forgot")), Ui.Ink, Ui.Body, 10);
            AddText(Lang.T("Rc.Keep"), Ui.Ink, Ui.Body, 18);

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

            var copy = new RoundButton(Lang.T("Rc.Copy"), false) { Margin = Padding.Empty };
            var save = new RoundButton(Lang.T("Rc.Save"), false);
            copy.Click += delegate
            {
                try
                {
                    Clipboard.SetDataObject(ClipboardData(_code), true);
                    copy.Text = Lang.T("Rc.Copied");
                }
                catch { /* clipboard busy */ }
            };
            save.Click += delegate { SaveToFile(); };
            var tools = new FlowLayoutPanel { AutoSize = true, WrapContents = false, Margin = Ui.S(0, 0, 0, 18) };
            tools.Controls.Add(copy);
            tools.Controls.Add(save);
            Body.Controls.Add(tools);

            var check = new CheckBox { Text = Lang.T("Rc.Confirm"), AutoSize = true, Margin = Padding.Empty };
            Body.Controls.Add(check);

            var ok = AddButton(Lang.T("Button.Done"), true);
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

        // the code as clipboard text, marked so Windows keeps it out of clipboard history (Win+V)
        // and doesn't sync it to other devices, where someone holding the laptop could find it later
        internal static DataObject ClipboardData(string code)
        {
            var data = new DataObject();
            data.SetData(DataFormats.UnicodeText, code);
            data.SetData(DataFormats.Text, code);
            data.SetData("ExcludeClipboardContentFromMonitorProcessing", new MemoryStream(new byte[4]));
            data.SetData("CanIncludeInClipboardHistory", new MemoryStream(BitConverter.GetBytes(0)));
            data.SetData("CanUploadToCloudClipboard", new MemoryStream(BitConverter.GetBytes(0)));
            return data;
        }

        // a drive that's part of the laptop (not a USB stick or network share)
        internal static bool IsOnThisLaptop(string path)
        {
            try
            {
                string root = Path.GetPathRoot(Path.GetFullPath(path));
                return !root.StartsWith(@"\\") && new DriveInfo(root).DriveType == DriveType.Fixed;
            }
            catch
            {
                return false;
            }
        }

        private void SaveToFile()
        {
            using (var dlg = new SaveFileDialog
            {
                FileName = Lang.T("Rc.FileTitle") + ".txt",
                Filter = Lang.T("Rc.FileType") + " (*.txt)|*.txt",
                // start at "This PC", so a USB stick is as easy to pick as Documents
                InitialDirectory = "::{20D04FE0-3AEA-1069-A2D8-08002B30309D}"
            })
            {
                if (dlg.ShowDialog(this) != DialogResult.OK)
                    return;
                try
                {
                    File.WriteAllText(dlg.FileName, RecoveryFileText(_code, DateTime.Now), new System.Text.UTF8Encoding(true));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, Lang.T("Rc.SaveFailed", ex.Message), "UnplugLock",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (IsOnThisLaptop(dlg.FileName))
                    MessageBox.Show(this, Lang.T("Rc.SavedOnLaptop"),
                        "UnplugLock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // saved with a UTF-8 BOM, so Notepad shows non-English text right on any Windows version
        internal static string RecoveryFileText(string code, DateTime created)
        {
            return Lang.T("Rc.FileTitle") + "\r\n\r\n" + code + "\r\n\r\n" +
                Lang.T("Rc.FileHelp", Lang.T("Pw.Forgot")) + "\r\n" +
                Lang.T("Rc.FileCreated", created.ToString(Lang.T("Rc.DateFormat"), Lang.Culture)) + "\r\n";
        }

        public static void Present(string code)
        {
            using (var d = new RecoveryCodeDialog(code))
                d.ShowDialog();
        }
    }

    internal sealed class AboutDialog : Dialog
    {
        public const string WebsiteUrl = "https://unpluglock.com";
        public const string SourceUrl = "https://github.com/itsyohanes/UnplugLock";
        public const string ReleasesUrl = "https://github.com/itsyohanes/UnplugLock/releases";
        public const string SupportUrl = "https://donate.stripe.com/14A6oH5tB2aog7J8HAfIs00";

        private AboutDialog()
            : base(Lang.T("About.Window"), "UnplugLock", Lang.T("Version", Program.Version), 340, true)
        {
            AddText(Lang.T("AppTagline"), Ui.Ink, Ui.Body, 14);
            AddLink("www.unpluglock.com", WebsiteUrl);
            AddLink(Lang.T("About.Source"), SourceUrl);
            AddLink(Lang.T("About.Releases"), ReleasesUrl);
            AddLink(Lang.T("About.Support"), SupportUrl).Margin = Padding.Empty;

            SetFooterNote(Ui.Text(Lang.T("About.License"), ContentWidth - Ui.S(110), Ui.Muted, Ui.Small, 0));
            var close = AddButton(Lang.T("Button.Close"), true);
            close.DialogResult = DialogResult.OK;
            AcceptButton = close;
            CancelButton = close;
        }

        private LinkLabel AddLink(string text, string url)
        {
            var link = new LinkLabel
            {
                Text = text,
                AutoSize = true,
                LinkBehavior = LinkBehavior.HoverUnderline,
                LinkColor = Ui.Brand,
                ActiveLinkColor = Ui.BrandPressed,
                BackColor = Color.Transparent,
                Margin = Ui.S(0, 0, 0, 8)
            };
            link.LinkClicked += delegate
            {
                try { Process.Start(url); }
                catch { /* no browser set up */ }
            };
            Body.Controls.Add(link);
            return link;
        }

        public static void Present()
        {
            using (var d = new AboutDialog())
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

        [DllImport("netapi32.dll", CharSet = CharSet.Unicode)]
        private static extern int NetUserGetInfo(string server, string user, int level, out IntPtr buffer);

        [DllImport("netapi32.dll")]
        private static extern int NetApiBufferFree(IntPtr buffer);

        public const string SettingsPage = "ms-settings:signinoptions";

        // A Microsoft account looks like a local one (its domain is the PC name), but always has a
        // password. USER_INFO_24 starts with a BOOL that says whether it's linked to one.
        public static bool IsMicrosoftAccount()
        {
            IntPtr buffer;
            if (NetUserGetInfo(null, Environment.UserName, 24, out buffer) != 0)
                return false;
            try { return Marshal.ReadInt32(buffer) != 0; }
            finally { NetApiBufferFree(buffer); }
        }

        // Tries an empty password for the current user. On an account that has a password this
        // is one failed logon (which counts towards Windows' lockout limit), so only call it once
        // at startup - not after the guard restarts us - or while we already think it's blank.
        public static bool HasNoPassword()
        {
            if (!string.Equals(Environment.UserDomainName, Environment.MachineName, StringComparison.OrdinalIgnoreCase))
                return false; // not a local account
            try
            {
                if (IsMicrosoftAccount())
                    return false;
            }
            catch { /* can't tell, fall through to the logon check */ }

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

    // LockWorkStation fails while another desktop is showing, like a UAC prompt. Without a retry,
    // someone could open a UAC prompt, pull the plug, then cancel the prompt and walk off with
    // the laptop unlocked. So keep trying until it works.
    internal sealed class Locker : IDisposable
    {
        private readonly Func<bool> _lock;
        private readonly TimeSpan _giveUpAfter;
        private readonly System.Windows.Forms.Timer _retry;
        private DateTime _giveUpAt;

        public Locker(Func<bool> lockFn, int retryMs, TimeSpan giveUpAfter)
        {
            _lock = lockFn;
            _giveUpAfter = giveUpAfter;
            _retry = new System.Windows.Forms.Timer { Interval = retryMs };
            _retry.Tick += delegate { Attempt(); };
        }

        public bool Retrying { get { return _retry.Enabled; } }

        // true if it locked straight away; otherwise it keeps retrying in the background
        public bool Lock()
        {
            _giveUpAt = DateTime.UtcNow + _giveUpAfter;
            return Attempt();
        }

        public void Cancel()
        {
            _retry.Stop();
        }

        private bool Attempt()
        {
            bool ok = false;
            try { ok = _lock(); }
            catch { /* treat as failed */ }
            if (ok || DateTime.UtcNow >= _giveUpAt)
                _retry.Stop();
            else if (!_retry.Enabled)
                _retry.Start();
            return ok;
        }

        public void Dispose()
        {
            _retry.Dispose();
        }
    }

    internal enum NetKind { Ethernet, Wifi }

    // which physical network adapters are connected right now. Reads the adapter table
    // straight from Windows (GetIfTable2), so it needs no permissions and sends nothing anywhere.
    // Virtual adapters (Hyper-V, WSL, VPNs, Wi-Fi Direct) and Bluetooth are left out.
    internal static class NetworkLinks
    {
        [DllImport("iphlpapi.dll")]
        private static extern int GetIfTable2(out IntPtr table);

        [DllImport("iphlpapi.dll")]
        private static extern void FreeMibTable(IntPtr table);

        // MIB_IF_ROW2 layout (same on x86 and x64)
        private const int RowSize = 1352;
        private const int TableHeader = 8; // NumEntries, padded to 8
        private const int OffLuid = 0;
        private const int OffType = 1128;
        private const int OffPhysicalMedium = 1140;
        private const int OffFlags = 1152;
        private const int OffOperStatus = 1156;
        private const int OffMediaConnectState = 1164;

        private const int IfTypeEthernet = 6;
        private const int IfTypeWifi = 71;
        private const int MediumWirelessLan = 1;
        private const int MediumNative80211 = 9;
        private const int MediumBluetooth = 10;
        private const byte FlagHardware = 0x01;
        private const byte FlagFilter = 0x02;
        private const int OperUp = 1;
        private const int MediaConnected = 1;

        // adapter LUID -> kind, for every physical adapter that has a link. Null if the table can't be read.
        public static Dictionary<long, NetKind> Connected()
        {
            IntPtr table;
            if (GetIfTable2(out table) != 0)
                return null;
            try
            {
                var result = new Dictionary<long, NetKind>();
                int count = Marshal.ReadInt32(table);
                for (int i = 0; i < count; i++)
                {
                    IntPtr row = new IntPtr(table.ToInt64() + TableHeader + (long)i * RowSize);
                    byte flags = Marshal.ReadByte(row, OffFlags);
                    if ((flags & FlagHardware) == 0 || (flags & FlagFilter) != 0)
                        continue;
                    if (Marshal.ReadInt32(row, OffOperStatus) != OperUp ||
                        Marshal.ReadInt32(row, OffMediaConnectState) != MediaConnected)
                        continue;

                    int type = Marshal.ReadInt32(row, OffType);
                    int medium = Marshal.ReadInt32(row, OffPhysicalMedium);
                    NetKind kind;
                    if (type == IfTypeWifi || medium == MediumNative80211 || medium == MediumWirelessLan)
                        kind = NetKind.Wifi;
                    else if (type == IfTypeEthernet && medium != MediumBluetooth)
                        kind = NetKind.Ethernet;
                    else
                        continue;
                    result[Marshal.ReadInt64(row, OffLuid)] = kind;
                }
                return result;
            }
            finally
            {
                FreeMibTable(table);
            }
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

        // network drops always wait at least this long, so a brief blip (a driver hiccup,
        // Wi-Fi hopping between access points) doesn't lock you out
        private const int NetMinDelaySeconds = 2;
        // ignore drops for this long after waking, while Wi-Fi and Ethernet reconnect
        private const int NetSettleSeconds = 15;

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
        private readonly ToolStripMenuItem _ethernetItem;
        private readonly ToolStripMenuItem _wifiItem;
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

        private readonly EventWaitHandle _guardStop; // tells the helper to re-check the guard setting
        private readonly QuitListener _quitListener;
        private readonly Locker _locker;
        private Process _helper;
        private bool _exiting;
        private int _ticks;

        // for getting back onto the UI thread
        private readonly Control _invoker;
        private readonly EventWaitHandle _showSignal;
        private readonly RegisteredWaitHandle _showWait;

        private PowerLineStatus _lastStatus;

        // what's waiting on the lock delay
        private bool _powerPending;
        private readonly Dictionary<long, NetKind> _netPending = new Dictionary<long, NetKind>();
        // adapters connected at the last check; null = start fresh without locking
        private Dictionary<long, NetKind> _netLast;
        private DateTime _netSettleUntil;
        private DateTime _lastTick = DateTime.UtcNow;
        private readonly Icon _iconOn;
        private readonly Icon _iconOff;
        private bool _busy; // true while a dialog is open

        // set only by Quit(): the helper reads it from our exit code and doesn't restart us
        public bool QuitRequested { get; private set; }

        public TrayApp(bool restarted)
        {
            _settings = Settings.Load();
            _lastStatus = CurrentPowerStatus();

            _invoker = new Control();
            IntPtr forceHandle = _invoker.Handle;

            int traySize = SystemInformation.SmallIconSize.Width;
            _iconOn = MakeIcon(true, traySize);
            _iconOff = MakeIcon(false, traySize);

            _statusItem = new ToolStripMenuItem { Enabled = false };

            _enabledItem = new ToolStripMenuItem(Lang.T("Menu.Enabled"), null, delegate { Guarded(ToggleEnabled); });

            _delayMenu = new ToolStripMenuItem(Lang.T("Menu.Delay"));
            foreach (int d in DelayChoices)
            {
                int seconds = d;
                var item = new ToolStripMenuItem(seconds == 0 ? Lang.T("Delay.Instant") : Lang.Seconds(seconds));
                item.Tag = seconds;
                item.Click += delegate { Guarded(delegate { SetDelay(seconds); }); };
                _delayMenu.DropDownItems.Add(item);
            }

            _ethernetItem = new ToolStripMenuItem(Lang.T("Menu.Ethernet"), null,
                delegate { Guarded(delegate { ToggleNetLock(NetKind.Ethernet); }); });
            _wifiItem = new ToolStripMenuItem(Lang.T("Menu.Wifi"), null,
                delegate { Guarded(delegate { ToggleNetLock(NetKind.Wifi); }); });
            var alsoMenu = new ToolStripMenuItem(Lang.T("Menu.AlsoLock"));
            alsoMenu.DropDownItems.Add(_ethernetItem);
            alsoMenu.DropDownItems.Add(_wifiItem);

            _startupItem = new ToolStripMenuItem(Lang.T("Menu.Startup"), null, delegate { Guarded(ToggleStartup); });
            var hideItem = new ToolStripMenuItem(Lang.T("Menu.Hide"), null, delegate { Guarded(HideTrayIcon); });
            _guardItem = new ToolStripMenuItem(Lang.T("Menu.Guard"), null, delegate { Guarded(ToggleGuard); });

            _setPasswordItem = new ToolStripMenuItem(Lang.T("Menu.SetPassword"), null, delegate { Guarded(SetPassword); });
            _removePasswordItem = new ToolStripMenuItem(Lang.T("Menu.RemovePassword"), null, delegate { Guarded(RemovePassword); });
            var passwordMenu = new ToolStripMenuItem(Lang.T("Menu.Password"));
            passwordMenu.DropDownItems.Add(_setPasswordItem);
            passwordMenu.DropDownItems.Add(_removePasswordItem);

            var lockNowItem = new ToolStripMenuItem(Lang.T("Menu.LockNow"), null, delegate { TestLock(); });
            var aboutItem = new ToolStripMenuItem(Lang.T("Menu.About"), null, delegate { Guarded(AboutDialog.Present); });
            var exitItem = new ToolStripMenuItem(Lang.T("Menu.Exit"), null, delegate { Guarded(Exit); });

            _signInItem = new ToolStripMenuItem(Lang.T("Menu.NoSignIn"),
                SystemIcons.Warning.ToBitmap(), delegate { WindowsSignIn.OpenSettings(); }) { Visible = false };
            _signInSeparator = new ToolStripSeparator { Visible = false };

            var menu = new ContextMenuStrip();
            menu.Items.Add(_signInItem);
            menu.Items.Add(_signInSeparator);
            menu.Items.Add(_statusItem);
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(_enabledItem);
            menu.Items.Add(_delayMenu);
            menu.Items.Add(alsoMenu);
            menu.Items.Add(_startupItem);
            menu.Items.Add(_guardItem);
            menu.Items.Add(hideItem);
            menu.Items.Add(passwordMenu);
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(lockNowItem);
            menu.Items.Add(aboutItem);
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

            _locker = new Locker(LockWorkStation, 500, TimeSpan.FromMinutes(10));

            // setup closes us through this, and has to send the password (or recovery code)
            _quitListener = new QuitListener(QuitListener.DefaultPipeName, _settings.Unlocks, delegate
            {
                try { _invoker.BeginInvoke((MethodInvoker)Quit); }
                catch (InvalidOperationException) { /* already shutting down */ }
            });
            _quitListener.Start();

            _guardStop = new EventWaitHandle(false, EventResetMode.AutoReset, Guard.StopSignalName);
            if (_settings.Guard)
                StartGuard();

            RefreshMenu();
            _tray.Visible = !_settings.HideIcon;

            // each check is a failed logon on an account with a password, so skip it after a
            // guard restart: a restart loop would otherwise lock the Windows account
            if (restarted)
                return;
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
            _tray.ShowBalloonTip(10000, Lang.T("NoSignIn.Title"), Lang.T("NoSignIn.Text"), ToolTipIcon.Warning);
        }

        private void OnTick()
        {
            // a long gap between ticks means we were asleep, even if we missed the resume event
            var now = DateTime.UtcNow;
            if ((now - _lastTick).TotalSeconds > 10)
                OnSleepOrWake();
            _lastTick = now;

            CheckPower();
            CheckNetwork();

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

        // call after saving Guard = false: the helper checks the saved setting before it stands down
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
            // logging off / shutting down - don't start a new helper. (The helper isn't told to stand
            // down: anything can send this message, and Windows ends the helper at sign-out anyway.)
            _exiting = true;
        }


        private static PowerLineStatus CurrentPowerStatus()
        {
            return SystemInformation.PowerStatus.PowerLineStatus;
        }

        private void OnPowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            if (e.Mode == PowerModes.Suspend || e.Mode == PowerModes.Resume)
                OnSleepOrWake(); // the network drops going to sleep and reconnects after waking
            if (e.Mode == PowerModes.StatusChange || e.Mode == PowerModes.Resume)
                CheckPower();
        }

        private void OnSleepOrWake()
        {
            _netLast = null;
            _netSettleUntil = DateTime.UtcNow.AddSeconds(NetSettleSeconds);
            _netPending.Clear();
            if (!_powerPending)
                _delayTimer.Stop();
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
                {
                    OnUnplugged();
                }
                else if (now == PowerLineStatus.Online)
                {
                    // power came back during the delay
                    _powerPending = false;
                    if (_netPending.Count == 0)
                        _delayTimer.Stop();
                }

                RefreshMenu();
            }
        }

        private void OnUnplugged()
        {
            if (!_settings.Enabled)
                return;

            if (_settings.DelaySeconds <= 0)
            {
                LockNow();
            }
            else
            {
                _powerPending = true;
                StartDelay(_settings.DelaySeconds);
            }
        }

        private bool WatchesNetwork
        {
            get { return _settings.Enabled && (_settings.LockOnEthernet || _settings.LockOnWifi); }
        }

        private bool Watches(NetKind kind)
        {
            return kind == NetKind.Ethernet ? _settings.LockOnEthernet : _settings.LockOnWifi;
        }

        // like the charger, only a connected -> disconnected change counts. Starting up,
        // turning an option on or waking up while disconnected never locks.
        private void CheckNetwork()
        {
            if (!WatchesNetwork)
            {
                _netLast = null;
                return;
            }
            var now = NetworkLinks.Connected();
            if (now == null)
                return;

            // reconnected during the delay
            foreach (long luid in new List<long>(_netPending.Keys))
                if (now.ContainsKey(luid))
                    _netPending.Remove(luid);
            if (_netPending.Count == 0 && !_powerPending)
                _delayTimer.Stop();

            var dropped = new List<KeyValuePair<long, NetKind>>();
            if (_netLast != null && DateTime.UtcNow >= _netSettleUntil)
                foreach (var link in _netLast)
                    if (!now.ContainsKey(link.Key) && Watches(link.Value))
                        dropped.Add(link);
            _netLast = now;

            if (dropped.Count == 0)
                return;
            foreach (var link in dropped)
                _netPending[link.Key] = link.Value;
            StartDelay(Math.Max(_settings.DelaySeconds, NetMinDelaySeconds));
        }

        // a delay that's already running keeps going, so a second trigger never pushes the lock back
        private void StartDelay(int seconds)
        {
            if (_delayTimer.Enabled)
                return;
            _delayTimer.Interval = Math.Max(1, Math.Min(Settings.MaxDelaySeconds, seconds)) * 1000;
            _delayTimer.Start();
        }

        private void OnDelayElapsed()
        {
            _delayTimer.Stop();
            bool lockIt = false;
            if (_settings.Enabled)
            {
                // still unplugged?
                if (_powerPending && CurrentPowerStatus() == PowerLineStatus.Offline)
                    lockIt = true;

                // still disconnected? (if the adapters can't be read, assume so)
                if (!lockIt && _netPending.Count > 0)
                {
                    var now = NetworkLinks.Connected();
                    foreach (var link in _netPending)
                        if (Watches(link.Value) && (now == null || !now.ContainsKey(link.Key)))
                            lockIt = true;
                }
            }
            ClearPending();
            if (lockIt)
                _locker.Lock();
        }

        private void LockNow()
        {
            ClearPending();
            _locker.Lock();
        }

        // "Lock now": one try, and say so if it didn't work rather than retrying later
        private void TestLock()
        {
            ClearPending();
            if (LockWorkStation())
                return;
            int error = Marshal.GetLastWin32Error();
            MessageBox.Show(Lang.T("LockFailed", error), "UnplugLock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void ClearPending()
        {
            _locker.Cancel();
            _delayTimer.Stop();
            _powerPending = false;
            _netPending.Clear();
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

        // promptKey is an Auth.* text saying what the password is for
        private bool Authorize(string promptKey)
        {
            if (!_settings.HasPassword)
                return true;

            while (true)
            {
                string pw;
                var result = PasswordDialog.Ask(Lang.T(promptKey), _settings, out pw);
                if (result == AskResult.Cancelled)
                    return false;
                if (result == AskResult.Recovered)
                {
                    _settings.SetPassword(null);
                    _settings.Save();
                    MessageBox.Show(Lang.T("Auth.Recovered"), "UnplugLock",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return true;
                }
                // hashing takes about a second, so do it off the UI thread: unplugging still locks meanwhile
                if (Ui.Busy(delegate { return _settings.CheckPassword(pw); }))
                    return true;

                Ui.Pause(1000); // slow down guessing
                MessageBox.Show(Lang.T("WrongPassword"), "UnplugLock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ToggleEnabled()
        {
            if (_settings.Enabled && !Authorize("Auth.TurnOff"))
                return;

            _settings.Enabled = !_settings.Enabled;
            if (!_settings.Enabled)
                ClearPending();
            _settings.Save();
        }

        private void ToggleNetLock(NetKind kind)
        {
            bool on = Watches(kind);
            if (on && !Authorize(kind == NetKind.Ethernet ? "Auth.StopEthernet" : "Auth.StopWifi"))
                return;

            if (kind == NetKind.Ethernet)
                _settings.LockOnEthernet = !on;
            else
                _settings.LockOnWifi = !on;
            _settings.Save();

            if (on)
            {
                foreach (var link in new List<KeyValuePair<long, NetKind>>(_netPending))
                    if (link.Value == kind)
                        _netPending.Remove(link.Key);
                if (_netPending.Count == 0 && !_powerPending)
                    _delayTimer.Stop();
            }
            CheckNetwork(); // start watching straight away
        }

        private void SetDelay(int seconds)
        {
            if (seconds == _settings.DelaySeconds || !Authorize("Auth.Delay"))
                return;
            _settings.DelaySeconds = seconds;
            _settings.Save();
        }

        private void ToggleGuard()
        {
            if (_settings.Guard)
            {
                if (!Authorize("Auth.Guard"))
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
            if (on && !Authorize("Auth.Startup"))
                return;
            try
            {
                Settings.StartsWithWindows = !on;
            }
            catch (Exception ex)
            {
                MessageBox.Show(Lang.T("StartupFailed", ex.Message), "UnplugLock",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void HideTrayIcon()
        {
            var answer = MessageBox.Show(Lang.T("HideConfirm"),
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
            _tray.ShowBalloonTip(4000, Lang.T("Running.Title"),
                Lang.T(_settings.Enabled ? "Running.On" : "Running.Off"), ToolTipIcon.Info);
        }

        private void SetPassword()
        {
            if (_settings.HasPassword && !Authorize("Auth.ChangePassword"))
                return;

            string pw = PasswordDialog.AskNew();
            if (pw == null)
                return;
            string code = Settings.NewRecoveryCode();
            _settings.CopyPasswordFrom(Ui.Busy(delegate
            {
                var hashed = new Settings();
                hashed.SetPassword(pw, code);
                return hashed;
            }));
            RecoveryCodeDialog.Present(code);
            _settings.Save();
        }

        private void RemovePassword()
        {
            if (!_settings.HasPassword || !Authorize("Auth.RemovePassword"))
                return;
            _settings.SetPassword(null);
            _settings.Save();
            MessageBox.Show(Lang.T("PasswordRemoved"), "UnplugLock", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Exit()
        {
            if (Authorize("Auth.Exit"))
                Quit();
        }

        // clean exit: our exit code tells the helper not to restart us
        private void Quit()
        {
            if (QuitRequested)
                return;
            QuitRequested = true;
            _exiting = true;
            ExitThread();
        }


        private void RefreshMenu()
        {
            string power;
            switch (_lastStatus)
            {
                case PowerLineStatus.Online: power = "Online"; break;
                case PowerLineStatus.Offline: power = "Offline"; break;
                default: power = "Unknown"; break;
            }

            _statusItem.Text = Lang.T("Power." + power);
            _enabledItem.Checked = _settings.Enabled;
            _startupItem.Checked = Settings.StartsWithWindows;
            _startupItem.Enabled = !_settings.Guard;
            _startupItem.Text = Lang.T(_settings.Guard ? "Menu.StartupGuarded" : "Menu.Startup");
            _guardItem.Checked = _settings.Guard;
            _ethernetItem.Checked = _settings.LockOnEthernet;
            _wifiItem.Checked = _settings.LockOnWifi;
            foreach (ToolStripItem item in _delayMenu.DropDownItems)
            {
                var mi = item as ToolStripMenuItem;
                if (mi != null)
                    mi.Checked = (int)mi.Tag == _settings.DelaySeconds;
            }
            _setPasswordItem.Text = Lang.T(_settings.HasPassword ? "Menu.ChangePassword" : "Menu.SetPassword");
            _removePasswordItem.Enabled = _settings.HasPassword;
            _signInItem.Visible = _noSignIn;
            _signInSeparator.Visible = _noSignIn;

            _tray.Icon = _settings.Enabled ? _iconOn : _iconOff;
            string tip = _noSignIn
                ? Lang.T("Tip.NoSignIn")
                : Lang.T(_settings.Enabled ? "Tip.On" : "Tip.Off", Lang.T("Tip." + power));
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
            _helperRestart.Stop();
            DropHelper();
            _showWait.Unregister(null);
            _showSignal.Dispose();
            _quitListener.Stop();
            _locker.Dispose();
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
