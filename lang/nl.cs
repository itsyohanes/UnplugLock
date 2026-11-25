// Dutch
using System.Collections.Generic;

namespace UnplugLock
{
    internal static partial class Lang
    {
        private static Dictionary<string, string> Dutch()
        {
            return new Dictionary<string, string>
            {
                { "LanguageName", "Nederlands" },

                // shared
                { "AppTagline", "Vergrendelt je laptop zodra de oplader wordt losgekoppeld." },
                { "Version", "Versie {0}" },
                { "Button.Cancel", "Annuleren" },
                { "Button.Continue", "Doorgaan" },
                { "Button.Close", "Sluiten" },
                { "Button.Done", "Gereed" },
                { "WrongPassword", "Onjuist wachtwoord." },
                { "AlreadyRunning", "UnplugLock is al actief." },

                // password prompt
                { "Pw.TitleEnter", "Voer je wachtwoord in" },
                { "Pw.TitleChoose", "Kies een wachtwoord" },
                { "Pw.TitleRecovery", "Voer je herstelcode in" },
                { "Pw.Label", "Wachtwoord" },
                { "Pw.LabelNew", "Nieuw wachtwoord" },
                { "Pw.LabelRecovery", "Herstelcode" },
                { "Pw.LabelConfirm", "Wachtwoord bevestigen" },
                { "Pw.Hint", "Minstens {0} tekens. Gebruik niet je Windows-wachtwoord of pincode." },
                { "Pw.Forgot", "Wachtwoord vergeten?" },
                { "Pw.Set", "Wachtwoord instellen" },
                { "Pw.TooShort", "Gebruik minstens {0} tekens." },
                { "Pw.Mismatch", "De wachtwoorden komen niet overeen." },
                { "Pw.NewPrompt", "Je hebt het nodig om UnplugLock uit te zetten, de instellingen te wijzigen of af te sluiten." },
                { "Pw.RecoveryPrompt", "Voer je herstelcode in. Die verwijdert je UnplugLock-wachtwoord zodat je een nieuw wachtwoord kunt instellen." },
                { "Pw.RecoveryWrong", "Die herstelcode klopt niet." },

                // what the password is asked for
                { "Auth.TurnOff", "Voer je UnplugLock-wachtwoord in om het uit te zetten." },
                { "Auth.StopEthernet", "Voer je UnplugLock-wachtwoord in om niet meer te vergrendelen als de ethernetkabel wordt losgekoppeld." },
                { "Auth.StopWifi", "Voer je UnplugLock-wachtwoord in om niet meer te vergrendelen als de wifi wegvalt." },
                { "Auth.Delay", "Voer je UnplugLock-wachtwoord in om de vergrendelvertraging te wijzigen." },
                { "Auth.Guard", "Voer je UnplugLock-wachtwoord in om de procesbewaking uit te zetten." },
                { "Auth.Startup", "Voer je UnplugLock-wachtwoord in om het niet meer met Windows te laten starten." },
                { "Auth.ChangePassword", "Voer je UnplugLock-wachtwoord in om het wachtwoord te wijzigen." },
                { "Auth.RemovePassword", "Voer je UnplugLock-wachtwoord in om het wachtwoord te verwijderen." },
                { "Auth.Exit", "Voer je UnplugLock-wachtwoord in om af te sluiten." },
                { "Auth.Recovered", "Herstelcode geaccepteerd. Je wachtwoord is verwijderd. Stel een nieuw wachtwoord in via het menu Wachtwoord." },

                // recovery code window
                { "Rc.Title", "Bewaar je herstelcode" },
                { "Rc.Subtitle", "Dit is de enige manier om weer binnen te komen als je je wachtwoord vergeet." },
                { "Rc.Explain", "Als je je wachtwoord vergeet, klik je op '{0}' bij de wachtwoordvraag en voer je deze code in. Die verwijdert het wachtwoord zodat je een nieuw wachtwoord kunt instellen." },
                { "Rc.Keep", "Bewaar hem ergens anders dan op deze laptop, bijvoorbeeld op je telefoon, in een wachtwoordbeheerder of op papier. Hij wordt niet opnieuw getoond." },
                { "Rc.Copy", "Kopiëren" },
                { "Rc.Copied", "Gekopieerd" },
                { "Rc.Save", "Opslaan als bestand..." },
                { "Rc.Confirm", "Ik heb mijn herstelcode bewaard" },
                { "Rc.FileTitle", "UnplugLock-herstelcode" },
                { "Rc.FileType", "Tekstbestand" },
                { "Rc.FileHelp", "Als je je UnplugLock-wachtwoord vergeet, klik je op '{0}' bij de wachtwoordvraag en voer je deze code in." },
                { "Rc.FileCreated", "Gemaakt op {0}." },
                { "Rc.DateFormat", "d MMMM yyyy" },
                { "Rc.SaveFailed", "Kan het bestand niet opslaan:\n{0}" },
                { "Rc.SavedOnLaptop", "Opgeslagen. Dit bestand staat op de schijf van de laptop, dus iemand die de laptop meeneemt kan het vinden en met de code je wachtwoord verwijderen.\n\nKopieer het naar een USB-stick, je telefoon of een wachtwoordbeheerder en verwijder het daarna van deze laptop." },

                // about window
                { "About.Window", "Over UnplugLock" },
                { "About.Source", "Broncode op GitHub" },
                { "About.Releases", "Zoeken naar nieuwe versies" },
                { "About.Support", "UnplugLock steunen" },
                { "About.License", "Gratis en open source, MIT-licentie" },

                // tray menu
                { "Menu.Enabled", "Ingeschakeld" },
                { "Menu.Delay", "Vergrendelvertraging" },
                { "Delay.Instant", "Direct" },
                { "Delay.One", "{0} seconde" },
                { "Delay.Other", "{0} seconden" },
                { "Menu.AlsoLock", "Ook vergrendelen als" },
                { "Menu.Ethernet", "de ethernetkabel wordt losgekoppeld" },
                { "Menu.Wifi", "de wifi wegvalt" },
                { "Menu.Startup", "Starten met Windows" },
                { "Menu.StartupGuarded", "Starten met Windows (nodig voor bewaking)" },
                { "Menu.Guard", "Procesbewaking (automatisch herstarten)" },
                { "Menu.Hide", "Pictogram in systeemvak verbergen" },
                { "Menu.Password", "Wachtwoord" },
                { "Menu.SetPassword", "Wachtwoord instellen..." },
                { "Menu.ChangePassword", "Wachtwoord wijzigen..." },
                { "Menu.RemovePassword", "Wachtwoord verwijderen" },
                { "Menu.LockNow", "Nu vergrendelen" },
                { "Menu.About", "Over UnplugLock..." },
                { "Menu.Exit", "Afsluiten" },
                { "Menu.NoSignIn", "Windows heeft geen wachtwoord - stel er een in..." },
                { "Power.Online", "Aangesloten" },
                { "Power.Offline", "Op batterij" },
                { "Power.Unknown", "Voedingsstatus onbekend" },

                // tray icon tooltip (63 characters at most)
                { "Tip.On", "UnplugLock - aan ({0})" },
                { "Tip.Off", "UnplugLock - uit ({0})" },
                { "Tip.Online", "aangesloten" },
                { "Tip.Offline", "op batterij" },
                { "Tip.Unknown", "voedingsstatus onbekend" },
                { "Tip.NoSignIn", "UnplugLock - geen Windows-wachtwoord, vergrendelen helpt niet" },

                // tray messages
                { "NoSignIn.Title", "Je Windows-account heeft geen wachtwoord" },
                { "NoSignIn.Text", "UnplugLock kan het scherm vergrendelen, maar iedereen kan met één klik weer binnenkomen. Klik hier om een wachtwoord of pincode in te stellen." },
                { "Running.Title", "UnplugLock is actief" },
                { "Running.On", "Je laptop wordt vergrendeld als hij wordt losgekoppeld. Klik met de rechtermuisknop op het pictogram voor opties." },
                { "Running.Off", "UnplugLock staat uit. Klik met de rechtermuisknop op het pictogram voor opties." },
                { "LockFailed", "Windows is niet vergrendeld (fout {0}). Als de oplader wordt losgekoppeld, blijft UnplugLock het 10 minuten proberen en vergrendelt het zodra Windows dat toestaat." },
                { "StartupFailed", "Kan de opstartinstelling niet wijzigen:\n{0}" },
                { "HideConfirm", "Het pictogram in het systeemvak wordt verborgen, maar UnplugLock blijft actief en beschermt je laptop.\n\nOpen UnplugLock opnieuw om het pictogram weer te tonen (via het menu Start of UnplugLock.exe)." },
                { "PasswordRemoved", "Wachtwoord verwijderd." },

                // installer
                { "Setup.Window", "UnplugLock installeren" },
                { "Setup.Language", "Taal" },
                { "Setup.TitleInstall", "UnplugLock installeren" },
                { "Setup.TitleUpdate", "UnplugLock bijwerken" },
                { "Setup.Guard", "UnplugLock actief houden" },
                { "Setup.GuardHint", "Als iemand het sluit, start het meteen weer op." },
                { "Setup.Startup", "UnplugLock starten als ik me aanmeld bij Windows" },
                { "Setup.Desktop", "Snelkoppeling op het bureaublad maken" },
                { "Setup.Launch", "UnplugLock openen na de installatie" },
                { "Setup.InstallsTo", "Wordt geïnstalleerd in" },
                { "Setup.PerUser", "Alleen voor jouw account, dus er zijn geen beheerdersrechten nodig." },
                { "Setup.Install", "Installeren" },
                { "Setup.Update", "Bijwerken" },
                { "Setup.Installing", "Installeren..." },
                { "Setup.Updating", "Bijwerken..." },
                { "Setup.Finish", "Voltooien" },
                { "Setup.Failed", "De installatie kon niet worden voltooid:\n\n{0}" },
                { "Setup.Installed", "UnplugLock is geïnstalleerd" },
                { "Setup.Updated", "UnplugLock is bijgewerkt" },
                { "Setup.DoneSubtitle", "Je laptop wordt nu vergrendeld als de oplader wordt losgekoppeld." },
                { "Setup.DoneRunning", "Het draait in het systeemvak, naast de klok. Zie je het pictogram niet, klik dan daar op het pijltje ^." },
                { "Setup.DoneNotRunning", "Open het via het menu Start wanneer je wilt. Het staat dan in het systeemvak, naast de klok." },
                { "Setup.DoneHint", "Klik met de rechtermuisknop op het pictogram om een vertraging in te stellen, een wachtwoord toe te voegen of het even uit te zetten." },
                { "Setup.AuthUpdate", "UnplugLock is met een wachtwoord beveiligd. Voer het wachtwoord in om het bij te werken." },
                { "Setup.AuthUninstall", "UnplugLock is met een wachtwoord beveiligd. Voer het wachtwoord in om het te verwijderen." },
                { "Setup.StillRunning", "UnplugLock is nog actief en kon niet worden gesloten. Sluit het af via het menu in het systeemvak en start de installatie opnieuw." },
                { "Setup.Damaged", "Dit installatiebestand is beschadigd (UnplugLock.exe ontbreekt)." },
                { "Setup.ShortcutDescription", "Vergrendelt je laptop als de oplader wordt losgekoppeld" },
                { "Uninstall.Title", "UnplugLock verwijderen" },
                { "Uninstall.Confirm", "UnplugLock en alle instellingen van deze pc verwijderen?" },
                { "Uninstall.StillRunning", "UnplugLock is nog actief en kon niet worden gesloten. Sluit het af via het menu in het systeemvak en probeer het daarna opnieuw te verwijderen." },
                { "Uninstall.Done", "UnplugLock is verwijderd." },
            };
        }
    }
}
