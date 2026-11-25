// Japanese
using System.Collections.Generic;

namespace UnplugLock
{
    internal static partial class Lang
    {
        private static Dictionary<string, string> Japanese()
        {
            return new Dictionary<string, string>
            {
                { "LanguageName", "日本語" },

                // shared
                { "AppTagline", "充電器を抜いた瞬間にノート PC をロックします。" },
                { "Version", "バージョン {0}" },
                { "Button.Cancel", "キャンセル" },
                { "Button.Continue", "続行" },
                { "Button.Close", "閉じる" },
                { "Button.Done", "完了" },
                { "WrongPassword", "パスワードが違います。" },
                { "AlreadyRunning", "UnplugLock は既に実行中です。" },

                // password prompt
                { "Pw.TitleEnter", "パスワードを入力" },
                { "Pw.TitleChoose", "パスワードを設定" },
                { "Pw.TitleRecovery", "回復コードを入力" },
                { "Pw.Label", "パスワード" },
                { "Pw.LabelNew", "新しいパスワード" },
                { "Pw.LabelRecovery", "回復コード" },
                { "Pw.LabelConfirm", "パスワードの確認" },
                { "Pw.Hint", "{0} 文字以上。Windows のパスワードや PIN は使い回さないでください。" },
                { "Pw.Forgot", "パスワードを忘れた場合" },
                { "Pw.Set", "パスワードを設定" },
                { "Pw.TooShort", "{0} 文字以上にしてください。" },
                { "Pw.Mismatch", "パスワードが一致しません。" },
                { "Pw.NewPrompt", "UnplugLock をオフにする、設定を変更する、終了するときに必要になります。" },
                { "Pw.RecoveryPrompt", "回復コードを入力してください。UnplugLock のパスワードが削除され、新しいパスワードを設定できます。" },
                { "Pw.RecoveryWrong", "回復コードが正しくありません。" },

                // what the password is asked for
                { "Auth.TurnOff", "オフにするには UnplugLock のパスワードを入力してください。" },
                { "Auth.StopEthernet", "イーサネット ケーブルが抜かれたときのロックを止めるには、UnplugLock のパスワードを入力してください。" },
                { "Auth.StopWifi", "Wi-Fi が切断されたときのロックを止めるには、UnplugLock のパスワードを入力してください。" },
                { "Auth.Delay", "ロックの遅延を変更するには UnplugLock のパスワードを入力してください。" },
                { "Auth.Guard", "プロセス保護をオフにするには UnplugLock のパスワードを入力してください。" },
                { "Auth.Startup", "Windows と一緒に起動しないようにするには、UnplugLock のパスワードを入力してください。" },
                { "Auth.ChangePassword", "パスワードを変更するには UnplugLock のパスワードを入力してください。" },
                { "Auth.RemovePassword", "パスワードを削除するには UnplugLock のパスワードを入力してください。" },
                { "Auth.Exit", "終了するには UnplugLock のパスワードを入力してください。" },
                { "Auth.Recovered", "回復コードを確認しました。パスワードは削除されました。[パスワード] メニューから新しいパスワードを設定してください。" },

                // recovery code window
                { "Rc.Title", "回復コードを保存してください" },
                { "Rc.Subtitle", "パスワードを忘れたときに戻れる唯一の方法です。" },
                { "Rc.Explain", "パスワードを忘れたら、パスワード入力画面で [{0}] をクリックし、このコードを入力してください。パスワードが削除され、新しいパスワードを設定できます。" },
                { "Rc.Keep", "このノート PC 以外の場所 (スマートフォン、パスワード マネージャー、紙など) に保管してください。このコードは二度と表示されません。" },
                { "Rc.Copy", "コピー" },
                { "Rc.Copied", "コピーしました" },
                { "Rc.Save", "ファイルに保存..." },
                { "Rc.Confirm", "回復コードを保存しました" },
                { "Rc.FileTitle", "UnplugLock 回復コード" },
                { "Rc.FileType", "テキスト ファイル" },
                { "Rc.FileHelp", "UnplugLock のパスワードを忘れたら、パスワード入力画面で [{0}] をクリックし、このコードを入力してください。" },
                { "Rc.FileCreated", "作成日: {0}" },
                { "Rc.DateFormat", "yyyy'年'M'月'd'日'" },
                { "Rc.SaveFailed", "ファイルを保存できませんでした:\n{0}" },
                { "Rc.SavedOnLaptop", "保存しました。このファイルはノート PC 本体のドライブにあるため、ノート PC を持ち去った人が見つけて、コードでパスワードを削除できてしまいます。\n\nUSB メモリ、スマートフォン、パスワード マネージャーにコピーしてから、このノート PC から削除してください。" },

                // about window
                { "About.Window", "UnplugLock について" },
                { "About.Source", "GitHub のソース コード" },
                { "About.Releases", "新しいバージョンを確認" },
                { "About.Support", "UnplugLock を支援する" },
                { "About.License", "無料のオープンソース (MIT ライセンス)" },

                // tray menu
                { "Menu.Enabled", "有効" },
                { "Menu.Delay", "ロックの遅延" },
                { "Delay.Instant", "すぐに" },
                { "Delay.One", "{0} 秒" },
                { "Delay.Other", "{0} 秒" },
                { "Menu.AlsoLock", "次の場合もロック" },
                { "Menu.Ethernet", "イーサネット ケーブルが抜かれたとき" },
                { "Menu.Wifi", "Wi-Fi が切断されたとき" },
                { "Menu.Startup", "Windows と一緒に起動" },
                { "Menu.StartupGuarded", "Windows と一緒に起動 (保護に必要)" },
                { "Menu.Guard", "プロセス保護 (自動再起動)" },
                { "Menu.Hide", "トレイ アイコンを非表示" },
                { "Menu.Password", "パスワード" },
                { "Menu.SetPassword", "パスワードを設定..." },
                { "Menu.ChangePassword", "パスワードを変更..." },
                { "Menu.RemovePassword", "パスワードを削除" },
                { "Menu.LockNow", "今すぐロック" },
                { "Menu.About", "UnplugLock について..." },
                { "Menu.Exit", "終了" },
                { "Menu.NoSignIn", "Windows にパスワードがありません - 設定する..." },
                { "Power.Online", "電源に接続中" },
                { "Power.Offline", "バッテリー駆動" },
                { "Power.Unknown", "電源の状態が不明" },

                // tray icon tooltip (63 characters at most)
                { "Tip.On", "UnplugLock - オン ({0})" },
                { "Tip.Off", "UnplugLock - オフ ({0})" },
                { "Tip.Online", "電源に接続中" },
                { "Tip.Offline", "バッテリー駆動" },
                { "Tip.Unknown", "電源の状態が不明" },
                { "Tip.NoSignIn", "UnplugLock - Windows にパスワードがないため、ロックしても無効です" },

                // tray messages
                { "NoSignIn.Title", "Windows アカウントにパスワードがありません" },
                { "NoSignIn.Text", "UnplugLock は画面をロックできますが、誰でもクリックするだけで戻れます。ここをクリックしてパスワードか PIN を設定してください。" },
                { "Running.Title", "UnplugLock は実行中です" },
                { "Running.On", "電源を抜くとノート PC がロックされます。オプションはアイコンを右クリックしてください。" },
                { "Running.Off", "UnplugLock はオフです。オプションはアイコンを右クリックしてください。" },
                { "LockFailed", "Windows をロックできませんでした (エラー {0})。充電器が抜かれたときは、UnplugLock が 10 分間再試行し、Windows が許可しだいロックします。" },
                { "StartupFailed", "スタートアップの設定を変更できませんでした:\n{0}" },
                { "HideConfirm", "トレイ アイコンは非表示になりますが、UnplugLock は動作を続け、ノート PC を保護します。\n\nアイコンを再表示するには、もう一度 UnplugLock を開いてください (スタート メニューまたは UnplugLock.exe から)。" },
                { "PasswordRemoved", "パスワードを削除しました。" },

                // installer
                { "Setup.Window", "UnplugLock セットアップ" },
                { "Setup.Language", "言語" },
                { "Setup.TitleInstall", "UnplugLock をインストール" },
                { "Setup.TitleUpdate", "UnplugLock を更新" },
                { "Setup.Guard", "UnplugLock を常に実行" },
                { "Setup.GuardHint", "誰かが閉じても、すぐに自動で再起動します。" },
                { "Setup.Startup", "Windows へのサインイン時に UnplugLock を起動" },
                { "Setup.Desktop", "デスクトップにショートカットを作成" },
                { "Setup.Launch", "セットアップ完了後に UnplugLock を開く" },
                { "Setup.InstallsTo", "インストール先" },
                { "Setup.PerUser", "このアカウント専用なので、管理者権限は不要です。" },
                { "Setup.Install", "インストール" },
                { "Setup.Update", "更新" },
                { "Setup.Installing", "インストール中..." },
                { "Setup.Updating", "更新中..." },
                { "Setup.Finish", "完了" },
                { "Setup.Failed", "セットアップを完了できませんでした:\n\n{0}" },
                { "Setup.Installed", "UnplugLock をインストールしました" },
                { "Setup.Updated", "UnplugLock を更新しました" },
                { "Setup.DoneSubtitle", "これで、充電器を抜くとノート PC がロックされます。" },
                { "Setup.DoneRunning", "時計の横の通知領域で動作しています。アイコンが見えない場合は、そこにある ^ 矢印をクリックしてください。" },
                { "Setup.DoneNotRunning", "準備ができたらスタート メニューから開いてください。時計の横の通知領域に常駐します。" },
                { "Setup.DoneHint", "アイコンを右クリックすると、ロックの遅延の設定、パスワードの追加、一時的なオフができます。" },
                { "Setup.AuthUpdate", "UnplugLock はパスワードで保護されています。更新するにはパスワードを入力してください。" },
                { "Setup.AuthUninstall", "UnplugLock はパスワードで保護されています。アンインストールするにはパスワードを入力してください。" },
                { "Setup.StillRunning", "UnplugLock がまだ実行中で、終了できませんでした。トレイ メニューから終了してから、もう一度セットアップを実行してください。" },
                { "Setup.Damaged", "このセットアップ ファイルは破損しています (UnplugLock.exe が含まれていません)。" },
                { "Setup.ShortcutDescription", "充電器を抜くとノート PC をロックします" },
                { "Uninstall.Title", "UnplugLock のアンインストール" },
                { "Uninstall.Confirm", "UnplugLock とそのすべての設定をこの PC から削除しますか?" },
                { "Uninstall.StillRunning", "UnplugLock がまだ実行中で、終了できませんでした。トレイ メニューから終了してから、もう一度アンインストールしてください。" },
                { "Uninstall.Done", "UnplugLock をアンインストールしました。" },
            };
        }
    }
}
