/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;

namespace sones.GraphQL.GQL.Manager.Select
{
    #region GroupingKey and GroupingValuesKey

    /// <summary>
    /// A structure to store an attribute with the related alias
    /// </summary>
    public struct GroupingValuesKey
    {
        public IAttributeDefinition TypeAttribute;
        public String AttributeAlias;

        public GroupingValuesKey(IAttributeDefinition myTypeAttribute, String myAttributeAlias)
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
    public class GroupingKey : IComparable<Dictionary<GroupingValuesKey, IComparable>>
    {

        Dictionary<GroupingValuesKey, IComparable > _Values;

        public Dictionary<GroupingValuesKey, IComparable> Values
        {
            get { return _Values; }
            set { _Values = value; }
        }

        public GroupingKey(Dictionary<GroupingValuesKey, IComparable> myValues)
        {
            _Values = myValues;
        }

        #region IComparable<ADBBaseObject> Members

        public int CompareTo(Dictionary<GroupingValuesKey, IComparable> other)
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
            foreach (KeyValuePair<GroupingValuesKey, IComparable> keyValPair in _Values)
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
