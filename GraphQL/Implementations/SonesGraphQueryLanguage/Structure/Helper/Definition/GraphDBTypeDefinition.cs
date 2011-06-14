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
using sones.GraphQL.ErrorHandling;

namespace sones.GraphQL.GQL.Structure.Helper.Definition
{
    public class GraphDBTypeDefinition
    {
        #region Properties

        #region Name

        public String Name { get; private set; }

        #endregion

        #region ParentType

        public String ParentType
        {
            get;
            private set;
        }

        #endregion

        #region Attributes

        public Dictionary<AttributeDefinition, String> Attributes
        {
            get;
            private set;
        }

        #endregion

        #region BackwardEdgeNode

        public List<IncomingEdgeDefinition> BackwardEdgeNodes
        {
            get;
            private set;
        }

        #endregion

        #region Indices

        public List<IndexDefinition> Indices
        {
            get;
            private set;
        }

        #endregion

        #region Abstract

        public Boolean IsAbstract
        {
            get;
            private set;
        }

        #endregion

        #region Comment

        /// <summary>
        /// A comment for the type
        /// </summary>
        public String Comment
        {
            get;
            private set;
        }

        #endregion

        #endregion

        #region Constructor

        public GraphDBTypeDefinition(String myName, String myParentType, Boolean myIsAbstract, Dictionary<AttributeDefinition, String> myAttributes, List<IncomingEdgeDefinition> myBackwardEdgeNodes = null, List<IndexDefinition> myIndices = null, String myComment = null)
        {

            Name = myName;
            ParentType = myParentType;
            Attributes = myAttributes;

            //check that no SET or LIST Attribute is set as UNIQUE, it's not possible at this time
            foreach (var item in Attributes.Keys)
            {
                if (item.AttributeType.TypeCharacteristics.IsUnique && 
                        (item.AttributeType.Type.ToUpper().Equals("SET") || item.AttributeType.Type.ToUpper().Equals("LIST")))
                    throw new InvalidVertexAttributeKindException(item.AttributeType.Type, "Single");
            }

            BackwardEdgeNodes = myBackwardEdgeNodes;
            Indices = myIndices;
            IsAbstract = myIsAbstract;
            Comment = myComment;

        }

        #endregion

    }
}
