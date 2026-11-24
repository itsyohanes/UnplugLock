// UnplugLock tests. Built by test.cmd together with UnplugLock.cs and Setup.cs, using the
// csc.exe that comes with Windows, so there's nothing to install.
//
// Everything runs against a separate registry key, backup folder and startup entry, so the
// tests never touch a real UnplugLock install on the same machine. They never call
// LockWorkStation, and never call WindowsSignIn.HasNoPassword (each call is a failed logon).

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Win32;

namespace UnplugLock.Tests
{
    internal static class TestProgram
    {
        private const string TestKey = @"Software\UnplugLock.Tests";
        private const string RunKey = @"Software\Microsoft\Windows\CurrentVersion\Run";

        private static int _passed, _failed;

        [STAThread]
        private static int Main(string[] args)
        {
            string filter = args.Length > 0 ? args[0] : null;

            // isolate from any real install before anything can be written
            Settings.KeyPath = TestKey;
            Settings.RunValueName = "UnplugLock.Tests";
            Settings.BackupDirOverride = Path.Combine(Path.GetTempPath(), "UnplugLock.Tests." + Process.GetCurrentProcess().Id);
            if (Settings.KeyPath == @"Software\UnplugLock" || Settings.RunValueName == "UnplugLock")
            {
                Console.WriteLine("refusing to run: tests would use the real settings");
                return 2;
            }

            var tests = new List<KeyValuePair<string, Action>>
            {
                // settings: storage, repair and tamper resistance
                Case("Settings: nothing stored loads defaults and writes nothing", NothingStoredLoadsDefaults),
                Case("Settings: save and load round trip", SaveLoadRoundTrip),
                Case("Settings: deleted registry key is restored from the backup", RegistryRestoredFromBackup),
                Case("Settings: deleted backup is restored from the registry", BackupRestoredFromRegistry),
                Case("Settings: when the copies disagree the stricter value wins", StricterValueWins),
                Case("Settings: password removed from the registry comes back from the backup", PasswordRestoredFromBackup),
                Case("Settings: password locks the registry key, removing it unlocks", PasswordLocksKey),
                Case("Settings: IsInSync spots a registry edit and Save repairs it", InSyncSpotsEdits),
                Case("Settings: a bad line in the backup only loses that line", CorruptBackupLine),
                Case("Settings: a bad backup line can't make an edited registry win", CorruptBackupStillStrict),
                Case("Settings: wrong registry value type falls back to the backup", BadRegistryTypeFallsBack),
                Case("Settings: negative delay is read as instant", NegativeDelayClamped),
                Case("Settings: huge delay is cut down to the longest menu choice", HugeDelayClamped),
                Case("Settings: Load(false) reads without repairing", LoadWithoutRepair),
                Case("Settings: DeleteAll removes a locked key and the backup", DeleteAllRemovesEverything),

                // password + recovery code
                Case("Password: right password passes, wrong ones fail", PasswordCheck),
                Case("Password: no password set lets anything through", NoPasswordAllowsAll),
                Case("Password: changing it retires the old one", ChangePasswordRetiresOld),
                Case("Password: non-ASCII passwords work", UnicodePassword),
                Case("Password: removing it also removes the recovery code", RemoveClearsRecovery),
                Case("Recovery code: format and alphabet", RecoveryCodeFormat),
                Case("Recovery code: case, spaces and dashes are ignored", RecoveryCodeNormalised),
                Case("Recovery code: wrong, empty and null codes fail", RecoveryCodeRejects),
                Case("Recovery code: none stored means none accepted", NoRecoveryCodeStored),
                Case("Recovery code: Unlocks takes the password or the code", UnlocksTakesEither),
                Case("Recovery code: copying keeps it out of clipboard history and cloud sync", ClipboardExcludesHistory),
                Case("Recovery code: a file on the system drive counts as on the laptop", SavedFileOnLaptop),
                Case("Password: minimum length is 6", PasswordMinimum),

                // startup entry
                Case("Startup: entry is the quoted exe path and can be removed", StartupEntry),
                Case("Startup: an entry pointing somewhere else counts as off", StartupEntryElsewhere),

                // quitting
                Case("Quit: with no password, setup's request is accepted", QuitNoPassword),
                Case("Quit: with a password, a wrong one is refused", QuitWrongPassword),
                Case("Quit: with a password, the password or recovery code is accepted", QuitRightSecret),
                Case("Quit: nothing listening reports as not running", QuitNothingListening),

                // process guard
                Case("Guard: Protect denies terminate, Unprotect restores it", GuardProtect),
                Case("Guard: Protect also blocks permission changes, injection and memory access", GuardProtectFull),
                Case("Guard: helper restarts after anything but a clean quit", HelperRestartRule),
                Case("Guard: helper lives next to the app", HelperPath),

                // locking
                Case("Lock: a failed lock is retried until it works", LockRetries),
                Case("Lock: retrying gives up after the time limit", LockGivesUp),
                Case("Lock: cancel stops the retries", LockCancel),
                Case("UI: slow work keeps timers running", BusyKeepsTimersRunning),

                // sign-in check
                Case("Sign-in: Microsoft account lookup works without a logon attempt", MicrosoftAccountLookup),

                // network + icon
                Case("Network: adapter table reads without error", NetworkTableReads),
                Case("Icon: renders at every size and writes a valid .ico", IconWrites),
            };

            try
            {
                Cleanup();
                foreach (var t in tests)
                {
                    if (filter != null && t.Key.IndexOf(filter, StringComparison.OrdinalIgnoreCase) < 0)
                        continue;
                    Run(t.Key, t.Value);
                }
            }
            finally
            {
                Cleanup();
            }

            Console.WriteLine();
            Console.WriteLine("{0} passed, {1} failed", _passed, _failed);
            return _failed == 0 ? 0 : 1;
        }

