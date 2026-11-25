// German
using System.Collections.Generic;

namespace UnplugLock
{
    internal static partial class Lang
    {
        private static Dictionary<string, string> German()
        {
            return new Dictionary<string, string>
            {
                { "LanguageName", "Deutsch" },

                // shared
                { "AppTagline", "Sperrt Ihren Laptop, sobald das Ladegerät abgezogen wird." },
                { "Version", "Version {0}" },
                { "Button.Cancel", "Abbrechen" },
                { "Button.Continue", "Weiter" },
                { "Button.Close", "Schließen" },
                { "Button.Done", "Fertig" },
                { "WrongPassword", "Falsches Kennwort." },
                { "AlreadyRunning", "UnplugLock wird bereits ausgeführt." },

                // password prompt
                { "Pw.TitleEnter", "Kennwort eingeben" },
                { "Pw.TitleChoose", "Kennwort festlegen" },
                { "Pw.TitleRecovery", "Wiederherstellungscode eingeben" },
                { "Pw.Label", "Kennwort" },
                { "Pw.LabelNew", "Neues Kennwort" },
                { "Pw.LabelRecovery", "Wiederherstellungscode" },
                { "Pw.LabelConfirm", "Kennwort bestätigen" },
                { "Pw.Hint", "Mindestens {0} Zeichen. Verwenden Sie nicht Ihr Windows-Kennwort oder Ihre PIN." },
                { "Pw.Forgot", "Kennwort vergessen?" },
                { "Pw.Set", "Kennwort festlegen" },
                { "Pw.TooShort", "Verwenden Sie mindestens {0} Zeichen." },
                { "Pw.Mismatch", "Die Kennwörter stimmen nicht überein." },
                { "Pw.NewPrompt", "Sie brauchen es, um UnplugLock auszuschalten, die Einstellungen zu ändern oder das Programm zu beenden." },
                { "Pw.RecoveryPrompt", "Geben Sie Ihren Wiederherstellungscode ein. Er entfernt Ihr UnplugLock-Kennwort, damit Sie ein neues festlegen können." },
                { "Pw.RecoveryWrong", "Dieser Wiederherstellungscode ist falsch." },

                // what the password is asked for
                { "Auth.TurnOff", "Geben Sie Ihr UnplugLock-Kennwort ein, um UnplugLock auszuschalten." },
                { "Auth.StopEthernet", "Geben Sie Ihr UnplugLock-Kennwort ein, damit nicht mehr gesperrt wird, wenn das Ethernetkabel abgezogen wird." },
                { "Auth.StopWifi", "Geben Sie Ihr UnplugLock-Kennwort ein, damit nicht mehr gesperrt wird, wenn die WLAN-Verbindung getrennt wird." },
                { "Auth.Delay", "Geben Sie Ihr UnplugLock-Kennwort ein, um die Sperrverzögerung zu ändern." },
                { "Auth.Guard", "Geben Sie Ihr UnplugLock-Kennwort ein, um den Prozessschutz auszuschalten." },
                { "Auth.Startup", "Geben Sie Ihr UnplugLock-Kennwort ein, damit UnplugLock nicht mehr mit Windows startet." },
                { "Auth.ChangePassword", "Geben Sie Ihr UnplugLock-Kennwort ein, um das Kennwort zu ändern." },
                { "Auth.RemovePassword", "Geben Sie Ihr UnplugLock-Kennwort ein, um das Kennwort zu entfernen." },
                { "Auth.Exit", "Geben Sie Ihr UnplugLock-Kennwort ein, um UnplugLock zu beenden." },
                { "Auth.Recovered", "Wiederherstellungscode akzeptiert. Ihr Kennwort wurde entfernt. Legen Sie im Menü „Kennwort“ ein neues fest." },

                // recovery code window
                { "Rc.Title", "Wiederherstellungscode speichern" },
                { "Rc.Subtitle", "Nur damit kommen Sie wieder hinein, wenn Sie Ihr Kennwort vergessen." },
                { "Rc.Explain", "Wenn Sie Ihr Kennwort vergessen, klicken Sie bei der Kennwortabfrage auf „{0}“ und geben diesen Code ein. Er entfernt das Kennwort, damit Sie ein neues festlegen können." },
                { "Rc.Keep", "Bewahren Sie ihn an einem anderen Ort als diesem Laptop auf, z. B. auf Ihrem Handy, in einem Kennwort-Manager oder auf Papier. Er wird nicht noch einmal angezeigt." },
                { "Rc.Copy", "Kopieren" },
                { "Rc.Copied", "Kopiert" },
                { "Rc.Save", "In Datei speichern..." },
                { "Rc.Confirm", "Ich habe meinen Wiederherstellungscode gespeichert" },
                { "Rc.FileTitle", "UnplugLock-Wiederherstellungscode" },
                { "Rc.FileType", "Textdatei" },
                { "Rc.FileHelp", "Wenn Sie Ihr UnplugLock-Kennwort vergessen, klicken Sie bei der Kennwortabfrage auf „{0}“ und geben diesen Code ein." },
                { "Rc.FileCreated", "Erstellt am {0}." },
                { "Rc.DateFormat", "d. MMMM yyyy" },
                { "Rc.SaveFailed", "Die Datei konnte nicht gespeichert werden:\n{0}" },
                { "Rc.SavedOnLaptop", "Gespeichert. Die Datei liegt auf dem Laufwerk des Laptops. Wer den Laptop mitnimmt, könnte sie finden und mit dem Code Ihr Kennwort entfernen.\n\nKopieren Sie sie auf einen USB-Stick, Ihr Handy oder in einen Kennwort-Manager und löschen Sie sie dann von diesem Laptop." },

                // about window
                { "About.Window", "Info zu UnplugLock" },
                { "About.Source", "Quellcode auf GitHub" },
                { "About.Releases", "Nach neuen Versionen suchen" },
                { "About.Support", "UnplugLock unterstützen" },
                { "About.License", "Kostenlos und Open Source, MIT-Lizenz" },

                // tray menu
                { "Menu.Enabled", "Aktiviert" },
                { "Menu.Delay", "Sperrverzögerung" },
                { "Delay.Instant", "Sofort" },
                { "Delay.One", "{0} Sekunde" },
                { "Delay.Other", "{0} Sekunden" },
                { "Menu.AlsoLock", "Auch sperren, wenn" },
                { "Menu.Ethernet", "das Ethernetkabel abgezogen wird" },
                { "Menu.Wifi", "die WLAN-Verbindung getrennt wird" },
                { "Menu.Startup", "Mit Windows starten" },
                { "Menu.StartupGuarded", "Mit Windows starten (für Prozessschutz nötig)" },
                { "Menu.Guard", "Prozessschutz (automatischer Neustart)" },
                { "Menu.Hide", "Symbol im Infobereich ausblenden" },
                { "Menu.Password", "Kennwort" },
                { "Menu.SetPassword", "Kennwort festlegen..." },
                { "Menu.ChangePassword", "Kennwort ändern..." },
                { "Menu.RemovePassword", "Kennwort entfernen" },
                { "Menu.LockNow", "Jetzt sperren" },
                { "Menu.About", "Info zu UnplugLock..." },
                { "Menu.Exit", "Beenden" },
                { "Menu.NoSignIn", "Windows hat kein Kennwort - jetzt einrichten..." },
                { "Power.Online", "Netzbetrieb" },
                { "Power.Offline", "Akkubetrieb" },
                { "Power.Unknown", "Stromversorgung unbekannt" },

                // tray icon tooltip (63 characters at most)
                { "Tip.On", "UnplugLock - ein ({0})" },
                { "Tip.Off", "UnplugLock - aus ({0})" },
                { "Tip.Online", "Netzbetrieb" },
                { "Tip.Offline", "Akkubetrieb" },
                { "Tip.Unknown", "Stromversorgung unbekannt" },
                { "Tip.NoSignIn", "UnplugLock - Windows hat kein Kennwort, Sperren nützt nichts" },

                // tray messages
                { "NoSignIn.Title", "Ihr Windows-Konto hat kein Kennwort" },
                { "NoSignIn.Text", "UnplugLock kann den Bildschirm sperren, aber jeder kann sich mit einem Klick wieder anmelden. Klicken Sie hier, um ein Kennwort oder eine PIN einzurichten." },
                { "Running.Title", "UnplugLock wird ausgeführt" },
                { "Running.On", "Ihr Laptop wird gesperrt, wenn das Ladegerät abgezogen wird. Optionen finden Sie per Rechtsklick auf das Symbol." },
                { "Running.Off", "UnplugLock ist ausgeschaltet. Optionen finden Sie per Rechtsklick auf das Symbol." },
                { "LockFailed", "Windows wurde nicht gesperrt (Fehler {0}). Wenn das Ladegerät abgezogen wird, versucht UnplugLock es 10 Minuten lang weiter und sperrt, sobald Windows es zulässt." },
                { "StartupFailed", "Die Autostart-Einstellung konnte nicht geändert werden:\n{0}" },
                { "HideConfirm", "Das Symbol im Infobereich wird ausgeblendet, aber UnplugLock läuft weiter und schützt Ihren Laptop.\n\nUm das Symbol wieder anzuzeigen, öffnen Sie UnplugLock erneut (über das Startmenü oder UnplugLock.exe)." },
                { "PasswordRemoved", "Kennwort entfernt." },

                // installer
                { "Setup.Window", "UnplugLock-Setup" },
                { "Setup.Language", "Sprache" },
                { "Setup.TitleInstall", "UnplugLock installieren" },
                { "Setup.TitleUpdate", "UnplugLock aktualisieren" },
                { "Setup.Guard", "UnplugLock am Laufen halten" },
                { "Setup.GuardHint", "Wird es beendet, startet es sich sofort wieder." },
                { "Setup.Startup", "UnplugLock bei der Windows-Anmeldung starten" },
                { "Setup.Desktop", "Desktopverknüpfung erstellen" },
                { "Setup.Launch", "UnplugLock nach dem Setup öffnen" },
                { "Setup.InstallsTo", "Wird installiert in" },
                { "Setup.PerUser", "Nur für Ihr Konto, daher sind keine Administratorrechte nötig." },
                { "Setup.Install", "Installieren" },
                { "Setup.Update", "Aktualisieren" },
                { "Setup.Installing", "Wird installiert..." },
                { "Setup.Updating", "Wird aktualisiert..." },
                { "Setup.Finish", "Fertig stellen" },
                { "Setup.Failed", "Das Setup konnte nicht abgeschlossen werden:\n\n{0}" },
                { "Setup.Installed", "UnplugLock ist installiert" },
                { "Setup.Updated", "UnplugLock ist aktualisiert" },
                { "Setup.DoneSubtitle", "Ihr Laptop wird jetzt gesperrt, wenn das Ladegerät abgezogen wird." },
                { "Setup.DoneRunning", "Es läuft im Infobereich neben der Uhr. Wenn Sie das Symbol nicht sehen, klicken Sie dort auf den Pfeil ^." },
                { "Setup.DoneNotRunning", "Öffnen Sie es über das Startmenü, wann immer Sie möchten. Es läuft dann im Infobereich neben der Uhr." },
                { "Setup.DoneHint", "Klicken Sie mit der rechten Maustaste auf das Symbol, um eine Sperrverzögerung einzustellen, ein Kennwort hinzuzufügen oder es vorübergehend auszuschalten." },
                { "Setup.AuthUpdate", "UnplugLock ist kennwortgeschützt. Geben Sie das Kennwort ein, um es zu aktualisieren." },
                { "Setup.AuthUninstall", "UnplugLock ist kennwortgeschützt. Geben Sie das Kennwort ein, um es zu deinstallieren." },
                { "Setup.StillRunning", "UnplugLock läuft noch und ließ sich nicht schließen. Beenden Sie es über das Menü im Infobereich und starten Sie das Setup dann erneut." },
                { "Setup.Damaged", "Diese Setupdatei ist beschädigt (UnplugLock.exe fehlt darin)." },
                { "Setup.ShortcutDescription", "Sperrt Ihren Laptop, wenn das Ladegerät abgezogen wird" },
                { "Uninstall.Title", "UnplugLock deinstallieren" },
                { "Uninstall.Confirm", "UnplugLock und alle Einstellungen von diesem PC entfernen?" },
                { "Uninstall.StillRunning", "UnplugLock läuft noch und ließ sich nicht schließen. Beenden Sie es über das Menü im Infobereich und deinstallieren Sie es dann erneut." },
                { "Uninstall.Done", "UnplugLock wurde deinstalliert." },
            };
        }
    }
}
