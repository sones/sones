/* <id name="GraphDB – abstract class for all reference list edges" />
 * <copyright file="AListReferenceEdgeType.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>This abstract class should be implemented for all reference list edges. It provides the base methods which are needed from the Database to retrieve all reference related values like ObjectUUID etc.</summary>
 */

using System;
using System.Collections.Generic;
using sones.GraphDB.ObjectManagement;

using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphFS.DataStructures;
using sones.Lib.ErrorHandling;
using sones.GraphDB.Result;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.NewAPI;
using sones.Lib;

namespace sones.GraphDB.Structures.EdgeTypes
{
    /// <summary>
    /// This abstract class should be implemented for all reference list edges. It provides the base methods which are needed from the Database to retrieve all reference related values like ObjectUUID etc.
    /// </summary>

    public abstract class ASetOfReferencesEdgeType : IReferenceEdge, IListOrSetEdgeType
    {
        

        /// <summary>
        /// Get a list of readouts. Additional to the standard readout (generated from <paramref name="GetAllAttributesFromDBO"/>)
        /// some special information will be added (like Weight etc)
        /// </summary>
        /// <param name="GetAllAttributesFromDBO">A delegate which will retriev the standard DBObjectReadout for a ObjectUUID</param>
        /// <returns>The standard readouts with some additional infos</returns>
        public abstract IEnumerable<Vertex> GetVertices(Func<ObjectUUID, Vertex> GetAllAttributesFromDBO);

        /// <summary>
        /// Extracts a list of readouts defined by <paramref name="myDBObjectStreams"/> from the edge. 
        /// Additional to the standard readout (generated from <paramref name="GetAllAttributesFromDBO"/>)
        /// some special information will be added (like Weight etc)
        /// If we had a function like TOP(1) the GetAllAttributesFromDBO might not contains elements which are in myObjectUUIDs (which is coming from the where)
        /// </summary>
        /// <param name="GetAllAttributesFromDBO">A delegate which will retriev the standard DBObjectReadout for a ObjectUUID</param>
        /// <param name="myDBObjectStreams">The Objects which should be extracted from the edge</param>
        /// <returns>The standard readouts with some additional infos</returns>
        public abstract IEnumerable<Vertex> GetReadouts(Func<ObjectUUID, Vertex> GetAllAttributesFromDBO, IEnumerable<Exceptional<DBObjectStream>> myDBObjectStreams);

        /// <summary>
        /// Adds a set of ObjectUUID with parameters
        /// </summary>
        /// <param name="hashSet"></param>
        /// <param name="myParameters"></param>
        public abstract void AddRange(IEnumerable<ObjectUUID> hashSet, TypeUUID typeOfObjects, params ADBBaseObject[] myParameters);

        /// <summary>
        /// Adds a new value with some optional parameters
        /// </summary>
        /// <param name="myValue"></param>
        /// <param name="myParameters"></param>
        public abstract void Add(ObjectUUID myValue, TypeUUID typeOfObjects, params ADBBaseObject[] myParameters);


        #region IReferenceEdge Members

        /// <summary>
        /// Check for a containing element
        /// </summary>
        /// <param name="myValue"></param>
        /// <returns></returns>
        public abstract Boolean Contains(ObjectUUID myValue);

        /// <summary>
        /// This is just a helper for BackwardEdges
        /// </summary>
        /// <returns></returns>
        public abstract ObjectUUID FirstOrDefault();


        /// <summary>
        /// Get all added objectUUIDs
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<ObjectUUID> GetAllReferenceIDs();

        /// <summary>
        /// Get all added references
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<Reference> GetAllReferences();

        /// <summary>
        /// Get all destinations of an edge
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<Exceptional<DBObjectStream>> GetAllEdgeDestinations(DBObjectCache dbObjectCache);

        public abstract IReferenceEdge GetNewInstance(IEnumerable<Exceptional<DBObjectStream>> iEnumerable);

        public abstract IReferenceEdge GetNewInstance(IEnumerable<ObjectUUID> iEnumerable, TypeUUID typeOfObjects);

        /// <summary>
        /// removes a specific reference
        /// </summary>
        /// <param name="myObjectUUID">the object uuid of the object, that should remove</param>
        public abstract Boolean RemoveUUID(ObjectUUID myObjectUUID);

        /// <summary>
        /// remove some specifics references
        /// </summary>
        /// <param name="myObjectUUIDs">the object uuid's of the objects, that should remove</param>
        public abstract Boolean RemoveUUID(IEnumerable<ObjectUUID> myObjectUUIDs);

        #endregion

        #region IEdgeType Members

        public abstract string EdgeTypeName
        {
            get;
        }

        public abstract EdgeTypeUUID EdgeTypeUUID
        {
            get;
        }

        public abstract void ApplyParams(params Managers.Structures.EdgeTypeParamDefinition[] myParams);

        public abstract string GetDescribeOutput(GraphDBType myGraphDBType);

        public abstract string GetGDDL(GraphDBType myGraphDBType);

        public abstract IEdgeType GetNewInstance();

        #endregion

        #region IFastSerialize Members

        public bool isDirty
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public DateTime ModificationTime
        {
            get { throw new NotImplementedException(); }
        }

        public abstract void Serialize(ref Lib.NewFastSerializer.SerializationWriter mySerializationWriter);

        public abstract void Deserialize(ref Lib.NewFastSerializer.SerializationReader mySerializationReader);

        #endregion

        #region IFastSerializationTypeSurrogate Members

        public abstract bool SupportsType(Type type);

        public abstract void Serialize(Lib.NewFastSerializer.SerializationWriter writer, object value);

        public abstract object Deserialize(Lib.NewFastSerializer.SerializationReader reader, Type type);

        public abstract uint TypeCode
        {
            get;
        }

        #endregion

        #region IListOrSetEdgeType Members

        public abstract ulong Count();

        public abstract IListOrSetEdgeType GetTopAsEdge(ulong myNumOfEntries);

        public abstract void UnionWith(IListOrSetEdgeType myAListEdgeType);

        public abstract void Distinction();

        public abstract void Clear();

        #endregion
        
        #region IComparable Members

        public abstract Int32 CompareTo(object obj);

        #endregion

        #region IObject Members

        public abstract ulong GetEstimatedSize();

        #endregion


        protected ulong GetBaseSize()
        {
            //EdgeTypeName + EdgeTypeUUID + TypeCode + EstimatedSize + CLassDefaltSize
            return EstimatedSizeConstants.CalcStringSize(EdgeTypeName) + EstimatedSizeConstants.CalcUUIDSize(EdgeTypeUUID) + EstimatedSizeConstants.UInt32 + EstimatedSizeConstants.UInt64 + EstimatedSizeConstants.ClassDefaultSize;
        }
    }
}
