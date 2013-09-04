MvcModelGenerator
=================

MvcModelGenerator is a DSL package (a plugin module) for [Rhetos development platform](https://github.com/Rhetos/Rhetos).

MvcModelGenerator automatically generates ASP.NET MVC model for all entities and other queryable data structures that are defined in a Rhetos application.

See [rhetos.org](http://www.rhetos.org/) for more information on Rhetos.

Prerequisites
=============

Utilities in this project are based on relative path to Rhetos repository. [Rhetos source](https://github.com/Rhetos/Rhetos) must be downloaded to a folder with relative path "..\..\Rhetos". 

Sample folder structure:
 
	\ROOT
		\Rhetos
		\RhetosPackages
			\MvcModelGenerator


Build and Installation
======================

Build package with Build.bat. Check BuildError.log for errors.

Instalation package creation:

1. Set the new version number in "ChangeVersion.bat" and start it.
2. Run "Build.bat"
3. Run "CreatePackage.bat". Instalation package (.zip) is going to be created in parent directory of MvcModelGenerator.
