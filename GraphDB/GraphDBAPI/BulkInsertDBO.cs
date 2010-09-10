/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using sones.GraphDB;
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.ObjectManagement;

using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphFS.DataStructures;
using sones.GraphDBInterface.TypeManagement;


namespace GraphDBAPI
{
    public class BulkInsertDBO
    {

        private DBContext _DBContext;
        private DBObjectStream _DBObjectStream;
        private BulkInsert _BulkInsert;
        private GraphDBType _DBTypeStream;
        private BackwardEdgeStream _BackwardEdge;

        public UInt64 EdgesCount = 0;
        public UInt64 BackwardEdgesCount = 0;

        /// <summary>
        /// This will be invoked by the BulkInsert class.
        /// </summary>
        /// <param name="myTypeManager"></param>
        /// <param name="myDBTypeStream"></param>
        /// <param name="myDBObjectStream"></param>
        /// <param name="myBulkInsert"></param>
        internal BulkInsertDBO(DBContext myDBContext, GraphDBType myDBTypeStream, DBObjectStream myDBObjectStream, BulkInsert myBulkInsert)
        {
            _DBContext = myDBContext;
            _DBTypeStream = myDBTypeStream;
            _DBObjectStream = myDBObjectStream;
            _BulkInsert = myBulkInsert;
        }

        /// <summary>
        /// Adds a non edge attribute.
        /// Do not forget to call Flush() after doing all changes!
        /// </summary>
        /// <param name="myAttribute">The attribute type name</param>
        /// <param name="myValue">The value</param>
        /// <returns></returns>
        public BulkInsertDBO AddAttribute(String myAttribute, IObject myValue)
        {
            return AddAttribute(_DBTypeStream.GetTypeAttributeByName(myAttribute), myValue);
        }

        /// <summary>
        /// Adds a non edge attribute.
        /// Do not forget to call Flush() after doing all changes!
        /// </summary>
        /// <param name="myAttribute">The attribute type</param>
        /// <param name="myValue">The value</param>
        /// <returns></returns>
        public BulkInsertDBO AddAttribute(TypeAttribute myAttribute, IObject myValue)
        {
            _DBObjectStream.AddAttribute(myAttribute.UUID, myValue);
            return this;
        }

        /// <summary>
        /// Adds a new edge from the current DBObject to the destination defined by <paramref name="myObjectReference"/>.
        /// Do not forget to call Flush() after doing all changes!
        /// </summary>
        /// <param name="myEdgeAttribute">The edge attribute</param>
        /// <param name="myObjectReference">The destination DBObjectUUID</param>
        /// <param name="myEdgeParams">Optional parameters like weight, etc.</param>
        /// <returns></returns>
        public BulkInsertDBO AddEdge(TypeAttribute myEdgeAttribute, ObjectUUID myObjectReference, params ADBBaseObject[] myEdgeParams)
        {

            #region Currently only ListRefernceEdgeType is implemented

            if (!myEdgeAttribute.GetDBType(_DBContext.DBTypeManager).IsUserDefined)
                throw new GraphDBException(new Error_InvalidEdgeType(typeof(ASetOfReferencesEdgeType), typeof(ASingleReferenceEdgeType)));

            #endregion

            if (!_DBObjectStream.HasAttribute(myEdgeAttribute.UUID, myEdgeAttribute.GetRelatedType(_DBContext.DBTypeManager)))
                _DBObjectStream.AddAttribute(myEdgeAttribute.UUID, myEdgeAttribute.EdgeType.GetNewInstance());

            if (myEdgeAttribute.KindOfType == KindsOfType.SetOfReferences)
                (_DBObjectStream.GetAttribute(myEdgeAttribute.UUID) as ASetOfReferencesEdgeType).Add(myObjectReference, myEdgeAttribute.DBTypeUUID, myEdgeParams);           
            else
                (_DBObjectStream.GetAttribute(myEdgeAttribute.UUID) as ASingleReferenceEdgeType).Set(myObjectReference, myEdgeAttribute.DBTypeUUID, myEdgeParams);

            EdgesCount++;

            return this;
        }