        private static KeyValuePair<string, Action> Case(string name, Action body)
        {
            return new KeyValuePair<string, Action>(name, body);
        }

        private static void Run(string name, Action body)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                Cleanup();
                body();
                _passed++;
                Console.WriteLine("PASS  {0} ({1} ms)", name, sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                _failed++;
                Console.WriteLine("FAIL  {0}", name);
                Console.WriteLine("      " + ex.GetType().Name + ": " + ex.Message);
            }
        }

        private static void Cleanup()
        {
            try { Settings.DeleteAll(); }
            catch { /* nothing there */ }
            using (var run = Registry.CurrentUser.OpenSubKey(RunKey, true))
                if (run != null)
                    run.DeleteValue(Settings.RunValueName, false);
        }

        // ---- tiny assert helpers

        private static void Check(bool condition, string what)
        {
            if (!condition)
                throw new Exception("expected: " + what);
        }

        private static void Equal<T>(T expected, T actual, string what)
        {
            if (!EqualityComparer<T>.Default.Equals(expected, actual))
                throw new Exception(what + ": expected " + expected + ", got " + actual);
        }

        private static void Throws<TEx>(Action body, string what) where TEx : Exception
        {
            try { body(); }
            catch (TEx) { return; }
            throw new Exception("expected " + typeof(TEx).Name + ": " + what);
        }

        // ---- direct access to the stored copies, the way a tamperer would

        private static string BackupFile { get { return Path.Combine(Settings.BackupDir, "settings.dat"); } }

        private static bool KeyExists()
        {
            using (var k = Registry.CurrentUser.OpenSubKey(TestKey))
                return k != null;
        }

        private static void SetRegistry(string name, object value, RegistryValueKind kind)
        {
            Settings.Unlock();
            using (var k = Registry.CurrentUser.OpenSubKey(TestKey, true))
                k.SetValue(name, value, kind);
        }

        private static void DeleteRegistryValue(string name)
        {
            Settings.Unlock();
            using (var k = Registry.CurrentUser.OpenSubKey(TestKey, true))
                k.DeleteValue(name, false);
        }

        private static object GetRegistry(string name)
        {
            using (var k = Registry.CurrentUser.OpenSubKey(TestKey))
                return k == null ? null : k.GetValue(name);
        }

