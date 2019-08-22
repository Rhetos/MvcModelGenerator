using System.ComponentModel.Composition;
using Rhetos.Dsl;
using Rhetos.Dsl.DefaultConcepts;

namespace Rhetos.MvcModelGenerator.DefaultConcepts
{
	[Export(typeof(IConceptInfo))]
	[ConceptKeyword("ImplementInMvcModel")]
	public class ImplementInMvcModelInfo : IConceptInfo
	{
		[ConceptKey]
		public ImplementsInterfaceInfo Implements { get; set; }
	}
}