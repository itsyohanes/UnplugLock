// Spanish
using System.Collections.Generic;

namespace UnplugLock
{
    internal static partial class Lang
    {
        private static Dictionary<string, string> Spanish()
        {
            return new Dictionary<string, string>
            {
                { "LanguageName", "Español" },

                // shared
                { "AppTagline", "Bloquea tu portátil en cuanto se desconecta el cargador." },
                { "Version", "Versión {0}" },
                { "Button.Cancel", "Cancelar" },
                { "Button.Continue", "Continuar" },
                { "Button.Close", "Cerrar" },
                { "Button.Done", "Listo" },
                { "WrongPassword", "Contraseña incorrecta." },
                { "AlreadyRunning", "UnplugLock ya se está ejecutando." },

                // password prompt
                { "Pw.TitleEnter", "Escribe tu contraseña" },
                { "Pw.TitleChoose", "Elige una contraseña" },
                { "Pw.TitleRecovery", "Escribe tu código de recuperación" },
                { "Pw.Label", "Contraseña" },
                { "Pw.LabelNew", "Nueva contraseña" },
                { "Pw.LabelRecovery", "Código de recuperación" },
                { "Pw.LabelConfirm", "Confirmar contraseña" },
                { "Pw.Hint", "Al menos {0} caracteres. No reutilices tu contraseña o PIN de Windows." },
                { "Pw.Forgot", "¿Olvidaste la contraseña?" },
                { "Pw.Set", "Establecer contraseña" },
                { "Pw.TooShort", "Usa al menos {0} caracteres." },
                { "Pw.Mismatch", "Las contraseñas no coinciden." },
                { "Pw.NewPrompt", "La necesitarás para desactivar UnplugLock, cambiar su configuración o salir de él." },
                { "Pw.RecoveryPrompt", "Escribe tu código de recuperación. Quita tu contraseña de UnplugLock para que puedas establecer una nueva." },
                { "Pw.RecoveryWrong", "Ese código de recuperación no es correcto." },

                // what the password is asked for
                { "Auth.TurnOff", "Escribe tu contraseña de UnplugLock para desactivarlo." },
                { "Auth.StopEthernet", "Escribe tu contraseña de UnplugLock para dejar de bloquear al desconectar el cable Ethernet." },
                { "Auth.StopWifi", "Escribe tu contraseña de UnplugLock para dejar de bloquear cuando se desconecte la wifi." },
                { "Auth.Delay", "Escribe tu contraseña de UnplugLock para cambiar el retraso de bloqueo." },
                { "Auth.Guard", "Escribe tu contraseña de UnplugLock para desactivar la protección del proceso." },
                { "Auth.Startup", "Escribe tu contraseña de UnplugLock para que deje de iniciarse con Windows." },
                { "Auth.ChangePassword", "Escribe tu contraseña de UnplugLock para cambiar la contraseña." },
                { "Auth.RemovePassword", "Escribe tu contraseña de UnplugLock para quitar la contraseña." },
                { "Auth.Exit", "Escribe tu contraseña de UnplugLock para salir." },
                { "Auth.Recovered", "Código de recuperación aceptado. Se ha quitado tu contraseña. Establece una nueva desde el menú Contraseña." },

                // recovery code window
                { "Rc.Title", "Guarda tu código de recuperación" },
                { "Rc.Subtitle", "Es la única forma de volver a entrar si olvidas tu contraseña." },
                { "Rc.Explain", "Si olvidas tu contraseña, haz clic en «{0}» en la ventana de contraseña y escribe este código. Quita la contraseña para que puedas establecer una nueva." },
                { "Rc.Keep", "Guárdalo en otro sitio que no sea este portátil, como tu teléfono, un administrador de contraseñas o en papel. No se volverá a mostrar." },
                { "Rc.Copy", "Copiar" },
                { "Rc.Copied", "Copiado" },
                { "Rc.Save", "Guardar en archivo..." },
                { "Rc.Confirm", "He guardado mi código de recuperación" },
                { "Rc.FileTitle", "Código de recuperación de UnplugLock" },
                { "Rc.FileType", "Archivo de texto" },
                { "Rc.FileHelp", "Si olvidas tu contraseña de UnplugLock, haz clic en «{0}» en la ventana de contraseña y escribe este código." },
                { "Rc.FileCreated", "Creado el {0}." },
                { "Rc.DateFormat", "d 'de' MMMM 'de' yyyy" },
                { "Rc.SaveFailed", "No se pudo guardar el archivo:\n{0}" },
                { "Rc.SavedOnLaptop", "Guardado. Este archivo está en el disco del portátil, así que quien se lleve el portátil podría encontrarlo y usar el código para quitar tu contraseña.\n\nCópialo a una memoria USB, a tu teléfono o a un administrador de contraseñas y luego elimínalo de este portátil." },

                // about window
                { "About.Window", "Acerca de UnplugLock" },
                { "About.Source", "Código fuente en GitHub" },
                { "About.Releases", "Buscar nuevas versiones" },
                { "About.Support", "Apoyar UnplugLock" },
                { "About.License", "Gratuito y de código abierto, licencia MIT" },

                // tray menu
                { "Menu.Enabled", "Activado" },
                { "Menu.Delay", "Retraso de bloqueo" },
                { "Delay.Instant", "Inmediato" },
                { "Delay.One", "{0} segundo" },
                { "Delay.Other", "{0} segundos" },
                { "Menu.AlsoLock", "Bloquear también cuando" },
                { "Menu.Ethernet", "se desconecte el cable Ethernet" },
                { "Menu.Wifi", "se desconecte la wifi" },
                { "Menu.Startup", "Iniciar con Windows" },
                { "Menu.StartupGuarded", "Iniciar con Windows (necesario para la protección)" },
                { "Menu.Guard", "Proteger el proceso (reinicio automático)" },
                { "Menu.Hide", "Ocultar el icono de la bandeja" },
                { "Menu.Password", "Contraseña" },
                { "Menu.SetPassword", "Establecer contraseña..." },
                { "Menu.ChangePassword", "Cambiar contraseña..." },
                { "Menu.RemovePassword", "Quitar contraseña" },
                { "Menu.LockNow", "Bloquear ahora" },
                { "Menu.About", "Acerca de UnplugLock..." },
                { "Menu.Exit", "Salir" },
                { "Menu.NoSignIn", "Windows no tiene contraseña - configura una..." },
                { "Power.Online", "Conectado a la corriente" },
                { "Power.Offline", "Con batería" },
                { "Power.Unknown", "Estado de energía desconocido" },

                // tray icon tooltip (63 characters at most)
                { "Tip.On", "UnplugLock - activado ({0})" },
                { "Tip.Off", "UnplugLock - desactivado ({0})" },
                { "Tip.Online", "conectado" },
                { "Tip.Offline", "con batería" },
                { "Tip.Unknown", "energía desconocida" },
                { "Tip.NoSignIn", "UnplugLock - Windows no tiene contraseña, bloquear no sirve" },

                // tray messages
                { "NoSignIn.Title", "Tu cuenta de Windows no tiene contraseña" },
                { "NoSignIn.Text", "UnplugLock puede bloquear la pantalla, pero cualquiera puede volver a entrar con un clic. Haz clic aquí para configurar una contraseña o un PIN." },
                { "Running.Title", "UnplugLock se está ejecutando" },
                { "Running.On", "Tu portátil se bloquea al desconectarlo. Haz clic con el botón derecho en el icono para ver las opciones." },
                { "Running.Off", "UnplugLock está desactivado. Haz clic con el botón derecho en el icono para ver las opciones." },
                { "LockFailed", "Windows no se bloqueó (error {0}). Cuando se desconecta el cargador, UnplugLock lo sigue intentando durante 10 minutos y bloquea en cuanto Windows lo permite." },
                { "StartupFailed", "No se pudo cambiar la configuración de inicio:\n{0}" },
                { "HideConfirm", "El icono de la bandeja se ocultará, pero UnplugLock seguirá ejecutándose y protegiendo tu portátil.\n\nPara volver a mostrar el icono, abre UnplugLock de nuevo (desde el menú Inicio o UnplugLock.exe)." },
                { "PasswordRemoved", "Contraseña quitada." },

                // installer
                { "Setup.Window", "Instalación de UnplugLock" },
                { "Setup.Language", "Idioma" },
                { "Setup.TitleInstall", "Instalar UnplugLock" },
                { "Setup.TitleUpdate", "Actualizar UnplugLock" },
                { "Setup.Guard", "Mantener UnplugLock en ejecución" },
                { "Setup.GuardHint", "Si alguien lo cierra, vuelve a iniciarse al instante." },
                { "Setup.Startup", "Iniciar UnplugLock al iniciar sesión en Windows" },
                { "Setup.Desktop", "Crear un acceso directo en el escritorio" },
                { "Setup.Launch", "Abrir UnplugLock al terminar la instalación" },
                { "Setup.InstallsTo", "Se instala en" },
                { "Setup.PerUser", "Solo para tu cuenta, así que no necesita permisos de administrador." },
                { "Setup.Install", "Instalar" },
                { "Setup.Update", "Actualizar" },
                { "Setup.Installing", "Instalando..." },
                { "Setup.Updating", "Actualizando..." },
                { "Setup.Finish", "Finalizar" },
                { "Setup.Failed", "No se pudo completar la instalación:\n\n{0}" },
                { "Setup.Installed", "UnplugLock está instalado" },
                { "Setup.Updated", "UnplugLock está actualizado" },
                { "Setup.DoneSubtitle", "Tu portátil ahora se bloquea cuando se desconecta el cargador." },
                { "Setup.DoneRunning", "Se está ejecutando en la bandeja del sistema, junto al reloj. Si no ves el icono, haz clic en la flecha ^ de esa zona." },
                { "Setup.DoneNotRunning", "Ábrelo desde el menú Inicio cuando quieras. Se queda en la bandeja del sistema, junto al reloj." },
                { "Setup.DoneHint", "Haz clic con el botón derecho en el icono para elegir un retraso de bloqueo, añadir una contraseña o desactivarlo un rato." },
                { "Setup.AuthUpdate", "UnplugLock está protegido con contraseña. Escribe la contraseña para actualizarlo." },
                { "Setup.AuthUninstall", "UnplugLock está protegido con contraseña. Escribe la contraseña para desinstalarlo." },
                { "Setup.StillRunning", "UnplugLock sigue en ejecución y no se pudo cerrar. Sal desde su menú en la bandeja y vuelve a ejecutar la instalación." },
                { "Setup.Damaged", "Este archivo de instalación está dañado (le falta UnplugLock.exe)." },
                { "Setup.ShortcutDescription", "Bloquea tu portátil cuando se desconecta el cargador" },
                { "Uninstall.Title", "Desinstalar UnplugLock" },
                { "Uninstall.Confirm", "¿Quitar UnplugLock y toda su configuración de este PC?" },
                { "Uninstall.StillRunning", "UnplugLock sigue en ejecución y no se pudo cerrar. Sal desde su menú en la bandeja y vuelve a desinstalarlo." },
                { "Uninstall.Done", "UnplugLock se ha desinstalado." },
            };
        }
    }
}