        private static void SetBackup(string name, string value)
        {
            var lines = new List<string>(File.ReadAllLines(BackupFile));
            bool found = false;
            for (int i = 0; i < lines.Count; i++)
                if (lines[i].StartsWith(name + "="))
                {
                    lines[i] = name + "=" + value;
                    found = true;
                }
            if (!found)
                lines.Add(name + "=" + value);
            File.SetAttributes(BackupFile, FileAttributes.Normal);
            File.WriteAllLines(BackupFile, lines);
        }

        private static string GetBackup(string name)
        {
            foreach (var line in File.ReadAllLines(BackupFile))
                if (line.StartsWith(name + "="))
                    return line.Substring(name.Length + 1);
            return null;
        }

        // ---- settings

        private static void NothingStoredLoadsDefaults()
        {
            var s = Settings.Load();
            Check(s.Enabled, "enabled by default");
            Equal(0, s.DelaySeconds, "delay");
            Check(s.Guard, "guard on by default");
            Check(!s.HideIcon, "icon shown by default");
            Check(!s.LockOnEthernet && !s.LockOnWifi, "network locks off by default");
            Check(!s.HasPassword, "no password");
            Check(!KeyExists(), "no registry key created");
            Check(!File.Exists(BackupFile), "no backup created");
        }

        private static void SaveLoadRoundTrip()
        {
            var s = Settings.Load();
            s.Enabled = false;
            s.DelaySeconds = 5;
            s.HideIcon = true;
            s.Guard = false;
            s.LockOnEthernet = true;
            s.LockOnWifi = true;
            s.Save();

            var r = Settings.Load();
            Check(!r.Enabled && r.DelaySeconds == 5 && r.HideIcon && !r.Guard && r.LockOnEthernet && r.LockOnWifi,
                "all values survive a save and load");
            Check(r.IsInSync(), "both copies in sync");
            Check((File.GetAttributes(BackupFile) & FileAttributes.Hidden) != 0, "backup is hidden");
        }

        private static void RegistryRestoredFromBackup()
        {
            var s = Settings.Load();
            s.DelaySeconds = 3;
            s.LockOnWifi = true;
            s.SetPassword("hunter22", Settings.NewRecoveryCode());
            s.Save();

            Settings.Unlock();
            Registry.CurrentUser.DeleteSubKeyTree(TestKey);
            Check(!KeyExists(), "key deleted");

            var r = Settings.Load();
            Equal(3, r.DelaySeconds, "delay");
            Check(r.LockOnWifi, "wifi lock");
            Check(r.CheckPassword("hunter22"), "password survives");
            Check(KeyExists(), "registry key rewritten");
            Check(r.IsInSync(), "copies in sync and key locked again");
        }

        private static void BackupRestoredFromRegistry()
        {
            var s = Settings.Load();
            s.DelaySeconds = 10;
            s.Save();
            File.SetAttributes(BackupFile, FileAttributes.Normal);
            File.Delete(BackupFile);

            var r = Settings.Load();
            Equal(10, r.DelaySeconds, "delay");
            Check(File.Exists(BackupFile), "backup rewritten");
            Equal("10", GetBackup("DelaySeconds"), "backup delay");
        }

        private static void StricterValueWins()
        {
            var s = Settings.Load();
            s.Save();

            SetRegistry("Enabled", 0, RegistryValueKind.DWord);
            SetRegistry("Guard", 0, RegistryValueKind.DWord);
            SetRegistry("DelaySeconds", 10, RegistryValueKind.DWord);
            SetRegistry("LockOnEthernet", 1, RegistryValueKind.DWord);
            SetBackup("DelaySeconds", "3");
            SetBackup("LockOnWifi", "1");

            var r = Settings.Load();
            Check(r.Enabled, "enabled wins over disabled");
            Check(r.Guard, "guard on wins over off");
            Equal(3, r.DelaySeconds, "shorter delay wins");
            Check(r.LockOnEthernet && r.LockOnWifi, "a network lock on in either copy stays on");
            Equal(1, (int)GetRegistry("Enabled"), "registry repaired");
            Equal("3", GetBackup("DelaySeconds"), "backup kept the stricter delay");
        }

