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

                // languages
                Case("Language: every text the code asks for exists in English", CodeKeysExist),
                Case("Language: every translation has the English keys and placeholders", TranslationsComplete),
                Case("Language: the Windows language picks the translation", WindowsLanguagePicked),
                Case("Language: a saved choice wins, an unknown one follows Windows", SavedLanguageWins),
                Case("Language: seconds use each language's plural forms", PluralForms),
                Case("Language: tray tooltips fit in 63 characters", TooltipsFit),
                Case("Language: the setup picker lists every language by its own name", PickerChoices),
                Case("Language: the setup picker has a flag for every language", PickerFlags),
                Case("Language: the recovery file is written in the chosen language", RecoveryFileTranslated),
                Case("Language: every window builds in every language", WindowsBuildInEveryLanguage),

                // network + icon
                Case("Network: adapter table reads without error", NetworkTableReads),
                Case("Icon: renders at every size and writes a valid .ico", IconWrites),

                // installer packaging
                Case("Setup: the app packed onto setup comes back out byte for byte", PackRoundTrip),
                Case("Setup: Uninstall.exe is written without the packed app", UninstallerLeavesAppOut),
                Case("Setup: a file with no packed app reads as having none", NoPackedApp),
                Case("Setup: the installer borrows every language from its packed app", SetupBorrowsPackedLanguages),
                Case("Setup: the uninstaller borrows every language from the installed app", UninstallerBorrowsInstalledLanguages),
                Case("Setup: with no app to borrow from, setup is in English", SetupEnglishWithoutApp),
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
            s.Language = "de";
            s.Save();

            var r = Settings.Load();
            Check(!r.Enabled && r.DelaySeconds == 5 && r.HideIcon && !r.Guard && r.LockOnEthernet && r.LockOnWifi,
                "all values survive a save and load");
            Equal("de", r.Language, "language survives a save and load");
            Equal("de", GetBackup("Language"), "language in the backup");
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

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetCurrentProcess();

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool OpenProcessToken(IntPtr process, int access, out IntPtr token);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool LookupPrivilegeValue(string system, string name, out long luid);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool AdjustTokenPrivileges(IntPtr token, bool disableAll, ref TokenPrivilege state,
            int length, IntPtr previous, IntPtr returnLength);

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        private struct TokenPrivilege
        {
            public int Count;
            public long Luid;
            public int Attributes;
        }

        private static bool _debugDropped;

        // An elevated admin with SeDebugPrivilege skips process permission checks entirely, which is
        // the documented limit of the guard (GitHub's Windows runners run like that). Removing the
        // privilege from this test process puts it back under the deny rules, so the guard tests
        // still test something real there.
        private static void DropDebugPrivilege()
        {
            if (_debugDropped)
                return;
            _debugDropped = true;
            bool admin;
            using (var me = System.Security.Principal.WindowsIdentity.GetCurrent())
                admin = new System.Security.Principal.WindowsPrincipal(me)
                    .IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);

            IntPtr token;
            long luid;
            if (!OpenProcessToken(GetCurrentProcess(), 0x0020 | 0x0008, out token)) // ADJUST_PRIVILEGES | QUERY
                return;
            try
            {
                if (!LookupPrivilegeValue(null, "SeDebugPrivilege", out luid))
                    return;
                var state = new TokenPrivilege { Count = 1, Luid = luid, Attributes = 0x00000004 }; // SE_PRIVILEGE_REMOVED
                bool ok = AdjustTokenPrivileges(token, false, ref state, Marshal.SizeOf(typeof(TokenPrivilege)),
                    IntPtr.Zero, IntPtr.Zero);
                int error = Marshal.GetLastWin32Error();
                Console.WriteLine("      (elevated admin: " + admin + ", SeDebugPrivilege " +
                    (ok && error == 0 ? "removed for these tests" : "not held") + ")");
            }
            finally
            {
                CloseHandle(token);
            }
        }

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
            DropDebugPrivilege();
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
            DropDebugPrivilege();
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

        // ---- languages

        private static readonly Regex Placeholder = new Regex(@"\{\d+\}");

        private static string SourceDir
        {
            get { return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "..")); }
        }

        // runs body in each language, then goes back to the Windows one
        private static void InEachLanguage(Action<string> body)
        {
            try
            {
                foreach (var name in AllLanguages())
                {
                    Lang.Use(name);
                    body(name);
                }
            }
            finally
            {
                Lang.Use((string)null);
            }
        }

        private static List<string> AllLanguages()
        {
            var names = new List<string> { "en" };
            names.AddRange(Lang.Available);
            return names;
        }

        private static void CodeKeysExist()
        {
            // Lang.T("Key"), Authorize("Key") and keys picked with ?: are all quoted names like "Menu.Exit"
            var key = new Regex(@"(?:Lang\.T|Authorize|AuthorizeExisting)\(\s*""(\w+(?:\.\w+)?)""|""([A-Z]\w*\.[A-Z]\w*)""");
            var groups = new HashSet<string>(); // "Menu", "Pw", ... - so "WScript.Shell" isn't taken for a key
            foreach (var k in Lang.EnglishTable.Keys)
                if (k.Contains("."))
                    groups.Add(k.Substring(0, k.IndexOf('.')));
            int found = 0;
            foreach (var file in new[] { "UnplugLock.cs", "Setup.cs" })
                foreach (Match m in key.Matches(File.ReadAllText(Path.Combine(SourceDir, file))))
                {
                    string name = m.Groups[1].Success ? m.Groups[1].Value : m.Groups[2].Value;
                    if (!m.Groups[1].Success && !groups.Contains(name.Substring(0, name.IndexOf('.'))))
                        continue;
                    Check(Lang.EnglishTable.ContainsKey(name), file + " uses \"" + name + "\", which isn't in lang\\en.cs");
                    found++;
                }
            Check(found > 100, "found the texts in the code (" + found + ")");
            // built from parts in RefreshMenu
            foreach (var part in new[] { "Online", "Offline", "Unknown" })
                Check(Lang.EnglishTable.ContainsKey("Power." + part) && Lang.EnglishTable.ContainsKey("Tip." + part), part);
        }

        private static void TranslationsComplete()
        {
            var english = Lang.EnglishTable;
            foreach (var name in Lang.Available)
            {
                var table = Lang.TableFor(name);
                foreach (var k in english.Keys)
                    Check(table.ContainsKey(k), name + " has " + k);
                foreach (var k in table.Keys)
                {
                    bool few = k == "Delay.Few" && Lang.PluralOf(name, 3) == "Few";
                    Check(english.ContainsKey(k) || few, name + ": " + k + " isn't an English key");
                    Check(table[k].Trim().Length > 0, name + ": " + k + " is empty");
                    string source = english.ContainsKey(k) ? english[k] : english["Delay.Other"];
                    Equal(Placeholders(source), Placeholders(table[k]), name + ": placeholders in " + k);
                }
                if (Lang.PluralOf(name, 3) == "Few")
                    Check(table.ContainsKey("Delay.Few"), name + " has Delay.Few");
            }
        }

        private static string Placeholders(string text)
        {
            var list = new List<string>();
            foreach (Match m in Placeholder.Matches(text))
                list.Add(m.Value);
            list.Sort(StringComparer.Ordinal);
            return string.Join(" ", list);
        }

        private static void WindowsLanguagePicked()
        {
            var cases = new[]
            {
                new[] { "de-AT", "de" }, new[] { "fr-CA", "fr" }, new[] { "pt-PT", "pt" }, new[] { "pt-BR", "pt" },
                new[] { "zh-CN", "zh-Hans" }, new[] { "zh-SG", "zh-Hans" }, new[] { "zh-TW", "en" },
                new[] { "id-ID", "id" }, new[] { "th-TH", "th" }, new[] { "uk-UA", "uk" }, new[] { "ja-JP", "ja" },
                new[] { "en-GB", "en" }, new[] { "tr-TR", "en" },
            };
            try
            {
                foreach (var c in cases)
                {
                    Lang.Use(System.Globalization.CultureInfo.GetCultureInfo(c[0]));
                    Equal(c[1], Lang.Name, c[0]);
                }
                Lang.Use(System.Globalization.CultureInfo.GetCultureInfo("de-DE"));
                Equal("Jetzt sperren", Lang.T("Menu.LockNow"), "German text");
                Equal("Version 1.0", Lang.T("Version", "1.0"), "placeholder filled");
            }
            finally
            {
                Lang.Use((string)null);
            }
        }

        private static void SavedLanguageWins()
        {
            try
            {
                Lang.Use("fr");
                Equal("fr", Lang.Name, "picked language");
                Equal("Quitter", Lang.T("Menu.Exit"), "French text");
                Lang.Use("ZH-HANS");
                Equal("zh-Hans", Lang.Name, "case doesn't matter");
                Lang.Use("en");
                Equal("Exit", Lang.T("Menu.Exit"), "English picked over Windows");
                Lang.Use("xx");
                Equal(Lang.WindowsLanguage, Lang.Name, "unknown follows Windows");
                Lang.Use("");
                Equal(Lang.WindowsLanguage, Lang.Name, "empty follows Windows");
                Equal("Nope.Missing", Lang.T("Nope.Missing"), "unknown key shows the key, not a crash");
            }
            finally
            {
                Lang.Use((string)null);
            }
        }

        private static void PluralForms()
        {
            var expected = new Dictionary<string, string[]> // 1, 3, 5, 10 seconds
            {
                { "en", new[] { "1 second", "3 seconds", "5 seconds", "10 seconds" } },
                { "ru", new[] { "1 секунда", "3 секунды", "5 секунд", "10 секунд" } },
                { "uk", new[] { "1 секунда", "3 секунди", "5 секунд", "10 секунд" } },
                { "pl", new[] { "1 sekunda", "3 sekundy", "5 sekund", "10 sekund" } },
                { "cs", new[] { "1 sekunda", "3 sekundy", "5 sekund", "10 sekund" } },
                { "fr", new[] { "1 seconde", "3 secondes", "5 secondes", "10 secondes" } },
                { "ja", new[] { "1 秒", "3 秒", "5 秒", "10 秒" } },
            };
            try
            {
                foreach (var e in expected)
                {
                    Lang.Use(e.Key);
                    int[] n = { 1, 3, 5, 10 };
                    for (int i = 0; i < n.Length; i++)
                        Equal(e.Value[i], Lang.Seconds(n[i]), e.Key + " " + n[i]);
                }
                Equal("Few", Lang.PluralOf("ru", 22), "ru 22");
                Equal("Other", Lang.PluralOf("ru", 12), "ru 12");
                Equal("One", Lang.PluralOf("ru", 21), "ru 21");
            }
            finally
            {
                Lang.Use((string)null);
            }
        }

        private static void TooltipsFit()
        {
            InEachLanguage(delegate(string name)
            {
                Check(Lang.T("Tip.NoSignIn").Length <= 63, name + " Tip.NoSignIn");
                foreach (var state in new[] { "Tip.On", "Tip.Off" })
                    foreach (var power in new[] { "Tip.Online", "Tip.Offline", "Tip.Unknown" })
                    {
                        string tip = Lang.T(state, Lang.T(power));
                        Check(tip.Length <= 63, name + ": " + tip + " (" + tip.Length + ")");
                    }
            });
        }

        private static void PickerChoices()
        {
            var choices = Lang.Choices();
            Equal("en", choices[0].Key, "English first");
            Equal(AllLanguages().Count, choices.Count, "every language listed");
            var seen = new HashSet<string>();
            foreach (var c in choices)
                Check(seen.Add(c.Value), "names are distinct: " + c.Value);
            Check(seen.Contains("Deutsch") && seen.Contains("日本語") && seen.Contains("Bahasa Indonesia"), "own names");
        }

        private static void RecoveryFileTranslated()
        {
            try
            {
                Lang.Use("de");
                string text = RecoveryCodeDialog.RecoveryFileText("ABCDE-FGHJK", new DateTime(2026, 11, 25));
                Check(text.StartsWith("UnplugLock-Wiederherstellungscode\r\n\r\nABCDE-FGHJK\r\n"), "title and code");
                Check(text.Contains("„Kennwort vergessen?“"), "names the German link");
                Check(text.Contains("25. November 2026"), "German date: " + text);

                Lang.Use("ja");
                text = RecoveryCodeDialog.RecoveryFileText("ABCDE-FGHJK", new DateTime(2026, 11, 25));
                Check(text.Contains("2026年11月25日"), "Japanese date: " + text);
            }
            finally
            {
                Lang.Use((string)null);
            }
        }

        // each flag is drawn, fills its box and is more than one colour
        private static void PickerFlags()
        {
            foreach (var choice in Lang.Choices())
            {
                using (var bitmap = new Bitmap(30, 20))
                using (var g = Graphics.FromImage(bitmap))
                {
                    g.Clear(Color.Magenta);
                    Check(Flags.Draw(g, choice.Key, new Rectangle(0, 0, 30, 20)), choice.Key + " has a flag");
                    var colours = new HashSet<int>();
                    for (int x = 1; x < 29; x++)
                        for (int y = 1; y < 19; y++)
                        {
                            int c = bitmap.GetPixel(x, y).ToArgb();
                            Check(c != Color.Magenta.ToArgb(), choice.Key + " flag fills its box");
                            colours.Add(c);
                        }
                    Check(colours.Count > 1, choice.Key + " flag has more than one colour");
                }
            }
            Check(!Flags.Draw(Graphics.FromImage(new Bitmap(1, 1)), "xx", new Rectangle(0, 0, 1, 1)), "no flag for an unknown language");
        }

        private static void WindowsBuildInEveryLanguage()
        {
            var flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
            var passwordCtor = typeof(PasswordDialog).GetConstructors(flags)[0];
            var modes = passwordCtor.GetParameters()[1].ParameterType;
            InEachLanguage(delegate(string name)
            {
                var forms = new List<Form>
                {
                    (Form)Activator.CreateInstance(typeof(AboutDialog), true),
                    (Form)Activator.CreateInstance(typeof(RecoveryCodeDialog), flags, null, new object[] { "ABCDE-FGHJK" }, null),
                    new SetupForm(),
                };
                foreach (var mode in Enum.GetValues(modes))
                    forms.Add((Form)passwordCtor.Invoke(new object[] { Lang.T("Auth.Exit"), mode, null }));
                foreach (var f in forms)
                {
                    using (f)
                    {
                        f.PerformLayout();
                        Check(f.PreferredSize.Width < 900 && f.PreferredSize.Height < 900, name + " " + f.GetType().Name + " size");
                    }
                }
            });
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

        // ---- installer packaging

        // a folder of its own under the test folder, emptied first
        private static string PackDir(string name)
        {
            string dir = Path.Combine(Settings.BackupDir, name);
            if (Directory.Exists(dir))
                Directory.Delete(dir, true);
            Directory.CreateDirectory(dir);
            return dir;
        }

        // stands in for setup's own program and the app: any bytes do
        private static string WriteBytes(string dir, string name, int length, int seed)
        {
            var bytes = new byte[length];
            new Random(seed).NextBytes(bytes);
            string path = Path.Combine(dir, name);
            File.WriteAllBytes(path, bytes);
            return path;
        }

        private static bool SameBytes(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length)
                return false;
            for (int i = 0; i < a.Length; i++)
                if (a[i] != b[i])
                    return false;
            return true;
        }

        private static void PackRoundTrip()
        {
            string dir = PackDir("pack");
            string program = WriteBytes(dir, "setup.exe", 50000, 1);
            string app = WriteBytes(dir, "app.exe", 123457, 2);
            string installer = Path.Combine(dir, "installer.exe");
            Payload.Pack(program, app, installer);
            Check(SameBytes(File.ReadAllBytes(app), Payload.ReadApp(installer)), "app comes back out unchanged");
            Equal(50000 + 123457 + 16, (int)new FileInfo(installer).Length, "program + app + 16-byte trailer");

            bool refused = false;
            try { Payload.Pack(installer, app, Path.Combine(dir, "twice.exe")); }
            catch (InvalidOperationException) { refused = true; }
            Check(refused, "won't pack an app onto an installer twice");
        }

        private static void UninstallerLeavesAppOut()
        {
            string dir = PackDir("uninstaller");
            string program = WriteBytes(dir, "setup.exe", 50000, 3);
            string installer = Path.Combine(dir, "installer.exe");
            Payload.Pack(program, WriteBytes(dir, "app.exe", 70000, 4), installer);
            string uninstaller = Path.Combine(dir, "Uninstall.exe");
            Payload.WriteProgram(installer, uninstaller);
            Check(SameBytes(File.ReadAllBytes(program), File.ReadAllBytes(uninstaller)), "Uninstall.exe is setup's program alone");
            Check(Payload.ReadApp(uninstaller) == null, "Uninstall.exe has no app in it");
        }

        private static void NoPackedApp()
        {
            string dir = PackDir("none");
            Check(Payload.ReadApp(WriteBytes(dir, "plain.exe", 40000, 5)) == null, "a plain file has no app");
            Check(Payload.ReadApp(WriteBytes(dir, "tiny.exe", 3, 6)) == null, "nor does a file shorter than the trailer");

            // a plain program copies over whole
            string plain = Path.Combine(dir, "plain.exe"), copy = Path.Combine(dir, "copy.exe");
            Payload.WriteProgram(plain, copy);
            Check(SameBytes(File.ReadAllBytes(plain), File.ReadAllBytes(copy)), "a program with no app copies whole");
        }

        // this test program has every translation built in, so it stands in for UnplugLock.exe
        private static void CheckBorrowed(Dictionary<string, Func<Dictionary<string, string>>> borrowed, string from)
        {
            var expected = new List<string>(Lang.Available);
            Equal(expected.Count, borrowed.Count, from + ": number of languages");
            foreach (var name in expected)
            {
                Check(borrowed.ContainsKey(name), from + " has " + name);
                if (borrowed.ContainsKey(name))
                    Equal(Lang.TableFor(name)["LanguageName"], borrowed[name]()["LanguageName"], from + " " + name + " text");
            }
        }

        private static void SetupBorrowsPackedLanguages()
        {
            string dir = PackDir("borrow-packed");
            string installer = Path.Combine(dir, "UnplugLock-Setup.exe");
            Payload.Pack(WriteBytes(dir, "setup.exe", 30000, 7), Application.ExecutablePath, installer);
            CheckBorrowed(SetupLanguages.Load(installer), "installer");
        }

        private static void UninstallerBorrowsInstalledLanguages()
        {
            string dir = PackDir("borrow-installed");
            string uninstaller = WriteBytes(dir, "Uninstall.exe", 30000, 8);
            File.Copy(Application.ExecutablePath, Path.Combine(dir, "UnplugLock.exe"));
            CheckBorrowed(SetupLanguages.Load(uninstaller), "uninstaller");
        }

        private static void SetupEnglishWithoutApp()
        {
            string dir = PackDir("borrow-nothing");
            Equal(0, SetupLanguages.Load(WriteBytes(dir, "Uninstall.exe", 30000, 9)).Count, "no app: no other languages");

            // an app that isn't a program at all is ignored, not a crash
            string installer = Path.Combine(dir, "UnplugLock-Setup.exe");
            Payload.Pack(WriteBytes(dir, "setup.exe", 30000, 10), WriteBytes(dir, "junk.exe", 5000, 11), installer);
            Equal(0, SetupLanguages.Load(installer).Count, "a damaged app: no other languages");
        }
    }
}
