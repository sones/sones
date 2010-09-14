
#region usings

using System;
using System.Collections.Generic;
using sones.GraphDB.ObjectManagement;
using sones.GraphFS.DataStructures;
using sones.GraphDB.TypeManagement;
using sones.Lib.ErrorHandling;
using sones.GraphDB.TypeManagement;

#endregion

namespace sones.GraphDB.Structures.EdgeTypes
{

    public interface IReferenceEdge : IEdgeType
    {

        /// <summary>
        /// Check for a containing element
        /// </summary>
        /// <param name="myValue"></param>
        /// <returns></returns>
        Boolean Contains(ObjectUUID myValue);

        /// <summary>
        /// This is just a helper for BackwardEdges
        /// </summary>
        /// <returns></returns>
        ObjectUUID FirstOrDefault();

        /// <summary>
        /// Get all added objectUUIDs
        /// </summary>
        /// <returns></returns>
        IEnumerable<ObjectUUID> GetAllReferenceIDs();

        /// <summary>
        /// Get all references
        /// </summary>
        /// <returns></returns>
        IEnumerable<Reference> GetAllReferences();

        /// <summary>
        /// Get all destinations of an edge
        /// </summary>
        /// <returns></returns>
        IEnumerable<Exceptional<DBObjectStream>> GetAllEdgeDestinations(DBObjectCache dbObjectCache);

        IReferenceEdge GetNewInstance(IEnumerable<Exceptional<DBObjectStream>> iEnumerable);

        IReferenceEdge GetNewInstance(IEnumerable<ObjectUUID> iEnumerable, TypeUUID typeOfObjects);

        /// <summary>
        /// removes a specific reference
        /// </summary>
        /// <param name="myObjectUUID">the object uuid of the object, that should remove</param>
        /// <returns></returns>
        Boolean RemoveUUID(ObjectUUID myObjectUUID);

        /// <summary>
        /// remove some specifics references
        /// </summary>
        /// <param name="myObjectUUIDs">the object uuid's of the objects, that should remove</param>
        /// <returns></returns>
        Boolean RemoveUUID(IEnumerable<ObjectUUID> myObjectUUIDs);

    }

}
