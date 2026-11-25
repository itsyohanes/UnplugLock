// Turkish
using System.Collections.Generic;

namespace UnplugLock
{
    internal static partial class Lang
    {
        private static Dictionary<string, string> Turkish()
        {
            return new Dictionary<string, string>
            {
                { "LanguageName", "Türkçe" },

                // shared
                { "AppTagline", "Şarj cihazı çıkarıldığı anda dizüstü bilgisayarınızı kilitler." },
                { "Version", "Sürüm {0}" },
                { "Button.Cancel", "İptal" },
                { "Button.Continue", "Devam" },
                { "Button.Close", "Kapat" },
                { "Button.Done", "Tamam" },
                { "WrongPassword", "Parola yanlış." },
                { "AlreadyRunning", "UnplugLock zaten çalışıyor." },

                // password prompt
                { "Pw.TitleEnter", "Parolanızı girin" },
                { "Pw.TitleChoose", "Bir parola seçin" },
                { "Pw.TitleRecovery", "Kurtarma kodunuzu girin" },
                { "Pw.Label", "Parola" },
                { "Pw.LabelNew", "Yeni parola" },
                { "Pw.LabelRecovery", "Kurtarma kodu" },
                { "Pw.LabelConfirm", "Parolayı onaylayın" },
                { "Pw.Hint", "En az {0} karakter. Windows parolanızı veya PIN'inizi yeniden kullanmayın." },
                { "Pw.Forgot", "Parolanızı mı unuttunuz?" },
                { "Pw.Set", "Parolayı ayarla" },
                { "Pw.TooShort", "En az {0} karakter kullanın." },
                { "Pw.Mismatch", "Parolalar eşleşmiyor." },
                { "Pw.NewPrompt", "UnplugLock'u kapatmak, ayarlarını değiştirmek veya ondan çıkmak için bu parola gerekecek." },
                { "Pw.RecoveryPrompt", "Kurtarma kodunuzu girin. Kod, UnplugLock parolanızı kaldırır; böylece yeni bir parola belirleyebilirsiniz." },
                { "Pw.RecoveryWrong", "Bu kurtarma kodu doğru değil." },

                // what the password is asked for
                { "Auth.TurnOff", "UnplugLock'u kapatmak için UnplugLock parolanızı girin." },
                { "Auth.StopEthernet", "Ethernet kablosu çıkarıldığında kilitlemeyi durdurmak için UnplugLock parolanızı girin." },
                { "Auth.StopWifi", "Wi-Fi bağlantısı kesildiğinde kilitlemeyi durdurmak için UnplugLock parolanızı girin." },
                { "Auth.Delay", "Kilitleme gecikmesini değiştirmek için UnplugLock parolanızı girin." },
                { "Auth.Guard", "İşlem korumasını kapatmak için UnplugLock parolanızı girin." },
                { "Auth.Startup", "UnplugLock'un Windows ile başlamasını durdurmak için UnplugLock parolanızı girin." },
                { "Auth.ChangePassword", "Parolayı değiştirmek için UnplugLock parolanızı girin." },
                { "Auth.RemovePassword", "Parolayı kaldırmak için UnplugLock parolanızı girin." },
                { "Auth.Exit", "Çıkmak için UnplugLock parolanızı girin." },
                { "Auth.Recovered", "Kurtarma kodu kabul edildi. Parolanız kaldırıldı. Parola menüsünden yeni bir parola belirleyin." },

                // recovery code window
                { "Rc.Title", "Kurtarma kodunuzu kaydedin" },
                { "Rc.Subtitle", "Parolanızı unutursanız yeniden girmenin tek yolu budur." },
                { "Rc.Explain", "Parolanızı unutursanız parola penceresinde \"{0}\" bağlantısına tıklayın ve bu kodu girin. Kod parolayı kaldırır; böylece yeni bir parola belirleyebilirsiniz." },
                { "Rc.Keep", "Kodu bu dizüstü bilgisayarın dışında bir yerde saklayın: telefonunuzda, bir parola yöneticisinde ya da kâğıtta. Kod bir daha gösterilmeyecek." },
                { "Rc.Copy", "Kopyala" },
                { "Rc.Copied", "Kopyalandı" },
                { "Rc.Save", "Dosyaya kaydet..." },
                { "Rc.Confirm", "Kurtarma kodumu kaydettim" },
                { "Rc.FileTitle", "UnplugLock kurtarma kodu" },
                { "Rc.FileType", "Metin dosyası" },
                { "Rc.FileHelp", "UnplugLock parolanızı unutursanız parola penceresinde \"{0}\" bağlantısına tıklayın ve bu kodu girin." },
                { "Rc.FileCreated", "Oluşturulma tarihi: {0}." },
                { "Rc.DateFormat", "d MMMM yyyy" },
                { "Rc.SaveFailed", "Dosya kaydedilemedi:\n{0}" },
                { "Rc.SavedOnLaptop", "Kaydedildi. Bu dosya dizüstü bilgisayarın kendi diskinde; bilgisayarı alan biri dosyayı bulup kodla parolanızı kaldırabilir.\n\nDosyayı bir USB belleğe, telefonunuza veya bir parola yöneticisine kopyalayın, sonra bu dizüstü bilgisayardan silin." },

                // about window
                { "About.Window", "UnplugLock hakkında" },
                { "About.Source", "GitHub'da kaynak kodu" },
                { "About.Releases", "Yeni sürümleri denetle" },
                { "About.Support", "UnplugLock'u destekle" },
                { "About.License", "Ücretsiz ve açık kaynak, MIT Lisansı" },

                // tray menu
                { "Menu.Enabled", "Etkin" },
                { "Menu.Delay", "Kilitleme gecikmesi" },
                { "Delay.Instant", "Hemen" },
                { "Delay.One", "{0} saniye" },
                { "Delay.Other", "{0} saniye" },
                { "Menu.AlsoLock", "Şu durumlarda da kilitle" },
                { "Menu.Ethernet", "Ethernet kablosu çıkarıldığında" },
                { "Menu.Wifi", "Wi-Fi bağlantısı kesildiğinde" },
                { "Menu.Startup", "Windows ile başlat" },
                { "Menu.StartupGuarded", "Windows ile başlat (koruma için gerekli)" },
                { "Menu.Guard", "İşlemi koru (otomatik yeniden başlat)" },
                { "Menu.Hide", "Tepsi simgesini gizle" },
                { "Menu.Password", "Parola" },
                { "Menu.SetPassword", "Parola ayarla..." },
                { "Menu.ChangePassword", "Parolayı değiştir..." },
                { "Menu.RemovePassword", "Parolayı kaldır" },
                { "Menu.LockNow", "Şimdi kilitle" },
                { "Menu.About", "UnplugLock hakkında..." },
                { "Menu.Exit", "Çıkış" },
                { "Menu.NoSignIn", "Windows'un parolası yok - bir parola ayarlayın..." },
                { "Power.Online", "Prize takılı" },
                { "Power.Offline", "Pil gücünde" },
                { "Power.Unknown", "Güç durumu bilinmiyor" },

                // tray icon tooltip (63 characters at most)
                { "Tip.On", "UnplugLock - açık ({0})" },
                { "Tip.Off", "UnplugLock - kapalı ({0})" },
                { "Tip.Online", "prize takılı" },
                { "Tip.Offline", "pil gücünde" },
                { "Tip.Unknown", "güç durumu bilinmiyor" },
                { "Tip.NoSignIn", "UnplugLock - Windows'un parolası yok, kilitlemek işe yaramaz" },

                // tray messages
                { "NoSignIn.Title", "Windows hesabınızın parolası yok" },
                { "NoSignIn.Text", "UnplugLock ekranı kilitleyebilir, ancak herkes tek tıklamayla yeniden girebilir. Parola veya PIN ayarlamak için buraya tıklayın." },
                { "Running.Title", "UnplugLock çalışıyor" },
                { "Running.On", "Dizüstü bilgisayarınız fişten çekildiğinde kilitlenir. Seçenekler için simgeye sağ tıklayın." },
                { "Running.Off", "UnplugLock kapalı. Seçenekler için simgeye sağ tıklayın." },
                { "LockFailed", "Windows kilitlenmedi (hata {0}). Şarj cihazı çekildiğinde UnplugLock 10 dakika boyunca denemeye devam eder ve Windows izin verir vermez kilitler." },
                { "StartupFailed", "Başlangıç ayarı değiştirilemedi:\n{0}" },
                { "HideConfirm", "Tepsi simgesi gizlenecek, ancak UnplugLock çalışmaya ve dizüstü bilgisayarınızı korumaya devam eder.\n\nSimgeyi yeniden göstermek için UnplugLock'u tekrar açın (Başlat menüsünden veya UnplugLock.exe ile)." },
                { "PasswordRemoved", "Parola kaldırıldı." },

                // installer
                { "Setup.Window", "UnplugLock Kurulumu" },
                { "Setup.Language", "Dil" },
                { "Setup.TitleInstall", "UnplugLock'u yükle" },
                { "Setup.TitleUpdate", "UnplugLock'u güncelle" },
                { "Setup.Guard", "UnplugLock çalışmaya devam etsin" },
                { "Setup.GuardHint", "Biri kapatırsa hemen kendini yeniden başlatır." },
                { "Setup.Startup", "Windows'ta oturum açtığımda UnplugLock'u başlat" },
                { "Setup.Desktop", "Masaüstü kısayolu oluştur" },
                { "Setup.Launch", "Kurulum bitince UnplugLock'u aç" },
                { "Setup.InstallsTo", "Yükleneceği yer" },
                { "Setup.PerUser", "Yalnızca sizin hesabınız için, bu yüzden yönetici izni gerekmez." },
                { "Setup.Install", "Yükle" },
                { "Setup.Update", "Güncelle" },
                { "Setup.Installing", "Yükleniyor..." },
                { "Setup.Updating", "Güncelleniyor..." },
                { "Setup.Finish", "Bitir" },
                { "Setup.Failed", "Kurulum tamamlanamadı:\n\n{0}" },
                { "Setup.Installed", "UnplugLock yüklendi" },
                { "Setup.Updated", "UnplugLock güncellendi" },
                { "Setup.DoneSubtitle", "Dizüstü bilgisayarınız artık şarj cihazı çekildiğinde kilitlenir." },
                { "Setup.DoneRunning", "Saatin yanındaki sistem tepsisinde çalışıyor. Simgeyi göremiyorsanız oradaki ^ okuna tıklayın." },
                { "Setup.DoneNotRunning", "Hazır olduğunuzda Başlat menüsünden açın. Saatin yanındaki sistem tepsisinde durur." },
                { "Setup.DoneHint", "Kilitleme gecikmesi ayarlamak, parola eklemek veya bir süreliğine kapatmak için simgeye sağ tıklayın." },
                { "Setup.AuthUpdate", "UnplugLock parola korumalı. Güncellemek için parolayı girin." },
                { "Setup.AuthUninstall", "UnplugLock parola korumalı. Kaldırmak için parolayı girin." },
                { "Setup.StillRunning", "UnplugLock hâlâ çalışıyor ve kapanmadı. Tepsi menüsünden çıkın, sonra kurulumu yeniden çalıştırın." },
                { "Setup.Damaged", "Bu kurulum dosyası bozuk (içinde UnplugLock.exe yok)." },
                { "Setup.ShortcutDescription", "Şarj cihazı çıkarıldığında dizüstü bilgisayarınızı kilitler" },
                { "Uninstall.Title", "UnplugLock'u kaldır" },
                { "Uninstall.Confirm", "UnplugLock ve tüm ayarları bu bilgisayardan kaldırılsın mı?" },
                { "Uninstall.StillRunning", "UnplugLock hâlâ çalışıyor ve kapanmadı. Tepsi menüsünden çıkın, sonra yeniden kaldırmayı deneyin." },
                { "Uninstall.Done", "UnplugLock kaldırıldı." },
            };
        }
    }
}
