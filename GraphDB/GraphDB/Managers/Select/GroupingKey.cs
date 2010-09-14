using System;
using System.Collections.Generic;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement;

namespace sones.GraphDB.Managers.Select
{

    #region GroupingKey and GroupingValuesKey

    /// <summary>
    /// A structure to store an attribute with the related alias
    /// </summary>
    public struct GroupingValuesKey
    {
        public TypeAttribute TypeAttribute;
        public String AttributeAlias;

        public GroupingValuesKey(TypeAttribute myTypeAttribute, String myAttributeAlias)
        {
            TypeAttribute = myTypeAttribute;
            AttributeAlias = myAttributeAlias;
        }

        public override string ToString()
        {
            return AttributeAlias;
        }

        public override int GetHashCode()
        {
            return TypeAttribute.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is GroupingValuesKey))
            {
                return false;
            }

            return ((GroupingValuesKey)obj).TypeAttribute.Equals(TypeAttribute);

        }

    }

    /// <summary>
    /// This structures stores multiple attribute-value combination for grouping.
    /// Each key can contain some values and can be compared with another key.
    /// </summary>
    public class GroupingKey : IComparable<Dictionary<GroupingValuesKey, IObject>>
    {

        Dictionary<GroupingValuesKey, IObject> _Values;

        public Dictionary<GroupingValuesKey, IObject> Values
        {
            get { return _Values; }
            set { _Values = value; }
        }

        public GroupingKey(Dictionary<GroupingValuesKey, IObject> myValues)
        {
            _Values = myValues;
        }

        #region IComparable<ADBBaseObject> Members

        public int CompareTo(Dictionary<GroupingValuesKey, IObject> other)
        {

            // it could happen, that not all grouped aatributes are existing in all DBOs, so use the group by from the select to check
            foreach (var attr in _Values.Keys)
            {
                if (_Values.ContainsKey(attr) && other.ContainsKey(attr))
                {
                    if (_Values[attr] == null && other[attr] == null)
                    {
                        continue;
                    }
                    else if (_Values[attr] == null)
                    {
                        return -1;
                    }
                    else if (_Values[attr].CompareTo(other[attr]) != 0)
                    {
                        return -1;
                    }
                }
            }

            return 0;
        }

        #endregion

        public override int GetHashCode()
        {
            Int32 retVal = -1;
            foreach (KeyValuePair<GroupingValuesKey, IObject> keyValPair in _Values)
            {
                if (keyValPair.Value == null)
                {
                    continue;
                }

                if (retVal == -1)
                    retVal = keyValPair.Value.GetHashCode();
                else
                    retVal = retVal ^ keyValPair.Value.GetHashCode();
            }

            return retVal;
        }

        public override bool Equals(object obj)
        {
            return CompareTo(((GroupingKey)obj).Values) == 0;
        }
    }
    
    #endregion

}
