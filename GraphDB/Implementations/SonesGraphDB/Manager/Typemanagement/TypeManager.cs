using sones.GraphDB.TypeSystem;
using sones.GraphDB.Request;
using sones.Library.Transaction;
using sones.Library.LanguageExtensions;
using System.Collections.Generic;



namespace sones.GraphDB.Manager.Typemanagement
{
    public sealed class TypeManager
    {
        public readonly VertexTypeManager VertexManager;

        public readonly EdgeTypeManager EdgeManager;


        public TypeManager()
        {
            VertexManager = new VertexTypeManager();
            EdgeManager = new EdgeTypeManager();
        }
    }
}
