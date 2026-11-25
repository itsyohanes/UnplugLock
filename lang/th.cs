// Thai
using System.Collections.Generic;

namespace UnplugLock
{
    internal static partial class Lang
    {
        private static Dictionary<string, string> Thai()
        {
            return new Dictionary<string, string>
            {
                { "LanguageName", "ไทย" },

                // shared
                { "AppTagline", "ล็อกแล็ปท็อปทันทีที่ถอดสายชาร์จ" },
                { "Version", "เวอร์ชัน {0}" },
                { "Button.Cancel", "ยกเลิก" },
                { "Button.Continue", "ดำเนินการต่อ" },
                { "Button.Close", "ปิด" },
                { "Button.Done", "เสร็จสิ้น" },
                { "WrongPassword", "รหัสผ่านไม่ถูกต้อง" },
                { "AlreadyRunning", "UnplugLock กำลังทำงานอยู่แล้ว" },

                // password prompt
                { "Pw.TitleEnter", "ใส่รหัสผ่าน" },
                { "Pw.TitleChoose", "ตั้งรหัสผ่าน" },
                { "Pw.TitleRecovery", "ใส่รหัสกู้คืน" },
                { "Pw.Label", "รหัสผ่าน" },
                { "Pw.LabelNew", "รหัสผ่านใหม่" },
                { "Pw.LabelRecovery", "รหัสกู้คืน" },
                { "Pw.LabelConfirm", "ยืนยันรหัสผ่าน" },
                { "Pw.Hint", "อย่างน้อย {0} ตัวอักษร ห้ามใช้รหัสผ่านหรือ PIN เดียวกับ Windows" },
                { "Pw.Forgot", "ลืมรหัสผ่าน?" },
                { "Pw.Set", "ตั้งรหัสผ่าน" },
                { "Pw.TooShort", "ใช้อย่างน้อย {0} ตัวอักษร" },
                { "Pw.Mismatch", "รหัสผ่านไม่ตรงกัน" },
                { "Pw.NewPrompt", "คุณจะต้องใช้รหัสผ่านนี้เพื่อปิด UnplugLock เปลี่ยนการตั้งค่า หรือออกจากโปรแกรม" },
                { "Pw.RecoveryPrompt", "ใส่รหัสกู้คืนของคุณ รหัสนี้จะลบรหัสผ่าน UnplugLock เพื่อให้คุณตั้งรหัสผ่านใหม่ได้" },
                { "Pw.RecoveryWrong", "รหัสกู้คืนไม่ถูกต้อง" },

                // what the password is asked for
                { "Auth.TurnOff", "ใส่รหัสผ่าน UnplugLock เพื่อปิดการทำงาน" },
                { "Auth.StopEthernet", "ใส่รหัสผ่าน UnplugLock เพื่อเลิกล็อกเมื่อถอดสาย Ethernet" },
                { "Auth.StopWifi", "ใส่รหัสผ่าน UnplugLock เพื่อเลิกล็อกเมื่อ Wi-Fi หลุด" },
                { "Auth.Delay", "ใส่รหัสผ่าน UnplugLock เพื่อเปลี่ยนเวลาหน่วงก่อนล็อก" },
                { "Auth.Guard", "ใส่รหัสผ่าน UnplugLock เพื่อปิดการป้องกันโปรเซส" },
                { "Auth.Startup", "ใส่รหัสผ่าน UnplugLock เพื่อไม่ให้เริ่มพร้อม Windows" },
                { "Auth.ChangePassword", "ใส่รหัสผ่าน UnplugLock เพื่อเปลี่ยนรหัสผ่าน" },
                { "Auth.RemovePassword", "ใส่รหัสผ่าน UnplugLock เพื่อลบรหัสผ่าน" },
                { "Auth.Exit", "ใส่รหัสผ่าน UnplugLock เพื่อออกจากโปรแกรม" },
                { "Auth.Recovered", "ยอมรับรหัสกู้คืนแล้ว รหัสผ่านของคุณถูกลบแล้ว ตั้งรหัสผ่านใหม่ได้จากเมนูรหัสผ่าน" },

                // recovery code window
                { "Rc.Title", "บันทึกรหัสกู้คืนของคุณ" },
                { "Rc.Subtitle", "นี่เป็นทางเดียวที่จะกลับเข้ามาได้หากคุณลืมรหัสผ่าน" },
                { "Rc.Explain", "หากลืมรหัสผ่าน ให้คลิก \"{0}\" ที่หน้าต่างถามรหัสผ่านแล้วใส่รหัสนี้ รหัสนี้จะลบรหัสผ่านเพื่อให้คุณตั้งรหัสผ่านใหม่ได้" },
                { "Rc.Keep", "เก็บไว้ที่อื่นที่ไม่ใช่แล็ปท็อปเครื่องนี้ เช่น ในโทรศัพท์ ในโปรแกรมจัดการรหัสผ่าน หรือจดลงกระดาษ รหัสนี้จะไม่แสดงอีก" },
                { "Rc.Copy", "คัดลอก" },
                { "Rc.Copied", "คัดลอกแล้ว" },
                { "Rc.Save", "บันทึกเป็นไฟล์..." },
                { "Rc.Confirm", "ฉันบันทึกรหัสกู้คืนแล้ว" },
                { "Rc.FileTitle", "รหัสกู้คืน UnplugLock" },
                { "Rc.FileType", "ไฟล์ข้อความ" },
                { "Rc.FileHelp", "หากลืมรหัสผ่าน UnplugLock ให้คลิก \"{0}\" ที่หน้าต่างถามรหัสผ่านแล้วใส่รหัสนี้" },
                { "Rc.FileCreated", "สร้างเมื่อ {0}" },
                { "Rc.DateFormat", "d MMMM yyyy" },
                { "Rc.SaveFailed", "บันทึกไฟล์ไม่ได้:\n{0}" },
                { "Rc.SavedOnLaptop", "บันทึกแล้ว ไฟล์นี้อยู่บนไดรฟ์ของแล็ปท็อปเอง ใครก็ตามที่เอาแล็ปท็อปไปอาจพบไฟล์และใช้รหัสลบรหัสผ่านของคุณได้\n\nคัดลอกไปไว้ในแฟลชไดรฟ์ โทรศัพท์ หรือโปรแกรมจัดการรหัสผ่าน แล้วลบออกจากแล็ปท็อปเครื่องนี้" },

                // about window
                { "About.Window", "เกี่ยวกับ UnplugLock" },
                { "About.Source", "ซอร์สโค้ดบน GitHub" },
                { "About.Releases", "ตรวจหาเวอร์ชันใหม่" },
                { "About.Support", "สนับสนุน UnplugLock" },
                { "About.License", "ฟรีและโอเพนซอร์ส สัญญาอนุญาต MIT" },

                // tray menu
                { "Menu.Enabled", "เปิดใช้งาน" },
                { "Menu.Delay", "หน่วงเวลาก่อนล็อก" },
                { "Delay.Instant", "ทันที" },
                { "Delay.One", "{0} วินาที" },
                { "Delay.Other", "{0} วินาที" },
                { "Menu.AlsoLock", "ล็อกด้วยเมื่อ" },
                { "Menu.Ethernet", "ถอดสาย Ethernet" },
                { "Menu.Wifi", "Wi-Fi หลุด" },
                { "Menu.Startup", "เริ่มพร้อม Windows" },
                { "Menu.StartupGuarded", "เริ่มพร้อม Windows (จำเป็นสำหรับการป้องกัน)" },
                { "Menu.Guard", "ป้องกันโปรเซส (รีสตาร์ทอัตโนมัติ)" },
                { "Menu.Hide", "ซ่อนไอคอนในถาด" },
                { "Menu.Password", "รหัสผ่าน" },
                { "Menu.SetPassword", "ตั้งรหัสผ่าน..." },
                { "Menu.ChangePassword", "เปลี่ยนรหัสผ่าน..." },
                { "Menu.RemovePassword", "ลบรหัสผ่าน" },
                { "Menu.LockNow", "ล็อกตอนนี้" },
                { "Menu.About", "เกี่ยวกับ UnplugLock..." },
                { "Menu.Exit", "ออก" },
                { "Menu.NoSignIn", "Windows ไม่มีรหัสผ่าน - ตั้งค่าเลย..." },
                { "Power.Online", "เสียบปลั๊กอยู่" },
                { "Power.Offline", "ใช้แบตเตอรี่" },
                { "Power.Unknown", "ไม่ทราบสถานะพลังงาน" },

                // tray icon tooltip (63 characters at most)
                { "Tip.On", "UnplugLock - เปิด ({0})" },
                { "Tip.Off", "UnplugLock - ปิด ({0})" },
                { "Tip.Online", "เสียบปลั๊กอยู่" },
                { "Tip.Offline", "ใช้แบตเตอรี่" },
                { "Tip.Unknown", "ไม่ทราบสถานะพลังงาน" },
                { "Tip.NoSignIn", "UnplugLock - Windows ไม่มีรหัสผ่าน การล็อกจึงไม่ช่วย" },

                // tray messages
                { "NoSignIn.Title", "บัญชี Windows ของคุณไม่มีรหัสผ่าน" },
                { "NoSignIn.Text", "UnplugLock ล็อกหน้าจอได้ แต่ใครก็คลิกเข้ากลับมาได้ทันที คลิกที่นี่เพื่อตั้งรหัสผ่านหรือ PIN" },
                { "Running.Title", "UnplugLock กำลังทำงาน" },
                { "Running.On", "แล็ปท็อปจะล็อกเมื่อถอดปลั๊ก คลิกขวาที่ไอคอนเพื่อดูตัวเลือก" },
                { "Running.Off", "UnplugLock ปิดอยู่ คลิกขวาที่ไอคอนเพื่อดูตัวเลือก" },
                { "LockFailed", "Windows ไม่ได้ล็อก (ข้อผิดพลาด {0}) เมื่อถอดสายชาร์จ UnplugLock จะพยายามต่อไป 10 นาที และล็อกทันทีที่ Windows อนุญาต" },
                { "StartupFailed", "เปลี่ยนการตั้งค่าการเริ่มต้นไม่ได้:\n{0}" },
                { "HideConfirm", "ไอคอนในถาดจะถูกซ่อน แต่ UnplugLock ยังทำงานและปกป้องแล็ปท็อปของคุณต่อไป\n\nหากต้องการให้ไอคอนกลับมา ให้เปิด UnplugLock อีกครั้ง (จากเมนูเริ่มหรือ UnplugLock.exe)" },
                { "PasswordRemoved", "ลบรหัสผ่านแล้ว" },

                // installer
                { "Setup.Window", "ติดตั้ง UnplugLock" },
                { "Setup.Language", "ภาษา" },
                { "Setup.TitleInstall", "ติดตั้ง UnplugLock" },
                { "Setup.TitleUpdate", "อัปเดต UnplugLock" },
                { "Setup.Guard", "ให้ UnplugLock ทำงานตลอด" },
                { "Setup.GuardHint", "หากมีคนปิด โปรแกรมจะเริ่มทำงานใหม่ทันที" },
                { "Setup.Startup", "เริ่ม UnplugLock เมื่อฉันลงชื่อเข้าใช้ Windows" },
                { "Setup.Desktop", "สร้างทางลัดบนเดสก์ท็อป" },
                { "Setup.Launch", "เปิด UnplugLock เมื่อติดตั้งเสร็จ" },
                { "Setup.InstallsTo", "ติดตั้งไปที่" },
                { "Setup.PerUser", "สำหรับบัญชีของคุณเท่านั้น จึงไม่ต้องใช้สิทธิ์ผู้ดูแลระบบ" },
                { "Setup.Install", "ติดตั้ง" },
                { "Setup.Update", "อัปเดต" },
                { "Setup.Installing", "กำลังติดตั้ง..." },
                { "Setup.Updating", "กำลังอัปเดต..." },
                { "Setup.Finish", "เสร็จสิ้น" },
                { "Setup.Failed", "ติดตั้งไม่สำเร็จ:\n\n{0}" },
                { "Setup.Installed", "ติดตั้ง UnplugLock แล้ว" },
                { "Setup.Updated", "อัปเดต UnplugLock แล้ว" },
                { "Setup.DoneSubtitle", "ตอนนี้แล็ปท็อปจะล็อกเมื่อถอดสายชาร์จ" },
                { "Setup.DoneRunning", "โปรแกรมทำงานอยู่ในถาดระบบข้างนาฬิกา หากไม่เห็นไอคอน ให้คลิกลูกศร ^ ตรงนั้น" },
                { "Setup.DoneNotRunning", "เปิดจากเมนูเริ่มได้ทุกเมื่อที่พร้อม โปรแกรมจะอยู่ในถาดระบบข้างนาฬิกา" },
                { "Setup.DoneHint", "คลิกขวาที่ไอคอนเพื่อตั้งเวลาหน่วง เพิ่มรหัสผ่าน หรือปิดไว้ชั่วคราว" },
                { "Setup.AuthUpdate", "UnplugLock ป้องกันด้วยรหัสผ่าน ใส่รหัสผ่านเพื่ออัปเดต" },
                { "Setup.AuthUninstall", "UnplugLock ป้องกันด้วยรหัสผ่าน ใส่รหัสผ่านเพื่อถอนการติดตั้ง" },
                { "Setup.StillRunning", "UnplugLock ยังทำงานอยู่และปิดไม่ได้ ออกจากโปรแกรมผ่านเมนูในถาด แล้วเรียกใช้การติดตั้งอีกครั้ง" },
                { "Setup.Damaged", "ไฟล์ติดตั้งนี้เสียหาย (ไม่มี UnplugLock.exe อยู่ในไฟล์)" },
                { "Setup.ShortcutDescription", "ล็อกแล็ปท็อปเมื่อถอดสายชาร์จ" },
                { "Uninstall.Title", "ถอนการติดตั้ง UnplugLock" },
                { "Uninstall.Confirm", "ลบ UnplugLock และการตั้งค่าทั้งหมดออกจากพีซีเครื่องนี้หรือไม่" },
                { "Uninstall.StillRunning", "UnplugLock ยังทำงานอยู่และปิดไม่ได้ ออกจากโปรแกรมผ่านเมนูในถาด แล้วถอนการติดตั้งอีกครั้ง" },
                { "Uninstall.Done", "ถอนการติดตั้ง UnplugLock แล้ว" },
            };
        }
    }
}
