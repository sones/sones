using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Manager.TypeManagement
{
    public class IDManager
    {
        private Dictionary<Int64, UniqueID> _vertexIDs;


        public IDManager()
        {
            VertexTypeID = new UniqueID();
            _vertexIDs = new Dictionary<long, UniqueID>();
        }

        public UniqueID VertexTypeID { get; private set; }
        
        public UniqueID this[long myVertexTypeID]
        {
            get 
            {
                
                if (!_vertexIDs.ContainsKey(myVertexTypeID))
                    lock (_vertexIDs)
                    {
                        if (!_vertexIDs.ContainsKey(myVertexTypeID))
                        {
                            _vertexIDs[myVertexTypeID] = new UniqueID();
                        }
                    }
                return _vertexIDs[myVertexTypeID];
            }
        }
    }
}
