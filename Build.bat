@REM HINT: USE /NOPAUSE PARAMETER WHEN AUTOMATING THE BUILD.

IF NOT DEFINED VisualStudioVersion CALL "%VS140COMNTOOLS%VsDevCmd.bat" || ECHO ERROR: Cannot find Visual Studio 2015, missing VS140COMNTOOLS variable. && GOTO Error0
@ECHO ON

PUSHD "%~dp0" || GOTO Error0
IF EXIST msbuild.log DEL msbuild.log || GOTO Error1

WHERE /Q NuGet.exe || ECHO ERROR: Please download the NuGet.exe command line tool. && GOTO Error1

NuGet.exe restore Rhetos.MvcModelGenerator.sln -NonInteractive || GOTO Error1
MSBuild.exe Rhetos.MvcModelGenerator.sln /target:rebuild /p:Configuration=Debug /verbosity:minimal /fileLogger || GOTO Error1
IF NOT EXIST Install md Install
NuGet.exe pack Rhetos.MvcModelGenerator.Client.nuspec -o Install || GOTO Error1
NuGet.exe pack Rhetos.MvcModelGenerator.nuspec -o Install || GOTO Error1

POPD

@REM ================================================

@ECHO.
@ECHO %~nx0 SUCCESSFULLY COMPLETED.
@EXIT /B 0

:Error1
@POPD
:Error0
@ECHO.
@ECHO %~nx0 FAILED.
@IF /I [%2] NEQ [/NOPAUSE] @PAUSE
@EXIT /B 1
