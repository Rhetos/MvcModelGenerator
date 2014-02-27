MvcModelGenerator
=================

MvcModelGenerator is a DSL package (a plugin module) for [Rhetos development platform](https://github.com/Rhetos/Rhetos).
It automatically generates **ASP.NET MVC model** for all entities and other queryable data structures that are defined in a Rhetos application.

See [rhetos.org](http://www.rhetos.org/) for more information on Rhetos.

Features and usage instructions
===============================

MvcModelGenerator package, when deployed, generates source and binary files in `\bin\Generated` folder inside Rhetos server.
**Include the generated files in your ASP.NET MVC application** (or any other),
for faster GUI development and easier integration with Rhetos application server.

### MVC model classes

* `Rhetos.Mvc.cs` contains MVC model classes for Rhetos entities and other data structures.
* The generated classes and their properties have standard *ComponentModel.DataAnnotations* attributes based on CommonConcepts features in DSL scripts:
  *Required*, *UIHint*, *MaxLength*, *MinLength*, *RegularExpression* and *Display*.
* Custom attributes are also assigned for additional functionality:
  *MaxValue*, *MinValue*, *RenderMode* and *LocalizedDisplayName*.

### Captions resource file

* The generated `Captions.resx` resource file contains default captions (display names) for properties, entities and other data structures.
  The MVC model classes are bound to the resource file by Display attribute.
* Option A: To use the generated captions include `Captions.dll` into your web application.   
* Option B: Include `Captions.resx` into your web application project.
  Make sure to set the file's properties in Visual Studio to match the following:
	* **Custom Tool Namespace: Rhetos.Mvc**
	* Build Action: Embedded Resource (default value)
	* Custom Tool: ResXFileCodeGenerator (default value) or PublicResXFileCodeGenerator

### Overriding default captions and localization

* To override default captions, include `Captions.resx` directly into your project (see the instructions above) and add a new resource file with your culture's name
  (for example `Captions.en-GB.resx` or `Captions.hr-HR.resx`, see [CurrentCulture](http://msdn.microsoft.com/en-us/library/vstudio/system.globalization.cultureinfo.currentculture%28v%3Dvs.100%29.aspx)).
  The ASP.NET will automatically use the captions you entered in the culture-specific resource file,
  or fallback to the default captions if the localized caption is not entered.
	* Make sure to set the same properties in Visual Studio for the localized resource file as for the default `Captions.resx` file.
* There are [free resx editors](https://www.google.hr/search?q=free+multilingual+parallel+net+resource+editor+resx) to help you enter the captions.

### Extending heuristics for default captions 

* MvcModelGenerator includes CamelCase splitter plugin (*CamelCaseCaptions*) that splits property names into words when generating default captions,
  reducing the need to override default captions.
* Similar caption processing plugins may be implemented in a Rhetos package by implementing *ICaptionsValuePlugin* interface.

Deployment
==========

### Prerequisites

* *CommonConcepts* package must be deployed along with *MvcModelGenerator*.

Building binaries from source
=============================

### Prerequisites

* Build utilities in this project are based on relative path to Rhetos repository.
  [Rhetos source](https://github.com/Rhetos/Rhetos) should be downloaded to a folder
  with relative path `..\..\Rhetos` and compiled (use `Build.bat`),
  before this package's `Build.bat` script is executed.

Sample folder structure:
 
	\ROOT
		\Rhetos
		\RhetosPackages
			\MvcModelGenerator


### Build

1. Build this package by executing `Build.bat`. The script will pause in case of an error.
   * The script automatically copies all needed dll files from Rhetos folder and builds the Rhetos.MvcModelGenerator.sln using Visual Studio (command-line).

### Create installation package

1. Set the new version number in `ChangeVersion.bat` and execute it.
2. Execute `Build.bat`.
3. Execute `CreatePackage.bat`. It creates installation package (.zip) in parent directory of MvcModelGenerator.
