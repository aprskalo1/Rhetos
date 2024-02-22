using Rhetos.Dsl.DefaultConcepts;
using Rhetos.Dsl;
using System.ComponentModel.Composition;

namespace Bookstore.RhetosExtensions
{
    [Export(typeof(IConceptInfo))]
    [ConceptKeyword("PhoneNumber")]
    public class PhoneNumberInfo : ShortStringPropertyInfo
    {
    }
}
