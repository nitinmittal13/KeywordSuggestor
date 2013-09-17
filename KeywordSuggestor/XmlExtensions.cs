using System.Collections;
using System.Linq;
using System.Xml.Linq;

namespace KeywordSuggestor
{
    /// <summary>
    /// http://simoncropp.com/workingaroundxmlnamespaces
    /// </summary>
    public static class XmlExtensions
    {
        public static void StripNamespace(this XDocument document)
        {
            if (document.Root == null)
            {
                return;
            }
            foreach (var element in document.Root.DescendantsAndSelf())
            {
                element.Name = element.Name.LocalName;
                element.ReplaceAttributes(GetAttributes(element));
            }
        }

        static IEnumerable GetAttributes(XElement xElement)
        {
            return xElement.Attributes()
                .Where(x => !x.IsNamespaceDeclaration)
                .Select(x => new XAttribute(x.Name.LocalName, x.Value));
        }
    }
}

