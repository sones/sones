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

namespace sones.GraphQL.GQL.Structure.Nodes.Expressions
{
    public sealed class TupleElement : IEqualityComparer<TupleElement>
    {

        #region Data

        AExpressionDefinition _Value = null;
        public Dictionary<String, object> Parameters { get; set; }

        #endregion

        public TupleElement(AExpressionDefinition Value)
        {
            _Value = Value;
            Parameters = new Dictionary<String, object>();
        }

        public AExpressionDefinition Value { get { return _Value; } }


        public override bool Equals(object obj)
        {
            if (obj is TupleElement)
            {
                return Equals((obj as TupleElement), this);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return GetHashCode(this);
        }

        #region IEqualityComparer<TupleElement> Members

        public bool Equals(TupleElement x, TupleElement y)
        {
            return 
                x.Value.Equals(y.Value) && 
                ((x.Parameters == null) == (y.Parameters == null)) && 
                (x.Parameters.Count == y.Parameters.Count);
        }

        public int GetHashCode(TupleElement obj)
        {
            return obj._Value.GetHashCode();
        }

        #endregion
    }
}