        private static void PasswordRestoredFromBackup()
        {
            var s = Settings.Load();
            s.SetPassword("correct horse", Settings.NewRecoveryCode());
            s.Save();

            DeleteRegistryValue("PasswordHash");
            DeleteRegistryValue("PasswordSalt");

            var r = Settings.Load();
            Check(r.HasPassword, "password back");
            Check(r.CheckPassword("correct horse"), "same password");
            Check(r.HasRecoveryCode, "recovery code back");
            Check(GetRegistry("PasswordHash") != null, "registry repaired");
        }

        private static void PasswordLocksKey()
        {
            var s = Settings.Load();
            s.SetPassword("hunter22");
            s.Save();

            Throws<Exception>(delegate
            {
                using (var k = Registry.CurrentUser.OpenSubKey(TestKey, true))
                    k.SetValue("Enabled", 0, RegistryValueKind.DWord);
            }, "writing to the locked key fails");

            s.SetPassword(null);
            s.Save();
            using (var k = Registry.CurrentUser.OpenSubKey(TestKey, true))
                k.SetValue("Enabled", 1, RegistryValueKind.DWord); // writable again
        }

        private static void InSyncSpotsEdits()
        {
            var s = Settings.Load();
            s.SetPassword("hunter22");
            s.Save();
            Check(s.IsInSync(), "in sync after save");

            SetRegistry("Enabled", 0, RegistryValueKind.DWord); // also unlocks the key
            Check(!s.IsInSync(), "edit noticed");

            s.Save();
            Check(s.IsInSync(), "save repairs it");
            Equal(1, (int)GetRegistry("Enabled"), "value put back");
        }

        private static void CorruptBackupLine()
        {
            var s = Settings.Load();
            s.DelaySeconds = 5;
            s.LockOnWifi = true;
            s.Save();
            SetBackup("DelaySeconds", "not a number");

            var r = Settings.Load();
            Check(r.LockOnWifi, "the rest of the backup still counts");
            Equal(0, r.DelaySeconds, "the bad line reads as the default, and the shorter delay wins");
            Equal("0", GetBackup("DelaySeconds"), "backup rewritten");
        }

        // regression: one bad line used to make the whole backup count as missing, so an edited
        // registry copy won outright
        private static void CorruptBackupStillStrict()
        {
            var s = Settings.Load();
            s.SetPassword("hunter22");
            s.Save();
            SetRegistry("Enabled", 0, RegistryValueKind.DWord);
            SetRegistry("Guard", 0, RegistryValueKind.DWord);
            File.SetAttributes(BackupFile, FileAttributes.Normal);
            File.AppendAllText(BackupFile, "DelaySeconds=x\r\ngarbage\r\n");

            var r = Settings.Load();
            Check(r.Enabled, "still enabled");
            Check(r.Guard, "guard still on");
            Check(r.HasPassword, "password kept");
        }

        // regression: no upper limit, so a big value silently turned locking off or overflowed the timer
        private static void HugeDelayClamped()
        {
            var s = Settings.Load();
            s.Save();
            SetRegistry("DelaySeconds", 3000000, RegistryValueKind.DWord);
            SetBackup("DelaySeconds", "3000000");
            Equal(Settings.MaxDelaySeconds, Settings.Load().DelaySeconds, "delay");
            Equal(10, Settings.MaxDelaySeconds, "longest menu choice");
        }

        private static void LoadWithoutRepair()
        {
            var s = Settings.Load();
            s.DelaySeconds = 3;
            s.Save();
            File.SetAttributes(BackupFile, FileAttributes.Normal);
            File.Delete(BackupFile);

            Equal(3, Settings.Load(false).DelaySeconds, "read from the registry");
            Check(!File.Exists(BackupFile), "backup not rewritten");
        }

        private static void BadRegistryTypeFallsBack()
        {
            var s = Settings.Load();
            s.Enabled = true;
            s.DelaySeconds = 1;
            s.Save();
            SetRegistry("Enabled", "off", RegistryValueKind.String);

            var r = Settings.Load();
            Check(r.Enabled, "backup used");
            Equal(1, r.DelaySeconds, "delay from backup");
            Equal(RegistryValueKind.DWord, Registry.CurrentUser.OpenSubKey(TestKey).GetValueKind("Enabled"), "registry repaired");
        }

