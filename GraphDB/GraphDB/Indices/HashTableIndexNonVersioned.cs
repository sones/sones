using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphFS.DataStructures;
using sones.Lib.DataStructures.Indices;
using sones.Lib.ErrorHandling;
using sones.GraphDB.Errors;

namespace sones.GraphDB.Indices
{
    public class HashTableIndexNonVersioned : AAttributeIndex
    {
        public const String INDEX_TYPE = "HashTableNonVersioned";

        public override string IndexType
        {
            get { return INDEX_TYPE; }
        }

        private Boolean _IsUUIDIndex;
        public override Boolean IsUUIDIndex
        {
            get
            {
                return _IsUUIDIndex;
            }
        }

        private Dictionary<IndexKey, HashSet<ObjectUUID>> _Index;
        public HashTableIndexNonVersioned()
        {
            _Index = new Dictionary<IndexKey, HashSet<ObjectUUID>>();
        }

        public override Lib.ErrorHandling.Exceptional Update(ObjectManagement.DBObjectStream myDBObject, TypeManagement.GraphDBType myTypeOfDBObject, DBContext myDBContext)
        {

            #region insert new values

            if (myDBObject.HasAtLeastOneAttribute(this.IndexKeyDefinition.IndexKeyAttributeUUIDs, myTypeOfDBObject, myDBContext.SessionSettings))
            {
                //insert
                foreach (var aIndexKey in GetIndexkeysFromDBObject(myDBObject, myTypeOfDBObject, myDBContext))
                {
                    SetIndexKeyAndValue(aIndexKey, myDBObject.ObjectUUID, IndexSetStrategy.MERGE);
                }
            }

            #endregion

            return Exceptional.OK;

        }

        private void SetIndexKeyAndValue(IndexKey aIndexKey, ObjectUUID objectUUID, IndexSetStrategy indexSetStrategy)
        {
            HashSet<ObjectUUID> values = null;
            if (_Index.TryGetValue(aIndexKey, out values))
            {
                if (indexSetStrategy == IndexSetStrategy.MERGE)
                {
                    values.Add(objectUUID);
                    return;
                }
            }
            _Index[aIndexKey] = new HashSet<ObjectUUID>() { objectUUID };
        }

        public override Lib.ErrorHandling.Exceptional Insert(ObjectManagement.DBObjectStream myDBObject, TypeManagement.GraphDBType myTypeOfDBobject, DBContext myDBContext)
        {
            return Insert(myDBObject, IndexSetStrategy.MERGE, myTypeOfDBobject, myDBContext);
        }

        public override Lib.ErrorHandling.Exceptional Insert(ObjectManagement.DBObjectStream myDBObject, Lib.DataStructures.Indices.IndexSetStrategy myIndexSetStrategy, TypeManagement.GraphDBType myTypeOfDBObject, DBContext myDBContext)
        {
            foreach (var aIndexKex in GetIndexkeysFromDBObject(myDBObject, myTypeOfDBObject, myDBContext))
            {
                #region Check for uniqueness - TODO: remove me as soon as we have a unique indexObject implementation

                if (IsUniqueAttributeIndex)
                {
                    if (_Index.ContainsKey(aIndexKex))
                    {
                        return new Exceptional(new Error_UniqueConstrainViolation(myTypeOfDBObject.Name, IndexName));
                    }
                }

                #endregion

                SetIndexKeyAndValue(aIndexKex, myDBObject.ObjectUUID, myIndexSetStrategy);
            }

            return Exceptional.OK;
        }

