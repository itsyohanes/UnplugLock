// French.   is the non-breaking space French puts before ? : and inside « ».
using System.Collections.Generic;

namespace UnplugLock
{
    internal static partial class Lang
    {
        private static Dictionary<string, string> French()
        {
            return new Dictionary<string, string>
            {
                { "LanguageName", "Français" },

                // shared
                { "AppTagline", "Verrouille votre ordinateur portable dès que le chargeur est débranché." },
                { "Version", "Version {0}" },
                { "Button.Cancel", "Annuler" },
                { "Button.Continue", "Continuer" },
                { "Button.Close", "Fermer" },
                { "Button.Done", "Terminé" },
                { "WrongPassword", "Mot de passe incorrect." },
                { "AlreadyRunning", "UnplugLock est déjà en cours d'exécution." },

                // password prompt
                { "Pw.TitleEnter", "Entrez votre mot de passe" },
                { "Pw.TitleChoose", "Choisissez un mot de passe" },
                { "Pw.TitleRecovery", "Entrez votre code de récupération" },
                { "Pw.Label", "Mot de passe" },
                { "Pw.LabelNew", "Nouveau mot de passe" },
                { "Pw.LabelRecovery", "Code de récupération" },
                { "Pw.LabelConfirm", "Confirmez le mot de passe" },
                { "Pw.Hint", "Au moins {0} caractères. N'utilisez pas votre mot de passe ou code PIN Windows." },
                { "Pw.Forgot", "Mot de passe oublié ?" },
                { "Pw.Set", "Définir le mot de passe" },
                { "Pw.TooShort", "Utilisez au moins {0} caractères." },
                { "Pw.Mismatch", "Les mots de passe ne correspondent pas." },
                { "Pw.NewPrompt", "Il vous sera demandé pour désactiver UnplugLock, modifier ses paramètres ou le quitter." },
                { "Pw.RecoveryPrompt", "Entrez votre code de récupération. Il supprime votre mot de passe UnplugLock pour que vous puissiez en définir un nouveau." },
                { "Pw.RecoveryWrong", "Ce code de récupération est incorrect." },

                // what the password is asked for
                { "Auth.TurnOff", "Entrez votre mot de passe UnplugLock pour le désactiver." },
                { "Auth.StopEthernet", "Entrez votre mot de passe UnplugLock pour ne plus verrouiller quand le câble Ethernet est débranché." },
                { "Auth.StopWifi", "Entrez votre mot de passe UnplugLock pour ne plus verrouiller quand le Wi-Fi se déconnecte." },
                { "Auth.Delay", "Entrez votre mot de passe UnplugLock pour modifier le délai de verrouillage." },
                { "Auth.Guard", "Entrez votre mot de passe UnplugLock pour désactiver la protection du processus." },
                { "Auth.Startup", "Entrez votre mot de passe UnplugLock pour qu'il ne démarre plus avec Windows." },
                { "Auth.ChangePassword", "Entrez votre mot de passe UnplugLock pour modifier le mot de passe." },
                { "Auth.RemovePassword", "Entrez votre mot de passe UnplugLock pour supprimer le mot de passe." },
                { "Auth.Exit", "Entrez votre mot de passe UnplugLock pour quitter." },
                { "Auth.Recovered", "Code de récupération accepté. Votre mot de passe a été supprimé. Définissez-en un nouveau dans le menu Mot de passe." },

                // recovery code window
                { "Rc.Title", "Enregistrez votre code de récupération" },
                { "Rc.Subtitle", "C'est le seul moyen de revenir si vous oubliez votre mot de passe." },
                { "Rc.Explain", "Si vous oubliez votre mot de passe, cliquez sur « {0} » dans la demande de mot de passe et entrez ce code. Il supprime le mot de passe pour que vous puissiez en définir un nouveau." },
                { "Rc.Keep", "Conservez-le ailleurs que sur cet ordinateur portable : sur votre téléphone, dans un gestionnaire de mots de passe ou sur papier. Il ne sera plus affiché." },
                { "Rc.Copy", "Copier" },
                { "Rc.Copied", "Copié" },
                { "Rc.Save", "Enregistrer dans un fichier..." },
                { "Rc.Confirm", "J'ai enregistré mon code de récupération" },
                { "Rc.FileTitle", "Code de récupération UnplugLock" },
                { "Rc.FileType", "Fichier texte" },
                { "Rc.FileHelp", "Si vous oubliez votre mot de passe UnplugLock, cliquez sur « {0} » dans la demande de mot de passe et entrez ce code." },
                { "Rc.FileCreated", "Créé le {0}." },
                { "Rc.DateFormat", "d MMMM yyyy" },
                { "Rc.SaveFailed", "Impossible d'enregistrer le fichier :\n{0}" },
                { "Rc.SavedOnLaptop", "Enregistré. Ce fichier se trouve sur le disque de l'ordinateur portable : quiconque emporte l'ordinateur pourrait le trouver et utiliser le code pour supprimer votre mot de passe.\n\nCopiez-le sur une clé USB, votre téléphone ou dans un gestionnaire de mots de passe, puis supprimez-le de cet ordinateur." },

                // about window
                { "About.Window", "À propos d'UnplugLock" },
                { "About.Source", "Code source sur GitHub" },
                { "About.Releases", "Rechercher une nouvelle version" },
                { "About.Support", "Soutenir UnplugLock" },
                { "About.License", "Gratuit et open source, licence MIT" },

                // tray menu
                { "Menu.Enabled", "Activé" },
                { "Menu.Delay", "Délai de verrouillage" },
                { "Delay.Instant", "Immédiat" },
                { "Delay.One", "{0} seconde" },
                { "Delay.Other", "{0} secondes" },
                { "Menu.AlsoLock", "Verrouiller aussi quand" },
                { "Menu.Ethernet", "le câble Ethernet est débranché" },
                { "Menu.Wifi", "le Wi-Fi se déconnecte" },
                { "Menu.Startup", "Démarrer avec Windows" },
                { "Menu.StartupGuarded", "Démarrer avec Windows (requis par la protection)" },
                { "Menu.Guard", "Protéger le processus (redémarrage auto)" },
                { "Menu.Hide", "Masquer l'icône de la zone de notification" },
                { "Menu.Password", "Mot de passe" },
                { "Menu.SetPassword", "Définir un mot de passe..." },
                { "Menu.ChangePassword", "Modifier le mot de passe..." },
                { "Menu.RemovePassword", "Supprimer le mot de passe" },
                { "Menu.LockNow", "Verrouiller maintenant" },
                { "Menu.About", "À propos d'UnplugLock..." },
                { "Menu.Exit", "Quitter" },
                { "Menu.NoSignIn", "Windows n'a pas de mot de passe - en définir un..." },
                { "Power.Online", "Sur secteur" },
                { "Power.Offline", "Sur batterie" },
                { "Power.Unknown", "Alimentation inconnue" },

                // tray icon tooltip (63 characters at most)
                { "Tip.On", "UnplugLock - activé ({0})" },
                { "Tip.Off", "UnplugLock - désactivé ({0})" },
                { "Tip.Online", "sur secteur" },
                { "Tip.Offline", "sur batterie" },
                { "Tip.Unknown", "alimentation inconnue" },
                { "Tip.NoSignIn", "UnplugLock - Windows sans mot de passe, verrouiller est inutile" },

                // tray messages
                { "NoSignIn.Title", "Votre compte Windows n'a pas de mot de passe" },
                { "NoSignIn.Text", "UnplugLock peut verrouiller l'écran, mais n'importe qui peut se reconnecter d'un clic. Cliquez ici pour définir un mot de passe ou un code PIN." },
                { "Running.Title", "UnplugLock est en cours d'exécution" },
                { "Running.On", "Votre ordinateur se verrouille quand il est débranché. Cliquez avec le bouton droit sur l'icône pour les options." },
                { "Running.Off", "UnplugLock est désactivé. Cliquez avec le bouton droit sur l'icône pour les options." },
                { "LockFailed", "Windows ne s'est pas verrouillé (erreur {0}). Quand le chargeur est débranché, UnplugLock réessaie pendant 10 minutes et verrouille dès que Windows le permet." },
                { "StartupFailed", "Impossible de modifier le paramètre de démarrage :\n{0}" },
                { "HideConfirm", "L'icône de la zone de notification sera masquée, mais UnplugLock continue de fonctionner et de protéger votre ordinateur.\n\nPour réafficher l'icône, ouvrez de nouveau UnplugLock (depuis le menu Démarrer ou UnplugLock.exe)." },
                { "PasswordRemoved", "Mot de passe supprimé." },

                // installer
                { "Setup.Window", "Installation d'UnplugLock" },
                { "Setup.Language", "Langue" },
                { "Setup.TitleInstall", "Installer UnplugLock" },
                { "Setup.TitleUpdate", "Mettre à jour UnplugLock" },
                { "Setup.Guard", "Garder UnplugLock en marche" },
                { "Setup.GuardHint", "Si quelqu'un le ferme, il redémarre aussitôt." },
                { "Setup.Startup", "Démarrer UnplugLock quand je me connecte à Windows" },
                { "Setup.Desktop", "Créer un raccourci sur le Bureau" },
                { "Setup.Launch", "Ouvrir UnplugLock à la fin de l'installation" },
                { "Setup.InstallsTo", "Emplacement d'installation" },
                { "Setup.PerUser", "Uniquement pour votre compte, sans droits d'administrateur." },
                { "Setup.Install", "Installer" },
                { "Setup.Update", "Mettre à jour" },
                { "Setup.Installing", "Installation..." },
                { "Setup.Updating", "Mise à jour..." },
                { "Setup.Finish", "Terminer" },
                { "Setup.Failed", "L'installation n'a pas pu se terminer :\n\n{0}" },
                { "Setup.Installed", "UnplugLock est installé" },
                { "Setup.Updated", "UnplugLock est à jour" },
                { "Setup.DoneSubtitle", "Votre ordinateur se verrouille maintenant quand le chargeur est débranché." },
                { "Setup.DoneRunning", "Il fonctionne dans la zone de notification, près de l'horloge. Si vous ne voyez pas l'icône, cliquez sur la flèche ^ à cet endroit." },
                { "Setup.DoneNotRunning", "Ouvrez-le depuis le menu Démarrer quand vous voulez. Il se place dans la zone de notification, près de l'horloge." },
                { "Setup.DoneHint", "Cliquez avec le bouton droit sur l'icône pour choisir un délai, ajouter un mot de passe ou le désactiver un moment." },
                { "Setup.AuthUpdate", "UnplugLock est protégé par un mot de passe. Entrez le mot de passe pour le mettre à jour." },
                { "Setup.AuthUninstall", "UnplugLock est protégé par un mot de passe. Entrez le mot de passe pour le désinstaller." },
                { "Setup.StillRunning", "UnplugLock est toujours en cours d'exécution et n'a pas pu être fermé. Quittez-le depuis son menu dans la zone de notification, puis relancez l'installation." },
                { "Setup.Damaged", "Ce fichier d'installation est endommagé (UnplugLock.exe est manquant)." },
                { "Setup.ShortcutDescription", "Verrouille votre ordinateur portable quand le chargeur est débranché" },
                { "Uninstall.Title", "Désinstaller UnplugLock" },
                { "Uninstall.Confirm", "Supprimer UnplugLock et tous ses paramètres de ce PC ?" },
                { "Uninstall.StillRunning", "UnplugLock est toujours en cours d'exécution et n'a pas pu être fermé. Quittez-le depuis son menu dans la zone de notification, puis relancez la désinstallation." },
                { "Uninstall.Done", "UnplugLock a été désinstallé." },
            };
        }
    }
}
