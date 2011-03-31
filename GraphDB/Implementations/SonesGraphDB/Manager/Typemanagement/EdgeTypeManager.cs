using sones.GraphDB.TypeSystem;
using sones.GraphDB.Request;
using sones.Library.VertexStore;

namespace sones.GraphDB.Manager.TypeManagement
{
    /* This class is splitted in three partial classes:
     * - TypeManager.cs declares the public methods for vertex and edge types
     * - EdgeTypeManager.cs declares the private methods for edge types
     * - VertexTypeManager.cs declares the private methods for vertex types
     */
    public sealed partial class TypeManager
    {
        #region VertexTypeManager

        #region Add

        private IEdgeType DoAddEdge(EdgeTypeDefinition myEdgeTypeDefinition)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #endregion
    }
}
