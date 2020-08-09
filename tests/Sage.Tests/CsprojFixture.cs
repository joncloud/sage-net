using System.Xml.Linq;

namespace Sage.Tests
{
    public class CsprojFixture
    {
        public XDocument Document { get; }

        public CsprojFixture()
        {
            var path = "../../../../../src/Sage/Sage.csproj";

            Document = XDocument.Load(path);
        }
    }
}
