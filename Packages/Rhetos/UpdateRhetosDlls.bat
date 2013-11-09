DEL /Q "%~dp0*.dll"
DEL /Q "%~dp0*.xml"
DEL /Q "%~dp0*.pdb"

XCOPY /Y/D/R "%~dp0..\..\..\..\Rhetos\Source\Rhetos.Compiler.Interfaces\bin\Debug\Rhetos.Compiler.Interfaces.???" "%~dp0" || EXIT /B 1
XCOPY /Y/D/R "%~dp0..\..\..\..\Rhetos\Source\Rhetos.Dsl.Interfaces\bin\Debug\Rhetos.Dsl.Interfaces.???" "%~dp0" || EXIT /B 1
XCOPY /Y/D/R "%~dp0..\..\..\..\Rhetos\Source\Rhetos.Extensibility\bin\Debug\Rhetos.Extensibility.???" "%~dp0" || EXIT /B 1
XCOPY /Y/D/R "%~dp0..\..\..\..\Rhetos\Source\Rhetos.Extensibility.Interfaces\bin\Debug\Rhetos.Extensibility.Interfaces.???" "%~dp0" || EXIT /B 1
XCOPY /Y/D/R "%~dp0..\..\..\..\Rhetos\Source\Rhetos.Logging.Interfaces\bin\Debug\Rhetos.Logging.Interfaces.???" "%~dp0" || EXIT /B 1
XCOPY /Y/D/R "%~dp0..\..\..\..\Rhetos\Source\Rhetos.Utilities\bin\Debug\Rhetos.Utilities.???" "%~dp0" || EXIT /B 1

XCOPY /Y/D/R "%~dp0..\..\..\..\Rhetos\CommonConcepts\Plugins\Rhetos.Dsl.DefaultConcepts\bin\Debug\Rhetos.Dsl.DefaultConcepts.???" "%~dp0" || EXIT /B 1
