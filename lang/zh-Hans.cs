// Simplified Chinese
using System.Collections.Generic;

namespace UnplugLock
{
    internal static partial class Lang
    {
        private static Dictionary<string, string> ChineseSimplified()
        {
            return new Dictionary<string, string>
            {
                { "LanguageName", "简体中文" },

                // shared
                { "AppTagline", "一拔下充电器，立即锁定笔记本电脑。" },
                { "Version", "版本 {0}" },
                { "Button.Cancel", "取消" },
                { "Button.Continue", "继续" },
                { "Button.Close", "关闭" },
                { "Button.Done", "完成" },
                { "WrongPassword", "密码错误。" },
                { "AlreadyRunning", "UnplugLock 已在运行。" },

                // password prompt
                { "Pw.TitleEnter", "输入密码" },
                { "Pw.TitleChoose", "设置密码" },
                { "Pw.TitleRecovery", "输入恢复代码" },
                { "Pw.Label", "密码" },
                { "Pw.LabelNew", "新密码" },
                { "Pw.LabelRecovery", "恢复代码" },
                { "Pw.LabelConfirm", "确认密码" },
                { "Pw.Hint", "至少 {0} 个字符。请勿重复使用 Windows 密码或 PIN。" },
                { "Pw.Forgot", "忘记密码?" },
                { "Pw.Set", "设置密码" },
                { "Pw.TooShort", "请至少使用 {0} 个字符。" },
                { "Pw.Mismatch", "两次输入的密码不一致。" },
                { "Pw.NewPrompt", "关闭 UnplugLock、更改其设置或退出它时需要此密码。" },
                { "Pw.RecoveryPrompt", "请输入恢复代码。它会删除你的 UnplugLock 密码，以便你设置新密码。" },
                { "Pw.RecoveryWrong", "恢复代码不正确。" },

                // what the password is asked for
                { "Auth.TurnOff", "输入 UnplugLock 密码以将其关闭。" },
                { "Auth.StopEthernet", "输入 UnplugLock 密码，以便在拔出以太网线时不再锁定。" },
                { "Auth.StopWifi", "输入 UnplugLock 密码，以便在 Wi-Fi 断开时不再锁定。" },
                { "Auth.Delay", "输入 UnplugLock 密码以更改锁定延迟。" },
                { "Auth.Guard", "输入 UnplugLock 密码以关闭进程保护。" },
                { "Auth.Startup", "输入 UnplugLock 密码，使其不再随 Windows 启动。" },
                { "Auth.ChangePassword", "输入 UnplugLock 密码以更改密码。" },
                { "Auth.RemovePassword", "输入 UnplugLock 密码以删除密码。" },
                { "Auth.Exit", "输入 UnplugLock 密码以退出。" },
                { "Auth.Recovered", "恢复代码已接受，你的密码已删除。请在“密码”菜单中设置新密码。" },

                // recovery code window
                { "Rc.Title", "保存恢复代码" },
                { "Rc.Subtitle", "忘记密码时，这是重新进入的唯一方法。" },
                { "Rc.Explain", "如果忘记密码，请在密码提示中点击“{0}”并输入此代码。它会删除密码，以便你设置新密码。" },
                { "Rc.Keep", "请把它保存在这台笔记本电脑以外的地方，例如手机、密码管理器或纸上。它不会再次显示。" },
                { "Rc.Copy", "复制" },
                { "Rc.Copied", "已复制" },
                { "Rc.Save", "保存到文件..." },
                { "Rc.Confirm", "我已保存恢复代码" },
                { "Rc.FileTitle", "UnplugLock 恢复代码" },
                { "Rc.FileType", "文本文件" },
                { "Rc.FileHelp", "如果忘记 UnplugLock 密码，请在密码提示中点击“{0}”并输入此代码。" },
                { "Rc.FileCreated", "创建于 {0}。" },
                { "Rc.DateFormat", "yyyy'年'M'月'd'日'" },
                { "Rc.SaveFailed", "无法保存文件:\n{0}" },
                { "Rc.SavedOnLaptop", "已保存。此文件位于笔记本电脑自身的磁盘上，拿走笔记本电脑的人可能找到它，并用代码删除你的密码。\n\n请将其复制到 U 盘、手机或密码管理器，然后从这台笔记本电脑上删除。" },

                // about window
                { "About.Window", "关于 UnplugLock" },
                { "About.Source", "GitHub 上的源代码" },
                { "About.Releases", "检查新版本" },
                { "About.Support", "支持 UnplugLock" },
                { "About.License", "免费开源，MIT 许可证" },

                // tray menu
                { "Menu.Enabled", "已启用" },
                { "Menu.Delay", "锁定延迟" },
                { "Delay.Instant", "立即" },
                { "Delay.One", "{0} 秒" },
                { "Delay.Other", "{0} 秒" },
                { "Menu.AlsoLock", "以下情况也锁定" },
                { "Menu.Ethernet", "拔出以太网线时" },
                { "Menu.Wifi", "Wi-Fi 断开时" },
                { "Menu.Startup", "随 Windows 启动" },
                { "Menu.StartupGuarded", "随 Windows 启动 (进程保护需要)" },
                { "Menu.Guard", "进程保护 (自动重启)" },
                { "Menu.Hide", "隐藏托盘图标" },
                { "Menu.Password", "密码" },
                { "Menu.SetPassword", "设置密码..." },
                { "Menu.ChangePassword", "更改密码..." },
                { "Menu.RemovePassword", "删除密码" },
                { "Menu.LockNow", "立即锁定" },
                { "Menu.About", "关于 UnplugLock..." },
                { "Menu.Exit", "退出" },
                { "Menu.NoSignIn", "Windows 没有密码 - 去设置..." },
                { "Power.Online", "已接通电源" },
                { "Power.Offline", "使用电池" },
                { "Power.Unknown", "电源状态未知" },

                // tray icon tooltip (63 characters at most)
                { "Tip.On", "UnplugLock - 开 ({0})" },
                { "Tip.Off", "UnplugLock - 关 ({0})" },
                { "Tip.Online", "已接通电源" },
                { "Tip.Offline", "使用电池" },
                { "Tip.Unknown", "电源状态未知" },
                { "Tip.NoSignIn", "UnplugLock - Windows 没有密码，锁定也没用" },

                // tray messages
                { "NoSignIn.Title", "你的 Windows 帐户没有密码" },
                { "NoSignIn.Text", "UnplugLock 可以锁定屏幕，但任何人点一下就能重新进入。点击此处设置密码或 PIN。" },
                { "Running.Title", "UnplugLock 正在运行" },
                { "Running.On", "拔下电源时笔记本电脑会锁定。右键点击图标查看选项。" },
                { "Running.Off", "UnplugLock 已关闭。右键点击图标查看选项。" },
                { "LockFailed", "Windows 未能锁定 (错误 {0})。拔下充电器时，UnplugLock 会持续尝试 10 分钟，并在 Windows 允许时立即锁定。" },
                { "StartupFailed", "无法更改启动设置:\n{0}" },
                { "HideConfirm", "托盘图标将被隐藏，但 UnplugLock 会继续运行并保护你的笔记本电脑。\n\n要再次显示图标，请重新打开 UnplugLock (从“开始”菜单或 UnplugLock.exe)。" },
                { "PasswordRemoved", "密码已删除。" },

                // installer
                { "Setup.Window", "UnplugLock 安装程序" },
                { "Setup.Language", "语言" },
                { "Setup.TitleInstall", "安装 UnplugLock" },
                { "Setup.TitleUpdate", "更新 UnplugLock" },
                { "Setup.Guard", "保持 UnplugLock 运行" },
                { "Setup.GuardHint", "如果有人关闭它，它会立即重新启动。" },
                { "Setup.Startup", "登录 Windows 时启动 UnplugLock" },
                { "Setup.Desktop", "创建桌面快捷方式" },
                { "Setup.Launch", "安装完成后打开 UnplugLock" },
                { "Setup.InstallsTo", "安装位置" },
                { "Setup.PerUser", "仅为你的帐户安装，因此不需要管理员权限。" },
                { "Setup.Install", "安装" },
                { "Setup.Update", "更新" },
                { "Setup.Installing", "正在安装..." },
                { "Setup.Updating", "正在更新..." },
                { "Setup.Finish", "完成" },
                { "Setup.Failed", "安装未能完成:\n\n{0}" },
                { "Setup.Installed", "UnplugLock 已安装" },
                { "Setup.Updated", "UnplugLock 已更新" },
                { "Setup.DoneSubtitle", "现在拔下充电器时，笔记本电脑会锁定。" },
                { "Setup.DoneRunning", "它正在时钟旁边的通知区域中运行。如果看不到图标，请点击那里的 ^ 箭头。" },
                { "Setup.DoneNotRunning", "准备好后从“开始”菜单打开它。它会驻留在时钟旁边的通知区域。" },
                { "Setup.DoneHint", "右键点击图标可设置锁定延迟、添加密码或暂时关闭。" },
                { "Setup.AuthUpdate", "UnplugLock 受密码保护。请输入密码以更新它。" },
                { "Setup.AuthUninstall", "UnplugLock 受密码保护。请输入密码以卸载它。" },
                { "Setup.StillRunning", "UnplugLock 仍在运行，无法关闭。请从其托盘菜单退出，然后重新运行安装程序。" },
                { "Setup.Damaged", "此安装文件已损坏 (缺少 UnplugLock.exe)。" },
                { "Setup.ShortcutDescription", "拔下充电器时锁定笔记本电脑" },
                { "Uninstall.Title", "卸载 UnplugLock" },
                { "Uninstall.Confirm", "要从这台电脑中删除 UnplugLock 及其所有设置吗?" },
                { "Uninstall.StillRunning", "UnplugLock 仍在运行，无法关闭。请从其托盘菜单退出，然后再次卸载。" },
                { "Uninstall.Done", "UnplugLock 已卸载。" },
            };
        }
    }
}