        public override bool Contains(ObjectManagement.DBObjectStream myDBObject, TypeManagement.GraphDBType myTypeOfDBObject, DBContext myDBContext)
        {

            foreach (var aIndexKex in GetIndexkeysFromDBObject(myDBObject, myTypeOfDBObject, myDBContext))
            {
                HashSet<ObjectUUID> values = null;
                if (_Index.TryGetValue(aIndexKex, out values))
                {
                    if (values.Contains(myDBObject.ObjectUUID))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public override bool Contains(IndexKey myIndexKey, TypeManagement.GraphDBType myTypeOfDBObject, DBContext myDBContext)
        {
            return _Index.ContainsKey(myIndexKey);
        }

        public override Lib.ErrorHandling.Exceptional Remove(ObjectManagement.DBObjectStream myDBObject, TypeManagement.GraphDBType myTypeOfDBObjects, DBContext myDBContext)
        {

            foreach (var aIndexKey in GetIndexkeysFromDBObject(myDBObject, myTypeOfDBObjects, myDBContext))
            {
                HashSet<ObjectUUID> values = null;
                if (_Index.TryGetValue(aIndexKey, out values))
                {
                    values.Remove(myDBObject.ObjectUUID);
                    if (values.Count == 0)
                    {
                        _Index.Remove(aIndexKey);
                    }
                }
            }

            return Exceptional.OK;

        }

        public override Lib.ErrorHandling.Exceptional ClearAndRemoveFromDisc(DBContext myDBContext)
        {
            _Index.Clear();
            return Exceptional.OK;
        }

        public override IEnumerable<IndexKey> GetKeys(TypeManagement.GraphDBType myTypeOfDBObject, DBContext myDBContext)
        {
            return _Index.Keys;
        }

        public override IEnumerable<IEnumerable<GraphFS.DataStructures.ObjectUUID>> GetAllValues(TypeManagement.GraphDBType myTypeOfDBObject, DBContext myDBContext)
        {
            foreach (var elem in _Index)
            {
                yield return elem.Value;
            }
        }

        public override IEnumerable<GraphFS.DataStructures.ObjectUUID> GetValues(IndexKey myIndeyKey, TypeManagement.GraphDBType myTypeOfDBObject, DBContext myDBContext)
        {
            HashSet<ObjectUUID> retVal;
            if (_Index.TryGetValue(myIndeyKey, out retVal))
            {
                return retVal;
            }
            return new HashSet<ObjectUUID>();
            //foreach (var elem in _Index)
            //{
            //    foreach (var obj in elem.Value)
            //    {
            //        yield return obj;
            //    }
            //}
        }

        public override IEnumerable<KeyValuePair<IndexKey, HashSet<GraphFS.DataStructures.ObjectUUID>>> GetKeyValues(TypeManagement.GraphDBType myTypeOfDBObject, DBContext myDBContext)
        {
            foreach (var elem in _Index)
            {
                yield return elem;
            }
        }

        public override ulong GetValueCount(DBContext myDBContext, TypeManagement.GraphDBType myTypeOfDBObject)
        {
            return _Index.Aggregate(0UL, (result, elem) => result += (ulong)elem.Value.Count);
        }

        public override ulong GetKeyCount(DBContext myDBContext, TypeManagement.GraphDBType myTypeOfDBObject)
        {
            return (ulong)_Index.Count;
        }

        public override IEnumerable<GraphFS.DataStructures.ObjectUUID> InRange(IndexKey myFromKey, IndexKey myToKey, bool myOrEqualFromKey, bool myOrEqualToKey, TypeManagement.GraphDBType myTypeOfDBObject, DBContext myDBContext)
        {

            HashSet<ObjectUUID> resultSet;

            #region myFromKey == myToKey

            if (myFromKey.CompareTo(myToKey) == 0) //from and to are the same
            {
                //lower or upper bound included?
                if (myOrEqualFromKey || myOrEqualToKey)
                {
                    if (_Index.TryGetValue(myFromKey, out resultSet))
                    {
                        foreach (var val in resultSet)
                        {
                            yield return val;
                        }
                    }
                }
                //keys are equal, but the bounds themselves are not included in the search
            }

            #endregion

            #region myFromKey > myToKey

            else if (myFromKey.CompareTo(myToKey) == 1)
            {
                //check bounds

                //1st return all values between fromKey and most right key in the tree
                foreach (var kvp in _Index.Where((kv) => ((myOrEqualFromKey) ? kv.Key.CompareTo(myFromKey) >= 0 : kv.Key.CompareTo(myFromKey) > 0)))
                {
                    foreach (var val in kvp.Value)
                    {
                        yield return val;
                    }
                }


                //2nd return all values between the most left key in the tree and the toKey
                foreach (var kvp in _Index.Where((kv) => ((myOrEqualToKey) ? kv.Key.CompareTo(myToKey) <= 0 : kv.Key.CompareTo(myToKey) < 0)))
                {
                    foreach (var val in kvp.Value)
                    {
                        yield return val;
                    }
                }
            }

            #endregion

            #region myFromKey < myToKey

            else if (myFromKey.CompareTo(myToKey) == -1)
            {
                #region start returning values

                //get indexValueHistoryLists
                var keyValuePairs = _Index.Where(
                        (kv) =>
                            ((myOrEqualFromKey) ? kv.Key.CompareTo(myFromKey) >= 0 : kv.Key.CompareTo(myFromKey) > 0)
                            &&
                            ((myOrEqualToKey) ? kv.Key.CompareTo(myToKey) <= 0 : kv.Key.CompareTo(myToKey) < 0));

                foreach (var kvp in keyValuePairs)
                {
                    foreach (var val in kvp.Value)
                    {
                        yield return val;
                    }
                }

                #endregion
            }

            #endregion

        }

        public override AAttributeIndex GetNewInstance()
        {
            return new HashTableIndexNonVersioned();
        }

        public override Lib.ErrorHandling.Exceptional Initialize(DBContext myDBContext, string indexName, IndexKeyDefinition idxKey, TypeManagement.GraphDBType correspondingType, string indexEdition = DBConstants.DEFAULTINDEX)
        {
            _Index = new Dictionary<IndexKey, HashSet<ObjectUUID>>();
            IndexKeyDefinition = idxKey;
            IndexEdition = indexEdition;
            IndexName = indexName;
            IndexRelatedTypeUUID = correspondingType.UUID;

            _IsUUIDIndex = idxKey.IndexKeyAttributeUUIDs.Count == 1 && idxKey.IndexKeyAttributeUUIDs[0].Equals(myDBContext.DBTypeManager.GetUUIDTypeAttribute().UUID);

            return Exceptional.OK;
        }

        public override bool SupportsType(Type type)
        {
            return true;
        }

        public override uint TypeCode
        {
            get { return 12345; }
        }

        public override void Serialize(ref Lib.NewFastSerializer.SerializationWriter mySerializationWriter)
        {
            throw new NotImplementedException();
        }

        public override void Deserialize(ref Lib.NewFastSerializer.SerializationReader mySerializationReader)
        {
            throw new NotImplementedException();
        }

        public override Lib.ErrorHandling.Exceptional Clear(DBContext myDBContext, TypeManagement.GraphDBType myTypeOfDBObject)
        {
            _Index.Clear();
            return Exceptional.OK;
        }
    }
}
