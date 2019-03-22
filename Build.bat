SETLOCAL
SET Version=2.3.0
SET Prerelease=auto

@IF DEFINED VisualStudioVersion GOTO SkipVcvarsall
@SET VSTOOLS=
@IF EXIST "%VS140COMNTOOLS%..\..\VC\vcvarsall.bat" SET VSTOOLS="%VS140COMNTOOLS%..\..\VC\vcvarsall.bat" x86
@IF EXIST "C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\Tools\VsDevCmd.bat" SET VSTOOLS="C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\Tools\VsDevCmd.bat" -arch=x86
CALL %VSTOOLS% || GOTO Error0
@ECHO ON
:SkipVcvarsall

PowerShell .\ChangeVersion.ps1 %Version% %Prerelease% || GOTO Error0
WHERE /Q NuGet.exe || ECHO ERROR: Please download the NuGet.exe command line tool. && GOTO Error0
NuGet restore -NonInteractive || GOTO Error0
MSBuild /target:rebuild /p:Configuration=Debug /verbosity:minimal /fileLogger || GOTO Error0
IF NOT EXIST Install md Install
NuGet pack Rhetos.MvcModelGenerator.Client.nuspec -OutputDirectory Install || GOTO Error0
NuGet pack Rhetos.MvcModelGenerator.nuspec -OutputDirectory Install || GOTO Error0
REM Updating the version of all projects back to "dev" (internal development build), to avoid spamming git history with timestamped prerelease versions.
PowerShell .\ChangeVersion.ps1 %Version% dev || GOTO Error0

@REM ================================================

@ECHO.
@ECHO %~nx0 SUCCESSFULLY COMPLETED.
@EXIT /B 0

:Error0
@ECHO.
@ECHO %~nx0 FAILED.
@IF /I [%1] NEQ [/NOPAUSE] @PAUSE
@EXIT /B 1
