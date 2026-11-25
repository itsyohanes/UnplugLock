// Installer/uninstaller. Builds together with UnplugLock.cs, and build.cmd adds UnplugLock.exe
// onto the end of it (see Payload).
// Installs per-user to %LOCALAPPDATA%\Programs\UnplugLock, no admin needed.
//
// UnplugLock-Setup.exe [/silent] [/nostartup] [/nolaunch] [/desktop] [/noguard]
// Uninstall.exe [/uninstall] [/silent]

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;

namespace UnplugLock
{
    internal static class SetupProgram
    {
        public const string Version = Program.Version;

        private const string UninstallKey = @"Software\Microsoft\Windows\CurrentVersion\Uninstall\UnplugLock";
        private const string RunKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string SettingsKey = @"Software\UnplugLock";

        public static string InstallDir
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Programs", "UnplugLock");
            }
        }

        private static string AppExe { get { return Path.Combine(InstallDir, "UnplugLock.exe"); } }
        private static string UninstallExe { get { return Path.Combine(InstallDir, "Uninstall.exe"); } }

        private static string StartMenuLink
        {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Programs), "UnplugLock.lnk"); }
        }

        private static string DesktopLink
        {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "UnplugLock.lnk"); }
        }

        // needs both the exe and the uninstall entry
        public static bool IsInstalled
        {
            get
            {
                if (!File.Exists(AppExe))
                    return false;
                using (var key = Registry.CurrentUser.OpenSubKey(UninstallKey))
                    return key != null;
            }
        }

        [STAThread]
        private static int Main(string[] args)
        {
            // build step (build.cmd): adds the app onto the end of this program to make the installer
            if (args.Length == 4 && args[0] == "--pack")
            {
                Payload.Pack(args[1], args[2], args[3]);
                return 0;
            }

            Ui.Init();
            Lang.Use(Settings.Load(false).Language);

            bool silent = HasArg(args, "/silent");
            bool uninstall = HasArg(args, "/uninstall") ||
                Path.GetFileName(Application.ExecutablePath).Equals("Uninstall.exe", StringComparison.OrdinalIgnoreCase);

            if (uninstall)
                return Uninstall(silent) ? 0 : 1;

            if (silent)
            {
                // can't ask for the password in silent mode
                if (Settings.Load().HasPassword)
                    return 2;
                try
                {
                    Install(!HasArg(args, "/nostartup"), HasArg(args, "/desktop"), !HasArg(args, "/nolaunch"),
                        !HasArg(args, "/noguard"), null, null);
                    return 0;
                }
                catch
                {
                    return 1;
                }
            }

            Application.Run(new SetupForm());
            return 0;
        }

        private static bool HasArg(string[] args, string name)
        {
            foreach (var a in args)
                if (a.Equals(name, StringComparison.OrdinalIgnoreCase))
                    return true;
            return false;
        }

        // secret is the password or recovery code that was accepted (null with no password set).
        // The running app needs it to agree to close.
        // promptKey is a Setup.Auth* text saying what the password is for
        public static bool AuthorizeExisting(string promptKey, out string secret)
        {
            secret = null;
            var settings = Settings.Load();
            if (!settings.HasPassword)
                return true;

            while (true)
            {
                string pw;
                var result = PasswordDialog.Ask(Lang.T(promptKey), settings, out pw);
                if (result == AskResult.Cancelled)
                    return false;
                if (result == AskResult.Recovered || Ui.Busy(delegate { return settings.CheckPassword(pw); }))
                {
                    secret = pw;
                    return true;
                }
                Ui.Pause(1000);
                MessageBox.Show(Lang.T("WrongPassword"), "UnplugLock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        // language: the one picked in setup, or null to keep what's saved
        public static void Install(bool startWithWindows, bool desktopShortcut, bool launch, bool guard, string secret,
            string language)
        {
            if (guard)
                startWithWindows = true; // the guard needs it

            if (!StopRunningCopies(secret))
                throw new InvalidOperationException(Lang.T("Setup.StillRunning"));

            Directory.CreateDirectory(InstallDir);
            TryDelete(Path.Combine(InstallDir, Guard.HelperName + ".exe")); // old version, app recreates it

            var settings = Settings.Load();
            settings.Guard = guard;
            if (language != null)
                settings.Language = language;
            settings.Save();

            byte[] app = Payload.ReadApp(Application.ExecutablePath);
            if (app == null)
                throw new InvalidOperationException(Lang.T("Setup.Damaged"));
            File.WriteAllBytes(AppExe, app);

            // copy ourselves over as the uninstaller, without the app it would never use
            if (!SamePath(Application.ExecutablePath, UninstallExe))
                Payload.WriteProgram(Application.ExecutablePath, UninstallExe);

            CreateShortcut(StartMenuLink, AppExe);
            if (desktopShortcut)
                CreateShortcut(DesktopLink, AppExe);
            else
                TryDelete(DesktopLink);

            using (var key = Registry.CurrentUser.CreateSubKey(RunKey))
            {
                if (startWithWindows)
                    key.SetValue("UnplugLock", Quote(AppExe));
                else
                    key.DeleteValue("UnplugLock", false);
            }

            long sizeKb = (new FileInfo(AppExe).Length + new FileInfo(UninstallExe).Length) / 1024;
            using (var key = Registry.CurrentUser.CreateSubKey(UninstallKey))
            {
                key.SetValue("DisplayName", "UnplugLock");
                key.SetValue("DisplayVersion", Version);
                key.SetValue("Publisher", "UnplugLock");
                key.SetValue("DisplayIcon", AppExe);
                key.SetValue("InstallLocation", InstallDir);
                key.SetValue("InstallDate", DateTime.Now.ToString("yyyyMMdd"));
                key.SetValue("UninstallString", Quote(UninstallExe) + " /uninstall");
                key.SetValue("QuietUninstallString", Quote(UninstallExe) + " /uninstall /silent");
                key.SetValue("EstimatedSize", (int)Math.Max(1, sizeKb), RegistryValueKind.DWord);
                key.SetValue("NoModify", 1, RegistryValueKind.DWord);
                key.SetValue("NoRepair", 1, RegistryValueKind.DWord);
            }

            if (launch)
                Process.Start(new ProcessStartInfo(AppExe) { WorkingDirectory = InstallDir });
        }


        private static bool Uninstall(bool silent)
        {
            string secret = null;
            if (silent)
            {
                if (Settings.Load().HasPassword)
                    return false; // can't prompt when silent
            }
            else
            {
                if (!AuthorizeExisting("Setup.AuthUninstall", out secret))
                    return false;
                var answer = MessageBox.Show(Lang.T("Uninstall.Confirm"),
                    Lang.T("Uninstall.Title"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (answer != DialogResult.Yes)
                    return false;
            }

            // a copy still running would put its settings straight back
            if (!StopRunningCopies(secret))
            {
                if (!silent)
                    MessageBox.Show(Lang.T("Uninstall.StillRunning"), "UnplugLock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            using (var key = Registry.CurrentUser.OpenSubKey(RunKey, true))
                if (key != null)
                    key.DeleteValue("UnplugLock", false);
            Settings.DeleteAll();
            Registry.CurrentUser.DeleteSubKeyTree(UninstallKey, false);

            TryDelete(StartMenuLink);
            TryDelete(DesktopLink);
            TryDelete(AppExe);

            if (!silent)
                MessageBox.Show(Lang.T("Uninstall.Done"), "UnplugLock",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

            // can't delete ourselves while running, so cmd cleans up the folder after we exit
            if (Directory.Exists(InstallDir))
            {
                var psi = new ProcessStartInfo("cmd.exe",
                    "/c ping -n 3 127.0.0.1 >nul & rmdir /s /q " + Quote(InstallDir))
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WorkingDirectory = Path.GetTempPath()
                };
                Process.Start(psi);
            }
            return true;
        }


        // true once nothing is left running
        private static bool StopRunningCopies(string secret)
        {
            // ask nicely first - a guarded copy can't be killed, and its helper would restart it anyway.
            // This version only closes when given the password (or recovery code), if one is set.
            QuitListener.RequestQuit(QuitListener.DefaultPipeName, secret, 1000);
            // 1.3.0 and earlier listen on a named event instead
            try
            {
                using (var quit = System.Threading.EventWaitHandle.OpenExisting(Guard.LegacyQuitSignalName))
                    quit.Set();
            }
            catch { /* not running, or a newer version */ }

            for (int i = 0; i < 50 && AnyRunning(); i++)
                System.Threading.Thread.Sleep(100);

            foreach (string name in new[] { "UnplugLock", Guard.HelperName })
            {
                foreach (var p in Process.GetProcessesByName(name))
                {
                    using (p)
                    {
                        try
                        {
                            p.Kill();
                            p.WaitForExit(5000);
                        }
                        catch { /* already gone, or not ours */ }
                    }
                }
            }
            return !AnyRunning();
        }

        // copies in this Windows session (another user's copy on the same PC isn't ours to close)
        private static bool AnyRunning()
        {
            int session = Process.GetCurrentProcess().SessionId;
            foreach (string name in new[] { "UnplugLock", Guard.HelperName })
            {
                var procs = Process.GetProcessesByName(name);
                bool found = false;
                foreach (var p in procs)
                {
                    try { found |= p.SessionId == session; }
                    catch { /* gone */ }
                    p.Dispose();
                }
                if (found)
                    return true;
            }
            return false;
        }

        private static void CreateShortcut(string linkPath, string target)
        {
            Type shellType = Type.GetTypeFromProgID("WScript.Shell");
            object shell = Activator.CreateInstance(shellType);
            try
            {
                object link = shellType.InvokeMember("CreateShortcut", BindingFlags.InvokeMethod, null, shell,
                    new object[] { linkPath });
                try
                {
                    Type t = link.GetType();
                    t.InvokeMember("TargetPath", BindingFlags.SetProperty, null, link, new object[] { target });
                    t.InvokeMember("WorkingDirectory", BindingFlags.SetProperty, null, link,
                        new object[] { Path.GetDirectoryName(target) });
                    t.InvokeMember("IconLocation", BindingFlags.SetProperty, null, link, new object[] { target + ",0" });
                    t.InvokeMember("Description", BindingFlags.SetProperty, null, link,
                        new object[] { Lang.T("Setup.ShortcutDescription") });
                    t.InvokeMember("Save", BindingFlags.InvokeMethod, null, link, null);
                }
                finally
                {
                    Marshal.FinalReleaseComObject(link);
                }
            }
            finally
            {
                Marshal.FinalReleaseComObject(shell);
            }
        }

        private static void TryDelete(string path)
        {
            try
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
            catch { /* best effort */ }
        }

        private static bool SamePath(string a, string b)
        {
            return string.Equals(Path.GetFullPath(a), Path.GetFullPath(b), StringComparison.OrdinalIgnoreCase);
        }

        private static string Quote(string s)
        {
            return "\"" + s + "\"";
        }
    }

    internal sealed class SetupForm : Dialog
    {
        private readonly bool _upgrading;
        private readonly CheckBox _startup;
        private readonly CheckBox _desktop;
        private readonly CheckBox _launch;
        private readonly CheckBox _guard;
        private readonly RoundButton _install;
        private readonly RoundButton _cancel;
        private readonly ComboBox _language;
        private readonly Label _languageLabel;
        private readonly Label _guardHint;
        private readonly Label _installsTo;
        private readonly Label _perUser;
        private readonly Label _version;

        public SetupForm() : this(SetupProgram.IsInstalled) { }

        // the text is filled in by ShowText, which runs again whenever another language is picked
        private SetupForm(bool upgrading)
            : base("", "", "", 440, true)
        {
            _upgrading = upgrading;
            MinimizeBox = true;

            // starts on the language saved by an earlier install, else the Windows language
            _languageLabel = AddText("", Ui.Muted, Ui.Small, 4);
            _language = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                DrawMode = DrawMode.OwnerDrawFixed,
                ItemHeight = Ui.S(20),
                MaxDropDownItems = 16,
                Width = Ui.S(220),
                Margin = Ui.S(0, 0, 0, 20)
            };
            _language.DrawItem += DrawLanguage;
            foreach (var choice in Lang.Choices())
            {
                int i = _language.Items.Add(new LanguageChoice(choice.Key, choice.Value));
                if (choice.Key == Lang.Name)
                    _language.SelectedIndex = i;
            }
            _language.SelectedIndexChanged += delegate
            {
                Lang.Use(((LanguageChoice)_language.SelectedItem).Name);
                ShowText();
            };
            Body.Controls.Add(_language);

            _guard = Option(0);
            _guardHint = AddText("", Ui.Muted, Ui.Small, 14);
            _guardHint.Padding = new Padding(Ui.S(18), 0, 0, 0);
            _startup = Option(12);
            _desktop = Option(12);
            _launch = Option(22);
            _guard.Checked = true;
            _startup.Checked = true;
            _launch.Checked = true;

            // the guard always starts with Windows
            _guard.CheckedChanged += delegate
            {
                if (_guard.Checked)
                    _startup.Checked = true;
                _startup.Enabled = !_guard.Checked;
            };
            _startup.Enabled = false;

            _installsTo = AddText("", Ui.Muted, Ui.Small, 2);
            AddText(SetupProgram.InstallDir, Ui.Ink, Ui.Small, 6);
            _perUser = AddText("", Ui.Muted, Ui.Small, 0);

            _version = Ui.Text("", ContentWidth, Ui.Muted, Ui.Small, 0);
            SetFooterNote(_version);
            _cancel = AddButton("", false);
            _install = AddButton("", true);
            _install.Click += OnInstall;
            // DialogResult doesn't close a non-modal form
            _cancel.Click += delegate { Close(); };

            AcceptButton = _install;
            CancelButton = _cancel;
            ShowText();
        }

        private void ShowText()
        {
            SuspendLayout();
            Text = Lang.T("Setup.Window");
            Heading.Text = Lang.T(_upgrading ? "Setup.TitleUpdate" : "Setup.TitleInstall");
            Subheading.Text = Lang.T("AppTagline");
            _languageLabel.Text = Lang.T("Setup.Language");
            _guard.Text = Lang.T("Setup.Guard");
            _guardHint.Text = Lang.T("Setup.GuardHint");
            _startup.Text = Lang.T("Setup.Startup");
            _desktop.Text = Lang.T("Setup.Desktop");
            _launch.Text = Lang.T("Setup.Launch");
            _installsTo.Text = Lang.T("Setup.InstallsTo");
            _perUser.Text = Lang.T("Setup.PerUser");
            _version.Text = Lang.T("Version", SetupProgram.Version);
            _cancel.Text = Lang.T("Button.Cancel");
            _install.Text = Lang.T(_upgrading ? "Setup.Update" : "Setup.Install");
            ResumeLayout(true);
        }

        // each language with its flag in front, as on the website's picker
        private void DrawLanguage(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            if (e.Index < 0)
                return;
            var choice = (LanguageChoice)_language.Items[e.Index];
            int w = Ui.S(21), h = Ui.S(14);
            var flag = new Rectangle(e.Bounds.X + Ui.S(4), e.Bounds.Y + (e.Bounds.Height - h) / 2, w, h);
            Flags.Draw(e.Graphics, choice.Name, flag);
            var text = new Rectangle(flag.Right + Ui.S(8), e.Bounds.Y, e.Bounds.Right - flag.Right - Ui.S(8), e.Bounds.Height);
            TextRenderer.DrawText(e.Graphics, choice.ToString(), e.Font, text, e.ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis);
            e.DrawFocusRectangle();
        }

        private sealed class LanguageChoice
        {
            public readonly string Name;
            private readonly string _display;

            public LanguageChoice(string name, string display)
            {
                Name = name;
                _display = display;
            }

            public override string ToString() { return _display; }
        }

        private CheckBox Option(float spaceAfter)
        {
            var box = new CheckBox
            {
                AutoSize = true,
                ForeColor = Ui.Ink,
                Margin = Ui.S(0, 0, 0, spaceAfter)
            };
            Body.Controls.Add(box);
            return box;
        }

        private void OnInstall(object sender, EventArgs e)
        {
            // asks whenever a password is set, however the existing install looks
            // (the uninstall entry, for example, can be deleted without a password)
            string secret;
            if (!SetupProgram.AuthorizeExisting("Setup.AuthUpdate", out secret))
                return;

            _install.Enabled = false;
            _cancel.Enabled = false;
            _install.Text = Lang.T(_upgrading ? "Setup.Updating" : "Setup.Installing");
            Cursor = Cursors.WaitCursor;
            Update();
            try
            {
                SetupProgram.Install(_startup.Checked, _desktop.Checked, _launch.Checked, _guard.Checked, secret,
                    ((LanguageChoice)_language.SelectedItem).Name);
                Cursor = Cursors.Default;
                ShowDone(_launch.Checked);
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                _install.Text = Lang.T(_upgrading ? "Setup.Update" : "Setup.Install");
                MessageBox.Show(this, Lang.T("Setup.Failed", ex.Message), "UnplugLock",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                _install.Enabled = true;
                _cancel.Enabled = true;
            }
        }

        // swaps the options for a short "you're done" page
        private void ShowDone(bool launched)
        {
            SuspendLayout();
            Heading.Text = Lang.T(_upgrading ? "Setup.Updated" : "Setup.Installed");
            Subheading.Text = Lang.T("Setup.DoneSubtitle");

            Body.Controls.Clear();
            AddText(Lang.T(launched ? "Setup.DoneRunning" : "Setup.DoneNotRunning"), Ui.Ink, Ui.Body, 10);
            AddText(Lang.T("Setup.DoneHint"),
                Ui.Muted, Ui.Body, 0);

            _cancel.Visible = false;
            _install.Text = Lang.T("Setup.Finish");
            _install.Enabled = true;
            _install.Click -= OnInstall;
            _install.Click += delegate { Close(); };
            ResumeLayout(true);
            _install.Focus();
        }
    }

    // The installer is setup's own program with UnplugLock.exe added on the end, then a 16-byte
    // trailer: the app's length and a marker. build.cmd puts them together with "--pack".
    // Uninstall.exe is the program on its own, so it doesn't carry a copy of the app it never uses.
    internal static class Payload
    {
        private static readonly byte[] Marker = { 0x55, 0x6C, 0x41, 0x70, 0x70, 0x30, 0x30, 0x31 }; // "UlApp001"
        private const int TrailerSize = 16;

        public static void Pack(string program, string app, string output)
        {
            byte[] appBytes = File.ReadAllBytes(app);
            using (var input = OpenRead(program))
            using (var file = File.Create(output))
            {
                if (AppLength(input) != 0)
                    throw new InvalidOperationException(program + " already has an app packed in");
                input.Position = 0;
                input.CopyTo(file);
                file.Write(appBytes, 0, appBytes.Length);
                file.Write(BitConverter.GetBytes((long)appBytes.Length), 0, 8);
                file.Write(Marker, 0, Marker.Length);
            }
        }

        // the app packed onto the end of file, or null if it has none
        public static byte[] ReadApp(string file)
        {
            using (var input = OpenRead(file))
            {
                long length = AppLength(input);
                if (length == 0)
                    return null;
                var app = new byte[length];
                input.Position = input.Length - TrailerSize - length;
                ReadFully(input, app);
                return app;
            }
        }

        // copies file to output without the app on its end
        public static void WriteProgram(string file, string output)
        {
            using (var input = OpenRead(file))
            using (var copy = File.Create(output))
            {
                long length = AppLength(input);
                long remaining = length == 0 ? input.Length : input.Length - TrailerSize - length;
                input.Position = 0;
                var buffer = new byte[81920];
                while (remaining > 0)
                {
                    int read = input.Read(buffer, 0, (int)Math.Min(buffer.Length, remaining));
                    if (read <= 0)
                        throw new EndOfStreamException();
                    copy.Write(buffer, 0, read);
                    remaining -= read;
                }
            }
        }

        // a running program can still be read
        private static FileStream OpenRead(string file)
        {
            return new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
        }

        private static long AppLength(FileStream input)
        {
            if (input.Length < TrailerSize)
                return 0;
            var trailer = new byte[TrailerSize];
            input.Position = input.Length - TrailerSize;
            ReadFully(input, trailer);
            for (int i = 0; i < Marker.Length; i++)
                if (trailer[8 + i] != Marker[i])
                    return 0;
            long length = BitConverter.ToInt64(trailer, 0);
            return length > 0 && length <= input.Length - TrailerSize ? length : 0;
        }

        private static void ReadFully(Stream input, byte[] buffer)
        {
            for (int done = 0; done < buffer.Length; )
            {
                int read = input.Read(buffer, done, buffer.Length - done);
                if (read <= 0)
                    throw new EndOfStreamException();
                done += read;
            }
        }
    }

    // Setup has only English built in. Its other languages come from the app instead of a second
    // copy: the installer reads them from the app packed on its end, and Uninstall.exe from the
    // installed UnplugLock.exe next to it. With neither there, setup is in English.
    internal static class SetupLanguages
    {
        public static Dictionary<string, Func<Dictionary<string, string>>> Load()
        {
            return Load(Application.ExecutablePath);
        }

        internal static Dictionary<string, Func<Dictionary<string, string>>> Load(string program)
        {
            try
            {
                byte[] app = Payload.ReadApp(program);
                if (app == null)
                {
                    string installed = Path.Combine(Path.GetDirectoryName(program), "UnplugLock.exe");
                    if (File.Exists(installed))
                        app = File.ReadAllBytes(installed); // loaded from memory, so it can still be deleted
                }
                if (app != null)
                    return FromApp(Assembly.Load(app));
            }
            catch { /* a damaged or unexpected app: English it is */ }
            return new Dictionary<string, Func<Dictionary<string, string>>>(StringComparer.OrdinalIgnoreCase);
        }

        internal static Dictionary<string, Func<Dictionary<string, string>>> FromApp(Assembly app)
        {
            const BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            Type lang = app.GetType("UnplugLock.Lang", true);
            var available = (IEnumerable<string>)lang.GetProperty("Available", flags).GetValue(null, null);
            MethodInfo tableFor = lang.GetMethod("TableFor", flags);
            var languages = new Dictionary<string, Func<Dictionary<string, string>>>(StringComparer.OrdinalIgnoreCase);
            foreach (string name in available)
            {
                string language = name;
                languages[language] = delegate
                {
                    return (Dictionary<string, string>)tableFor.Invoke(null, new object[] { language });
                };
            }
            return languages;
        }
    }

    // the flags in the setup language picker, drawn to match the website's html/img/flags/*.svg
    internal static class Flags
    {
        // draws the flag of language into r; false if there's no flag for it
        public static bool Draw(Graphics g, string language, Rectangle r)
        {
            var state = g.Save();
            bool known = true;
            try
            {
                g.SetClip(r);
                g.TranslateTransform(r.X, r.Y);
                // every flag is drawn on a 3 x 2 grid, as in the SVGs
                g.ScaleTransform(r.Width / 3f, r.Height / 2f);
                switch (language)
                {
                    case "cs":
                        Stripes(g, false, 0xFFFFFF, 0xD7141A);
                        Shape(g, 0x11457E, delegate(GraphicsPath p)
                        {
                            p.AddPolygon(new[] { new PointF(0, 0), new PointF(1.5f, 1), new PointF(0, 2) });
                        });
                        break;
                    case "de": Stripes(g, false, 0x000000, 0xDD0000, 0xFFCE00); break;
                    case "en": UnionJack(g); break;
                    case "es": Bands(g, 0xAA151B, 0.5f, 0xF1BF00, 1, 0xAA151B, 0.5f); break;
                    case "fr": Stripes(g, true, 0x002654, 0xFFFFFF, 0xCE1126); break;
                    case "id": Stripes(g, false, 0xCE1126, 0xFFFFFF); break;
                    case "it": Stripes(g, true, 0x009246, 0xFFFFFF, 0xCE2B37); break;
                    case "ja":
                        Stripes(g, false, 0xFFFFFF);
                        Shape(g, 0xBC002D, delegate(GraphicsPath p) { p.AddEllipse(0.9f, 0.4f, 1.2f, 1.2f); });
                        break;
                    case "ko": Taegukgi(g); break;
                    case "nl": Stripes(g, false, 0xAE1C28, 0xFFFFFF, 0x21468B); break;
                    case "pl": Stripes(g, false, 0xFFFFFF, 0xDC143C); break;
                    case "pt": Brazil(g); break;
                    case "ru": Stripes(g, false, 0xFFFFFF, 0x0039A6, 0xD52B1E); break;
                    case "th":
                        float u = 2f / 6;
                        Bands(g, 0xA51931, u, 0xF4F5F8, u, 0x2D2A4A, 2 * u, 0xF4F5F8, u, 0xA51931, u);
                        break;
                    case "tr": Turkey(g); break;
                    case "uk": Stripes(g, false, 0x0057B7, 0xFFD700); break;
                    case "vi":
                        Stripes(g, false, 0xDA251D);
                        Star(g, 1.5f, 1, 0, 0.6f, 0xFFFF00);
                        break;
                    case "zh-Hans": China(g); break;
                    case "zh-Hant": Script(g, "繁"); break;
                    default: known = false; break;
                }
            }
            finally
            {
                g.Restore(state);
            }
            if (known)
            {
                // a faint edge, so white parts don't melt into the background
                using (var pen = new Pen(Color.FromArgb(48, 0, 0, 0)))
                    g.DrawRectangle(pen, r.X, r.Y, r.Width - 1, r.Height - 1);
            }
            return known;
        }

        private static Color Rgb(int rgb) { return Color.FromArgb(255, Color.FromArgb(rgb)); }

        // equal stripes, top to bottom or left to right
        private static void Stripes(Graphics g, bool vertical, params int[] colors)
        {
            float step = (vertical ? 3f : 2f) / colors.Length;
            for (int i = 0; i < colors.Length; i++)
                using (var b = new SolidBrush(Rgb(colors[i])))
                {
                    if (vertical)
                        g.FillRectangle(b, i * step, 0, step + 0.01f, 2);
                    else
                        g.FillRectangle(b, 0, i * step, 3, step + 0.01f);
                }
        }

        // horizontal bands of given heights: color, height, color, height, ...
        private static void Bands(Graphics g, params object[] colorsAndHeights)
        {
            float y = 0;
            for (int i = 0; i < colorsAndHeights.Length; i += 2)
            {
                float height = Convert.ToSingle(colorsAndHeights[i + 1]);
                using (var b = new SolidBrush(Rgb((int)colorsAndHeights[i])))
                    g.FillRectangle(b, 0, y, 3, height + 0.01f);
                y += height;
            }
        }

        // an antialiased shape on top of the stripes
        private static void Shape(Graphics g, int color, Action<GraphicsPath> build)
        {
            var smoothing = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (var p = new GraphicsPath())
            using (var b = new SolidBrush(Rgb(color)))
            {
                build(p);
                g.FillPath(b, p);
            }
            g.SmoothingMode = smoothing;
        }

        private static void UnionJack(Graphics g)
        {
            g.ScaleTransform(3f / 60, 2f / 40);
            using (var b = new SolidBrush(Rgb(0x012169)))
                g.FillRectangle(b, 0, 0, 60, 40);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (var white = new Pen(Color.White, 8))
            {
                g.DrawLine(white, 0, 0, 60, 40);
                g.DrawLine(white, 60, 0, 0, 40);
            }
            // the red diagonals sit off-centre, on the side of each white band that turns clockwise
            var clip = g.Clip;
            using (var p = new GraphicsPath())
            {
                p.AddPolygon(new[] { new PointF(30, 20), new PointF(60, 20), new PointF(60, 40) });
                p.AddPolygon(new[] { new PointF(30, 20), new PointF(30, 40), new PointF(0, 40) });
                p.AddPolygon(new[] { new PointF(30, 20), new PointF(0, 20), new PointF(0, 0) });
                p.AddPolygon(new[] { new PointF(30, 20), new PointF(30, 0), new PointF(60, 0) });
                g.SetClip(p, CombineMode.Intersect);
            }
            using (var red = new Pen(Rgb(0xC8102E), 5))
            {
                g.DrawLine(red, 0, 0, 60, 40);
                g.DrawLine(red, 60, 0, 0, 40);
            }
            g.Clip = clip;
            g.SmoothingMode = SmoothingMode.None;
            using (var b = new SolidBrush(Color.White))
            {
                g.FillRectangle(b, 24, 0, 12, 40);
                g.FillRectangle(b, 0, 14, 60, 12);
            }
            using (var b = new SolidBrush(Rgb(0xC8102E)))
            {
                g.FillRectangle(b, 26.5f, 0, 7, 40);
                g.FillRectangle(b, 0, 16.5f, 60, 7);
            }
        }

        private static void Taegukgi(Graphics g)
        {
            Stripes(g, false, 0xFFFFFF);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TranslateTransform(1.5f, 1);
            var centre = g.Save();
            g.RotateTransform(33.69f);
            using (var b = new SolidBrush(Rgb(0x0047A0)))
                g.FillEllipse(b, -0.5f, -0.5f, 1, 1);
            using (var p = new GraphicsPath())
            using (var b = new SolidBrush(Rgb(0xCD2E3A)))
            {
                p.AddArc(-0.5f, -0.5f, 1, 1, 180, 180);
                p.AddArc(0, -0.25f, 0.5f, 0.5f, 0, 180);
                p.AddArc(-0.5f, -0.25f, 0.5f, 0.5f, 0, -180);
                p.CloseFigure();
                g.FillPath(b, p);
            }
            g.Restore(centre);
            // the four trigrams, one towards each corner
            foreach (float angle in new[] { 56.31f, -56.31f, 123.69f, -123.69f })
            {
                var s = g.Save();
                g.RotateTransform(angle);
                for (int i = 0; i < 3; i++)
                    g.FillRectangle(Brushes.Black, -0.15f, -0.86f + i * 0.08f, 0.3f, 0.05f);
                g.Restore(s);
            }
        }

        private static void Brazil(Graphics g)
        {
            Stripes(g, false, 0x009C3B);
            Shape(g, 0xFFDF00, delegate(GraphicsPath p)
            {
                p.AddPolygon(new[] { new PointF(0.25f, 1), new PointF(1.5f, 0.17f), new PointF(2.75f, 1), new PointF(1.5f, 1.83f) });
            });
            Shape(g, 0x002776, delegate(GraphicsPath p) { p.AddEllipse(1, 0.5f, 1, 1); });
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (var band = new Pen(Color.White, 0.07f))
                g.DrawBezier(band, 1.02f, 0.88f, 1.3733f, 0.84f, 1.6933f, 0.92f, 1.98f, 1.12f);
        }

        private static void China(Graphics g)
        {
            Stripes(g, false, 0xEE1C25);
            Star(g, 0.5f, 0.5f, 0, 0.3f, 0xFFFF00);
            Star(g, 1, 0.2f, 23, 0.1f, 0xFFFF00);
            Star(g, 1.2f, 0.4f, 46, 0.1f, 0xFFFF00);
            Star(g, 1.2f, 0.7f, 70, 0.1f, 0xFFFF00);
            Star(g, 1, 0.9f, 21, 0.1f, 0xFFFF00);
        }

        // crescent from two circles, and a star with a point towards it
        private static void Turkey(Graphics g)
        {
            Stripes(g, false, 0xE30A17);
            Shape(g, 0xFFFFFF, delegate(GraphicsPath p) { p.AddEllipse(0.5f, 0.5f, 1, 1); });
            Shape(g, 0xE30A17, delegate(GraphicsPath p) { p.AddEllipse(0.725f, 0.6f, 0.8f, 0.8f); });
            Star(g, 1.6f, 1, -90, 0.25f, 0xFFFFFF);
        }

        // no national flag for a script used in several places: the character that names it
        // (繁 for Traditional), as Chinese sites label their script switch
        private static void Script(Graphics g, string character)
        {
            Stripes(g, false, 0xC8102E);
            FontFamily family;
            try { family = new FontFamily("Microsoft JhengHei"); }
            catch (ArgumentException) { family = FontFamily.GenericSansSerif; }
            using (family)
            using (var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                Shape(g, 0xFFFFFF, delegate(GraphicsPath p)
                {
                    p.AddString(character, family, (int)FontStyle.Bold, 1.6f, new RectangleF(0, 0.06f, 3, 2), format);
                });
        }

        private static readonly PointF[] StarPoints =
        {
            new PointF(0, -1), new PointF(0.2245f, -0.309f), new PointF(0.9511f, -0.309f), new PointF(0.3633f, 0.118f),
            new PointF(0.5878f, 0.809f), new PointF(0, 0.382f), new PointF(-0.5878f, 0.809f), new PointF(-0.3633f, 0.118f),
            new PointF(-0.9511f, -0.309f), new PointF(-0.2245f, -0.309f),
        };

        private static void Star(Graphics g, float x, float y, float angle, float size, int color)
        {
            var s = g.Save();
            g.TranslateTransform(x, y);
            g.RotateTransform(angle);
            g.ScaleTransform(size, size);
            Shape(g, color, delegate(GraphicsPath p) { p.AddPolygon(StarPoints); });
            g.Restore(s);
        }
    }
}
