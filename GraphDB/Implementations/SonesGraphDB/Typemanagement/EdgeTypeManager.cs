using sones.GraphDB.TypeSystem;
using sones.GraphDB.Request;
using sones.Library.VertexStore;

namespace sones.GraphDB.Manager.TypeManagement
{
    public sealed partial class TypeManager
    {
        #region Data

        private EdgeTypeManager _EdgeManager;

        #endregion

        #region EdgeTypeManager

        private sealed class EdgeTypeManager
        {

            internal IEdgeType Add(EdgeTypeDefinition myEdgeTypeDefinition)
            {
                throw new System.NotImplementedException();
            }
        }

        #endregion

    }
}