        /// <summary>
        /// Adds a new edge from the current DBObject to the destination defined by <paramref name="myObjectReference"/>.
        /// Do not forget to call Flush() after doing all changes!
        /// </summary>
        /// <param name="myEdgeAttribute">The edge attribute</param>
        /// <param name="myObjectReference">The destination DBObjectUUID</param>
        /// <param name="myEdgeParams">Optional parameters like weight, etc.</param>
        /// <returns></returns>
        public BulkInsertDBO AddEdge(TypeAttribute myEdgeAttribute, IEnumerable<ObjectUUID> myObjectReferences, params ADBBaseObject[] myEdgeParams)
        {

            #region Currently only ListRefernceEdgeType is implemented

            if (myEdgeAttribute.KindOfType != KindsOfType.SetOfReferences)
                throw new GraphDBException(new Error_InvalidEdgeType(typeof(ASetOfReferencesEdgeType), typeof(ASingleReferenceEdgeType)));

            #endregion

            if (!_DBObjectStream.HasAttribute(myEdgeAttribute.UUID, myEdgeAttribute.GetRelatedType(_DBContext.DBTypeManager)))
                _DBObjectStream.AddAttribute(myEdgeAttribute.UUID, myEdgeAttribute.EdgeType.GetNewInstance());

            if (myEdgeAttribute.KindOfType == KindsOfType.SetOfReferences)
                (_DBObjectStream.GetAttribute(myEdgeAttribute.UUID) as ASetOfReferencesEdgeType).AddRange(myObjectReferences, myEdgeAttribute.DBTypeUUID,  myEdgeParams);

            EdgesCount += (UInt64)myObjectReferences.Count();

            return this;
        }

        /// <summary>
        /// Adds a new edge from the current DBObject to the destination defined by <paramref name="myObjectReference"/>.
        /// Do not forget to call Flush() after doing all changes!
        /// </summary>
        /// <param name="myEdgeAttribute">The edge attribute</param>
        /// <param name="myObjectReference">The destination DBObjectUUID</param>
        /// <param name="myEdgeParams">Optional parameters like weight, etc.</param>
        /// <returns></returns>
        public BulkInsertDBO AddEdge(String myEdgeAttribute, ObjectUUID myObjectReference, params ADBBaseObject[] myEdgeParams)
        {
            return AddEdge(_DBTypeStream.GetTypeAttributeByName(myEdgeAttribute), myObjectReference, myEdgeParams);
        }

        /// <summary>
        /// Adds a new BackwardEdge from the current DBObject to the destination attribute identified by <paramref name="myBackwardEdgeAttribute"/>.
        /// Do not forget to call Flush() after doing all changes!
        /// </summary>
        /// <param name="myBackwardEdgeAttribute">The destination type and attribute</param>
        /// <param name="myObjectReference">The destination DBObject</param>
        /// <returns></returns>
        public BulkInsertDBO AddBackwardEdge(EdgeKey myBackwardEdgeAttribute, ObjectUUID myObjectReference)
        {
            if (_BackwardEdge == null)
                _BackwardEdge = new BackwardEdgeStream(_DBObjectStream.ObjectLocation);
            _BackwardEdge.AddBackwardEdge(myBackwardEdgeAttribute, myObjectReference, _DBContext.DBObjectManager);

            BackwardEdgesCount++;

            return this;
        }

        /// <summary>
        /// Adds some new BackwardEdges from the current DBObject to the destination attribute identified by <paramref name="myBackwardEdgeAttribute"/>.
        /// Do not forget to call Flush() after doing all changes!
        /// </summary>
        /// <param name="myBackwardEdgeAttribute">The destination type and attribute</param>
        /// <param name="myObjectReference">The destination DBObject</param>
        /// <returns></returns>
        public BulkInsertDBO AddBackwardEdge(EdgeKey myBackwardEdgeAttribute, IEnumerable<ObjectUUID> myObjectReference)
        {
            if (_BackwardEdge == null)
                _BackwardEdge = new BackwardEdgeStream(_DBObjectStream.ObjectLocation);
            _BackwardEdge.AddBackwardEdge(myBackwardEdgeAttribute, myObjectReference, _DBContext.DBObjectManager);

            BackwardEdgesCount += (UInt64)myObjectReference.Count();

            return this;
        }

        /// <summary>
        /// Stores the last DBObject into the filesystem.
        /// </summary>
        public void Flush()
        {
            _BulkInsert.Insert(_DBObjectStream, _BackwardEdge, EdgesCount, BackwardEdgesCount);
        }

    }
}
