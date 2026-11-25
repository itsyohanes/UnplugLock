// Czech
using System.Collections.Generic;

namespace UnplugLock
{
    internal static partial class Lang
    {
        private static Dictionary<string, string> Czech()
        {
            return new Dictionary<string, string>
            {
                { "LanguageName", "Čeština" },

                // shared
                { "AppTagline", "Zamkne notebook ve chvíli, kdy odpojíte nabíječku." },
                { "Version", "Verze {0}" },
                { "Button.Cancel", "Zrušit" },
                { "Button.Continue", "Pokračovat" },
                { "Button.Close", "Zavřít" },
                { "Button.Done", "Hotovo" },
                { "WrongPassword", "Nesprávné heslo." },
                { "AlreadyRunning", "UnplugLock už je spuštěný." },

                // password prompt
                { "Pw.TitleEnter", "Zadejte heslo" },
                { "Pw.TitleChoose", "Zvolte heslo" },
                { "Pw.TitleRecovery", "Zadejte kód pro obnovení" },
                { "Pw.Label", "Heslo" },
                { "Pw.LabelNew", "Nové heslo" },
                { "Pw.LabelRecovery", "Kód pro obnovení" },
                { "Pw.LabelConfirm", "Potvrďte heslo" },
                { "Pw.Hint", "Alespoň {0} znaků. Nepoužívejte heslo ani PIN k Windows." },
                { "Pw.Forgot", "Zapomněli jste heslo?" },
                { "Pw.Set", "Nastavit heslo" },
                { "Pw.TooShort", "Použijte alespoň {0} znaků." },
                { "Pw.Mismatch", "Hesla se neshodují." },
                { "Pw.NewPrompt", "Budete ho potřebovat k vypnutí UnplugLocku, ke změně jeho nastavení nebo k jeho ukončení." },
                { "Pw.RecoveryPrompt", "Zadejte kód pro obnovení. Odstraní heslo k UnplugLocku, abyste mohli nastavit nové." },
                { "Pw.RecoveryWrong", "Tento kód pro obnovení není správný." },

                // what the password is asked for
                { "Auth.TurnOff", "Zadejte heslo k UnplugLocku, abyste ho vypnuli." },
                { "Auth.StopEthernet", "Zadejte heslo k UnplugLocku, aby se přestalo zamykat při odpojení ethernetového kabelu." },
                { "Auth.StopWifi", "Zadejte heslo k UnplugLocku, aby se přestalo zamykat při odpojení Wi-Fi." },
                { "Auth.Delay", "Zadejte heslo k UnplugLocku, abyste změnili zpoždění zamknutí." },
                { "Auth.Guard", "Zadejte heslo k UnplugLocku, abyste vypnuli ochranu procesu." },
                { "Auth.Startup", "Zadejte heslo k UnplugLocku, aby se přestal spouštět s Windows." },
                { "Auth.ChangePassword", "Zadejte heslo k UnplugLocku, abyste změnili heslo." },
                { "Auth.RemovePassword", "Zadejte heslo k UnplugLocku, abyste odstranili heslo." },
                { "Auth.Exit", "Zadejte heslo k UnplugLocku, abyste ho ukončili." },
                { "Auth.Recovered", "Kód pro obnovení byl přijat. Heslo bylo odstraněno. Nové nastavíte v nabídce Heslo." },

                // recovery code window
                { "Rc.Title", "Uložte si kód pro obnovení" },
                { "Rc.Subtitle", "Je to jediný způsob, jak se dostat zpět, když zapomenete heslo." },
                { "Rc.Explain", "Když zapomenete heslo, klikněte v okně hesla na „{0}“ a zadejte tento kód. Odstraní heslo, abyste mohli nastavit nové." },
                { "Rc.Keep", "Uchovejte ho jinde než na tomto notebooku, například v telefonu, ve správci hesel nebo na papíře. Znovu se už nezobrazí." },
                { "Rc.Copy", "Kopírovat" },
                { "Rc.Copied", "Zkopírováno" },
                { "Rc.Save", "Uložit do souboru..." },
                { "Rc.Confirm", "Kód pro obnovení mám uložený" },
                { "Rc.FileTitle", "Kód pro obnovení UnplugLock" },
                { "Rc.FileType", "Textový soubor" },
                { "Rc.FileHelp", "Když zapomenete heslo k UnplugLocku, klikněte v okně hesla na „{0}“ a zadejte tento kód." },
                { "Rc.FileCreated", "Vytvořeno {0}." },
                { "Rc.DateFormat", "d. MMMM yyyy" },
                { "Rc.SaveFailed", "Soubor se nepodařilo uložit:\n{0}" },
                { "Rc.SavedOnLaptop", "Uloženo. Soubor je na disku notebooku, takže kdokoli, kdo notebook vezme, ho může najít a pomocí kódu odstranit vaše heslo.\n\nZkopírujte ho na flash disk, do telefonu nebo do správce hesel a pak ho z tohoto notebooku smažte." },

                // about window
                { "About.Window", "O aplikaci UnplugLock" },
                { "About.Source", "Zdrojový kód na GitHubu" },
                { "About.Releases", "Zkontrolovat nové verze" },
                { "About.Support", "Podpořit UnplugLock" },
                { "About.License", "Zdarma a open source, licence MIT" },

                // tray menu
                { "Menu.Enabled", "Zapnuto" },
                { "Menu.Delay", "Zpoždění zamknutí" },
                { "Delay.Instant", "Okamžitě" },
                { "Delay.One", "{0} sekunda" },
                { "Delay.Few", "{0} sekundy" },
                { "Delay.Other", "{0} sekund" },
                { "Menu.AlsoLock", "Zamknout také, když" },
                { "Menu.Ethernet", "se odpojí ethernetový kabel" },
                { "Menu.Wifi", "se odpojí Wi-Fi" },
                { "Menu.Startup", "Spouštět s Windows" },
                { "Menu.StartupGuarded", "Spouštět s Windows (vyžaduje ochrana)" },
                { "Menu.Guard", "Ochrana procesu (automatický restart)" },
                { "Menu.Hide", "Skrýt ikonu v oznamovací oblasti" },
                { "Menu.Password", "Heslo" },
                { "Menu.SetPassword", "Nastavit heslo..." },
                { "Menu.ChangePassword", "Změnit heslo..." },
                { "Menu.RemovePassword", "Odstranit heslo" },
                { "Menu.LockNow", "Zamknout teď" },
                { "Menu.About", "O aplikaci UnplugLock..." },
                { "Menu.Exit", "Ukončit" },
                { "Menu.NoSignIn", "Windows nemají heslo - nastavit..." },
                { "Power.Online", "Napájení ze sítě" },
                { "Power.Offline", "Napájení z baterie" },
                { "Power.Unknown", "Neznámý stav napájení" },

                // tray icon tooltip (63 characters at most)
                { "Tip.On", "UnplugLock - zapnuto ({0})" },
                { "Tip.Off", "UnplugLock - vypnuto ({0})" },
                { "Tip.Online", "ze sítě" },
                { "Tip.Offline", "z baterie" },
                { "Tip.Unknown", "neznámé napájení" },
                { "Tip.NoSignIn", "UnplugLock - Windows nemají heslo, zamykání nepomůže" },

                // tray messages
                { "NoSignIn.Title", "Váš účet Windows nemá heslo" },
                { "NoSignIn.Text", "UnplugLock umí zamknout obrazovku, ale kdokoli se jedním kliknutím dostane zpět. Klikněte sem a nastavte heslo nebo PIN." },
                { "Running.Title", "UnplugLock běží" },
                { "Running.On", "Notebook se po odpojení zamkne. Možnosti najdete po kliknutí pravým tlačítkem na ikonu." },
                { "Running.Off", "UnplugLock je vypnutý. Možnosti najdete po kliknutí pravým tlačítkem na ikonu." },
                { "LockFailed", "Windows se nezamkly (chyba {0}). Po odpojení nabíječky to UnplugLock zkouší ještě 10 minut a zamkne, jakmile to Windows dovolí." },
                { "StartupFailed", "Nastavení spouštění se nepodařilo změnit:\n{0}" },
                { "HideConfirm", "Ikona v oznamovací oblasti se skryje, ale UnplugLock dál poběží a bude notebook chránit.\n\nIkonu znovu zobrazíte tak, že UnplugLock znovu otevřete (z nabídky Start nebo UnplugLock.exe)." },
                { "PasswordRemoved", "Heslo bylo odstraněno." },

                // installer
                { "Setup.Window", "Instalace UnplugLock" },
                { "Setup.Language", "Jazyk" },
                { "Setup.TitleInstall", "Nainstalovat UnplugLock" },
                { "Setup.TitleUpdate", "Aktualizovat UnplugLock" },
                { "Setup.Guard", "Udržovat UnplugLock spuštěný" },
                { "Setup.GuardHint", "Když ho někdo zavře, hned se znovu spustí." },
                { "Setup.Startup", "Spustit UnplugLock po přihlášení do Windows" },
                { "Setup.Desktop", "Vytvořit zástupce na ploše" },
                { "Setup.Launch", "Po dokončení instalace otevřít UnplugLock" },
                { "Setup.InstallsTo", "Instaluje se do" },
                { "Setup.PerUser", "Jen pro váš účet, takže nepotřebuje oprávnění správce." },
                { "Setup.Install", "Nainstalovat" },
                { "Setup.Update", "Aktualizovat" },
                { "Setup.Installing", "Instalace..." },
                { "Setup.Updating", "Aktualizace..." },
                { "Setup.Finish", "Dokončit" },
                { "Setup.Failed", "Instalaci se nepodařilo dokončit:\n\n{0}" },
                { "Setup.Installed", "UnplugLock je nainstalovaný" },
                { "Setup.Updated", "UnplugLock je aktualizovaný" },
                { "Setup.DoneSubtitle", "Notebook se teď zamkne, když odpojíte nabíječku." },
                { "Setup.DoneRunning", "Běží v oznamovací oblasti vedle hodin. Pokud ikonu nevidíte, klikněte tam na šipku ^." },
                { "Setup.DoneNotRunning", "Otevřete ho z nabídky Start, kdykoli budete chtít. Poběží v oznamovací oblasti vedle hodin." },
                { "Setup.DoneHint", "Kliknutím pravým tlačítkem na ikonu nastavíte zpoždění, přidáte heslo nebo ho na chvíli vypnete." },
                { "Setup.AuthUpdate", "UnplugLock je chráněný heslem. Zadejte heslo, abyste ho aktualizovali." },
                { "Setup.AuthUninstall", "UnplugLock je chráněný heslem. Zadejte heslo, abyste ho odinstalovali." },
                { "Setup.StillRunning", "UnplugLock pořád běží a nepodařilo se ho zavřít. Ukončete ho z jeho nabídky v oznamovací oblasti a spusťte instalaci znovu." },
                { "Setup.Damaged", "Tento instalační soubor je poškozený (chybí v něm UnplugLock.exe)." },
                { "Setup.ShortcutDescription", "Zamkne notebook po odpojení nabíječky" },
                { "Uninstall.Title", "Odinstalovat UnplugLock" },
                { "Uninstall.Confirm", "Odebrat UnplugLock a všechna jeho nastavení z tohoto počítače?" },
                { "Uninstall.StillRunning", "UnplugLock pořád běží a nepodařilo se ho zavřít. Ukončete ho z jeho nabídky v oznamovací oblasti a odinstalujte ho znovu." },
                { "Uninstall.Done", "UnplugLock byl odinstalován." },
            };
        }
    }
}
