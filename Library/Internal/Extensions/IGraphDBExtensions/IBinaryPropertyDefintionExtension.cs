using sones.Library.PropertyHyperGraph;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.Extensions
{
    public static class IBinaryPropertyDefintionExtension
    {
        public static bool HasValue(this IBinaryPropertyDefinition myProperty, IVertex myVertex)
        {
            return myVertex.HasProperty(myProperty.ID);
        }
    }
}
