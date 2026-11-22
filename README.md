<p align="center">
  <img src="UnplugLock.png" width="96" alt="UnplugLock icon">
</p>

<h1 align="center">UnplugLock</h1>

<p align="center">
  Lightweight Windows tray app that locks your laptop the moment the charger is unplugged.
</p>

<p align="center">
  <a href="https://unpluglock.com">unpluglock.com</a>
</p>

---

Leave your laptop on charge in a café, library or office. If someone pulls the plug to walk off with it, Windows locks straight away, and they're left holding a laptop at the lock screen.

The laptop itself can be replaced. What's harder to replace is everything you're signed into: email, browser sessions, banking, work chat, photos. An unlocked laptop hands all of that over. UnplugLock makes sure that if your laptop goes, it goes locked.

> [!IMPORTANT]
> Locking only helps if your Windows account has a password, PIN or Windows Hello. Without one, anyone can click straight past the lock screen. Set one up in **Settings → Accounts → Sign-in options**. UnplugLock warns you if your account doesn't have one.

## Features

- **Locks on unplug.** Locks Windows as soon as the laptop switches from AC power to battery.
- **Optional lock delay.** Instant, or 1, 3, 5 or 10 seconds. If power comes back within the delay (for example, a loose cable), it doesn't lock.
- **Password protection.** Once a password is set, UnplugLock asks for it before you can turn it off, change the lock delay, turn off the process guard or Start with Windows, change or remove the password, exit, update or uninstall.
- **Recovery code.** Setting a password gives you a one-time recovery code, in case you forget the password.
- **Process guard (on by default).** UnplugLock runs a small helper process, and the two restart each other within about a second if either one is closed. Programs running without admin rights can't end either process at all.
- **Tamper-resistant settings.** With a password set, the settings are locked against ordinary registry edits, a hidden backup copy restores anything that's deleted, and the running app repairs changed settings within a second.
- **Sign-in check.** If your Windows account has no password, UnplugLock warns you when it starts and keeps a reminder at the top of the tray menu, since locking can't keep anyone out without one. Clicking either opens Sign-in options. (It only checks local accounts. Work, school and Microsoft accounts always have a password.)
- **Hidden mode.** Hide the tray icon. UnplugLock keeps running, and opening it again brings the icon back.
- **Starts with Windows**, if you want it to. It always does while the process guard is on.
- **Small, with no dependencies.** The app is 75 KB and the installer 160 KB. It uses .NET Framework 4.7.2 or later, which is built into Windows 10 (April 2018 Update and later) and Windows 11.
- **No admin rights needed.** Installs and runs as your normal user.
- **No telemetry.** No analytics, tracking or update checks. UnplugLock never connects to the internet, so nothing about you or your laptop is sent anywhere, and its settings stay on your laptop.

## Install