        private static void NegativeDelayClamped()
        {
            var s = Settings.Load();
            s.Save();
            SetRegistry("DelaySeconds", -5, RegistryValueKind.DWord);
            SetBackup("DelaySeconds", "-5");
            Equal(0, Settings.Load().DelaySeconds, "delay");
        }

        private static void DeleteAllRemovesEverything()
        {
            var s = Settings.Load();
            s.SetPassword("hunter22");
            s.Save();
            Settings.DeleteAll();
            Check(!KeyExists(), "key gone");
            Check(!Directory.Exists(Settings.BackupDir), "backup folder gone");
        }

        // ---- password + recovery code

        private static void PasswordCheck()
        {
            var s = new Settings();
            s.SetPassword("hunter22");
            Check(s.HasPassword, "has password");
            Check(s.CheckPassword("hunter22"), "right password");
            Check(!s.CheckPassword("hunter2"), "prefix rejected");
            Check(!s.CheckPassword("HUNTER22"), "case matters");
            Check(!s.CheckPassword(""), "empty rejected");
            Check(!s.CheckPassword(null), "null rejected");
        }

        private static void NoPasswordAllowsAll()
        {
            var s = new Settings();
            Check(!s.HasPassword, "no password");
            Check(s.CheckPassword("anything") && s.CheckPassword(null), "anything accepted");
        }

        private static void ChangePasswordRetiresOld()
        {
            var s = new Settings();
            string oldCode = Settings.NewRecoveryCode();
            s.SetPassword("first one", oldCode);
            string newCode = Settings.NewRecoveryCode();
            s.SetPassword("second one", newCode);
            Check(!s.CheckPassword("first one"), "old password rejected");
            Check(s.CheckPassword("second one"), "new password accepted");
            Check(!s.CheckRecoveryCode(oldCode), "old recovery code rejected");
            Check(s.CheckRecoveryCode(newCode), "new recovery code accepted");
        }

        private static void UnicodePassword()
        {
            var s = new Settings();
            s.SetPassword("pässwörd ключ 🔒");
            Check(s.CheckPassword("pässwörd ключ 🔒"), "exact match");
            Check(!s.CheckPassword("passwort ключ 🔒"), "different text rejected");
        }

        private static void RemoveClearsRecovery()
        {
            var s = new Settings();
            string code = Settings.NewRecoveryCode();
            s.SetPassword("hunter22", code);
            s.SetPassword(null);
            Check(!s.HasPassword && !s.HasRecoveryCode, "both gone");
            Check(!s.CheckRecoveryCode(code), "code no longer works");
        }

        private static void RecoveryCodeFormat()
        {
            var format = new Regex("^[A-HJ-NP-Z2-9]{5}(-[A-HJ-NP-Z2-9]{5}){4}$");
            var seen = new HashSet<string>();
            for (int i = 0; i < 500; i++)
            {
                string c = Settings.NewRecoveryCode();
                Check(format.IsMatch(c), "XXXXX-XXXXX-XXXXX-XXXXX-XXXXX without 0, O, 1 or I, got " + c);
                Check(seen.Add(c), "codes don't repeat");
            }
        }

        private static void RecoveryCodeNormalised()
        {
            var s = new Settings();
            string code = Settings.NewRecoveryCode();
            s.SetPassword("hunter22", code);
            Check(s.CheckRecoveryCode(code), "as shown");
            Check(s.CheckRecoveryCode(code.ToLowerInvariant()), "lower case");
            Check(s.CheckRecoveryCode(code.Replace("-", "")), "no dashes");
            Check(s.CheckRecoveryCode("  " + code.Replace("-", " ") + "  "), "spaces instead of dashes");
        }

        private static void RecoveryCodeRejects()
        {
            var s = new Settings();
            string code = Settings.NewRecoveryCode();
            s.SetPassword("hunter22", code);
            char last = code[code.Length - 1];
            string wrong = code.Substring(0, code.Length - 1) + (last == 'A' ? 'B' : 'A');
            Check(!s.CheckRecoveryCode(wrong), "one character off");
            Check(!s.CheckRecoveryCode(code.Substring(0, 23)), "truncated");
            Check(!s.CheckRecoveryCode(""), "empty");
            Check(!s.CheckRecoveryCode(null), "null");
            Check(!s.CheckRecoveryCode("hunter22"), "the password isn't a recovery code");
        }

