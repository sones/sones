/* <id name="GraphdbDB – IndexKeyDefinition" />
 * <copyright file="IndexKeyDefinition.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 */

#region Usings

using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using sones.GraphDB.TypeManagement;
using sones.Lib.NewFastSerializer;
using sones.Lib.Serializer;
using sones.GraphDB.Managers.Structures;
using sones.Lib;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Errors;
using sones.Lib.ErrorHandling;


#endregion

namespace sones.GraphDB.Indices
{
    /// <summary>
    /// IndexKeyDefinition for any AttributeIndex
    /// </summary>
    public class IndexKeyDefinition : IFastSerialize
    {
        #region Data

        int _hashCode = 0;

        private List<AttributeUUID> _indexKeyAttributeUUIDs = new List<AttributeUUID>();
        public List<AttributeUUID> IndexKeyAttributeUUIDs { get { return _indexKeyAttributeUUIDs; } }

        private Dictionary<AttributeUUID, String> _functions = new Dictionary<AttributeUUID,string>();

        #endregion

        #region Static

        public static Exceptional<IndexKeyDefinition> CreateFromIDChainDefinitions(IEnumerable<IDChainDefinition> myIDChainDefinitions)
        {
            var indexKeyDefinition = new IndexKeyDefinition();
            var retVal = new Exceptional<IndexKeyDefinition>(indexKeyDefinition);
            foreach (var idChainDefinition in myIDChainDefinitions)
            {
                retVal.PushIExceptional(indexKeyDefinition.AddIDChainDefinition(idChainDefinition));
            }
            return retVal;
        }