1. Download `UnplugLock-Setup-v1.2.0.exe` from the [Releases](https://github.com/itsyohanes/UnplugLock/releases) page.
2. Run it and click **Install**.
3. Look for the blue plug icon in the system tray. You may need to click the **^** arrow next to the clock.

> Windows SmartScreen may warn about an unrecognised app because the installer isn't code-signed. Click **More info → Run anyway**.

UnplugLock installs to `%LOCALAPPDATA%\Programs\UnplugLock` and appears in **Settings → Apps**, where you can uninstall it. Uninstalling removes the program, its shortcuts, its startup entry and all of its settings.

## Usage

Right-click the tray icon:

| Menu item | What it does |
|---|---|
| *Plugged in / On battery* | Current power status |
| **Enabled** | Turn UnplugLock on or off (double-clicking the icon does the same). The icon goes grey when it's off. |
| **Lock delay** | How long to wait after unplugging before locking |
| **Start with Windows** | Launch automatically when you sign in. Always on while the process guard is on. |
| **Guard process (auto-restart)** | Keep UnplugLock running even if it's closed, for example from Task Manager |
| **Hide tray icon** | Hide the icon while UnplugLock keeps running. Open UnplugLock again from the Start menu to bring it back. |
| **Password** | Set, change or remove the password. Setting or changing it shows a new recovery code. |
| **Lock now (test)** | Lock immediately, to check locking works |
| **Exit** | Close UnplugLock (and its helper process) |

UnplugLock only reacts to the change from plugged in to battery. Starting it while already on battery won't lock you out, and plugging back in never unlocks anything.

### Forgot your password?

When you set or change a password, UnplugLock shows a recovery code like `XXXXX-XXXXX-XXXXX-XXXXX-XXXXX`. This is the only time it's shown. Copy it or save it somewhere other than the laptop: your phone, a password manager, or on paper.

If you forget the password, click **Forgot password?** on the password prompt and enter the code. Case, spaces and dashes don't matter.

- **In the app:** the code removes the password. You can then set a new one, which gives you a new code.
- **In the installer or uninstaller:** the code lets that update or uninstall go ahead, without removing the password.

Keep the code safe. Because the settings are tamper-resistant, there's no easy way back in without it.

### Silent install (for scripts)

```
UnplugLock-Setup-v1.2.0.exe /silent [/nostartup] [/nolaunch] [/desktop] [/noguard]
"%LOCALAPPDATA%\Programs\UnplugLock\Uninstall.exe" /uninstall /silent
```

A password-protected install can't be updated or removed silently.

## Limitations

UnplugLock is a deterrent, not a security boundary.

- It doesn't stop anyone taking the laptop. It just makes sure they get a locked one. If you want to stop the laptop leaving the table, use a cable lock as well.
- The lock screen protects your signed-in session, not the drive itself. Without encryption, someone who takes the drive out can still read your files. Turn on BitLocker or Device encryption in Windows settings so your files stay protected too.
- The password, process guard and settings protection make UnplugLock hard to switch off casually, but not impossible. On an account with admin rights, Task Manager can end the processes (the guard restarts them), and someone who knows exactly what to do can stop both processes and change the settings. No ordinary app can stop an administrator.
- It only protects you while you're signed in and the laptop is plugged in. Your Windows sign-in is what actually keeps people out, so use a strong password or PIN. With no sign-in at all, the lock screen doesn't stop anyone.
- If the laptop is asleep when it's unplugged, UnplugLock locks when it wakes up.

## Building from source

There's nothing to install. The C# compiler that ships with Windows is enough.

```
build.cmd
```

This produces:

- `UnplugLock.exe`: the app
- `UnplugLock-Setup-v<version>.exe`: the installer, with the app embedded

| File | Purpose |
|---|---|
| `UnplugLock.cs` | The tray app: power detection, locking, process guard, settings, password and recovery code, and the icon drawing |
| `Setup.cs` | Installer and uninstaller (compiled together with `UnplugLock.cs`) |
| `setup.manifest` | Runs setup without asking for admin rights |
| `build.cmd` | Build script |
| `UnplugLock.ico` / `.png` | App icon, drawn by the app itself (delete the `.ico` and rebuild to regenerate both) |

To release a new version, update `VERSION` in `build.cmd` and `Version` in `Setup.cs`.

### How it works

- **Detection:** listens for Windows power-change notifications, and also checks the power status once a second as a backup.
- **Locking:** the standard Windows lock call (`LockWorkStation`), the same as pressing `Win + L`.
- **Process guard:** the helper is a copy of the app, `UnplugLockHelper.exe`, in the install folder. Each process watches the other and restarts it if it ends unexpectedly. Both deny "terminate" and "suspend" access to other processes, which stops anything running without admin rights. If the startup entry is deleted, it's put back within about 5 seconds.
- **Settings:** stored in `HKCU\Software\UnplugLock`, with a hidden backup in `%LOCALAPPDATA%\UnplugLock\settings.dat`. If one copy is missing, it's restored from the other the next time UnplugLock starts. If the two disagree, the stricter setting wins. With a password set, the registry key denies changes to everyone, and the running app checks both copies once a second and rewrites them if they've changed.
- **Passwords:** neither the password nor the recovery code is stored. Only salted PBKDF2-SHA256 hashes (200,000 iterations) are kept.

## License

[MIT](LICENSE)
