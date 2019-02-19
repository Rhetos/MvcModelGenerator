<Query Kind="Statements">
  <Reference Relative="..\..\..\Rhetos\Source\Rhetos\bin\Generated\Captions.dll">C:\My Projects\Rhetos\Source\Rhetos\bin\Generated\Captions.dll</Reference>
  <Namespace>System.Globalization</Namespace>
</Query>

var assembly = typeof(Rhetos.Mvc.Captions).Assembly;
assembly.Location.Dump("Location");
assembly.GetManifestResourceNames().Dump("ManifestResourceNames");

if (Rhetos.Mvc.Captions.Common_Principal.Dump("Captions.Common_Principal") != "Principal")
	throw new ApplicationException("Unexpected test result.");

"Passed.".Dump();