        public static Exceptional<IndexKeyDefinition> CreateFromIDChainDefinition(IDChainDefinition myIDChainDefinition)
        {
            var indexKeyDefinition = new IndexKeyDefinition();
            var retVal = new Exceptional<IndexKeyDefinition>(indexKeyDefinition);
            retVal.PushIExceptional(indexKeyDefinition.AddIDChainDefinition(myIDChainDefinition));
            return retVal;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Needed for IFastSerialize
        /// </summary>
        public IndexKeyDefinition()
        {

        }

        /// <summary>
        /// Adds a single AttributeUUID to the IndexKeyDefinition
        /// </summary>
        /// <param name="myIndexKeyDefinition">The AttributeUUID that is going to be added</param>
        public IndexKeyDefinition(AttributeUUID myIndexKeyDefinition)
        {
            _indexKeyAttributeUUIDs.Add(myIndexKeyDefinition);

            CalcNewHashCode(myIndexKeyDefinition);
        }

        /// <summary>
        /// Creates a new IndexKeyDefinition with a list of AttributeUUIDs
        /// </summary>
        /// <param name="myIndexKeyDefinitions">List of AttributeUUIDs</param>
        public IndexKeyDefinition(List<AttributeUUID> myIndexKeyDefinitions)
        {
            foreach (var aAttributeUUID in myIndexKeyDefinitions)
            {
                _indexKeyAttributeUUIDs.Add(aAttributeUUID);

                CalcNewHashCode(aAttributeUUID);
            }
        }

        #endregion

        #region private helper

        internal Exceptional AddIDChainDefinition(IDChainDefinition myIDChainDefinition)
        {

            var attr = (myIDChainDefinition.First() as ChainPartTypeOrAttributeDefinition).TypeAttribute;

            if (myIDChainDefinition.Count() == 2) // we validate the ID in the IndexAttributeNode
            {

                if ((myIDChainDefinition.Last() as ChainPartFuncDefinition).Parameters.Count > 0)
                {
                    return new Exceptional(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                }

                _functions[attr.UUID] = (myIDChainDefinition.Last() as ChainPartFuncDefinition).FuncName;

            }

            _indexKeyAttributeUUIDs.Add(attr.UUID);

            CalcNewHashCode(attr.UUID);

            return Exceptional.OK;
        }

        /// <summary>
        /// Calculates a hashcode with the help of the old hashcode and a new AttributeUUID
        /// </summary>
        /// <param name="aAttributeUUID">The AttributeUUID which should be integrated into the hashcode</param>
        private void CalcNewHashCode(AttributeUUID aAttributeUUID)
        {
            _hashCode = _hashCode ^ aAttributeUUID.GetHashCode();
        }

        #endregion


        internal Exceptional<HashSet<IndexKey>> GetIndexkeysFromDBObject(DBObjectStream myDBObject, GraphDBType myTypeOfDBObject, DBContext myDBContext)
        {
            HashSet<IndexKey> result = new HashSet<IndexKey>();
            TypeAttribute currentAttribute;

            foreach (var aIndexAttributeUUID in IndexKeyAttributeUUIDs)
            {
                currentAttribute = myTypeOfDBObject.GetTypeAttributeByUUID(aIndexAttributeUUID);

                if (!currentAttribute.GetDBType(myDBContext.DBTypeManager).IsUserDefined || this._functions.ContainsKey(aIndexAttributeUUID))
                {
                    #region base attribute

                    if (myDBObject.HasAttribute(aIndexAttributeUUID, myTypeOfDBObject))
                    {
                        ADBBaseObject newIndexKeyItem = null;

                        if (this._functions.ContainsKey(aIndexAttributeUUID))
                        {
                            var func = myDBContext.DBPluginManager.GetFunction(this._functions[aIndexAttributeUUID]);
                            func.CallingAttribute = myTypeOfDBObject.GetTypeAttributeByUUID(aIndexAttributeUUID);
                            func.CallingDBObjectStream = myDBObject;
                            func.CallingObject = myDBObject.GetAttribute(aIndexAttributeUUID, myTypeOfDBObject, myDBContext);

                            var funcResult = func.ExecFunc(myDBContext);

                            if (funcResult.Failed())
                            {
                                return new Exceptional<HashSet<IndexKey>>(funcResult);
                            }

                            if (!(funcResult.Value.Value is ADBBaseObject))
                            {
                                return new Exceptional<HashSet<IndexKey>>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                            }

                            result.Add(new IndexKey(aIndexAttributeUUID, funcResult.Value.Value as ADBBaseObject, this));

                            continue;

                        }


                        switch (currentAttribute.KindOfType)
                        {
                            #region List/Set

                            case KindsOfType.ListOfNoneReferences:
                            case KindsOfType.SetOfNoneReferences:

                                var helperSet = new List<ADBBaseObject>();

                                foreach (var aBaseObject in ((IBaseEdge)myDBObject.GetAttribute(aIndexAttributeUUID, myTypeOfDBObject, myDBContext)).GetBaseObjects())
                                {
                                    helperSet.Add((ADBBaseObject)aBaseObject);
                                }

                                if (result.Count != 0)
                                {
                                    #region update

                                    HashSet<IndexKey> helperResultSet = new HashSet<IndexKey>();

                                    foreach (var aNewItem in helperSet)
                                    {
                                        foreach (var aReturnVal in result)
                                        {
                                            helperResultSet.Add(new IndexKey(aReturnVal, aIndexAttributeUUID, aNewItem, this));
                                        }
                                    }

                                    result = helperResultSet;

                                    #endregion
                                }
                                else
                                {
                                    #region create new

                                    foreach (var aNewItem in helperSet)
                                    {
                                        result.Add(new IndexKey(aIndexAttributeUUID, aNewItem, this));
                                    }

                                    #endregion
                                }

                                break;

                            #endregion

                            #region single/special

                            case KindsOfType.SingleReference:
                            case KindsOfType.SingleNoneReference:
                            case KindsOfType.SpecialAttribute:

                                newIndexKeyItem = (ADBBaseObject)myDBObject.GetAttribute(aIndexAttributeUUID, myTypeOfDBObject, myDBContext);

                                if (result.Count != 0)
                                {
                                    #region update

                                    foreach (var aResultItem in result)
                                    {
                                        aResultItem.AddAADBBAseObject(aIndexAttributeUUID, newIndexKeyItem);
                                    }

                                    #endregion
                                }
                                else
                                {
                                    #region create new

                                    result.Add(new IndexKey(aIndexAttributeUUID, newIndexKeyItem, this));

                                    #endregion
                                }

                                break;

                            #endregion

                            #region not implemented

                            case KindsOfType.SetOfReferences:
                            default:

                                throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true), "Currently its not implemented to insert anything else than a List/Set/Single of base types"));

                            #endregion
                        }
                    }
                    else
                    {
                        //add default value

                        var defaultADBBAseObject = GraphDBTypeMapper.GetADBBaseObjectFromUUID(currentAttribute.DBTypeUUID);
                        defaultADBBAseObject.SetValue(DBObjectInitializeType.Default);

                        if (result.Count != 0)
                        {
                            #region update

                            foreach (var aResultItem in result)
                            {
                                aResultItem.AddAADBBAseObject(aIndexAttributeUUID, defaultADBBAseObject);
                            }

                            #endregion
                        }
                        else
                        {
                            #region create new

                            result.Add(new IndexKey(aIndexAttributeUUID, defaultADBBAseObject, this));

                            #endregion
                        }

                    }
                    #endregion
                }
                else
                {
                    #region reference attribute

                    throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));

