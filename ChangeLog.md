# MvcModelGenerator release notes

## 3.1.0 (2020-10-19)

* Build optimization: Sorting entries in Captions.resx file to avoid unnecessary rebuilds of Captions.dll and Rhetos.Mvc.dll.

## 3.0.0 (2020-09-23)

* Upgrade to Rhetos 4.0.
* Build optimization: Caching of resources to avoid recompilation of unchanged captions.

## 2.5.0 (2020-05-14)

* Support for Rhetos 4.0: New interface for generating 'assets' assembly files that are not part of the main application.

## 2.4.0 (2019-09-16)

## Internal improvements

* Bugfix: Deployment on Rhetos v3 fails with FileNotFoundException: Could not find file 'C:\My Projects\Rhetos\Source\Rhetos\bin\System.dll'.

## 2.3.0 (2019-09-09)

### New features

* New DSL concept: **ImplementInMvcModel**, for implementing an interface in the generated MVC model class.
  See [Readme.md](Readme.md) for details.
