// Portuguese (Brazilian wording; Portugal gets it too)
using System.Collections.Generic;

namespace UnplugLock
{
    internal static partial class Lang
    {
        private static Dictionary<string, string> Portuguese()
        {
            return new Dictionary<string, string>
            {
                { "LanguageName", "Português" },

                // shared
                { "AppTagline", "Bloqueia o notebook assim que o carregador é desconectado." },
                { "Version", "Versão {0}" },
                { "Button.Cancel", "Cancelar" },
                { "Button.Continue", "Continuar" },
                { "Button.Close", "Fechar" },
                { "Button.Done", "Concluído" },
                { "WrongPassword", "Senha incorreta." },
                { "AlreadyRunning", "O UnplugLock já está em execução." },

                // password prompt
                { "Pw.TitleEnter", "Digite sua senha" },
                { "Pw.TitleChoose", "Escolha uma senha" },
                { "Pw.TitleRecovery", "Digite seu código de recuperação" },
                { "Pw.Label", "Senha" },
                { "Pw.LabelNew", "Nova senha" },
                { "Pw.LabelRecovery", "Código de recuperação" },
                { "Pw.LabelConfirm", "Confirmar senha" },
                { "Pw.Hint", "Pelo menos {0} caracteres. Não reutilize sua senha ou PIN do Windows." },
                { "Pw.Forgot", "Esqueceu a senha?" },
                { "Pw.Set", "Definir senha" },
                { "Pw.TooShort", "Use pelo menos {0} caracteres." },
                { "Pw.Mismatch", "As senhas não coincidem." },
                { "Pw.NewPrompt", "Você vai precisar dela para desativar o UnplugLock, alterar as configurações ou sair dele." },
                { "Pw.RecoveryPrompt", "Digite seu código de recuperação. Ele remove sua senha do UnplugLock para que você possa definir uma nova." },
                { "Pw.RecoveryWrong", "Esse código de recuperação não está correto." },

                // what the password is asked for
                { "Auth.TurnOff", "Digite sua senha do UnplugLock para desativá-lo." },
                { "Auth.StopEthernet", "Digite sua senha do UnplugLock para parar de bloquear quando o cabo Ethernet for desconectado." },
                { "Auth.StopWifi", "Digite sua senha do UnplugLock para parar de bloquear quando o Wi-Fi desconectar." },
                { "Auth.Delay", "Digite sua senha do UnplugLock para alterar o atraso do bloqueio." },
                { "Auth.Guard", "Digite sua senha do UnplugLock para desativar a proteção do processo." },
                { "Auth.Startup", "Digite sua senha do UnplugLock para que ele não inicie mais com o Windows." },
                { "Auth.ChangePassword", "Digite sua senha do UnplugLock para alterar a senha." },
                { "Auth.RemovePassword", "Digite sua senha do UnplugLock para remover a senha." },
                { "Auth.Exit", "Digite sua senha do UnplugLock para sair." },
                { "Auth.Recovered", "Código de recuperação aceito. Sua senha foi removida. Defina uma nova no menu Senha." },

                // recovery code window
                { "Rc.Title", "Salve seu código de recuperação" },
                { "Rc.Subtitle", "É a única forma de entrar de novo se você esquecer a senha." },
                { "Rc.Explain", "Se você esquecer a senha, clique em \"{0}\" na janela de senha e digite este código. Ele remove a senha para que você possa definir uma nova." },
                { "Rc.Keep", "Guarde-o em outro lugar que não seja este notebook, como no celular, em um gerenciador de senhas ou no papel. Ele não será mostrado de novo." },
                { "Rc.Copy", "Copiar" },
                { "Rc.Copied", "Copiado" },
                { "Rc.Save", "Salvar em arquivo..." },
                { "Rc.Confirm", "Salvei meu código de recuperação" },
                { "Rc.FileTitle", "Código de recuperação do UnplugLock" },
                { "Rc.FileType", "Arquivo de texto" },
                { "Rc.FileHelp", "Se você esquecer a senha do UnplugLock, clique em \"{0}\" na janela de senha e digite este código." },
                { "Rc.FileCreated", "Criado em {0}." },
                { "Rc.DateFormat", "d 'de' MMMM 'de' yyyy" },
                { "Rc.SaveFailed", "Não foi possível salvar o arquivo:\n{0}" },
                { "Rc.SavedOnLaptop", "Salvo. Este arquivo está no disco do notebook, então quem levar o notebook pode encontrá-lo e usar o código para remover sua senha.\n\nCopie-o para um pendrive, o celular ou um gerenciador de senhas e depois exclua-o deste notebook." },

                // about window
                { "About.Window", "Sobre o UnplugLock" },
                { "About.Source", "Código-fonte no GitHub" },
                { "About.Releases", "Procurar novas versões" },
                { "About.Support", "Apoiar o UnplugLock" },
                { "About.License", "Gratuito e de código aberto, licença MIT" },

                // tray menu
                { "Menu.Enabled", "Ativado" },
                { "Menu.Delay", "Atraso do bloqueio" },
                { "Delay.Instant", "Imediato" },
                { "Delay.One", "{0} segundo" },
                { "Delay.Other", "{0} segundos" },
                { "Menu.AlsoLock", "Bloquear também quando" },
                { "Menu.Ethernet", "o cabo Ethernet for desconectado" },
                { "Menu.Wifi", "o Wi-Fi desconectar" },
                { "Menu.Startup", "Iniciar com o Windows" },
                { "Menu.StartupGuarded", "Iniciar com o Windows (exigido pela proteção)" },
                { "Menu.Guard", "Proteger o processo (reinício automático)" },
                { "Menu.Hide", "Ocultar ícone da bandeja" },
                { "Menu.Password", "Senha" },
                { "Menu.SetPassword", "Definir senha..." },
                { "Menu.ChangePassword", "Alterar senha..." },
                { "Menu.RemovePassword", "Remover senha" },
                { "Menu.LockNow", "Bloquear agora" },
                { "Menu.About", "Sobre o UnplugLock..." },
                { "Menu.Exit", "Sair" },
                { "Menu.NoSignIn", "O Windows não tem senha - configure uma..." },
                { "Power.Online", "Na tomada" },
                { "Power.Offline", "Na bateria" },
                { "Power.Unknown", "Alimentação desconhecida" },

                // tray icon tooltip (63 characters at most)
                { "Tip.On", "UnplugLock - ativado ({0})" },
                { "Tip.Off", "UnplugLock - desativado ({0})" },
                { "Tip.Online", "na tomada" },
                { "Tip.Offline", "na bateria" },
                { "Tip.Unknown", "alimentação desconhecida" },
                { "Tip.NoSignIn", "UnplugLock - Windows sem senha, bloquear não adianta" },

                // tray messages
                { "NoSignIn.Title", "Sua conta do Windows não tem senha" },
                { "NoSignIn.Text", "O UnplugLock pode bloquear a tela, mas qualquer pessoa pode entrar de novo com um clique. Clique aqui para configurar uma senha ou PIN." },
                { "Running.Title", "O UnplugLock está em execução" },
                { "Running.On", "Seu notebook bloqueia quando é desconectado. Clique com o botão direito no ícone para ver as opções." },
                { "Running.Off", "O UnplugLock está desativado. Clique com o botão direito no ícone para ver as opções." },
                { "LockFailed", "O Windows não bloqueou (erro {0}). Quando o carregador é desconectado, o UnplugLock continua tentando por 10 minutos e bloqueia assim que o Windows permitir." },
                { "StartupFailed", "Não foi possível alterar a configuração de inicialização:\n{0}" },
                { "HideConfirm", "O ícone da bandeja será ocultado, mas o UnplugLock continua em execução e protegendo seu notebook.\n\nPara mostrar o ícone de novo, abra o UnplugLock outra vez (pelo menu Iniciar ou pelo UnplugLock.exe)." },
                { "PasswordRemoved", "Senha removida." },

                // installer
                { "Setup.Window", "Instalação do UnplugLock" },
                { "Setup.Language", "Idioma" },
                { "Setup.TitleInstall", "Instalar o UnplugLock" },
                { "Setup.TitleUpdate", "Atualizar o UnplugLock" },
                { "Setup.Guard", "Manter o UnplugLock em execução" },
                { "Setup.GuardHint", "Se alguém fechá-lo, ele se reinicia na hora." },
                { "Setup.Startup", "Iniciar o UnplugLock quando eu entrar no Windows" },
                { "Setup.Desktop", "Criar um atalho na área de trabalho" },
                { "Setup.Launch", "Abrir o UnplugLock ao terminar a instalação" },
                { "Setup.InstallsTo", "Será instalado em" },
                { "Setup.PerUser", "Só para a sua conta, então não precisa de permissões de administrador." },
                { "Setup.Install", "Instalar" },
                { "Setup.Update", "Atualizar" },
                { "Setup.Installing", "Instalando..." },
                { "Setup.Updating", "Atualizando..." },
                { "Setup.Finish", "Concluir" },
                { "Setup.Failed", "Não foi possível concluir a instalação:\n\n{0}" },
                { "Setup.Installed", "O UnplugLock está instalado" },
                { "Setup.Updated", "O UnplugLock está atualizado" },
                { "Setup.DoneSubtitle", "Agora seu notebook bloqueia quando o carregador é desconectado." },
                { "Setup.DoneRunning", "Ele está em execução na bandeja do sistema, ao lado do relógio. Se não vir o ícone, clique na seta ^ ali." },
                { "Setup.DoneNotRunning", "Abra-o pelo menu Iniciar quando quiser. Ele fica na bandeja do sistema, ao lado do relógio." },
                { "Setup.DoneHint", "Clique com o botão direito no ícone para definir um atraso, adicionar uma senha ou desativá-lo por um tempo." },
                { "Setup.AuthUpdate", "O UnplugLock é protegido por senha. Digite a senha para atualizá-lo." },
                { "Setup.AuthUninstall", "O UnplugLock é protegido por senha. Digite a senha para desinstalá-lo." },
                { "Setup.StillRunning", "O UnplugLock ainda está em execução e não fechou. Saia dele pelo menu da bandeja e execute a instalação de novo." },
                { "Setup.Damaged", "Este arquivo de instalação está danificado (falta o UnplugLock.exe)." },
                { "Setup.ShortcutDescription", "Bloqueia o notebook quando o carregador é desconectado" },
                { "Uninstall.Title", "Desinstalar o UnplugLock" },
                { "Uninstall.Confirm", "Remover o UnplugLock e todas as configurações dele deste PC?" },
                { "Uninstall.StillRunning", "O UnplugLock ainda está em execução e não fechou. Saia dele pelo menu da bandeja e tente desinstalar de novo." },
                { "Uninstall.Done", "O UnplugLock foi desinstalado." },
            };
        }
    }
}
