using System.ComponentModel.Composition;
using Rhetos.Compiler;
using Rhetos.Dsl;
using Rhetos.Extensibility;

namespace Rhetos.MvcModelGenerator.DefaultConcepts
{
	[Export(typeof(IMvcModelGeneratorPlugin))]
	[ExportMetadata(MefProvider.Implements, typeof(ImplementInMvcModelInfo))]
	public class ImplementInMvcModelCodeGenerator : IMvcModelGeneratorPlugin
	{
		public void GenerateCode(IConceptInfo conceptInfo, ICodeBuilder codeBuilder)
		{
			var info = (ImplementInMvcModelInfo)conceptInfo;
			Dom.DefaultConcepts.DataStructureCodeGenerator.AddInterfaceAndReference(codeBuilder, info.Implements.GetInterfaceType(), info.Implements.DataStructure);
		}
	}
}