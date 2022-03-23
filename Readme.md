# MvcModelGenerator

MvcModelGenerator is a plugin package for [Rhetos development platform](https://github.com/Rhetos/Rhetos).
It automatically generates **ASP.NET MVC model** for all entities and other queryable data structures that are defined in a Rhetos application.

See [rhetos.org](http://www.rhetos.org/) for more information on Rhetos.

Contents:

1. [Features and usage instructions](#features-and-usage-instructions)
   1. [MVC model classes](#mvc-model-classes)
   2. [Captions resource file](#captions-resource-file)
   3. [Overriding default captions and localization](#overriding-default-captions-and-localization)
   4. [Extending heuristics for default captions](#extending-heuristics-for-default-captions)
2. [Installation and configuration](#installation-and-configuration)
3. [How to contribute](#how-to-contribute)
   1. [Building and testing the source code](#building-and-testing-the-source-code)

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
* If you want to add implemented interfaces defined in DSL using Implements DSL concept, add ImplementInMvcModel concept to Implements:

```c
Implements 'MyNamespace.ITheInteface, MyAssembly' { ImplementInMvcModel; }
```

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

## Installation and configuration

Installing this package to a Rhetos application:

1. Add "Rhetos.MvcModelGenerator" NuGet package, available at the [NuGet.org](https://www.nuget.org/) on-line gallery.

## How to contribute

Contributions are very welcome. The easiest way is to fork this repo, and then
make a pull request from your fork. The first time you make a pull request, you
may be asked to sign a Contributor Agreement.
For more info see [How to Contribute](https://github.com/Rhetos/Rhetos/wiki/How-to-Contribute) on Rhetos wiki.

### Building and testing the source code

* Note: This package is already available at the [NuGet.org](https://www.nuget.org/) online gallery.
  You don't need to build it from source in order to use it in your application.
* To build the package from source, run `Clean.bat`, `Build.bat` and `Test.bat`.
* The build output is a NuGet package in the "Install" subfolder.
