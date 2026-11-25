// Indonesian
using System.Collections.Generic;

namespace UnplugLock
{
    internal static partial class Lang
    {
        private static Dictionary<string, string> Indonesian()
        {
            return new Dictionary<string, string>
            {
                { "LanguageName", "Bahasa Indonesia" },

                // shared
                { "AppTagline", "Mengunci laptop Anda begitu pengisi daya dicabut." },
                { "Version", "Versi {0}" },
                { "Button.Cancel", "Batal" },
                { "Button.Continue", "Lanjutkan" },
                { "Button.Close", "Tutup" },
                { "Button.Done", "Selesai" },
                { "WrongPassword", "Kata sandi salah." },
                { "AlreadyRunning", "UnplugLock sudah berjalan." },

                // password prompt
                { "Pw.TitleEnter", "Masukkan kata sandi" },
                { "Pw.TitleChoose", "Pilih kata sandi" },
                { "Pw.TitleRecovery", "Masukkan kode pemulihan" },
                { "Pw.Label", "Kata sandi" },
                { "Pw.LabelNew", "Kata sandi baru" },
                { "Pw.LabelRecovery", "Kode pemulihan" },
                { "Pw.LabelConfirm", "Konfirmasi kata sandi" },
                { "Pw.Hint", "Minimal {0} karakter. Jangan pakai ulang kata sandi atau PIN Windows Anda." },
                { "Pw.Forgot", "Lupa kata sandi?" },
                { "Pw.Set", "Atur kata sandi" },
                { "Pw.TooShort", "Gunakan minimal {0} karakter." },
                { "Pw.Mismatch", "Kata sandi tidak cocok." },
                { "Pw.NewPrompt", "Anda akan memerlukannya untuk mematikan UnplugLock, mengubah pengaturannya, atau keluar darinya." },
                { "Pw.RecoveryPrompt", "Masukkan kode pemulihan Anda. Kode ini menghapus kata sandi UnplugLock agar Anda bisa mengatur yang baru." },
                { "Pw.RecoveryWrong", "Kode pemulihan itu salah." },

                // what the password is asked for
                { "Auth.TurnOff", "Masukkan kata sandi UnplugLock untuk mematikannya." },
                { "Auth.StopEthernet", "Masukkan kata sandi UnplugLock untuk berhenti mengunci saat kabel Ethernet dicabut." },
                { "Auth.StopWifi", "Masukkan kata sandi UnplugLock untuk berhenti mengunci saat Wi-Fi terputus." },
                { "Auth.Delay", "Masukkan kata sandi UnplugLock untuk mengubah jeda penguncian." },
                { "Auth.Guard", "Masukkan kata sandi UnplugLock untuk mematikan pelindung proses." },
                { "Auth.Startup", "Masukkan kata sandi UnplugLock agar tidak lagi berjalan saat Windows dimulai." },
                { "Auth.ChangePassword", "Masukkan kata sandi UnplugLock untuk mengubah kata sandi." },
                { "Auth.RemovePassword", "Masukkan kata sandi UnplugLock untuk menghapus kata sandi." },
                { "Auth.Exit", "Masukkan kata sandi UnplugLock untuk keluar." },
                { "Auth.Recovered", "Kode pemulihan diterima. Kata sandi Anda telah dihapus. Atur yang baru dari menu Kata sandi." },

                // recovery code window
                { "Rc.Title", "Simpan kode pemulihan Anda" },
                { "Rc.Subtitle", "Ini satu-satunya cara masuk kembali jika Anda lupa kata sandi." },
                { "Rc.Explain", "Jika lupa kata sandi, klik \"{0}\" pada permintaan kata sandi dan masukkan kode ini. Kode ini menghapus kata sandi agar Anda bisa mengatur yang baru." },
                { "Rc.Keep", "Simpan di tempat selain laptop ini, misalnya di ponsel, pengelola kata sandi, atau di kertas. Kode ini tidak akan ditampilkan lagi." },
                { "Rc.Copy", "Salin" },
                { "Rc.Copied", "Tersalin" },
                { "Rc.Save", "Simpan ke file..." },
                { "Rc.Confirm", "Saya sudah menyimpan kode pemulihan" },
                { "Rc.FileTitle", "Kode pemulihan UnplugLock" },
                { "Rc.FileType", "File teks" },
                { "Rc.FileHelp", "Jika lupa kata sandi UnplugLock, klik \"{0}\" pada permintaan kata sandi dan masukkan kode ini." },
                { "Rc.FileCreated", "Dibuat {0}." },
                { "Rc.DateFormat", "d MMMM yyyy" },
                { "Rc.SaveFailed", "Tidak dapat menyimpan file:\n{0}" },
                { "Rc.SavedOnLaptop", "Tersimpan. File ini ada di drive laptop sendiri, jadi siapa pun yang membawa laptop ini bisa menemukannya dan memakai kodenya untuk menghapus kata sandi Anda.\n\nSalin ke flashdisk, ponsel, atau pengelola kata sandi, lalu hapus dari laptop ini." },

                // about window
                { "About.Window", "Tentang UnplugLock" },
                { "About.Source", "Kode sumber di GitHub" },
                { "About.Releases", "Periksa versi baru" },
                { "About.Support", "Dukung UnplugLock" },
                { "About.License", "Gratis dan sumber terbuka, Lisensi MIT" },

                // tray menu
                { "Menu.Enabled", "Aktif" },
                { "Menu.Delay", "Jeda penguncian" },
                { "Delay.Instant", "Langsung" },
                { "Delay.One", "{0} detik" },
                { "Delay.Other", "{0} detik" },
                { "Menu.AlsoLock", "Kunci juga saat" },
                { "Menu.Ethernet", "Kabel Ethernet dicabut" },
                { "Menu.Wifi", "Wi-Fi terputus" },
                { "Menu.Startup", "Mulai bersama Windows" },
                { "Menu.StartupGuarded", "Mulai bersama Windows (wajib untuk pelindung)" },
                { "Menu.Guard", "Pelindung proses (mulai ulang otomatis)" },
                { "Menu.Hide", "Sembunyikan ikon baki" },
                { "Menu.Password", "Kata sandi" },
                { "Menu.SetPassword", "Atur kata sandi..." },
                { "Menu.ChangePassword", "Ubah kata sandi..." },
                { "Menu.RemovePassword", "Hapus kata sandi" },
                { "Menu.LockNow", "Kunci sekarang" },
                { "Menu.About", "Tentang UnplugLock..." },
                { "Menu.Exit", "Keluar" },
                { "Menu.NoSignIn", "Windows tidak punya kata sandi - atur sekarang..." },
                { "Power.Online", "Tersambung ke listrik" },
                { "Power.Offline", "Memakai baterai" },
                { "Power.Unknown", "Status daya tidak diketahui" },

                // tray icon tooltip (63 characters at most)
                { "Tip.On", "UnplugLock - aktif ({0})" },
                { "Tip.Off", "UnplugLock - nonaktif ({0})" },
                { "Tip.Online", "tersambung ke listrik" },
                { "Tip.Offline", "memakai baterai" },
                { "Tip.Unknown", "status daya tidak diketahui" },
                { "Tip.NoSignIn", "UnplugLock - Windows tanpa kata sandi, mengunci tidak berguna" },

                // tray messages
                { "NoSignIn.Title", "Akun Windows Anda tidak punya kata sandi" },
                { "NoSignIn.Text", "UnplugLock bisa mengunci layar, tetapi siapa pun bisa masuk lagi dengan sekali klik. Klik di sini untuk mengatur kata sandi atau PIN." },
                { "Running.Title", "UnplugLock sedang berjalan" },
                { "Running.On", "Laptop Anda terkunci saat dicabut. Klik kanan ikonnya untuk melihat opsi." },
                { "Running.Off", "UnplugLock sedang nonaktif. Klik kanan ikonnya untuk melihat opsi." },
                { "LockFailed", "Windows tidak terkunci (galat {0}). Saat pengisi daya dicabut, UnplugLock terus mencoba selama 10 menit dan mengunci begitu Windows mengizinkan." },
                { "StartupFailed", "Tidak dapat mengubah pengaturan startup:\n{0}" },
                { "HideConfirm", "Ikon baki akan disembunyikan, tetapi UnplugLock tetap berjalan dan melindungi laptop Anda.\n\nUntuk menampilkan ikonnya lagi, buka UnplugLock sekali lagi (dari menu Start atau UnplugLock.exe)." },
                { "PasswordRemoved", "Kata sandi dihapus." },

                // installer
                { "Setup.Window", "Penyiapan UnplugLock" },
                { "Setup.Language", "Bahasa" },
                { "Setup.TitleInstall", "Pasang UnplugLock" },
                { "Setup.TitleUpdate", "Perbarui UnplugLock" },
                { "Setup.Guard", "Biarkan UnplugLock tetap berjalan" },
                { "Setup.GuardHint", "Jika ada yang menutupnya, ia langsung berjalan lagi." },
                { "Setup.Startup", "Jalankan UnplugLock saat saya masuk ke Windows" },
                { "Setup.Desktop", "Buat pintasan di desktop" },
                { "Setup.Launch", "Buka UnplugLock setelah penyiapan selesai" },
                { "Setup.InstallsTo", "Dipasang di" },
                { "Setup.PerUser", "Hanya untuk akun Anda, jadi tidak perlu hak administrator." },
                { "Setup.Install", "Pasang" },
                { "Setup.Update", "Perbarui" },
                { "Setup.Installing", "Memasang..." },
                { "Setup.Updating", "Memperbarui..." },
                { "Setup.Finish", "Selesai" },
                { "Setup.Failed", "Penyiapan tidak dapat diselesaikan:\n\n{0}" },
                { "Setup.Installed", "UnplugLock sudah terpasang" },
                { "Setup.Updated", "UnplugLock sudah diperbarui" },
                { "Setup.DoneSubtitle", "Laptop Anda sekarang terkunci saat pengisi daya dicabut." },
                { "Setup.DoneRunning", "Aplikasi berjalan di baki sistem, di samping jam. Jika ikonnya tidak terlihat, klik tanda panah ^ di sana." },
                { "Setup.DoneNotRunning", "Buka dari menu Start kapan pun Anda siap. Aplikasi akan berada di baki sistem, di samping jam." },
                { "Setup.DoneHint", "Klik kanan ikonnya untuk mengatur jeda penguncian, menambahkan kata sandi, atau mematikannya sementara." },
                { "Setup.AuthUpdate", "UnplugLock dilindungi kata sandi. Masukkan kata sandi untuk memperbaruinya." },
                { "Setup.AuthUninstall", "UnplugLock dilindungi kata sandi. Masukkan kata sandi untuk mencopotnya." },
                { "Setup.StillRunning", "UnplugLock masih berjalan dan tidak mau ditutup. Keluar dari menu bakinya, lalu jalankan penyiapan lagi." },
                { "Setup.Damaged", "File penyiapan ini rusak (UnplugLock.exe tidak ada di dalamnya)." },
                { "Setup.ShortcutDescription", "Mengunci laptop Anda saat pengisi daya dicabut" },
                { "Uninstall.Title", "Copot UnplugLock" },
                { "Uninstall.Confirm", "Hapus UnplugLock dan semua pengaturannya dari PC ini?" },
                { "Uninstall.StillRunning", "UnplugLock masih berjalan dan tidak mau ditutup. Keluar dari menu bakinya, lalu coba copot lagi." },
                { "Uninstall.Done", "UnplugLock telah dicopot." },
            };
        }
    }
}
