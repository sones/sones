/*
 * CollectionDefinition
 * (c) Stefan Licht, 2010
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using sones.GraphDB.Structures.EdgeTypes;
using sones.Lib.ErrorHandling;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Errors;

namespace sones.GraphDB.Managers.Structures
{
    public enum CollectionType
    {
        Set,
        List,
        SetOfUUIDs
    }

    public class CollectionDefinition
    {

        #region Properties

        public CollectionType CollectionType { get; private set; }
        public TupleDefinition TupleDefinition { get; private set; }

        #endregion

        #region Ctor

        public CollectionDefinition(CollectionType myCollectionType, TupleDefinition myTupleDefinition)
        {
            CollectionType = myCollectionType;
            TupleDefinition = myTupleDefinition;
        }

        #endregion

        #region GetEdge

        /// <summary>
        /// Get the content of the CollectionDefinition as an edge
        /// </summary>
        /// <param name="myTypeAttribute"></param>
        /// <param name="myGraphDBType"></param>
        /// <param name="myDBContext"></param>
        /// <returns></returns>
        public Exceptional<ASetOfReferencesEdgeType> GetEdge(TypeAttribute myTypeAttribute, GraphDBType myGraphDBType, DBContext myDBContext)
        {

            if (CollectionType == CollectionType.List || !(myTypeAttribute.EdgeType is ASetOfReferencesEdgeType))
            {
                return new Exceptional<ASetOfReferencesEdgeType>(new Error_InvalidAssignOfSet(myTypeAttribute.Name));
            }

            #region The Edge is empty

            if (TupleDefinition == null)
            {
                return new Exceptional<ASetOfReferencesEdgeType>(myTypeAttribute.EdgeType.GetNewInstance() as ASetOfReferencesEdgeType);
            }

            #endregion

            Exceptional<ASetOfReferencesEdgeType> uuids = null;
            if (CollectionType == CollectionType.SetOfUUIDs)
            {
                uuids = TupleDefinition.GetAsUUIDEdge(myDBContext, myTypeAttribute);
                if (uuids.Failed())
                {
                    return new Exceptional<ASetOfReferencesEdgeType>(uuids);
                }
            }
            else
            {
                uuids = TupleDefinition.GetCorrespondigDBObjectUUIDAsList(myGraphDBType, myDBContext, (ASetOfReferencesEdgeType)myTypeAttribute.EdgeType, myGraphDBType);
                if (CollectionType == CollectionType.Set)
                {
                    if (uuids.Success())
                    {
                        uuids.Value.Distinction();
                    }
                }
            }
            return uuids;

        }

        #endregion

    }
}
