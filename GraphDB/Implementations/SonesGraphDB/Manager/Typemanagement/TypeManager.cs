using sones.GraphDB.TypeSystem;
using sones.GraphDB.Request;
using sones.Library.Transaction;
using sones.Library.LanguageExtensions;
using System.Collections.Generic;
using sones.Library.VertexStore;



namespace sones.GraphDB.Manager.Typemanagement
{
    public sealed class TypeManager
    {
        public readonly VertexTypeManager VertexManager;

        public readonly EdgeTypeManager EdgeManager;


        public TypeManager(IVertexStore myVertexStore)
        {
            VertexManager = new VertexTypeManager();
            EdgeManager = new EdgeTypeManager();
        }
    }
}