        private static void NoRecoveryCodeStored()
        {
            var s = new Settings();
            s.SetPassword("hunter22");
            Check(!s.HasRecoveryCode, "none stored");
            Check(!s.CheckRecoveryCode(Settings.NewRecoveryCode()), "random code rejected");
            Check(!s.CheckRecoveryCode(""), "empty rejected");
        }

        private static void UnlocksTakesEither()
        {
            var s = new Settings();
            Check(s.Unlocks(null) && s.Unlocks("anything"), "no password: anything");
            string code = Settings.NewRecoveryCode();
            s.SetPassword("hunter22", code);
            Check(s.Unlocks("hunter22"), "password");
            Check(s.Unlocks(code.ToLowerInvariant()), "recovery code");
            Check(!s.Unlocks("hunter2"), "wrong password");
            Check(!s.Unlocks(""), "empty");
            Check(!s.Unlocks(null), "null");
        }

        // regression: Copy used plain Clipboard.SetText, which lands in Win+V history
        private static void ClipboardExcludesHistory()
        {
            var data = RecoveryCodeDialog.ClipboardData("ABCDE-FGHJK");
            Equal("ABCDE-FGHJK", data.GetData(DataFormats.UnicodeText) as string, "text");
            foreach (var format in new[] { "ExcludeClipboardContentFromMonitorProcessing",
                                           "CanIncludeInClipboardHistory", "CanUploadToCloudClipboard" })
                Check(data.GetDataPresent(format), format + " set");
            var history = (MemoryStream)data.GetData("CanIncludeInClipboardHistory");
            Equal(0, BitConverter.ToInt32(history.ToArray(), 0), "history not allowed");
        }