                    #endregion
                }
            }

            return new Exceptional<HashSet<IndexKey>>(result);

        }

        #region Overrides

        #region Equals Overrides

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override Boolean Equals(Object obj)
        {

            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            if (obj is IndexKeyDefinition)
            {
                IndexKeyDefinition p = (IndexKeyDefinition)obj;
                return Equals(p);
            }
            else
            {
                return false;
            }
        }

        public Boolean Equals(IndexKeyDefinition p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            if (this._indexKeyAttributeUUIDs.Count != p.IndexKeyAttributeUUIDs.Count)
            {
                return false;
            }

            if (this._functions.Count != p._functions.Count)
            {
                return false;
            }

            for (int i = 0; i < _indexKeyAttributeUUIDs.Count; i++)
            {
                if (this._indexKeyAttributeUUIDs[i] != p.IndexKeyAttributeUUIDs[i])
                {
                    return false;
                }
            }

            foreach(var keyVal in _functions)
            {
                if (!p._functions.ContainsKey(keyVal.Key) || keyVal.Value != p._functions[keyVal.Key])
                {
                    return false;
                }
            }

            return true;
        }

        public static Boolean operator ==(IndexKeyDefinition a, IndexKeyDefinition b)
        {
            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Equals(b);
        }

        public static Boolean operator !=(IndexKeyDefinition a, IndexKeyDefinition b)
        {
            return !(a == b);
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            int counter = 1;
            sb.AppendFormat("#{0}: ", _indexKeyAttributeUUIDs.Count);

            foreach (var aUUID in _indexKeyAttributeUUIDs)
            {
                sb.AppendFormat("{0}: {1},", counter, aUUID.ToString());
                counter++;
            }
            sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }

        #endregion

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

        public void Serialize(ref SerializationWriter mySerializationWriter)
        {
            mySerializationWriter.WriteUInt32((UInt32)_indexKeyAttributeUUIDs.Count);
            foreach (var attr in _indexKeyAttributeUUIDs)
                attr.Serialize(ref mySerializationWriter);
        }

        public void Deserialize(ref SerializationReader mySerializationReader)
        {
            UInt32 count = mySerializationReader.ReadUInt32();
            _indexKeyAttributeUUIDs = new List<AttributeUUID>();

            for (UInt32 i = 0; i < count; i++)
            {
                AttributeUUID AttributeUUID = new AttributeUUID();
                AttributeUUID.Deserialize(ref mySerializationReader);
                _indexKeyAttributeUUIDs.Add(AttributeUUID);

                CalcNewHashCode(AttributeUUID);
            }
        }

        #endregion
    }
}
