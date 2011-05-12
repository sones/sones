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
using sones.GraphDB.Expression.Tree;

namespace sones.GraphDB.Expression
{
    /// <summary>
    /// This class represents an property expression
    /// </summary>
    public sealed class PropertyExpression : IExpression
    {
        #region Data

        /// <summary>
        /// The name of the vertex type
        /// </summary>
        public readonly String NameOfVertexType;

        /// <summary>
        /// The name of the attribute
        /// </summary>
        public readonly String NameOfProperty;

        /// <summary>
        /// The edition that should be processed
        /// </summary>
        public readonly String Edition;

        /// <summary>
        /// The timespan that should be processed
        /// </summary>
        public readonly TimeSpanDefinition Timespan;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new property expression
        /// </summary>
        /// <param name="myNameOfVertexType">The name of the vertex type</param>
        /// <param name="myNameOfProperty">The name of the attribute</param>
        /// <param name="myEditionName">The edition that should be processed</param>
        /// <param name="myTimeSpanDefinition">The timespan that should be processed</param>
        public PropertyExpression(String myNameOfVertexType, String myNameOfProperty, String myEditionName = null, TimeSpanDefinition myTimeSpanDefinition = null)
        {
            NameOfVertexType = myNameOfVertexType;
            NameOfProperty = myNameOfProperty;
            Edition = myEditionName;
            Timespan = myTimeSpanDefinition;
        }

        #endregion

        #region IExpression Members

        public TypeOfExpression TypeOfExpression
        {
            get { return TypeOfExpression.Property; }
        }

        #endregion
    }
}
