// Traditional Chinese (Taiwan wording; Hong Kong and Macau get it too)
using System.Collections.Generic;

namespace UnplugLock
{
    internal static partial class Lang
    {
        private static Dictionary<string, string> ChineseTraditional()
        {
            return new Dictionary<string, string>
            {
                { "LanguageName", "繁體中文" },

                // shared
                { "AppTagline", "一拔下充電器，立即鎖定筆電。" },
                { "Version", "版本 {0}" },
                { "Button.Cancel", "取消" },
                { "Button.Continue", "繼續" },
                { "Button.Close", "關閉" },
                { "Button.Done", "完成" },
                { "WrongPassword", "密碼錯誤。" },
                { "AlreadyRunning", "UnplugLock 已在執行中。" },

                // password prompt
                { "Pw.TitleEnter", "輸入密碼" },
                { "Pw.TitleChoose", "設定密碼" },
                { "Pw.TitleRecovery", "輸入復原碼" },
                { "Pw.Label", "密碼" },
                { "Pw.LabelNew", "新密碼" },
                { "Pw.LabelRecovery", "復原碼" },
                { "Pw.LabelConfirm", "確認密碼" },
                { "Pw.Hint", "至少 {0} 個字元。請勿重複使用 Windows 密碼或 PIN。" },
                { "Pw.Forgot", "忘記密碼？" },
                { "Pw.Set", "設定密碼" },
                { "Pw.TooShort", "請至少使用 {0} 個字元。" },
                { "Pw.Mismatch", "兩次輸入的密碼不一致。" },
                { "Pw.NewPrompt", "關閉 UnplugLock、變更其設定或結束它時，都需要這組密碼。" },
                { "Pw.RecoveryPrompt", "請輸入復原碼。它會移除你的 UnplugLock 密碼，讓你設定新密碼。" },
                { "Pw.RecoveryWrong", "復原碼不正確。" },

                // what the password is asked for
                { "Auth.TurnOff", "輸入 UnplugLock 密碼以將其關閉。" },
                { "Auth.StopEthernet", "輸入 UnplugLock 密碼，拔除乙太網路線時就不再鎖定。" },
                { "Auth.StopWifi", "輸入 UnplugLock 密碼，Wi-Fi 中斷時就不再鎖定。" },
                { "Auth.Delay", "輸入 UnplugLock 密碼以變更鎖定延遲。" },
                { "Auth.Guard", "輸入 UnplugLock 密碼以關閉處理程序保護。" },
                { "Auth.Startup", "輸入 UnplugLock 密碼，讓它不再隨 Windows 啟動。" },
                { "Auth.ChangePassword", "輸入 UnplugLock 密碼以變更密碼。" },
                { "Auth.RemovePassword", "輸入 UnplugLock 密碼以移除密碼。" },
                { "Auth.Exit", "輸入 UnplugLock 密碼以結束。" },
                { "Auth.Recovered", "已接受復原碼，你的密碼已移除。請在「密碼」選單中設定新密碼。" },

                // recovery code window
                { "Rc.Title", "儲存復原碼" },
                { "Rc.Subtitle", "忘記密碼時，這是重新進入的唯一方法。" },
                { "Rc.Explain", "如果忘記密碼，請在密碼視窗中按一下「{0}」並輸入此代碼。它會移除密碼，讓你設定新密碼。" },
                { "Rc.Keep", "請把它存放在這台筆電以外的地方，例如手機、密碼管理工具或紙上。它不會再次顯示。" },
                { "Rc.Copy", "複製" },
                { "Rc.Copied", "已複製" },
                { "Rc.Save", "儲存成檔案..." },
                { "Rc.Confirm", "我已儲存復原碼" },
                { "Rc.FileTitle", "UnplugLock 復原碼" },
                { "Rc.FileType", "文字檔" },
                { "Rc.FileHelp", "如果忘記 UnplugLock 密碼，請在密碼視窗中按一下「{0}」並輸入此代碼。" },
                { "Rc.FileCreated", "建立於 {0}。" },
                { "Rc.DateFormat", "yyyy'年'M'月'd'日'" },
                { "Rc.SaveFailed", "無法儲存檔案：\n{0}" },
                { "Rc.SavedOnLaptop", "已儲存。這個檔案位於筆電本身的磁碟上，拿走筆電的人可能會找到它，並用代碼移除你的密碼。\n\n請將它複製到隨身碟、手機或密碼管理工具，然後從這台筆電上刪除。" },

                // about window
                { "About.Window", "關於 UnplugLock" },
                { "About.Source", "GitHub 上的原始碼" },
                { "About.Releases", "檢查新版本" },
                { "About.Support", "贊助 UnplugLock" },
                { "About.License", "免費且開放原始碼，MIT 授權" },

                // tray menu
                { "Menu.Enabled", "已啟用" },
                { "Menu.Delay", "鎖定延遲" },
                { "Delay.Instant", "立即" },
                { "Delay.One", "{0} 秒" },
                { "Delay.Other", "{0} 秒" },
                { "Menu.AlsoLock", "以下情況也鎖定" },
                { "Menu.Ethernet", "拔除乙太網路線時" },
                { "Menu.Wifi", "Wi-Fi 中斷時" },
                { "Menu.Startup", "隨 Windows 啟動" },
                { "Menu.StartupGuarded", "隨 Windows 啟動 (處理程序保護需要)" },
                { "Menu.Guard", "處理程序保護 (自動重新啟動)" },
                { "Menu.Hide", "隱藏系統匣圖示" },
                { "Menu.Password", "密碼" },
                { "Menu.SetPassword", "設定密碼..." },
                { "Menu.ChangePassword", "變更密碼..." },
                { "Menu.RemovePassword", "移除密碼" },
                { "Menu.LockNow", "立即鎖定" },
                { "Menu.About", "關於 UnplugLock..." },
                { "Menu.Exit", "結束" },
                { "Menu.NoSignIn", "Windows 沒有密碼 - 前往設定..." },
                { "Power.Online", "已接上電源" },
                { "Power.Offline", "使用電池" },
                { "Power.Unknown", "電源狀態不明" },

                // tray icon tooltip (63 characters at most)
                { "Tip.On", "UnplugLock - 開 ({0})" },
                { "Tip.Off", "UnplugLock - 關 ({0})" },
                { "Tip.Online", "已接上電源" },
                { "Tip.Offline", "使用電池" },
                { "Tip.Unknown", "電源狀態不明" },
                { "Tip.NoSignIn", "UnplugLock - Windows 沒有密碼，鎖定也沒用" },

                // tray messages
                { "NoSignIn.Title", "你的 Windows 帳戶沒有密碼" },
                { "NoSignIn.Text", "UnplugLock 可以鎖定螢幕，但任何人按一下就能重新進入。按一下這裡設定密碼或 PIN。" },
                { "Running.Title", "UnplugLock 正在執行" },
                { "Running.On", "拔掉電源時筆電會鎖定。以滑鼠右鍵按一下圖示即可查看選項。" },
                { "Running.Off", "UnplugLock 已關閉。以滑鼠右鍵按一下圖示即可查看選項。" },
                { "LockFailed", "Windows 未能鎖定 (錯誤 {0})。拔下充電器時，UnplugLock 會持續嘗試 10 分鐘，並在 Windows 允許時立即鎖定。" },
                { "StartupFailed", "無法變更啟動設定：\n{0}" },
                { "HideConfirm", "系統匣圖示將會隱藏，但 UnplugLock 會繼續執行並保護你的筆電。\n\n若要再次顯示圖示，請重新開啟 UnplugLock (從「開始」功能表或 UnplugLock.exe)。" },
                { "PasswordRemoved", "密碼已移除。" },

                // installer
                { "Setup.Window", "UnplugLock 安裝程式" },
                { "Setup.Language", "語言" },
                { "Setup.TitleInstall", "安裝 UnplugLock" },
                { "Setup.TitleUpdate", "更新 UnplugLock" },
                { "Setup.Guard", "讓 UnplugLock 保持執行" },
                { "Setup.GuardHint", "如果有人關閉它，它會立即重新啟動。" },
                { "Setup.Startup", "登入 Windows 時啟動 UnplugLock" },
                { "Setup.Desktop", "建立桌面捷徑" },
                { "Setup.Launch", "安裝完成後開啟 UnplugLock" },
                { "Setup.InstallsTo", "安裝位置" },
                { "Setup.PerUser", "只安裝給你的帳戶，因此不需要系統管理員權限。" },
                { "Setup.Install", "安裝" },
                { "Setup.Update", "更新" },
                { "Setup.Installing", "正在安裝..." },
                { "Setup.Updating", "正在更新..." },
                { "Setup.Finish", "完成" },
                { "Setup.Failed", "安裝無法完成：\n\n{0}" },
                { "Setup.Installed", "UnplugLock 已安裝" },
                { "Setup.Updated", "UnplugLock 已更新" },
                { "Setup.DoneSubtitle", "現在拔下充電器時，筆電就會鎖定。" },
                { "Setup.DoneRunning", "它正在時鐘旁的系統匣中執行。如果看不到圖示，請按一下那裡的 ^ 箭頭。" },
                { "Setup.DoneNotRunning", "準備好後，從「開始」功能表開啟它。它會常駐在時鐘旁的系統匣。" },
                { "Setup.DoneHint", "以滑鼠右鍵按一下圖示，即可設定鎖定延遲、新增密碼或暫時關閉。" },
                { "Setup.AuthUpdate", "UnplugLock 受密碼保護。請輸入密碼以更新。" },
                { "Setup.AuthUninstall", "UnplugLock 受密碼保護。請輸入密碼以解除安裝。" },
                { "Setup.StillRunning", "UnplugLock 仍在執行且無法關閉。請從系統匣選單結束它，然後重新執行安裝程式。" },
                { "Setup.Damaged", "此安裝檔已損毀 (缺少 UnplugLock.exe)。" },
                { "Setup.ShortcutDescription", "拔下充電器時鎖定筆電" },
                { "Uninstall.Title", "解除安裝 UnplugLock" },
                { "Uninstall.Confirm", "要從這台電腦移除 UnplugLock 及其所有設定嗎？" },
                { "Uninstall.StillRunning", "UnplugLock 仍在執行且無法關閉。請從系統匣選單結束它，然後再次解除安裝。" },
                { "Uninstall.Done", "UnplugLock 已解除安裝。" },
            };
        }
    }
}
