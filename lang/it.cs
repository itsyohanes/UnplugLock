// Italian
using System.Collections.Generic;

namespace UnplugLock
{
    internal static partial class Lang
    {
        private static Dictionary<string, string> Italian()
        {
            return new Dictionary<string, string>
            {
                { "LanguageName", "Italiano" },

                // shared
                { "AppTagline", "Blocca il portatile appena si scollega il caricabatterie." },
                { "Version", "Versione {0}" },
                { "Button.Cancel", "Annulla" },
                { "Button.Continue", "Continua" },
                { "Button.Close", "Chiudi" },
                { "Button.Done", "Fine" },
                { "WrongPassword", "Password errata." },
                { "AlreadyRunning", "UnplugLock è già in esecuzione." },

                // password prompt
                { "Pw.TitleEnter", "Inserisci la password" },
                { "Pw.TitleChoose", "Scegli una password" },
                { "Pw.TitleRecovery", "Inserisci il codice di ripristino" },
                { "Pw.Label", "Password" },
                { "Pw.LabelNew", "Nuova password" },
                { "Pw.LabelRecovery", "Codice di ripristino" },
                { "Pw.LabelConfirm", "Conferma password" },
                { "Pw.Hint", "Almeno {0} caratteri. Non riutilizzare la password o il PIN di Windows." },
                { "Pw.Forgot", "Password dimenticata?" },
                { "Pw.Set", "Imposta password" },
                { "Pw.TooShort", "Usa almeno {0} caratteri." },
                { "Pw.Mismatch", "Le password non coincidono." },
                { "Pw.NewPrompt", "Ti servirà per disattivare UnplugLock, cambiarne le impostazioni o chiuderlo." },
                { "Pw.RecoveryPrompt", "Inserisci il codice di ripristino. Rimuove la password di UnplugLock così puoi impostarne una nuova." },
                { "Pw.RecoveryWrong", "Il codice di ripristino non è corretto." },

                // what the password is asked for
                { "Auth.TurnOff", "Inserisci la password di UnplugLock per disattivarlo." },
                { "Auth.StopEthernet", "Inserisci la password di UnplugLock per non bloccare più quando si scollega il cavo Ethernet." },
                { "Auth.StopWifi", "Inserisci la password di UnplugLock per non bloccare più quando si disconnette il Wi-Fi." },
                { "Auth.Delay", "Inserisci la password di UnplugLock per cambiare il ritardo di blocco." },
                { "Auth.Guard", "Inserisci la password di UnplugLock per disattivare la protezione del processo." },
                { "Auth.Startup", "Inserisci la password di UnplugLock per non avviarlo più con Windows." },
                { "Auth.ChangePassword", "Inserisci la password di UnplugLock per cambiare la password." },
                { "Auth.RemovePassword", "Inserisci la password di UnplugLock per rimuovere la password." },
                { "Auth.Exit", "Inserisci la password di UnplugLock per chiuderlo." },
                { "Auth.Recovered", "Codice di ripristino accettato. La password è stata rimossa. Impostane una nuova dal menu Password." },

                // recovery code window
                { "Rc.Title", "Salva il codice di ripristino" },
                { "Rc.Subtitle", "È l'unico modo per rientrare se dimentichi la password." },
                { "Rc.Explain", "Se dimentichi la password, fai clic su \"{0}\" nella richiesta della password e inserisci questo codice. Rimuove la password così puoi impostarne una nuova." },
                { "Rc.Keep", "Conservalo lontano da questo portatile, per esempio sul telefono, in un gestore di password o su carta. Non verrà più mostrato." },
                { "Rc.Copy", "Copia" },
                { "Rc.Copied", "Copiato" },
                { "Rc.Save", "Salva su file..." },
                { "Rc.Confirm", "Ho salvato il codice di ripristino" },
                { "Rc.FileTitle", "Codice di ripristino di UnplugLock" },
                { "Rc.FileType", "File di testo" },
                { "Rc.FileHelp", "Se dimentichi la password di UnplugLock, fai clic su \"{0}\" nella richiesta della password e inserisci questo codice." },
                { "Rc.FileCreated", "Creato il {0}." },
                { "Rc.DateFormat", "d MMMM yyyy" },
                { "Rc.SaveFailed", "Impossibile salvare il file:\n{0}" },
                { "Rc.SavedOnLaptop", "Salvato. Il file si trova sul disco del portatile, quindi chi prende il portatile potrebbe trovarlo e usare il codice per rimuovere la password.\n\nCopialo su una chiavetta USB, sul telefono o in un gestore di password, poi eliminalo da questo portatile." },

                // about window
                { "About.Window", "Informazioni su UnplugLock" },
                { "About.Source", "Codice sorgente su GitHub" },
                { "About.Releases", "Cerca nuove versioni" },
                { "About.Support", "Sostieni UnplugLock" },
                { "About.License", "Gratuito e open source, licenza MIT" },

                // tray menu
                { "Menu.Enabled", "Attivo" },
                { "Menu.Delay", "Ritardo di blocco" },
                { "Delay.Instant", "Immediato" },
                { "Delay.One", "{0} secondo" },
                { "Delay.Other", "{0} secondi" },
                { "Menu.AlsoLock", "Blocca anche quando" },
                { "Menu.Ethernet", "si scollega il cavo Ethernet" },
                { "Menu.Wifi", "si disconnette il Wi-Fi" },
                { "Menu.Startup", "Avvia con Windows" },
                { "Menu.StartupGuarded", "Avvia con Windows (richiesto dalla protezione)" },
                { "Menu.Guard", "Proteggi il processo (riavvio automatico)" },
                { "Menu.Hide", "Nascondi l'icona nell'area di notifica" },
                { "Menu.Password", "Password" },
                { "Menu.SetPassword", "Imposta password..." },
                { "Menu.ChangePassword", "Cambia password..." },
                { "Menu.RemovePassword", "Rimuovi password" },
                { "Menu.LockNow", "Blocca ora" },
                { "Menu.About", "Informazioni su UnplugLock..." },
                { "Menu.Exit", "Esci" },
                { "Menu.NoSignIn", "Windows non ha una password - impostane una..." },
                { "Power.Online", "Collegato alla corrente" },
                { "Power.Offline", "A batteria" },
                { "Power.Unknown", "Alimentazione sconosciuta" },

                // tray icon tooltip (63 characters at most)
                { "Tip.On", "UnplugLock - attivo ({0})" },
                { "Tip.Off", "UnplugLock - disattivo ({0})" },
                { "Tip.Online", "collegato alla corrente" },
                { "Tip.Offline", "a batteria" },
                { "Tip.Unknown", "alimentazione sconosciuta" },
                { "Tip.NoSignIn", "UnplugLock - Windows senza password, il blocco non serve" },

                // tray messages
                { "NoSignIn.Title", "Il tuo account Windows non ha una password" },
                { "NoSignIn.Text", "UnplugLock può bloccare lo schermo, ma chiunque può rientrare con un clic. Fai clic qui per impostare una password o un PIN." },
                { "Running.Title", "UnplugLock è in esecuzione" },
                { "Running.On", "Il portatile si blocca quando viene scollegato. Fai clic con il pulsante destro sull'icona per le opzioni." },
                { "Running.Off", "UnplugLock è disattivato. Fai clic con il pulsante destro sull'icona per le opzioni." },
                { "LockFailed", "Windows non si è bloccato (errore {0}). Quando si scollega il caricabatterie, UnplugLock riprova per 10 minuti e blocca appena Windows lo consente." },
                { "StartupFailed", "Impossibile cambiare l'impostazione di avvio:\n{0}" },
                { "HideConfirm", "L'icona nell'area di notifica verrà nascosta, ma UnplugLock continua a funzionare e a proteggere il portatile.\n\nPer mostrare di nuovo l'icona, riapri UnplugLock (dal menu Start o da UnplugLock.exe)." },
                { "PasswordRemoved", "Password rimossa." },

                // installer
                { "Setup.Window", "Installazione di UnplugLock" },
                { "Setup.Language", "Lingua" },
                { "Setup.TitleInstall", "Installa UnplugLock" },
                { "Setup.TitleUpdate", "Aggiorna UnplugLock" },
                { "Setup.Guard", "Mantieni UnplugLock in esecuzione" },
                { "Setup.GuardHint", "Se qualcuno lo chiude, si riavvia subito." },
                { "Setup.Startup", "Avvia UnplugLock quando accedo a Windows" },
                { "Setup.Desktop", "Crea un collegamento sul desktop" },
                { "Setup.Launch", "Apri UnplugLock al termine dell'installazione" },
                { "Setup.InstallsTo", "Viene installato in" },
                { "Setup.PerUser", "Solo per il tuo account, quindi non servono diritti di amministratore." },
                { "Setup.Install", "Installa" },
                { "Setup.Update", "Aggiorna" },
                { "Setup.Installing", "Installazione..." },
                { "Setup.Updating", "Aggiornamento..." },
                { "Setup.Finish", "Fine" },
                { "Setup.Failed", "Impossibile completare l'installazione:\n\n{0}" },
                { "Setup.Installed", "UnplugLock è installato" },
                { "Setup.Updated", "UnplugLock è aggiornato" },
                { "Setup.DoneSubtitle", "Ora il portatile si blocca quando si scollega il caricabatterie." },
                { "Setup.DoneRunning", "È in esecuzione nell'area di notifica, accanto all'orologio. Se non vedi l'icona, fai clic sulla freccia ^ lì accanto." },
                { "Setup.DoneNotRunning", "Aprilo dal menu Start quando vuoi. Rimane nell'area di notifica, accanto all'orologio." },
                { "Setup.DoneHint", "Fai clic con il pulsante destro sull'icona per impostare un ritardo, aggiungere una password o disattivarlo per un po'." },
                { "Setup.AuthUpdate", "UnplugLock è protetto da password. Inserisci la password per aggiornarlo." },
                { "Setup.AuthUninstall", "UnplugLock è protetto da password. Inserisci la password per disinstallarlo." },
                { "Setup.StillRunning", "UnplugLock è ancora in esecuzione e non si è chiuso. Chiudilo dal suo menu nell'area di notifica, poi riavvia l'installazione." },
                { "Setup.Damaged", "Questo file di installazione è danneggiato (manca UnplugLock.exe)." },
                { "Setup.ShortcutDescription", "Blocca il portatile quando si scollega il caricabatterie" },
                { "Uninstall.Title", "Disinstalla UnplugLock" },
                { "Uninstall.Confirm", "Rimuovere UnplugLock e tutte le sue impostazioni da questo PC?" },
                { "Uninstall.StillRunning", "UnplugLock è ancora in esecuzione e non si è chiuso. Chiudilo dal suo menu nell'area di notifica, poi riprova a disinstallarlo." },
                { "Uninstall.Done", "UnplugLock è stato disinstallato." },
            };
        }
    }
}
