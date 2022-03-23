SETLOCAL

@SET Config=%1%
@IF [%1] == [] SET Config=Debug

@REM Using "no-build" option as optimization, because Test.bat should always be executed after Build.bat.
dotnet test --no-build || GOTO Error0

IF EXIST "%ProgramFiles%\LINQPad6\LPRun6-net5.exe" "%ProgramFiles%\LINQPad6\LPRun6-net5.exe" "Test\Test.linq" > nul || GOTO Error0       
IF EXIST "%ProgramFiles%\LINQPad6\LPRun6-net5.exe" "%ProgramFiles%\LINQPad6\LPRun6-net5.exe" "Test\Test Captions.linq" > nul || GOTO Error0

@REM ================================================

@ECHO.
@ECHO %~nx0 SUCCESSFULLY COMPLETED.
@EXIT /B 0

:Error0
@ECHO.
@ECHO %~nx0 FAILED.
@EXIT /B 1
