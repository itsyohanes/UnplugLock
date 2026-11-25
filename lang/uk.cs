// Ukrainian
using System.Collections.Generic;

namespace UnplugLock
{
    internal static partial class Lang
    {
        private static Dictionary<string, string> Ukrainian()
        {
            return new Dictionary<string, string>
            {
                { "LanguageName", "Українська" },

                // shared
                { "AppTagline", "Блокує ноутбук, щойно від'єднано зарядний пристрій." },
                { "Version", "Версія {0}" },
                { "Button.Cancel", "Скасувати" },
                { "Button.Continue", "Продовжити" },
                { "Button.Close", "Закрити" },
                { "Button.Done", "Готово" },
                { "WrongPassword", "Неправильний пароль." },
                { "AlreadyRunning", "UnplugLock уже запущено." },

                // password prompt
                { "Pw.TitleEnter", "Введіть пароль" },
                { "Pw.TitleChoose", "Придумайте пароль" },
                { "Pw.TitleRecovery", "Введіть код відновлення" },
                { "Pw.Label", "Пароль" },
                { "Pw.LabelNew", "Новий пароль" },
                { "Pw.LabelRecovery", "Код відновлення" },
                { "Pw.LabelConfirm", "Підтвердіть пароль" },
                { "Pw.Hint", "Щонайменше {0} символів. Не використовуйте пароль або PIN-код Windows." },
                { "Pw.Forgot", "Забули пароль?" },
                { "Pw.Set", "Установити пароль" },
                { "Pw.TooShort", "Використовуйте щонайменше {0} символів." },
                { "Pw.Mismatch", "Паролі не збігаються." },
                { "Pw.NewPrompt", "Він знадобиться, щоб вимкнути UnplugLock, змінити його налаштування або закрити його." },
                { "Pw.RecoveryPrompt", "Введіть код відновлення. Він видалить пароль UnplugLock, щоб ви могли встановити новий." },
                { "Pw.RecoveryWrong", "Неправильний код відновлення." },

                // what the password is asked for
                { "Auth.TurnOff", "Введіть пароль UnplugLock, щоб вимкнути його." },
                { "Auth.StopEthernet", "Введіть пароль UnplugLock, щоб не блокувати під час від'єднання кабелю Ethernet." },
                { "Auth.StopWifi", "Введіть пароль UnplugLock, щоб не блокувати під час відключення Wi-Fi." },
                { "Auth.Delay", "Введіть пароль UnplugLock, щоб змінити затримку блокування." },
                { "Auth.Guard", "Введіть пароль UnplugLock, щоб вимкнути захист процесу." },
                { "Auth.Startup", "Введіть пароль UnplugLock, щоб він більше не запускався разом із Windows." },
                { "Auth.ChangePassword", "Введіть пароль UnplugLock, щоб змінити пароль." },
                { "Auth.RemovePassword", "Введіть пароль UnplugLock, щоб видалити пароль." },
                { "Auth.Exit", "Введіть пароль UnplugLock, щоб закрити його." },
                { "Auth.Recovered", "Код відновлення прийнято. Пароль видалено. Установіть новий у меню «Пароль»." },

                // recovery code window
                { "Rc.Title", "Збережіть код відновлення" },
                { "Rc.Subtitle", "Це єдиний спосіб увійти, якщо ви забудете пароль." },
                { "Rc.Explain", "Якщо ви забудете пароль, натисніть «{0}» у вікні введення пароля та введіть цей код. Він видалить пароль, щоб ви могли встановити новий." },
                { "Rc.Keep", "Зберігайте його не на цьому ноутбуці: наприклад, у телефоні, менеджері паролів або на папері. Більше він не з'явиться." },
                { "Rc.Copy", "Копіювати" },
                { "Rc.Copied", "Скопійовано" },
                { "Rc.Save", "Зберегти у файл..." },
                { "Rc.Confirm", "Код відновлення збережено" },
                { "Rc.FileTitle", "Код відновлення UnplugLock" },
                { "Rc.FileType", "Текстовий файл" },
                { "Rc.FileHelp", "Якщо ви забудете пароль UnplugLock, натисніть «{0}» у вікні введення пароля та введіть цей код." },
                { "Rc.FileCreated", "Створено {0}." },
                { "Rc.DateFormat", "d MMMM yyyy" },
                { "Rc.SaveFailed", "Не вдалося зберегти файл:\n{0}" },
                { "Rc.SavedOnLaptop", "Збережено. Файл лежить на диску ноутбука, тож той, хто забере ноутбук, зможе його знайти й за допомогою коду видалити ваш пароль.\n\nСкопіюйте його на флешку, у телефон або менеджер паролів, а потім видаліть із цього ноутбука." },

                // about window
                { "About.Window", "Про UnplugLock" },
                { "About.Source", "Вихідний код на GitHub" },
                { "About.Releases", "Перевірити нові версії" },
                { "About.Support", "Підтримати UnplugLock" },
                { "About.License", "Безкоштовно й з відкритим кодом, ліцензія MIT" },

                // tray menu
                { "Menu.Enabled", "Увімкнено" },
                { "Menu.Delay", "Затримка блокування" },
                { "Delay.Instant", "Одразу" },
                { "Delay.One", "{0} секунда" },
                { "Delay.Few", "{0} секунди" },
                { "Delay.Other", "{0} секунд" },
                { "Menu.AlsoLock", "Блокувати також, коли" },
                { "Menu.Ethernet", "від'єднано кабель Ethernet" },
                { "Menu.Wifi", "вимикається Wi-Fi" },
                { "Menu.Startup", "Запускати разом із Windows" },
                { "Menu.StartupGuarded", "Запускати разом із Windows (потрібно для захисту)" },
                { "Menu.Guard", "Захист процесу (автоперезапуск)" },
                { "Menu.Hide", "Сховати значок в області сповіщень" },
                { "Menu.Password", "Пароль" },
                { "Menu.SetPassword", "Установити пароль..." },
                { "Menu.ChangePassword", "Змінити пароль..." },
                { "Menu.RemovePassword", "Видалити пароль" },
                { "Menu.LockNow", "Заблокувати зараз" },
                { "Menu.About", "Про UnplugLock..." },
                { "Menu.Exit", "Вихід" },
                { "Menu.NoSignIn", "Windows не має пароля - установити його..." },
                { "Power.Online", "Живлення від мережі" },
                { "Power.Offline", "Живлення від батареї" },
                { "Power.Unknown", "Джерело живлення невідоме" },

                // tray icon tooltip (63 characters at most)
                { "Tip.On", "UnplugLock - увімк. ({0})" },
                { "Tip.Off", "UnplugLock - вимк. ({0})" },
                { "Tip.Online", "від мережі" },
                { "Tip.Offline", "від батареї" },
                { "Tip.Unknown", "живлення невідоме" },
                { "Tip.NoSignIn", "UnplugLock - Windows без пароля, блокування марне" },

                // tray messages
                { "NoSignIn.Title", "Ваш обліковий запис Windows не має пароля" },
                { "NoSignIn.Text", "UnplugLock може заблокувати екран, але будь-хто зможе увійти одним клацанням. Клацніть тут, щоб установити пароль або PIN-код." },
                { "Running.Title", "UnplugLock працює" },
                { "Running.On", "Ноутбук блокується, коли його від'єднують від мережі. Клацніть значок правою кнопкою миші, щоб відкрити параметри." },
                { "Running.Off", "UnplugLock вимкнено. Клацніть значок правою кнопкою миші, щоб відкрити параметри." },
                { "LockFailed", "Windows не заблоковано (помилка {0}). Коли від'єднують зарядний пристрій, UnplugLock пробує ще 10 хвилин і блокує, щойно Windows дозволить." },
                { "StartupFailed", "Не вдалося змінити параметр автозапуску:\n{0}" },
                { "HideConfirm", "Значок в області сповіщень буде приховано, але UnplugLock і далі працюватиме й захищатиме ноутбук.\n\nЩоб знову показати значок, ще раз відкрийте UnplugLock (з меню «Пуск» або UnplugLock.exe)." },
                { "PasswordRemoved", "Пароль видалено." },

                // installer
                { "Setup.Window", "Інсталяція UnplugLock" },
                { "Setup.Language", "Мова" },
                { "Setup.TitleInstall", "Інсталювати UnplugLock" },
                { "Setup.TitleUpdate", "Оновити UnplugLock" },
                { "Setup.Guard", "Не дозволяти закрити UnplugLock" },
                { "Setup.GuardHint", "Якщо його закриють, він одразу запуститься знову." },
                { "Setup.Startup", "Запускати UnplugLock під час входу в Windows" },
                { "Setup.Desktop", "Створити ярлик на робочому столі" },
                { "Setup.Launch", "Відкрити UnplugLock після інсталяції" },
                { "Setup.InstallsTo", "Папка інсталяції" },
                { "Setup.PerUser", "Лише для вашого облікового запису, тому права адміністратора не потрібні." },
                { "Setup.Install", "Інсталювати" },
                { "Setup.Update", "Оновити" },
                { "Setup.Installing", "Інсталяція..." },
                { "Setup.Updating", "Оновлення..." },
                { "Setup.Finish", "Готово" },
                { "Setup.Failed", "Не вдалося завершити інсталяцію:\n\n{0}" },
                { "Setup.Installed", "UnplugLock інстальовано" },
                { "Setup.Updated", "UnplugLock оновлено" },
                { "Setup.DoneSubtitle", "Тепер ноутбук блокується, коли від'єднують зарядний пристрій." },
                { "Setup.DoneRunning", "Він працює в області сповіщень біля годинника. Якщо значка не видно, натисніть там стрілку ^." },
                { "Setup.DoneNotRunning", "Відкрийте його з меню «Пуск», коли будете готові. Він працюватиме в області сповіщень біля годинника." },
                { "Setup.DoneHint", "Клацніть значок правою кнопкою миші, щоб задати затримку, додати пароль або ненадовго вимкнути його." },
                { "Setup.AuthUpdate", "UnplugLock захищено паролем. Введіть пароль, щоб оновити його." },
                { "Setup.AuthUninstall", "UnplugLock захищено паролем. Введіть пароль, щоб видалити його." },
                { "Setup.StillRunning", "UnplugLock досі працює й не закрився. Закрийте його через меню в області сповіщень і знову запустіть інсталяцію." },
                { "Setup.Damaged", "Файл інсталяції пошкоджено (у ньому немає UnplugLock.exe)." },
                { "Setup.ShortcutDescription", "Блокує ноутбук, коли від'єднано зарядний пристрій" },
                { "Uninstall.Title", "Видалення UnplugLock" },
                { "Uninstall.Confirm", "Видалити UnplugLock і всі його налаштування з цього комп'ютера?" },
                { "Uninstall.StillRunning", "UnplugLock досі працює й не закрився. Закрийте його через меню в області сповіщень і повторіть видалення." },
                { "Uninstall.Done", "UnplugLock видалено." },
            };
        }
    }
}
