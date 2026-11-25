@echo off
rem Builds UnplugLock.exe + the installer using the csc.exe that comes with Windows.
rem If UnplugLock.ico is missing it gets generated first (delete it to redraw the icon).
setlocal
set CSC=%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\csc.exe
if not exist "%CSC%" set CSC=%WINDIR%\Microsoft.NET\Framework\v4.0.30319\csc.exe
set ROOT=%~dp0
rem must match Version in UnplugLock.cs
set VERSION=1.4.1
set SETUP=UnplugLock-Setup-v%VERSION%.exe
set ARGS=/nologo /codepage:65001 /target:winexe /optimize+ /r:System.dll /r:System.Drawing.dll /r:System.Windows.Forms.dll /r:System.Core.dll

rem the translations in lang\ are UTF-8, hence /codepage:65001
set SRC="%ROOT%UnplugLock.cs" "%ROOT%Lang.cs" "%ROOT%lang\*.cs"

if not exist "%ROOT%UnplugLock.ico" (
  "%CSC%" %ARGS% /out:"%ROOT%UnplugLock.exe" %SRC% || exit /b 1
  "%ROOT%UnplugLock.exe" --make-icon "%ROOT%UnplugLock.ico" "%ROOT%UnplugLock.png" || exit /b 1
)

rem app
"%CSC%" %ARGS% /win32icon:"%ROOT%UnplugLock.ico" /out:"%ROOT%UnplugLock.exe" %SRC% || exit /b 1
echo Built %ROOT%UnplugLock.exe

rem installer: setup with only English built in (SETUP borrows the other languages from the app),
rem then the app added onto its end. Setup.cs explains the format.
set SETUPCORE=%TEMP%\UnplugLock-Setup-%RANDOM%%RANDOM%.exe
"%CSC%" %ARGS% /define:SETUP /main:UnplugLock.SetupProgram /win32icon:"%ROOT%UnplugLock.ico" ^
  /win32manifest:"%ROOT%setup.manifest" /out:"%SETUPCORE%" ^
  "%ROOT%Setup.cs" "%ROOT%UnplugLock.cs" "%ROOT%Lang.cs" "%ROOT%lang\en.cs" || exit /b 1
"%SETUPCORE%" --pack "%SETUPCORE%" "%ROOT%UnplugLock.exe" "%ROOT%%SETUP%"
set PACKED=%ERRORLEVEL%
del "%SETUPCORE%" 2>nul
if not "%PACKED%"=="0" exit /b 1
echo Built %ROOT%%SETUP%
