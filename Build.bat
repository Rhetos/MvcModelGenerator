@REM HINT: SET SECOND ARGUMENT TO /NOPAUSE WHEN AUTOMATING THE BUILD.

@SET Config=%1%
@IF [%1] == [] SET Config=Debug

@IF DEFINED VisualStudioVersion GOTO SkipVcvarsall
IF "%VS140COMNTOOLS%" NEQ "" CALL "%VS140COMNTOOLS%VsDevCmd.bat" x86 && GOTO EndVcvarsall || GOTO Error0
IF "%VS120COMNTOOLS%" NEQ "" CALL "%VS120COMNTOOLS%\..\..\VC\vcvarsall.bat" x86 && GOTO EndVcvarsall || GOTO Error0
IF "%VS110COMNTOOLS%" NEQ "" CALL "%VS110COMNTOOLS%\..\..\VC\vcvarsall.bat" x86 && GOTO EndVcvarsall || GOTO Error0
IF "%VS100COMNTOOLS%" NEQ "" CALL "%VS100COMNTOOLS%\..\..\VC\vcvarsall.bat" x86 && GOTO EndVcvarsall || GOTO Error0
ECHO ERROR: Cannot detect Visual Studio, missing VSxxxCOMNTOOLS variable.
GOTO Error0
:EndVcvarsall
@ECHO ON
:SkipVcvarsall

NuGet.exe restore

IF EXIST Build.log DEL Build.log || GOTO Error0
DevEnv.com "%~dp0Rhetos.MvcModelGenerator.sln" /rebuild %Config% /out Build.log || TYPE Build.log && GOTO Error0

IF NOT EXIST Install md Install
NuGet.exe pack -o Install || PAUSE

@REM ================================================

@ECHO.
@ECHO %~nx0 SUCCESSFULLY COMPLETED.
@EXIT /B 0

:Error0
@ECHO.
@ECHO %~nx0 FAILED.
@IF /I [%2] NEQ [/NOPAUSE] @PAUSE
@EXIT /B 1
