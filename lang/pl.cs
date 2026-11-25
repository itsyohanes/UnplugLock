// Polish
using System.Collections.Generic;

namespace UnplugLock
{
    internal static partial class Lang
    {
        private static Dictionary<string, string> Polish()
        {
            return new Dictionary<string, string>
            {
                { "LanguageName", "Polski" },

                // shared
                { "AppTagline", "Blokuje laptopa, gdy tylko odłączysz ładowarkę." },
                { "Version", "Wersja {0}" },
                { "Button.Cancel", "Anuluj" },
                { "Button.Continue", "Dalej" },
                { "Button.Close", "Zamknij" },
                { "Button.Done", "Gotowe" },
                { "WrongPassword", "Nieprawidłowe hasło." },
                { "AlreadyRunning", "UnplugLock jest już uruchomiony." },

                // password prompt
                { "Pw.TitleEnter", "Wpisz hasło" },
                { "Pw.TitleChoose", "Wybierz hasło" },
                { "Pw.TitleRecovery", "Wpisz kod odzyskiwania" },
                { "Pw.Label", "Hasło" },
                { "Pw.LabelNew", "Nowe hasło" },
                { "Pw.LabelRecovery", "Kod odzyskiwania" },
                { "Pw.LabelConfirm", "Potwierdź hasło" },
                { "Pw.Hint", "Co najmniej {0} znaków. Nie używaj hasła ani kodu PIN z systemu Windows." },
                { "Pw.Forgot", "Nie pamiętasz hasła?" },
                { "Pw.Set", "Ustaw hasło" },
                { "Pw.TooShort", "Użyj co najmniej {0} znaków." },
                { "Pw.Mismatch", "Hasła nie są zgodne." },
                { "Pw.NewPrompt", "Będzie potrzebne, aby wyłączyć UnplugLock, zmienić jego ustawienia lub go zamknąć." },
                { "Pw.RecoveryPrompt", "Wpisz kod odzyskiwania. Usuwa on hasło UnplugLock, aby można było ustawić nowe." },
                { "Pw.RecoveryWrong", "Ten kod odzyskiwania jest nieprawidłowy." },

                // what the password is asked for
                { "Auth.TurnOff", "Wpisz hasło UnplugLock, aby go wyłączyć." },
                { "Auth.StopEthernet", "Wpisz hasło UnplugLock, aby przestać blokować po odłączeniu kabla Ethernet." },
                { "Auth.StopWifi", "Wpisz hasło UnplugLock, aby przestać blokować po rozłączeniu Wi-Fi." },
                { "Auth.Delay", "Wpisz hasło UnplugLock, aby zmienić opóźnienie blokady." },
                { "Auth.Guard", "Wpisz hasło UnplugLock, aby wyłączyć ochronę procesu." },
                { "Auth.Startup", "Wpisz hasło UnplugLock, aby nie uruchamiał się z systemem Windows." },
                { "Auth.ChangePassword", "Wpisz hasło UnplugLock, aby zmienić hasło." },
                { "Auth.RemovePassword", "Wpisz hasło UnplugLock, aby usunąć hasło." },
                { "Auth.Exit", "Wpisz hasło UnplugLock, aby go zamknąć." },
                { "Auth.Recovered", "Kod odzyskiwania zaakceptowany. Hasło zostało usunięte. Ustaw nowe w menu Hasło." },

                // recovery code window
                { "Rc.Title", "Zapisz kod odzyskiwania" },
                { "Rc.Subtitle", "To jedyny sposób, by wrócić, jeśli zapomnisz hasła." },
                { "Rc.Explain", "Jeśli zapomnisz hasła, kliknij „{0}” w oknie hasła i wpisz ten kod. Usunie on hasło, aby można było ustawić nowe." },
                { "Rc.Keep", "Przechowuj go poza tym laptopem, np. w telefonie, menedżerze haseł lub na papierze. Nie zostanie pokazany ponownie." },
                { "Rc.Copy", "Kopiuj" },
                { "Rc.Copied", "Skopiowano" },
                { "Rc.Save", "Zapisz do pliku..." },
                { "Rc.Confirm", "Kod odzyskiwania jest zapisany" },
                { "Rc.FileTitle", "Kod odzyskiwania UnplugLock" },
                { "Rc.FileType", "Plik tekstowy" },
                { "Rc.FileHelp", "Jeśli zapomnisz hasła UnplugLock, kliknij „{0}” w oknie hasła i wpisz ten kod." },
                { "Rc.FileCreated", "Utworzono {0}." },
                { "Rc.DateFormat", "d MMMM yyyy" },
                { "Rc.SaveFailed", "Nie można zapisać pliku:\n{0}" },
                { "Rc.SavedOnLaptop", "Zapisano. Plik znajduje się na dysku laptopa, więc każdy, kto zabierze laptopa, może go znaleźć i użyć kodu, aby usunąć hasło.\n\nSkopiuj go na pendrive, do telefonu lub menedżera haseł, a potem usuń go z tego laptopa." },

                // about window
                { "About.Window", "UnplugLock - informacje" },
                { "About.Source", "Kod źródłowy na GitHubie" },
                { "About.Releases", "Sprawdź nowe wersje" },
                { "About.Support", "Wesprzyj UnplugLock" },
                { "About.License", "Darmowy i otwartoźródłowy, licencja MIT" },

                // tray menu
                { "Menu.Enabled", "Włączony" },
                { "Menu.Delay", "Opóźnienie blokady" },
                { "Delay.Instant", "Natychmiast" },
                { "Delay.One", "{0} sekunda" },
                { "Delay.Few", "{0} sekundy" },
                { "Delay.Other", "{0} sekund" },
                { "Menu.AlsoLock", "Blokuj też, gdy" },
                { "Menu.Ethernet", "kabel Ethernet zostanie odłączony" },
                { "Menu.Wifi", "Wi-Fi się rozłączy" },
                { "Menu.Startup", "Uruchamiaj z systemem Windows" },
                { "Menu.StartupGuarded", "Uruchamiaj z systemem Windows (wymagane przez ochronę)" },
                { "Menu.Guard", "Ochrona procesu (automatyczny restart)" },
                { "Menu.Hide", "Ukryj ikonę w zasobniku" },
                { "Menu.Password", "Hasło" },
                { "Menu.SetPassword", "Ustaw hasło..." },
                { "Menu.ChangePassword", "Zmień hasło..." },
                { "Menu.RemovePassword", "Usuń hasło" },
                { "Menu.LockNow", "Zablokuj teraz" },
                { "Menu.About", "UnplugLock - informacje..." },
                { "Menu.Exit", "Zakończ" },
                { "Menu.NoSignIn", "Windows nie ma hasła - ustaw je..." },
                { "Power.Online", "Podłączony do zasilania" },
                { "Power.Offline", "Na baterii" },
                { "Power.Unknown", "Nieznany stan zasilania" },

                // tray icon tooltip (63 characters at most)
                { "Tip.On", "UnplugLock - włączony ({0})" },
                { "Tip.Off", "UnplugLock - wyłączony ({0})" },
                { "Tip.Online", "zasilanie sieciowe" },
                { "Tip.Offline", "na baterii" },
                { "Tip.Unknown", "nieznany stan zasilania" },
                { "Tip.NoSignIn", "UnplugLock - Windows nie ma hasła, blokada nic nie da" },

                // tray messages
                { "NoSignIn.Title", "Twoje konto Windows nie ma hasła" },
                { "NoSignIn.Text", "UnplugLock może zablokować ekran, ale każdy może wejść z powrotem jednym kliknięciem. Kliknij tutaj, aby ustawić hasło lub kod PIN." },
                { "Running.Title", "UnplugLock działa" },
                { "Running.On", "Laptop zablokuje się po odłączeniu zasilania. Kliknij ikonę prawym przyciskiem myszy, aby zobaczyć opcje." },
                { "Running.Off", "UnplugLock jest wyłączony. Kliknij ikonę prawym przyciskiem myszy, aby zobaczyć opcje." },
                { "LockFailed", "Windows się nie zablokował (błąd {0}). Po odłączeniu ładowarki UnplugLock próbuje przez 10 minut i blokuje, gdy tylko Windows na to pozwoli." },
                { "StartupFailed", "Nie można zmienić ustawienia autostartu:\n{0}" },
                { "HideConfirm", "Ikona w zasobniku zostanie ukryta, ale UnplugLock nadal działa i chroni laptopa.\n\nAby znów pokazać ikonę, otwórz ponownie UnplugLock (z menu Start lub UnplugLock.exe)." },
                { "PasswordRemoved", "Hasło usunięte." },

                // installer
                { "Setup.Window", "Instalator UnplugLock" },
                { "Setup.Language", "Język" },
                { "Setup.TitleInstall", "Zainstaluj UnplugLock" },
                { "Setup.TitleUpdate", "Zaktualizuj UnplugLock" },
                { "Setup.Guard", "Utrzymuj UnplugLock uruchomiony" },
                { "Setup.GuardHint", "Jeśli ktoś go zamknie, od razu uruchomi się ponownie." },
                { "Setup.Startup", "Uruchamiaj UnplugLock po zalogowaniu do Windows" },
                { "Setup.Desktop", "Utwórz skrót na pulpicie" },
                { "Setup.Launch", "Otwórz UnplugLock po zakończeniu instalacji" },
                { "Setup.InstallsTo", "Miejsce instalacji" },
                { "Setup.PerUser", "Tylko dla Twojego konta, więc nie są potrzebne uprawnienia administratora." },
                { "Setup.Install", "Zainstaluj" },
                { "Setup.Update", "Zaktualizuj" },
                { "Setup.Installing", "Instalowanie..." },
                { "Setup.Updating", "Aktualizowanie..." },
                { "Setup.Finish", "Zakończ" },
                { "Setup.Failed", "Nie udało się ukończyć instalacji:\n\n{0}" },
                { "Setup.Installed", "UnplugLock został zainstalowany" },
                { "Setup.Updated", "UnplugLock został zaktualizowany" },
                { "Setup.DoneSubtitle", "Teraz laptop zablokuje się po odłączeniu ładowarki." },
                { "Setup.DoneRunning", "Działa w zasobniku systemowym obok zegara. Jeśli nie widzisz ikony, kliknij tam strzałkę ^." },
                { "Setup.DoneNotRunning", "Otwórz go z menu Start, kiedy zechcesz. Będzie działać w zasobniku systemowym obok zegara." },
                { "Setup.DoneHint", "Kliknij ikonę prawym przyciskiem myszy, aby ustawić opóźnienie, dodać hasło lub na chwilę go wyłączyć." },
                { "Setup.AuthUpdate", "UnplugLock jest chroniony hasłem. Wpisz hasło, aby go zaktualizować." },
                { "Setup.AuthUninstall", "UnplugLock jest chroniony hasłem. Wpisz hasło, aby go odinstalować." },
                { "Setup.StillRunning", "UnplugLock nadal działa i nie dał się zamknąć. Zamknij go z menu w zasobniku, a potem uruchom instalator ponownie." },
                { "Setup.Damaged", "Ten plik instalatora jest uszkodzony (brakuje w nim UnplugLock.exe)." },
                { "Setup.ShortcutDescription", "Blokuje laptopa po odłączeniu ładowarki" },
                { "Uninstall.Title", "Odinstaluj UnplugLock" },
                { "Uninstall.Confirm", "Usunąć UnplugLock i wszystkie jego ustawienia z tego komputera?" },
                { "Uninstall.StillRunning", "UnplugLock nadal działa i nie dał się zamknąć. Zamknij go z menu w zasobniku, a potem spróbuj odinstalować ponownie." },
                { "Uninstall.Done", "UnplugLock został odinstalowany." },
            };
        }
    }
}
