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
using sones.GraphDB.Expression;

namespace sones.GraphDB.Request
{
    public sealed class RequestDelete : IRequest
    {
        #region Data

        public readonly RequestGetVertices ToBeDeletedVertices;
        public readonly HashSet<String> ToBeDeletedAttributes;

        public readonly List<IComparable> DeletedVertices;
        public readonly List<IComparable> DeletedAttributes;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new delete request that is able to delete attributes from certain vertices or 
        /// the vertices themself
        /// </summary>
        /// <param name="myToBeDeletedVertices">The vertices that should be deleted/changed</param>
        public RequestDelete(RequestGetVertices myToBeDeletedVertices)
        {
            ToBeDeletedAttributes = new HashSet<string>();
            ToBeDeletedVertices = myToBeDeletedVertices;

            DeletedAttributes = new List<IComparable>();
            DeletedVertices = new List<IComparable>();
        }

        #endregion

        #region IRequest Members

        public GraphDBAccessMode AccessMode
        {
            get { return GraphDBAccessMode.TypeChange; }
        }

        #endregion

        #region fluent methods

        /// <summary>
        /// Adds an attribute that should be deleted
        /// </summary>
        /// <param name="myAttributeName">The name of the attribute that should be deleted</param>
        /// <returns>The request itself</returns>
        public RequestDelete AddAttribute(String myAttributeName)
        {
            if (!String.IsNullOrWhiteSpace(myAttributeName))
            {
                ToBeDeletedAttributes.Add(myAttributeName);                
            }

            return this;
        }

        /// <summary>
        /// Adds attributes that should be deleted
        /// </summary>
        /// <param name="myAttributeNames">The names of the attributes that should be deleted</param>
        /// <returns>The request itself</returns>
        public RequestDelete AddAttributes(IEnumerable<String> myAttributeNames)
        {
            foreach (var aToBeDeletedAttribute in myAttributeNames)
            {
                this.AddAttribute(aToBeDeletedAttribute);
            }

            return this;
        }

        public RequestDelete AddDeletedAttribute(long myAttrID)
        {
            DeletedAttributes.Add(myAttrID);

            return this;
        }

        public RequestDelete AddDeletedVertex(long myVertexID)
        {
            DeletedVertices.Add(myVertexID);

            return this;
        }

        #endregion
    }
}
