using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphFS.DataStructures;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphDB.ObjectManagement;
using sones.Lib.ErrorHandling;

namespace sones.GraphDB.Structures.EdgeTypes
{
    public abstract class ASetOfReferencesWithInfoEdgeType : ASetOfReferencesEdgeType
    {

        /// <summary>
        /// Get all uuids and their edge infos
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<Tuple<ObjectUUID, ADBBaseObject>> GetAllReferenceIDsWeighted();

        /// <summary>
        /// Get all weighted destinations of an edge
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<Tuple<Exceptional<DBObjectStream>, ADBBaseObject>> GetAllEdgeDestinationsWeighted(DBObjectCache dbObjectCache);

    }
}
