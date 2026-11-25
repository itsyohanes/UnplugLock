// Korean
using System.Collections.Generic;

namespace UnplugLock
{
    internal static partial class Lang
    {
        private static Dictionary<string, string> Korean()
        {
            return new Dictionary<string, string>
            {
                { "LanguageName", "한국어" },

                // shared
                { "AppTagline", "충전기를 뽑는 순간 노트북을 잠급니다." },
                { "Version", "버전 {0}" },
                { "Button.Cancel", "취소" },
                { "Button.Continue", "계속" },
                { "Button.Close", "닫기" },
                { "Button.Done", "완료" },
                { "WrongPassword", "암호가 틀렸습니다." },
                { "AlreadyRunning", "UnplugLock이 이미 실행 중입니다." },

                // password prompt
                { "Pw.TitleEnter", "암호 입력" },
                { "Pw.TitleChoose", "암호 설정" },
                { "Pw.TitleRecovery", "복구 코드 입력" },
                { "Pw.Label", "암호" },
                { "Pw.LabelNew", "새 암호" },
                { "Pw.LabelRecovery", "복구 코드" },
                { "Pw.LabelConfirm", "암호 확인" },
                { "Pw.Hint", "{0}자 이상. Windows 암호나 PIN을 다시 사용하지 마세요." },
                { "Pw.Forgot", "암호를 잊으셨나요?" },
                { "Pw.Set", "암호 설정" },
                { "Pw.TooShort", "{0}자 이상 입력하세요." },
                { "Pw.Mismatch", "암호가 일치하지 않습니다." },
                { "Pw.NewPrompt", "UnplugLock을 끄거나, 설정을 바꾸거나, 종료할 때 필요합니다." },
                { "Pw.RecoveryPrompt", "복구 코드를 입력하세요. UnplugLock 암호가 제거되어 새 암호를 설정할 수 있습니다." },
                { "Pw.RecoveryWrong", "복구 코드가 올바르지 않습니다." },

                // what the password is asked for
                { "Auth.TurnOff", "끄려면 UnplugLock 암호를 입력하세요." },
                { "Auth.StopEthernet", "이더넷 케이블을 뽑을 때 잠그지 않으려면 UnplugLock 암호를 입력하세요." },
                { "Auth.StopWifi", "Wi-Fi 연결이 끊길 때 잠그지 않으려면 UnplugLock 암호를 입력하세요." },
                { "Auth.Delay", "잠금 지연 시간을 바꾸려면 UnplugLock 암호를 입력하세요." },
                { "Auth.Guard", "프로세스 보호를 끄려면 UnplugLock 암호를 입력하세요." },
                { "Auth.Startup", "Windows와 함께 시작하지 않게 하려면 UnplugLock 암호를 입력하세요." },
                { "Auth.ChangePassword", "암호를 변경하려면 UnplugLock 암호를 입력하세요." },
                { "Auth.RemovePassword", "암호를 제거하려면 UnplugLock 암호를 입력하세요." },
                { "Auth.Exit", "종료하려면 UnplugLock 암호를 입력하세요." },
                { "Auth.Recovered", "복구 코드가 확인되었습니다. 암호가 제거되었습니다. [암호] 메뉴에서 새 암호를 설정하세요." },

                // recovery code window
                { "Rc.Title", "복구 코드를 저장하세요" },
                { "Rc.Subtitle", "암호를 잊었을 때 다시 들어갈 수 있는 유일한 방법입니다." },
                { "Rc.Explain", "암호를 잊으면 암호 입력 창에서 \"{0}\"를 클릭하고 이 코드를 입력하세요. 암호가 제거되어 새 암호를 설정할 수 있습니다." },
                { "Rc.Keep", "이 노트북이 아닌 곳(휴대폰, 암호 관리자, 종이 등)에 보관하세요. 다시 표시되지 않습니다." },
                { "Rc.Copy", "복사" },
                { "Rc.Copied", "복사됨" },
                { "Rc.Save", "파일로 저장..." },
                { "Rc.Confirm", "복구 코드를 저장했습니다" },
                { "Rc.FileTitle", "UnplugLock 복구 코드" },
                { "Rc.FileType", "텍스트 파일" },
                { "Rc.FileHelp", "UnplugLock 암호를 잊으면 암호 입력 창에서 \"{0}\"를 클릭하고 이 코드를 입력하세요." },
                { "Rc.FileCreated", "만든 날짜: {0}" },
                { "Rc.DateFormat", "yyyy'년' M'월' d'일'" },
                { "Rc.SaveFailed", "파일을 저장할 수 없습니다:\n{0}" },
                { "Rc.SavedOnLaptop", "저장했습니다. 이 파일은 노트북 자체 드라이브에 있으므로, 노트북을 가져간 사람이 파일을 찾아 코드로 암호를 제거할 수 있습니다.\n\nUSB 메모리, 휴대폰 또는 암호 관리자에 복사한 다음 이 노트북에서 삭제하세요." },

                // about window
                { "About.Window", "UnplugLock 정보" },
                { "About.Source", "GitHub의 소스 코드" },
                { "About.Releases", "새 버전 확인" },
                { "About.Support", "UnplugLock 후원하기" },
                { "About.License", "무료 오픈 소스, MIT 라이선스" },

                // tray menu
                { "Menu.Enabled", "사용" },
                { "Menu.Delay", "잠금 지연" },
                { "Delay.Instant", "즉시" },
                { "Delay.One", "{0}초" },
                { "Delay.Other", "{0}초" },
                { "Menu.AlsoLock", "다음 경우에도 잠금" },
                { "Menu.Ethernet", "이더넷 케이블을 뽑을 때" },
                { "Menu.Wifi", "Wi-Fi 연결이 끊길 때" },
                { "Menu.Startup", "Windows와 함께 시작" },
                { "Menu.StartupGuarded", "Windows와 함께 시작 (보호에 필요)" },
                { "Menu.Guard", "프로세스 보호 (자동 다시 시작)" },
                { "Menu.Hide", "트레이 아이콘 숨기기" },
                { "Menu.Password", "암호" },
                { "Menu.SetPassword", "암호 설정..." },
                { "Menu.ChangePassword", "암호 변경..." },
                { "Menu.RemovePassword", "암호 제거" },
                { "Menu.LockNow", "지금 잠그기" },
                { "Menu.About", "UnplugLock 정보..." },
                { "Menu.Exit", "종료" },
                { "Menu.NoSignIn", "Windows에 암호가 없습니다 - 설정하기..." },
                { "Power.Online", "전원 연결됨" },
                { "Power.Offline", "배터리 사용 중" },
                { "Power.Unknown", "전원 상태 알 수 없음" },

                // tray icon tooltip (63 characters at most)
                { "Tip.On", "UnplugLock - 켜짐 ({0})" },
                { "Tip.Off", "UnplugLock - 꺼짐 ({0})" },
                { "Tip.Online", "전원 연결됨" },
                { "Tip.Offline", "배터리 사용 중" },
                { "Tip.Unknown", "전원 상태 알 수 없음" },
                { "Tip.NoSignIn", "UnplugLock - Windows에 암호가 없어 잠가도 소용없습니다" },

                // tray messages
                { "NoSignIn.Title", "Windows 계정에 암호가 없습니다" },
                { "NoSignIn.Text", "UnplugLock이 화면을 잠글 수는 있지만, 누구나 클릭 한 번으로 다시 들어올 수 있습니다. 여기를 클릭하여 암호나 PIN을 설정하세요." },
                { "Running.Title", "UnplugLock 실행 중" },
                { "Running.On", "전원을 뽑으면 노트북이 잠깁니다. 옵션을 보려면 아이콘을 마우스 오른쪽 단추로 클릭하세요." },
                { "Running.Off", "UnplugLock이 꺼져 있습니다. 옵션을 보려면 아이콘을 마우스 오른쪽 단추로 클릭하세요." },
                { "LockFailed", "Windows가 잠기지 않았습니다(오류 {0}). 충전기를 뽑으면 UnplugLock이 10분 동안 계속 시도하여 Windows가 허용하는 즉시 잠급니다." },
                { "StartupFailed", "시작 설정을 변경할 수 없습니다:\n{0}" },
                { "HideConfirm", "트레이 아이콘이 숨겨지지만 UnplugLock은 계속 실행되며 노트북을 보호합니다.\n\n아이콘을 다시 표시하려면 UnplugLock을 다시 여세요(시작 메뉴 또는 UnplugLock.exe에서)." },
                { "PasswordRemoved", "암호를 제거했습니다." },

                // installer
                { "Setup.Window", "UnplugLock 설치" },
                { "Setup.Language", "언어" },
                { "Setup.TitleInstall", "UnplugLock 설치" },
                { "Setup.TitleUpdate", "UnplugLock 업데이트" },
                { "Setup.Guard", "UnplugLock 계속 실행" },
                { "Setup.GuardHint", "누군가 닫아도 바로 다시 시작됩니다." },
                { "Setup.Startup", "Windows에 로그인할 때 UnplugLock 시작" },
                { "Setup.Desktop", "바탕 화면 바로 가기 만들기" },
                { "Setup.Launch", "설치가 끝나면 UnplugLock 열기" },
                { "Setup.InstallsTo", "설치 위치" },
                { "Setup.PerUser", "내 계정에만 설치되므로 관리자 권한이 필요하지 않습니다." },
                { "Setup.Install", "설치" },
                { "Setup.Update", "업데이트" },
                { "Setup.Installing", "설치 중..." },
                { "Setup.Updating", "업데이트 중..." },
                { "Setup.Finish", "마침" },
                { "Setup.Failed", "설치를 완료할 수 없습니다:\n\n{0}" },
                { "Setup.Installed", "UnplugLock이 설치되었습니다" },
                { "Setup.Updated", "UnplugLock이 업데이트되었습니다" },
                { "Setup.DoneSubtitle", "이제 충전기를 뽑으면 노트북이 잠깁니다." },
                { "Setup.DoneRunning", "시계 옆 알림 영역에서 실행 중입니다. 아이콘이 보이지 않으면 그곳의 ^ 화살표를 클릭하세요." },
                { "Setup.DoneNotRunning", "준비되면 시작 메뉴에서 여세요. 시계 옆 알림 영역에 상주합니다." },
                { "Setup.DoneHint", "아이콘을 마우스 오른쪽 단추로 클릭하여 잠금 지연을 설정하거나, 암호를 추가하거나, 잠시 끌 수 있습니다." },
                { "Setup.AuthUpdate", "UnplugLock이 암호로 보호되어 있습니다. 업데이트하려면 암호를 입력하세요." },
                { "Setup.AuthUninstall", "UnplugLock이 암호로 보호되어 있습니다. 제거하려면 암호를 입력하세요." },
                { "Setup.StillRunning", "UnplugLock이 아직 실행 중이며 닫히지 않았습니다. 트레이 메뉴에서 종료한 다음 설치를 다시 실행하세요." },
                { "Setup.Damaged", "이 설치 파일이 손상되었습니다(UnplugLock.exe가 없음)." },
                { "Setup.ShortcutDescription", "충전기를 뽑으면 노트북을 잠급니다" },
                { "Uninstall.Title", "UnplugLock 제거" },
                { "Uninstall.Confirm", "이 PC에서 UnplugLock과 모든 설정을 제거할까요?" },
                { "Uninstall.StillRunning", "UnplugLock이 아직 실행 중이며 닫히지 않았습니다. 트레이 메뉴에서 종료한 다음 다시 제거하세요." },
                { "Uninstall.Done", "UnplugLock을 제거했습니다." },
            };
        }
    }
}
