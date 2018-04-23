# MvcModelGenerator

MvcModelGenerator is a plugin package for [Rhetos development platform](https://github.com/Rhetos/Rhetos).
It automatically generates **ASP.NET MVC model** for all entities and other queryable data structures that are defined in a Rhetos application.

See [rhetos.org](http://www.rhetos.org/) for more information on Rhetos.

## Features and usage instructions

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

## Build

**Note:** This package is already available at the [NuGet.org](https://www.nuget.org/) online gallery.
You don't need to build it from source in order to use it in your application.

To build the package from source, run `Build.bat`.
The script will pause in case of an error.
The build output is a NuGet package in the "Install" subfolder.

## Installation

To install this package to a Rhetos server, add it to the Rhetos server's *RhetosPackages.config* file
and make sure the NuGet package location is listed in the *RhetosPackageSources.config* file.

* The package ID is "**Rhetos.MvcModelGenerator**".
  This package is available at the [NuGet.org](https://www.nuget.org/) online gallery.
  It can be downloaded or installed directly from there.
* For more information, see [Installing plugin packages](https://github.com/Rhetos/Rhetos/wiki/Installing-plugin-packages).
