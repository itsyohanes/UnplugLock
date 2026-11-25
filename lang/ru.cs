// Russian
using System.Collections.Generic;

namespace UnplugLock
{
    internal static partial class Lang
    {
        private static Dictionary<string, string> Russian()
        {
            return new Dictionary<string, string>
            {
                { "LanguageName", "Русский" },

                // shared
                { "AppTagline", "Блокирует ноутбук, как только отключается зарядное устройство." },
                { "Version", "Версия {0}" },
                { "Button.Cancel", "Отмена" },
                { "Button.Continue", "Продолжить" },
                { "Button.Close", "Закрыть" },
                { "Button.Done", "Готово" },
                { "WrongPassword", "Неверный пароль." },
                { "AlreadyRunning", "UnplugLock уже запущен." },

                // password prompt
                { "Pw.TitleEnter", "Введите пароль" },
                { "Pw.TitleChoose", "Придумайте пароль" },
                { "Pw.TitleRecovery", "Введите код восстановления" },
                { "Pw.Label", "Пароль" },
                { "Pw.LabelNew", "Новый пароль" },
                { "Pw.LabelRecovery", "Код восстановления" },
                { "Pw.LabelConfirm", "Подтвердите пароль" },
                { "Pw.Hint", "Не менее {0} символов. Не используйте пароль или ПИН-код Windows." },
                { "Pw.Forgot", "Забыли пароль?" },
                { "Pw.Set", "Задать пароль" },
                { "Pw.TooShort", "Используйте не менее {0} символов." },
                { "Pw.Mismatch", "Пароли не совпадают." },
                { "Pw.NewPrompt", "Он понадобится, чтобы выключить UnplugLock, изменить его настройки или закрыть его." },
                { "Pw.RecoveryPrompt", "Введите код восстановления. Он удалит пароль UnplugLock, чтобы вы могли задать новый." },
                { "Pw.RecoveryWrong", "Неверный код восстановления." },

                // what the password is asked for
                { "Auth.TurnOff", "Введите пароль UnplugLock, чтобы выключить его." },
                { "Auth.StopEthernet", "Введите пароль UnplugLock, чтобы не блокировать при отключении кабеля Ethernet." },
                { "Auth.StopWifi", "Введите пароль UnplugLock, чтобы не блокировать при отключении Wi-Fi." },
                { "Auth.Delay", "Введите пароль UnplugLock, чтобы изменить задержку блокировки." },
                { "Auth.Guard", "Введите пароль UnplugLock, чтобы выключить защиту процесса." },
                { "Auth.Startup", "Введите пароль UnplugLock, чтобы он больше не запускался вместе с Windows." },
                { "Auth.ChangePassword", "Введите пароль UnplugLock, чтобы изменить пароль." },
                { "Auth.RemovePassword", "Введите пароль UnplugLock, чтобы удалить пароль." },
                { "Auth.Exit", "Введите пароль UnplugLock, чтобы закрыть его." },
                { "Auth.Recovered", "Код восстановления принят. Пароль удалён. Задайте новый в меню «Пароль»." },

                // recovery code window
                { "Rc.Title", "Сохраните код восстановления" },
                { "Rc.Subtitle", "Это единственный способ войти, если вы забудете пароль." },
                { "Rc.Explain", "Если вы забудете пароль, нажмите «{0}» в окне ввода пароля и введите этот код. Он удалит пароль, чтобы вы могли задать новый." },
                { "Rc.Keep", "Храните его не на этом ноутбуке: например, в телефоне, менеджере паролей или на бумаге. Больше он показан не будет." },
                { "Rc.Copy", "Копировать" },
                { "Rc.Copied", "Скопировано" },
                { "Rc.Save", "Сохранить в файл..." },
                { "Rc.Confirm", "Код восстановления сохранён" },
                { "Rc.FileTitle", "Код восстановления UnplugLock" },
                { "Rc.FileType", "Текстовый файл" },
                { "Rc.FileHelp", "Если вы забудете пароль UnplugLock, нажмите «{0}» в окне ввода пароля и введите этот код." },
                { "Rc.FileCreated", "Создан {0}." },
                { "Rc.DateFormat", "d MMMM yyyy" },
                { "Rc.SaveFailed", "Не удалось сохранить файл:\n{0}" },
                { "Rc.SavedOnLaptop", "Сохранено. Файл находится на диске ноутбука, поэтому тот, кто заберёт ноутбук, сможет его найти и с помощью кода удалить ваш пароль.\n\nСкопируйте его на флешку, в телефон или менеджер паролей, а затем удалите с этого ноутбука." },

                // about window
                { "About.Window", "О программе UnplugLock" },
                { "About.Source", "Исходный код на GitHub" },
                { "About.Releases", "Проверить новые версии" },
                { "About.Support", "Поддержать UnplugLock" },
                { "About.License", "Бесплатно и с открытым кодом, лицензия MIT" },

                // tray menu
                { "Menu.Enabled", "Включено" },
                { "Menu.Delay", "Задержка блокировки" },
                { "Delay.Instant", "Сразу" },
                { "Delay.One", "{0} секунда" },
                { "Delay.Few", "{0} секунды" },
                { "Delay.Other", "{0} секунд" },
                { "Menu.AlsoLock", "Блокировать также, когда" },
                { "Menu.Ethernet", "отключён кабель Ethernet" },
                { "Menu.Wifi", "отключается Wi-Fi" },
                { "Menu.Startup", "Запускать вместе с Windows" },
                { "Menu.StartupGuarded", "Запускать вместе с Windows (нужно для защиты)" },
                { "Menu.Guard", "Защита процесса (автоперезапуск)" },
                { "Menu.Hide", "Скрыть значок в области уведомлений" },
                { "Menu.Password", "Пароль" },
                { "Menu.SetPassword", "Задать пароль..." },
                { "Menu.ChangePassword", "Изменить пароль..." },
                { "Menu.RemovePassword", "Удалить пароль" },
                { "Menu.LockNow", "Заблокировать сейчас" },
                { "Menu.About", "О программе UnplugLock..." },
                { "Menu.Exit", "Выход" },
                { "Menu.NoSignIn", "У Windows нет пароля - задать его..." },
                { "Power.Online", "Питание от сети" },
                { "Power.Offline", "Питание от батареи" },
                { "Power.Unknown", "Источник питания неизвестен" },

                // tray icon tooltip (63 characters at most)
                { "Tip.On", "UnplugLock - вкл. ({0})" },
                { "Tip.Off", "UnplugLock - выкл. ({0})" },
                { "Tip.Online", "от сети" },
                { "Tip.Offline", "от батареи" },
                { "Tip.Unknown", "питание неизвестно" },
                { "Tip.NoSignIn", "UnplugLock - у Windows нет пароля, блокировка бесполезна" },

                // tray messages
                { "NoSignIn.Title", "У вашей учётной записи Windows нет пароля" },
                { "NoSignIn.Text", "UnplugLock может заблокировать экран, но любой сможет войти одним щелчком. Щёлкните здесь, чтобы задать пароль или ПИН-код." },
                { "Running.Title", "UnplugLock работает" },
                { "Running.On", "Ноутбук блокируется при отключении от сети. Щёлкните значок правой кнопкой мыши, чтобы открыть параметры." },
                { "Running.Off", "UnplugLock выключен. Щёлкните значок правой кнопкой мыши, чтобы открыть параметры." },
                { "LockFailed", "Windows не заблокировалась (ошибка {0}). Когда зарядное устройство отключают, UnplugLock пытается ещё 10 минут и блокирует, как только Windows позволит." },
                { "StartupFailed", "Не удалось изменить параметр автозапуска:\n{0}" },
                { "HideConfirm", "Значок в области уведомлений будет скрыт, но UnplugLock продолжит работать и защищать ноутбук.\n\nЧтобы снова показать значок, ещё раз откройте UnplugLock (из меню «Пуск» или UnplugLock.exe)." },
                { "PasswordRemoved", "Пароль удалён." },

                // installer
                { "Setup.Window", "Установка UnplugLock" },
                { "Setup.Language", "Язык" },
                { "Setup.TitleInstall", "Установить UnplugLock" },
                { "Setup.TitleUpdate", "Обновить UnplugLock" },
                { "Setup.Guard", "Не давать закрыть UnplugLock" },
                { "Setup.GuardHint", "Если его закроют, он сразу запустится снова." },
                { "Setup.Startup", "Запускать UnplugLock при входе в Windows" },
                { "Setup.Desktop", "Создать ярлык на рабочем столе" },
                { "Setup.Launch", "Открыть UnplugLock после установки" },
                { "Setup.InstallsTo", "Папка установки" },
                { "Setup.PerUser", "Только для вашей учётной записи, поэтому права администратора не нужны." },
                { "Setup.Install", "Установить" },
                { "Setup.Update", "Обновить" },
                { "Setup.Installing", "Установка..." },
                { "Setup.Updating", "Обновление..." },
                { "Setup.Finish", "Готово" },
                { "Setup.Failed", "Не удалось завершить установку:\n\n{0}" },
                { "Setup.Installed", "UnplugLock установлен" },
                { "Setup.Updated", "UnplugLock обновлён" },
                { "Setup.DoneSubtitle", "Теперь ноутбук блокируется, когда отключают зарядное устройство." },
                { "Setup.DoneRunning", "Он работает в области уведомлений рядом с часами. Если значка не видно, нажмите там стрелку ^." },
                { "Setup.DoneNotRunning", "Откройте его из меню «Пуск», когда будете готовы. Он находится в области уведомлений рядом с часами." },
                { "Setup.DoneHint", "Щёлкните значок правой кнопкой мыши, чтобы задать задержку, добавить пароль или ненадолго выключить его." },
                { "Setup.AuthUpdate", "UnplugLock защищён паролем. Введите пароль, чтобы обновить его." },
                { "Setup.AuthUninstall", "UnplugLock защищён паролем. Введите пароль, чтобы удалить его." },
                { "Setup.StillRunning", "UnplugLock всё ещё работает и не закрылся. Закройте его через меню в области уведомлений и запустите установку снова." },
                { "Setup.Damaged", "Файл установки повреждён (в нём нет UnplugLock.exe)." },
                { "Setup.ShortcutDescription", "Блокирует ноутбук при отключении зарядного устройства" },
                { "Uninstall.Title", "Удаление UnplugLock" },
                { "Uninstall.Confirm", "Удалить UnplugLock и все его настройки с этого компьютера?" },
                { "Uninstall.StillRunning", "UnplugLock всё ещё работает и не закрылся. Закройте его через меню в области уведомлений и повторите удаление." },
                { "Uninstall.Done", "UnplugLock удалён." },
            };
        }
    }
}
