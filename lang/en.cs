// English: the source text, and what every other language falls back to.
using System.Collections.Generic;

namespace UnplugLock
{
    internal static partial class Lang
    {
        private static Dictionary<string, string> English()
        {
            return new Dictionary<string, string>
            {
                // this language's name for itself, for the setup language picker
                { "LanguageName", "English" },

                // shared
                { "AppTagline", "Locks your laptop the moment the charger is unplugged." },
                { "Version", "Version {0}" },
                { "Button.Cancel", "Cancel" },
                { "Button.Continue", "Continue" },
                { "Button.Close", "Close" },
                { "Button.Done", "Done" },
                { "WrongPassword", "Wrong password." },
                { "AlreadyRunning", "UnplugLock is already running." },

                // password prompt
                { "Pw.TitleEnter", "Enter your password" },
                { "Pw.TitleChoose", "Choose a password" },
                { "Pw.TitleRecovery", "Enter your recovery code" },
                { "Pw.Label", "Password" },
                { "Pw.LabelNew", "New password" },
                { "Pw.LabelRecovery", "Recovery code" },
                { "Pw.LabelConfirm", "Confirm password" },
                { "Pw.Hint", "At least {0} characters. Don't reuse your Windows password or PIN." },
                { "Pw.Forgot", "Forgot password?" },
                { "Pw.Set", "Set password" },
                { "Pw.TooShort", "Use at least {0} characters." },
                { "Pw.Mismatch", "The passwords don't match." },
                { "Pw.NewPrompt", "You'll need it to turn off, change the settings of, or exit UnplugLock." },
                { "Pw.RecoveryPrompt", "Enter your recovery code. It removes your UnplugLock password so you can set a new one." },
                { "Pw.RecoveryWrong", "That recovery code isn't right." },

                // what the password is asked for
                { "Auth.TurnOff", "Enter your UnplugLock password to turn it off." },
                { "Auth.StopEthernet", "Enter your UnplugLock password to stop locking when Ethernet is unplugged." },
                { "Auth.StopWifi", "Enter your UnplugLock password to stop locking when Wi-Fi disconnects." },
                { "Auth.Delay", "Enter your UnplugLock password to change the lock delay." },
                { "Auth.Guard", "Enter your UnplugLock password to turn off the process guard." },
                { "Auth.Startup", "Enter your UnplugLock password to stop it starting with Windows." },
                { "Auth.ChangePassword", "Enter your UnplugLock password to change the password." },
                { "Auth.RemovePassword", "Enter your UnplugLock password to remove the password." },
                { "Auth.Exit", "Enter your UnplugLock password to exit." },
                { "Auth.Recovered", "Recovery code accepted. Your password has been removed. Set a new one from the Password menu." },

                // recovery code window. {0} in Rc.Explain and Rc.FileHelp is the "Forgot password?" link text.
                { "Rc.Title", "Save your recovery code" },
                { "Rc.Subtitle", "It's the only way back in if you forget your password." },
                { "Rc.Explain", "If you forget your password, click \"{0}\" on the password prompt and enter this code. It removes the password so you can set a new one." },
                { "Rc.Keep", "Keep it somewhere other than this laptop, like your phone, a password manager or on paper. It won't be shown again." },
                { "Rc.Copy", "Copy" },
                { "Rc.Copied", "Copied" },
                { "Rc.Save", "Save to file..." },
                { "Rc.Confirm", "I've saved my recovery code" },
                { "Rc.FileTitle", "UnplugLock recovery code" },
                { "Rc.FileType", "Text file" },
                { "Rc.FileHelp", "If you forget your UnplugLock password, click \"{0}\" on the password prompt and enter this code." },
                { "Rc.FileCreated", "Created {0}." },
                { "Rc.DateFormat", "d MMMM yyyy" },
                { "Rc.SaveFailed", "Couldn't save the file:\n{0}" },
                { "Rc.SavedOnLaptop", "Saved. This file is on the laptop's own drive, so anyone who takes the laptop could find it and use the code to remove your password.\n\nCopy it to a USB stick, your phone or a password manager, then delete it from this laptop." },

                // about window
                { "About.Window", "About UnplugLock" },
                { "About.Source", "Source code on GitHub" },
                { "About.Releases", "Check for new releases" },
                { "About.Support", "Support UnplugLock" },
                { "About.License", "Free and open source, MIT License" },

                // tray menu
                { "Menu.Enabled", "Enabled" },
                { "Menu.Delay", "Lock delay" },
                { "Delay.Instant", "Instant" },
                { "Delay.One", "{0} second" },
                { "Delay.Other", "{0} seconds" },
                { "Menu.AlsoLock", "Also lock when" },
                { "Menu.Ethernet", "Ethernet is unplugged" },
                { "Menu.Wifi", "Wi-Fi disconnects" },
                { "Menu.Startup", "Start with Windows" },
                { "Menu.StartupGuarded", "Start with Windows (required by guard)" },
                { "Menu.Guard", "Guard process (auto-restart)" },
                { "Menu.Hide", "Hide tray icon" },
                { "Menu.Password", "Password" },
                { "Menu.SetPassword", "Set password..." },
                { "Menu.ChangePassword", "Change password..." },
                { "Menu.RemovePassword", "Remove password" },
                { "Menu.LockNow", "Lock now" },
                { "Menu.About", "About UnplugLock..." },
                { "Menu.Exit", "Exit" },
                { "Menu.NoSignIn", "Windows has no password - set one up..." },
                { "Power.Online", "Plugged in" },
                { "Power.Offline", "On battery" },
                { "Power.Unknown", "Power status unknown" },

                // tray icon tooltip (Windows cuts it off after 63 characters)
                { "Tip.On", "UnplugLock - on ({0})" },
                { "Tip.Off", "UnplugLock - off ({0})" },
                { "Tip.Online", "plugged in" },
                { "Tip.Offline", "on battery" },
                { "Tip.Unknown", "power status unknown" },
                { "Tip.NoSignIn", "UnplugLock - Windows has no password, locking won't help" },

                // tray messages
                { "NoSignIn.Title", "Your Windows account has no password" },
                { "NoSignIn.Text", "UnplugLock can lock the screen, but anyone can click straight back in. Click here to set up a password or PIN." },
                { "Running.Title", "UnplugLock is running" },
                { "Running.On", "Your laptop locks when unplugged. Right-click the icon for options." },
                { "Running.Off", "UnplugLock is turned off. Right-click the icon for options." },
                { "LockFailed", "Windows didn't lock (error {0}). When the charger is pulled, UnplugLock keeps trying for 10 minutes, so it locks as soon as Windows allows it." },
                { "StartupFailed", "Couldn't change the startup setting:\n{0}" },
                { "HideConfirm", "The tray icon will be hidden, but UnplugLock keeps running and protecting your laptop.\n\nTo show the icon again, open UnplugLock again (from the Start menu or UnplugLock.exe)." },
                { "PasswordRemoved", "Password removed." },

                // installer
                { "Setup.Window", "UnplugLock Setup" },
                { "Setup.Language", "Language" },
                { "Setup.TitleInstall", "Install UnplugLock" },
                { "Setup.TitleUpdate", "Update UnplugLock" },
                { "Setup.Guard", "Keep UnplugLock running" },
                { "Setup.GuardHint", "If someone closes it, it starts itself again straight away." },
                { "Setup.Startup", "Start UnplugLock when I sign in to Windows" },
                { "Setup.Desktop", "Create a desktop shortcut" },
                { "Setup.Launch", "Open UnplugLock when setup finishes" },
                { "Setup.InstallsTo", "Installs to" },
                { "Setup.PerUser", "Just for your account, so it doesn't need administrator rights." },
                { "Setup.Install", "Install" },
                { "Setup.Update", "Update" },
                { "Setup.Installing", "Installing..." },
                { "Setup.Updating", "Updating..." },
                { "Setup.Finish", "Finish" },
                { "Setup.Failed", "Setup couldn't finish:\n\n{0}" },
                { "Setup.Installed", "UnplugLock is installed" },
                { "Setup.Updated", "UnplugLock is updated" },
                { "Setup.DoneSubtitle", "Your laptop now locks when the charger is pulled out." },
                { "Setup.DoneRunning", "It's running in the system tray, next to the clock. If you can't see the icon, click the ^ arrow there." },
                { "Setup.DoneNotRunning", "Open it from the Start menu whenever you're ready. It sits in the system tray, next to the clock." },
                { "Setup.DoneHint", "Right-click the icon to set a lock delay, add a password, or turn it off for a while." },
                { "Setup.AuthUpdate", "UnplugLock is password-protected. Enter the password to update it." },
                { "Setup.AuthUninstall", "UnplugLock is password-protected. Enter the password to uninstall it." },
                { "Setup.StillRunning", "UnplugLock is still running and wouldn't close. Exit it from its tray menu, then run setup again." },
                { "Setup.Damaged", "This setup file is damaged (UnplugLock.exe is missing from it)." },
                { "Setup.ShortcutDescription", "Locks your laptop when the charger is unplugged" },
                { "Uninstall.Title", "Uninstall UnplugLock" },
                { "Uninstall.Confirm", "Remove UnplugLock and all of its settings from this PC?" },
                { "Uninstall.StillRunning", "UnplugLock is still running and wouldn't close. Exit it from its tray menu, then uninstall again." },
                { "Uninstall.Done", "UnplugLock has been uninstalled." },
            };
        }
    }
}
