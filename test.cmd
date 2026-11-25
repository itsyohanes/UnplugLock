@echo off
rem Builds and runs the tests with the csc.exe that comes with Windows.
rem   test.cmd            run everything
rem   test.cmd recovery   run only tests whose name contains "recovery"
rem The tests use their own registry key and folder, so a real install isn't touched.
setlocal
set CSC=%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\csc.exe
if not exist "%CSC%" set CSC=%WINDIR%\Microsoft.NET\Framework\v4.0.30319\csc.exe
set ROOT=%~dp0

"%CSC%" /nologo /codepage:65001 /target:exe /optimize+ /r:System.dll /r:System.Drawing.dll /r:System.Windows.Forms.dll /r:System.Core.dll ^
  /main:UnplugLock.Tests.TestProgram /out:"%ROOT%tests\UnplugLockTests.exe" ^
  "%ROOT%tests\Tests.cs" "%ROOT%UnplugLock.cs" "%ROOT%Setup.cs" "%ROOT%Lang.cs" "%ROOT%lang\*.cs" || exit /b 1

"%ROOT%tests\UnplugLockTests.exe" %*
