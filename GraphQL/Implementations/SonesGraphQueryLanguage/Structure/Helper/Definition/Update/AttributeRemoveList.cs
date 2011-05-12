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
using sones.GraphQL.GQL.Structure.Nodes.Expressions;
using sones.GraphQL.GQL.Structure.Nodes.Misc;

namespace sones.GraphQL.GQL.Structure.Helper.Definition.Update
{
    /// <summary>
    /// Removes some values from all DBOs of the given IDChain or attributeName
    /// </summary>
    public sealed class AttributeRemoveList : AAttributeRemove
    {

        #region Properties

        /// <summary>
        /// The name of the attribute
        /// </summary>
        public string AttributeName { get; private set; }
        public Object TupleDefinition { get; private set; }

        #endregion

        #region Ctor

        public AttributeRemoveList(IDChainDefinition myIDChainDefinition, string myAttributeName, Object myTupleDefinition)
        {
            // TODO: Complete member initialization
            this.AttributeIDChain = myIDChainDefinition;
            this.AttributeName = myAttributeName;
            this.TupleDefinition = myTupleDefinition;
        }

        #endregion
    }
}
