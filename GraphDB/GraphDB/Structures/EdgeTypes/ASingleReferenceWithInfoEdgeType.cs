using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphFS.DataStructures;
using sones.GraphDB.TypeManagement;

using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphDB.ObjectManagement;
using sones.Lib.ErrorHandling;
using sones.Lib;


namespace sones.GraphDB.Structures.EdgeTypes
{
    public abstract class ASingleReferenceWithInfoEdgeType : ASingleReferenceEdgeType
    {

        public abstract Tuple<Exceptional<DBObjectStream>, ADBBaseObject> GetEdgeDestinationWeighted(DBObjectCache dbObjectCache);
        public abstract Tuple<ObjectUUID, ADBBaseObject> GetReferenceIDWeighted();


        protected ulong GetBaseSize()
        {
            return base.GetBaseSize();
        }
    }
}
