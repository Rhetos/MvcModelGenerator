ECHO Target folder = [%1]
ECHO $(ConfigurationName) = [%2]

REM "%~dp0" is this script's folder.

XCOPY /Y/D/R "%~dp0"Plugins\Rhetos.Mvc.Client\bin\%2\Rhetos.Mvc.Client.dll %1 || EXIT /B 1
XCOPY /Y/D/R "%~dp0"Plugins\Rhetos.Mvc.Client\bin\%2\Rhetos.Mvc.Client.pdb %1 || EXIT /B 1
XCOPY /Y/D/R "%~dp0"Plugins\Rhetos.MvcModelGenerator\bin\%2\Rhetos.MvcModelGenerator.dll %1 || EXIT /B 1
XCOPY /Y/D/R "%~dp0"Plugins\Rhetos.MvcModelGenerator\bin\%2\Rhetos.MvcModelGenerator.pdb %1 || EXIT /B 1
XCOPY /Y/D/R "%~dp0"Plugins\Rhetos.MvcModelGenerator.DefaultConcepts\bin\%2\Rhetos.MvcModelGenerator.DefaultConcepts.dll %1 || EXIT /B 1
XCOPY /Y/D/R "%~dp0"Plugins\Rhetos.MvcModelGenerator.DefaultConcepts\bin\%2\Rhetos.MvcModelGenerator.DefaultConcepts.pdb %1 || EXIT /B 1

EXIT /B 0
