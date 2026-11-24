// Installer/uninstaller. Builds together with UnplugLock.cs and embeds UnplugLock.exe.
// Installs per-user to %LOCALAPPDATA%\Programs\UnplugLock, no admin needed.
//
// UnplugLock-Setup.exe [/silent] [/nostartup] [/nolaunch] [/desktop] [/noguard]
// Uninstall.exe [/uninstall] [/silent]

using System;
using System.Diagnostics;
using System.Drawing;
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
            Ui.Init();

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
                        !HasArg(args, "/noguard"), null);
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
        public static bool AuthorizeExisting(string toDoWhat, out string secret)
        {
            secret = null;
            var settings = Settings.Load();
            if (!settings.HasPassword)
                return true;

            while (true)
            {
                string pw;
                var result = PasswordDialog.Ask("UnplugLock is password-protected. Enter the password to " + toDoWhat + ":",
                    settings, out pw);
                if (result == AskResult.Cancelled)
                    return false;
                if (result == AskResult.Recovered || Ui.Busy(delegate { return settings.CheckPassword(pw); }))
                {
                    secret = pw;
                    return true;
                }
                Ui.Pause(1000);
                MessageBox.Show("Wrong password.", "UnplugLock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        public static void Install(bool startWithWindows, bool desktopShortcut, bool launch, bool guard, string secret)
        {
            if (guard)
                startWithWindows = true; // the guard needs it

            if (!StopRunningCopies(secret))
                throw new InvalidOperationException("UnplugLock is still running and wouldn't close. " +
                    "Exit it from its tray menu, then run setup again.");

            Directory.CreateDirectory(InstallDir);
            TryDelete(Path.Combine(InstallDir, Guard.HelperName + ".exe")); // old version, app recreates it

            var settings = Settings.Load();
            settings.Guard = guard;
            settings.Save();

            using (var payload = Assembly.GetExecutingAssembly().GetManifestResourceStream("UnplugLock.exe"))
            {
                if (payload == null)
                    throw new InvalidOperationException("This setup file is damaged (UnplugLock.exe is missing from it).");
                using (var file = File.Create(AppExe))
                    payload.CopyTo(file);
            }

            // copy ourselves over as the uninstaller
            if (!SamePath(Application.ExecutablePath, UninstallExe))
                File.Copy(Application.ExecutablePath, UninstallExe, true);

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
                if (!AuthorizeExisting("uninstall it", out secret))
                    return false;
                var answer = MessageBox.Show("Remove UnplugLock and all of its settings from this PC?",
                    "Uninstall UnplugLock", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (answer != DialogResult.Yes)
                    return false;
            }

            // a copy still running would put its settings straight back
            if (!StopRunningCopies(secret))
            {
                if (!silent)
                    MessageBox.Show("UnplugLock is still running and wouldn't close. Exit it from its tray menu, " +
                        "then uninstall again.", "UnplugLock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                MessageBox.Show("UnplugLock has been uninstalled.", "UnplugLock",
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
                        new object[] { "Locks your laptop when the charger is unplugged" });
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

        public SetupForm() : this(SetupProgram.IsInstalled) { }

        private SetupForm(bool upgrading)
            : base("UnplugLock Setup", upgrading ? "Update UnplugLock" : "Install UnplugLock",
                   "Locks your laptop the moment the charger is unplugged.", 440, true)
        {
            _upgrading = upgrading;
            MinimizeBox = true;

            _guard = Option("Keep UnplugLock running", 0);
            AddText("If someone closes it, it starts itself again straight away.", Ui.Muted, Ui.Small, 14)
                .Padding = new Padding(Ui.S(18), 0, 0, 0);
            _startup = Option("Start UnplugLock when I sign in to Windows", 12);
            _desktop = Option("Create a desktop shortcut", 12);
            _launch = Option("Open UnplugLock when setup finishes", 22);
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

            AddText("Installs to", Ui.Muted, Ui.Small, 2);
            AddText(SetupProgram.InstallDir, Ui.Ink, Ui.Small, 6);
            AddText("Just for your account, so it doesn't need administrator rights.", Ui.Muted, Ui.Small, 0);

            SetFooterNote(Ui.Text("Version " + SetupProgram.Version, ContentWidth, Ui.Muted, Ui.Small, 0));
            _cancel = AddButton("Cancel", false);
            _install = AddButton(upgrading ? "Update" : "Install", true);
            _install.Click += OnInstall;
            // DialogResult doesn't close a non-modal form
            _cancel.Click += delegate { Close(); };

            AcceptButton = _install;
            CancelButton = _cancel;
        }

        private CheckBox Option(string text, float spaceAfter)
        {
            var box = new CheckBox
            {
                Text = text,
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
            if (!SetupProgram.AuthorizeExisting("update it", out secret))
                return;

            _install.Enabled = false;
            _cancel.Enabled = false;
            _install.Text = _upgrading ? "Updating..." : "Installing...";
            Cursor = Cursors.WaitCursor;
            Update();
            try
            {
                SetupProgram.Install(_startup.Checked, _desktop.Checked, _launch.Checked, _guard.Checked, secret);
                Cursor = Cursors.Default;
                ShowDone(_launch.Checked);
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                _install.Text = _upgrading ? "Update" : "Install";
                MessageBox.Show(this, "Setup couldn't finish:\n\n" + ex.Message, "UnplugLock",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                _install.Enabled = true;
                _cancel.Enabled = true;
            }
        }

        // swaps the options for a short "you're done" page
        private void ShowDone(bool launched)
        {
            SuspendLayout();
            Heading.Text = _upgrading ? "UnplugLock is updated" : "UnplugLock is installed";
            Subheading.Text = "Your laptop now locks when the charger is pulled out.";

            Body.Controls.Clear();
            AddText(launched
                    ? "It's running in the system tray, next to the clock. If you can't see the icon, click the ^ arrow there."
                    : "Open it from the Start menu whenever you're ready. It sits in the system tray, next to the clock.",
                Ui.Ink, Ui.Body, 10);
            AddText("Right-click the icon to set a lock delay, add a password, or turn it off for a while.",
                Ui.Muted, Ui.Body, 0);

            _cancel.Visible = false;
            _install.Text = "Finish";
            _install.Enabled = true;
            _install.Click -= OnInstall;
            _install.Click += delegate { Close(); };
            ResumeLayout(true);
            _install.Focus();
        }
    }
}