        private static void SavedFileOnLaptop()
        {
            string systemDrive = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System));
            Check(RecoveryCodeDialog.IsOnThisLaptop(Path.Combine(systemDrive, "code.txt")), "system drive");
            Check(!RecoveryCodeDialog.IsOnThisLaptop(@"\\server\share\code.txt"), "network share");
        }

        private static void PasswordMinimum()
        {
            Equal(6, PasswordDialog.MinLength, "minimum length");
        }

        // ---- startup entry

        // regression: any non-empty value counted as "starts with Windows", so it was never repaired
        private static void StartupEntryElsewhere()
        {
            using (var run = Registry.CurrentUser.CreateSubKey(RunKey))
                run.SetValue(Settings.RunValueName, "x");
            Check(!Settings.StartsWithWindows, "\"x\" isn't us");
            using (var run = Registry.CurrentUser.CreateSubKey(RunKey))
                run.SetValue(Settings.RunValueName, @"C:\Windows\notepad.exe");
            Check(!Settings.StartsWithWindows, "another program isn't us");
            using (var run = Registry.CurrentUser.CreateSubKey(RunKey))
                run.SetValue(Settings.RunValueName, Application.ExecutablePath.ToUpperInvariant());
            Check(Settings.StartsWithWindows, "our path, unquoted and in other case");
        }

        // ---- quitting (regression: a named event let any program close the app without the password)

        private static string QuitPipe { get { return "UnplugLock.Tests.Quit." + Process.GetCurrentProcess().Id; } }

        private static int RunQuit(Settings s, string secret, out bool? answer)
        {
            int quits = 0;
            var listener = new QuitListener(QuitPipe, s.Unlocks, delegate { System.Threading.Interlocked.Increment(ref quits); });
            listener.Start();
            try
            {
                answer = QuitListener.RequestQuit(QuitPipe, secret, 3000);
                System.Threading.Thread.Sleep(100);
                return quits;
            }
            finally
            {
                listener.Stop();
                QuitListener.RequestQuit(QuitPipe, null, 500); // wake it so the thread ends
            }
        }

        private static void QuitNoPassword()
        {
            bool? answer;
            Equal(1, RunQuit(new Settings(), null, out answer), "quit callbacks");
            Equal((bool?)true, answer, "accepted");
        }

        private static void QuitWrongPassword()
        {
            var s = new Settings();
            s.SetPassword("hunter22", Settings.NewRecoveryCode());
            bool? answer;
            Equal(0, RunQuit(s, null, out answer), "no password sent");
            Equal((bool?)false, answer, "refused");
            Equal(0, RunQuit(s, "hunter2", out answer), "wrong password");
            Equal((bool?)false, answer, "refused");
        }

        private static void QuitRightSecret()
        {
            var s = new Settings();
            string code = Settings.NewRecoveryCode();
            s.SetPassword("hunter22", code);
            bool? answer;
            Equal(1, RunQuit(s, "hunter22", out answer), "password");
            Equal((bool?)true, answer, "accepted");
            Equal(1, RunQuit(s, code, out answer), "recovery code");
            Equal((bool?)true, answer, "accepted");
        }

        private static void QuitNothingListening()
        {
            Equal((bool?)null, QuitListener.RequestQuit(QuitPipe + ".nobody", "x", 300), "not running");
        }

        private static void StartupEntry()
        {
            Check(!Settings.StartsWithWindows, "off to begin with");
            Settings.StartsWithWindows = true;
            Check(Settings.StartsWithWindows, "on");
            using (var run = Registry.CurrentUser.OpenSubKey(RunKey))
                Equal("\"" + Application.ExecutablePath + "\"", run.GetValue(Settings.RunValueName) as string, "run entry");
            Settings.StartsWithWindows = false;
            Check(!Settings.StartsWithWindows, "off again");
        }

        // ---- process guard

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(int access, bool inherit, int pid);

        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(IntPtr handle);

        private static bool CanOpen(int access)
        {
            IntPtr h = OpenProcess(access, false, Process.GetCurrentProcess().Id);
            if (h == IntPtr.Zero)
                return false;
            CloseHandle(h);
            return true;
        }

        private static void GuardProtect()
        {
            const int Terminate = 0x0001, SuspendResume = 0x0800, Synchronize = 0x00100000;
            Check(CanOpen(Terminate), "terminate allowed before");
            Guard.Protect();
            try
            {
                Check(!CanOpen(Terminate), "terminate denied");
                Check(!CanOpen(SuspendResume), "suspend denied");
                Check(CanOpen(Synchronize), "waiting on it still works (the helper needs this)");
            }
            finally
            {
                Guard.Unprotect();
            }
            Check(CanOpen(Terminate), "terminate allowed again");
        }

        // regression: the process owner kept WRITE_DAC, so any program running as the user could
        // rewrite the permissions and then end it, or inject a thread
        private static void GuardProtectFull()
        {
            const int WriteDac = 0x00040000, WriteOwner = 0x00080000, CreateThread = 0x0002, VmWrite = 0x0020,
                VmOperation = 0x0008, VmRead = 0x0010, DupHandle = 0x0040, QueryLimited = 0x1000;
            Guard.Protect();
            try
            {
                Check(!CanOpen(WriteDac), "WRITE_DAC denied");
                Check(!CanOpen(WriteOwner), "WRITE_OWNER denied");
                Check(!CanOpen(CreateThread), "CREATE_THREAD denied");
                Check(!CanOpen(VmWrite | VmOperation), "VM_WRITE denied");
                Check(!CanOpen(VmRead), "VM_READ denied");
                Check(!CanOpen(DupHandle), "DUP_HANDLE denied");
                Check(CanOpen(QueryLimited), "basic queries still work (setup lists processes)");
            }
            finally
            {
                Guard.Unprotect();
            }
            Check(CanOpen(WriteDac), "WRITE_DAC allowed again");
        }

        private static void HelperRestartRule()
        {
            Check(!Guard.ShouldRestart(Guard.CleanExitCode, true), "clean quit: stay closed");
            Check(Guard.ShouldRestart(Guard.UnexpectedExitCode, true), "WM_QUIT or similar: restart");
            Check(Guard.ShouldRestart(1, true), "ended from Task Manager: restart");
            Check(Guard.ShouldRestart(unchecked((int)0xE0434352), true), "crashed: restart");
            Check(Guard.ShouldRestart(null, true), "unknown: restart");
            Check(!Guard.ShouldRestart(Guard.UnexpectedExitCode, false), "guard off: stay closed");
        }

        // ---- locking (never calls the real LockWorkStation)

        private static void PumpUntil(Func<bool> done, int timeoutMs)
        {
            var sw = Stopwatch.StartNew();
            while (!done() && sw.ElapsedMilliseconds < timeoutMs)
            {
                Application.DoEvents();
                System.Threading.Thread.Sleep(5);
            }
        }

        private static void LockRetries()
        {
            int calls = 0;
            using (var locker = new Locker(delegate { return ++calls >= 3; }, 20, TimeSpan.FromSeconds(10)))
            {
                Check(!locker.Lock(), "first try fails");
                Check(locker.Retrying, "retrying");
                PumpUntil(delegate { return !locker.Retrying; }, 3000);
                Equal(3, calls, "tried until it worked");
                Check(!locker.Retrying, "stopped once locked");
            }
        }

        private static void LockGivesUp()
        {
            int calls = 0;
            using (var locker = new Locker(delegate { calls++; return false; }, 20, TimeSpan.FromMilliseconds(200)))
            {
                locker.Lock();
                PumpUntil(delegate { return !locker.Retrying; }, 3000);
                Check(!locker.Retrying, "gave up");
                Check(calls > 2, "retried a few times first");
            }
        }

        private static void LockCancel()
        {
            int calls = 0;
            using (var locker = new Locker(delegate { calls++; return false; }, 20, TimeSpan.FromSeconds(10)))
            {
                locker.Lock();
                locker.Cancel();
                int after = calls;
                PumpUntil(delegate { return false; }, 150);
                Equal(after, calls, "no more tries");
            }
        }

        // regression: password hashing ran on the UI thread and stalled the power checks
        private static void BusyKeepsTimersRunning()
        {
            int ticks = 0;
            using (var timer = new System.Windows.Forms.Timer { Interval = 20 })
            {
                timer.Tick += delegate { ticks++; };
                timer.Start();
                int result = Ui.Busy(delegate { System.Threading.Thread.Sleep(400); return 42; });
                timer.Stop();
                Equal(42, result, "result");
                Check(ticks >= 5, "timer kept ticking (got " + ticks + ")");
            }
        }

        // ---- sign-in check

        private static void MicrosoftAccountLookup()
        {
            // only checks it runs; the answer depends on the machine. HasNoPassword isn't called
            // because on an account with a password it's a failed logon.
            bool ms = WindowsSignIn.IsMicrosoftAccount();
            Console.WriteLine("      (this account is " + (ms ? "" : "not ") + "a Microsoft account)");
        }

        private static void HelperPath()
        {
            Equal(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "UnplugLockHelper.exe"),
                Guard.HelperPath, "helper path");
        }

        // ---- network + icon

        private static void NetworkTableReads()
        {
            var links = NetworkLinks.Connected();
            Check(links != null, "adapter table readable");
            foreach (var kind in links.Values)
                Check(kind == NetKind.Ethernet || kind == NetKind.Wifi, "only Ethernet or Wi-Fi");
        }

        private static void IconWrites()
        {
            foreach (int size in new[] { 16, 24, 32, 48, 256 })
                using (var bmp = Logo.Render(size, size % 2 == 0))
                    Check(bmp.Width == size && bmp.Height == size, "render " + size);

            string ico = Path.Combine(Settings.BackupDir, "test.ico");
            Directory.CreateDirectory(Settings.BackupDir);
            Logo.WriteIco(ico);
            byte[] bytes = File.ReadAllBytes(ico);
            Equal(0, BitConverter.ToInt16(bytes, 0), "reserved");
            Equal(1, BitConverter.ToInt16(bytes, 2), "type icon");
            Equal(7, BitConverter.ToInt16(bytes, 4), "frame count");
            using (var icon = new Icon(ico, 32, 32))
                Equal(32, icon.Width, "Windows can read the 32px frame");
        }
    }
}
