<Query Kind="Statements">
  <Reference Relative="..\..\..\Rhetos\Source\Rhetos\bin\Generated\Captions.dll">C:\My Projects\Rhetos\Source\Rhetos\bin\Generated\Captions.dll</Reference>
  <Namespace>System</Namespace>
  <Namespace>System.Collections</Namespace>
  <Namespace>System.Collections.Generic</Namespace>
  <Namespace>System.Linq</Namespace>
  <Namespace>System.Text</Namespace>
</Query>

Assembly assembly = typeof(Rhetos.Mvc.Captions).Assembly;
assembly.GetManifestResourceNames().Dump("Embedded resources");

var resManager = new global::System.Resources.ResourceManager("Captions", typeof(Rhetos.Mvc.Captions).Assembly);
resManager.GetString("Common_Claim").Dump("Manually reading resource data");

Rhetos.Mvc.Captions.Common_Claim.Dump("Reading resource data with generated class");
