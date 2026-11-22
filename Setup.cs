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
        public const string Version = "1.1.0";

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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

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
                        !HasArg(args, "/noguard"));
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

        public static bool AuthorizeExisting(string toDoWhat)
        {
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
                if (result == AskResult.Recovered || settings.CheckPassword(pw))
                    return true;
                System.Threading.Thread.Sleep(1000);
                MessageBox.Show("Wrong password.", "UnplugLock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        public static void Install(bool startWithWindows, bool desktopShortcut, bool launch, bool guard)
        {
            if (guard)
                startWithWindows = true; // the guard needs it

            StopRunningCopies();

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
            if (silent)
            {
                if (Settings.Load().HasPassword)
                    return false; // can't prompt when silent
            }
            else
            {
                if (!AuthorizeExisting("uninstall it"))
                    return false;
                var answer = MessageBox.Show("Remove UnplugLock and all of its settings from this PC?",
                    "Uninstall UnplugLock", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (answer != DialogResult.Yes)
                    return false;
            }

            StopRunningCopies();

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


        private static void StopRunningCopies()
        {
            // ask nicely first - a guarded copy can't be killed, and its helper would restart it anyway
            try
            {
                using (var quit = System.Threading.EventWaitHandle.OpenExisting(Guard.QuitSignalName))
                    quit.Set();
            }
            catch { /* not running */ }

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
        }

        private static bool AnyRunning()
        {
            foreach (string name in new[] { "UnplugLock", Guard.HelperName })
            {
                var procs = Process.GetProcessesByName(name);
                foreach (var p in procs)
                    p.Dispose();
                if (procs.Length > 0)
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

    internal sealed class SetupForm : Form
    {
        private readonly CheckBox _startup;
        private readonly CheckBox _desktop;
        private readonly CheckBox _launch;
        private readonly CheckBox _guard;
        private readonly Button _install;
        private readonly Button _cancel;

        public SetupForm()
        {
            bool upgrading = SetupProgram.IsInstalled;

            Text = "UnplugLock Setup";
            Icon = TrayApp.AppIcon;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            Font = SystemFonts.MessageBoxFont;
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            BackColor = SystemColors.Window;

            var root = new TableLayoutPanel
            {
                ColumnCount = 1,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(20, 18, 20, 16)
            };

            // header
            var header = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 2,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 14)
            };
            var logo = new PictureBox
            {
                Image = Logo.Render(56, true),
                Size = new Size(56, 56),
                SizeMode = PictureBoxSizeMode.CenterImage,
                Margin = new Padding(0, 0, 14, 0)
            };
            header.Controls.Add(logo, 0, 0);
            header.SetRowSpan(logo, 2);
            header.Controls.Add(new Label
            {
                Text = upgrading ? "Update UnplugLock" : "Install UnplugLock",
                Font = new Font(Font.FontFamily, 15f, FontStyle.Bold),
                AutoSize = true,
                Margin = new Padding(0, 4, 0, 2)
            }, 1, 0);
            header.Controls.Add(new Label
            {
                Text = "Locks your laptop the moment the charger is unplugged.",
                ForeColor = SystemColors.GrayText,
                AutoSize = true,
                Margin = new Padding(0)
            }, 1, 1);
            root.Controls.Add(header);

            root.Controls.Add(new Label
            {
                Text = "Installs to: " + SetupProgram.InstallDir + "\r\nVersion " + SetupProgram.Version +
                       " - no administrator rights needed",
                ForeColor = SystemColors.GrayText,
                AutoSize = true,
                MaximumSize = new Size(420, 0),
                Margin = new Padding(0, 0, 0, 12)
            });

            _startup = new CheckBox { Text = "Start UnplugLock when I sign in to Windows", Checked = true, AutoSize = true };
            _desktop = new CheckBox { Text = "Create a desktop shortcut", Checked = false, AutoSize = true };
            _launch = new CheckBox { Text = "Run UnplugLock when setup finishes", Checked = true, AutoSize = true };
            _guard = new CheckBox
            {
                Text = "Keep UnplugLock running (restarts itself if it's closed)",
                Checked = true,
                AutoSize = true
            };
            // the guard always starts with Windows
            _guard.CheckedChanged += delegate
            {
                if (_guard.Checked)
                    _startup.Checked = true;
                _startup.Enabled = !_guard.Checked;
            };
            _startup.Enabled = false;
            root.Controls.Add(_guard);
            root.Controls.Add(_startup);
            root.Controls.Add(_desktop);
            root.Controls.Add(_launch);

            _install = new Button { Text = upgrading ? "Update" : "Install", AutoSize = true, MinimumSize = new Size(88, 0) };
            _cancel = new Button { Text = "Cancel", AutoSize = true, MinimumSize = new Size(88, 0), DialogResult = DialogResult.Cancel };
            _install.Click += OnInstall;
            // DialogResult doesn't close a non-modal form
            _cancel.Click += delegate { Close(); };

            var buttons = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true,
                Anchor = AnchorStyles.Right,
                Margin = new Padding(0, 18, 0, 0)
            };
            buttons.Controls.Add(_cancel);
            buttons.Controls.Add(_install);
            root.Controls.Add(buttons);

            Controls.Add(root);
            AcceptButton = _install;
            CancelButton = _cancel;
        }

        private void OnInstall(object sender, EventArgs e)
        {
            if (SetupProgram.IsInstalled || Process.GetProcessesByName("UnplugLock").Length > 0)
            {
                if (!SetupProgram.AuthorizeExisting("update it"))
                    return;
            }

            _install.Enabled = false;
            _cancel.Enabled = false;
            Cursor = Cursors.WaitCursor;
            try
            {
                SetupProgram.Install(_startup.Checked, _desktop.Checked, _launch.Checked, _guard.Checked);
                Cursor = Cursors.Default;
                MessageBox.Show(this,
                    "UnplugLock is installed.\n\nLook for the padlock icon in the system tray " +
                    "(you may need to click the ^ arrow next to the clock).",
                    "UnplugLock", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                MessageBox.Show(this, "Setup couldn't finish:\n\n" + ex.Message, "UnplugLock",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                _install.Enabled = true;
                _cancel.Enabled = true;
            }
        }
    }
}
