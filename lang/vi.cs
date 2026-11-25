// Vietnamese
using System.Collections.Generic;

namespace UnplugLock
{
    internal static partial class Lang
    {
        private static Dictionary<string, string> Vietnamese()
        {
            return new Dictionary<string, string>
            {
                { "LanguageName", "Tiếng Việt" },

                // shared
                { "AppTagline", "Khóa laptop ngay khi bộ sạc bị rút ra." },
                { "Version", "Phiên bản {0}" },
                { "Button.Cancel", "Hủy" },
                { "Button.Continue", "Tiếp tục" },
                { "Button.Close", "Đóng" },
                { "Button.Done", "Xong" },
                { "WrongPassword", "Sai mật khẩu." },
                { "AlreadyRunning", "UnplugLock đang chạy rồi." },

                // password prompt
                { "Pw.TitleEnter", "Nhập mật khẩu" },
                { "Pw.TitleChoose", "Chọn mật khẩu" },
                { "Pw.TitleRecovery", "Nhập mã khôi phục" },
                { "Pw.Label", "Mật khẩu" },
                { "Pw.LabelNew", "Mật khẩu mới" },
                { "Pw.LabelRecovery", "Mã khôi phục" },
                { "Pw.LabelConfirm", "Xác nhận mật khẩu" },
                { "Pw.Hint", "Ít nhất {0} ký tự. Đừng dùng lại mật khẩu hoặc mã PIN Windows của bạn." },
                { "Pw.Forgot", "Quên mật khẩu?" },
                { "Pw.Set", "Đặt mật khẩu" },
                { "Pw.TooShort", "Hãy dùng ít nhất {0} ký tự." },
                { "Pw.Mismatch", "Mật khẩu không khớp." },
                { "Pw.NewPrompt", "Bạn sẽ cần mật khẩu này để tắt UnplugLock, thay đổi cài đặt của nó hoặc thoát nó." },
                { "Pw.RecoveryPrompt", "Nhập mã khôi phục của bạn. Mã này xóa mật khẩu UnplugLock để bạn đặt mật khẩu mới." },
                { "Pw.RecoveryWrong", "Mã khôi phục không đúng." },

                // what the password is asked for
                { "Auth.TurnOff", "Nhập mật khẩu UnplugLock để tắt nó." },
                { "Auth.StopEthernet", "Nhập mật khẩu UnplugLock để ngừng khóa khi rút cáp Ethernet." },
                { "Auth.StopWifi", "Nhập mật khẩu UnplugLock để ngừng khóa khi mất kết nối Wi-Fi." },
                { "Auth.Delay", "Nhập mật khẩu UnplugLock để thay đổi độ trễ khóa." },
                { "Auth.Guard", "Nhập mật khẩu UnplugLock để tắt bảo vệ tiến trình." },
                { "Auth.Startup", "Nhập mật khẩu UnplugLock để nó không khởi động cùng Windows nữa." },
                { "Auth.ChangePassword", "Nhập mật khẩu UnplugLock để đổi mật khẩu." },
                { "Auth.RemovePassword", "Nhập mật khẩu UnplugLock để xóa mật khẩu." },
                { "Auth.Exit", "Nhập mật khẩu UnplugLock để thoát." },
                { "Auth.Recovered", "Đã chấp nhận mã khôi phục. Mật khẩu của bạn đã bị xóa. Hãy đặt mật khẩu mới trong menu Mật khẩu." },

                // recovery code window
                { "Rc.Title", "Lưu mã khôi phục" },
                { "Rc.Subtitle", "Đây là cách duy nhất để vào lại nếu bạn quên mật khẩu." },
                { "Rc.Explain", "Nếu quên mật khẩu, hãy bấm \"{0}\" trong hộp nhập mật khẩu rồi nhập mã này. Mã sẽ xóa mật khẩu để bạn đặt mật khẩu mới." },
                { "Rc.Keep", "Hãy cất mã ở nơi khác ngoài laptop này, như điện thoại, trình quản lý mật khẩu hoặc trên giấy. Mã sẽ không hiện lại." },
                { "Rc.Copy", "Sao chép" },
                { "Rc.Copied", "Đã sao chép" },
                { "Rc.Save", "Lưu vào tệp..." },
                { "Rc.Confirm", "Tôi đã lưu mã khôi phục" },
                { "Rc.FileTitle", "Mã khôi phục UnplugLock" },
                { "Rc.FileType", "Tệp văn bản" },
                { "Rc.FileHelp", "Nếu quên mật khẩu UnplugLock, hãy bấm \"{0}\" trong hộp nhập mật khẩu rồi nhập mã này." },
                { "Rc.FileCreated", "Tạo ngày {0}." },
                { "Rc.DateFormat", "d 'tháng' M 'năm' yyyy" },
                { "Rc.SaveFailed", "Không lưu được tệp:\n{0}" },
                { "Rc.SavedOnLaptop", "Đã lưu. Tệp này nằm trên ổ đĩa của chính laptop, nên ai lấy laptop cũng có thể tìm thấy nó và dùng mã để xóa mật khẩu của bạn.\n\nHãy chép nó sang USB, điện thoại hoặc trình quản lý mật khẩu, rồi xóa nó khỏi laptop này." },

                // about window
                { "About.Window", "Giới thiệu UnplugLock" },
                { "About.Source", "Mã nguồn trên GitHub" },
                { "About.Releases", "Kiểm tra phiên bản mới" },
                { "About.Support", "Ủng hộ UnplugLock" },
                { "About.License", "Miễn phí và mã nguồn mở, giấy phép MIT" },

                // tray menu
                { "Menu.Enabled", "Đang bật" },
                { "Menu.Delay", "Độ trễ khóa" },
                { "Delay.Instant", "Ngay lập tức" },
                { "Delay.One", "{0} giây" },
                { "Delay.Other", "{0} giây" },
                { "Menu.AlsoLock", "Khóa cả khi" },
                { "Menu.Ethernet", "rút cáp Ethernet" },
                { "Menu.Wifi", "mất kết nối Wi-Fi" },
                { "Menu.Startup", "Khởi động cùng Windows" },
                { "Menu.StartupGuarded", "Khởi động cùng Windows (bảo vệ cần điều này)" },
                { "Menu.Guard", "Bảo vệ tiến trình (tự khởi động lại)" },
                { "Menu.Hide", "Ẩn biểu tượng khay" },
                { "Menu.Password", "Mật khẩu" },
                { "Menu.SetPassword", "Đặt mật khẩu..." },
                { "Menu.ChangePassword", "Đổi mật khẩu..." },
                { "Menu.RemovePassword", "Xóa mật khẩu" },
                { "Menu.LockNow", "Khóa ngay" },
                { "Menu.About", "Giới thiệu UnplugLock..." },
                { "Menu.Exit", "Thoát" },
                { "Menu.NoSignIn", "Windows chưa có mật khẩu - hãy đặt một mật khẩu..." },
                { "Power.Online", "Đang cắm sạc" },
                { "Power.Offline", "Đang dùng pin" },
                { "Power.Unknown", "Không rõ trạng thái nguồn" },

                // tray icon tooltip (63 characters at most)
                { "Tip.On", "UnplugLock - bật ({0})" },
                { "Tip.Off", "UnplugLock - tắt ({0})" },
                { "Tip.Online", "đang cắm sạc" },
                { "Tip.Offline", "đang dùng pin" },
                { "Tip.Unknown", "không rõ trạng thái nguồn" },
                { "Tip.NoSignIn", "UnplugLock - Windows chưa có mật khẩu, khóa cũng vô ích" },

                // tray messages
                { "NoSignIn.Title", "Tài khoản Windows của bạn chưa có mật khẩu" },
                { "NoSignIn.Text", "UnplugLock có thể khóa màn hình, nhưng ai cũng có thể bấm một cái là vào lại. Bấm vào đây để đặt mật khẩu hoặc mã PIN." },
                { "Running.Title", "UnplugLock đang chạy" },
                { "Running.On", "Laptop sẽ khóa khi bị rút sạc. Bấm chuột phải vào biểu tượng để xem tùy chọn." },
                { "Running.Off", "UnplugLock đang tắt. Bấm chuột phải vào biểu tượng để xem tùy chọn." },
                { "LockFailed", "Windows không khóa được (lỗi {0}). Khi bộ sạc bị rút, UnplugLock sẽ thử lại trong 10 phút và khóa ngay khi Windows cho phép." },
                { "StartupFailed", "Không thay đổi được cài đặt khởi động:\n{0}" },
                { "HideConfirm", "Biểu tượng khay sẽ bị ẩn, nhưng UnplugLock vẫn chạy và bảo vệ laptop của bạn.\n\nĐể hiện lại biểu tượng, hãy mở lại UnplugLock (từ menu Bắt đầu hoặc UnplugLock.exe)." },
                { "PasswordRemoved", "Đã xóa mật khẩu." },

                // installer
                { "Setup.Window", "Trình cài đặt UnplugLock" },
                { "Setup.Language", "Ngôn ngữ" },
                { "Setup.TitleInstall", "Cài đặt UnplugLock" },
                { "Setup.TitleUpdate", "Cập nhật UnplugLock" },
                { "Setup.Guard", "Giữ UnplugLock luôn chạy" },
                { "Setup.GuardHint", "Nếu ai đó đóng nó, nó sẽ tự khởi động lại ngay." },
                { "Setup.Startup", "Khởi động UnplugLock khi tôi đăng nhập Windows" },
                { "Setup.Desktop", "Tạo lối tắt trên màn hình nền" },
                { "Setup.Launch", "Mở UnplugLock khi cài đặt xong" },
                { "Setup.InstallsTo", "Cài vào" },
                { "Setup.PerUser", "Chỉ cho tài khoản của bạn, nên không cần quyền quản trị." },
                { "Setup.Install", "Cài đặt" },
                { "Setup.Update", "Cập nhật" },
                { "Setup.Installing", "Đang cài đặt..." },
                { "Setup.Updating", "Đang cập nhật..." },
                { "Setup.Finish", "Hoàn tất" },
                { "Setup.Failed", "Không thể hoàn tất cài đặt:\n\n{0}" },
                { "Setup.Installed", "Đã cài đặt UnplugLock" },
                { "Setup.Updated", "Đã cập nhật UnplugLock" },
                { "Setup.DoneSubtitle", "Giờ laptop của bạn sẽ khóa khi bộ sạc bị rút ra." },
                { "Setup.DoneRunning", "Nó đang chạy trong khay hệ thống, cạnh đồng hồ. Nếu không thấy biểu tượng, hãy bấm vào mũi tên ^ ở đó." },
                { "Setup.DoneNotRunning", "Mở nó từ menu Bắt đầu khi bạn sẵn sàng. Nó nằm trong khay hệ thống, cạnh đồng hồ." },
                { "Setup.DoneHint", "Bấm chuột phải vào biểu tượng để đặt độ trễ khóa, thêm mật khẩu hoặc tạm tắt." },
                { "Setup.AuthUpdate", "UnplugLock được bảo vệ bằng mật khẩu. Nhập mật khẩu để cập nhật." },
                { "Setup.AuthUninstall", "UnplugLock được bảo vệ bằng mật khẩu. Nhập mật khẩu để gỡ cài đặt." },
                { "Setup.StillRunning", "UnplugLock vẫn đang chạy và không đóng được. Hãy thoát nó từ menu khay, rồi chạy lại trình cài đặt." },
                { "Setup.Damaged", "Tệp cài đặt này bị hỏng (thiếu UnplugLock.exe)." },
                { "Setup.ShortcutDescription", "Khóa laptop khi bộ sạc bị rút ra" },
                { "Uninstall.Title", "Gỡ cài đặt UnplugLock" },
                { "Uninstall.Confirm", "Gỡ UnplugLock và toàn bộ cài đặt của nó khỏi máy tính này?" },
                { "Uninstall.StillRunning", "UnplugLock vẫn đang chạy và không đóng được. Hãy thoát nó từ menu khay, rồi gỡ cài đặt lại." },
                { "Uninstall.Done", "Đã gỡ cài đặt UnplugLock." },
            };
        }
    }
}
