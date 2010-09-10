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

/* <id name="GraphDB – BackwardEdgeNode" />
 * <copyright file="BackwardEdgeNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>One single BackwardEdge definition node.</summary>
 */

#region Usings

using System;

using sones.GraphDB.Managers.Structures;
using sones.GraphDB.GraphQL.StructureNodes;
using sones.GraphDB.Structures.EdgeTypes;

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    public class BackwardEdgeNode : AStructureNode
    {

        #region Data

        public BackwardEdgeDefinition BackwardEdgeDefinition { get; private set; }

        /// <summary>
        /// The destination type of the backwardedge
        /// </summary>
        private String _TypeName;

        /// <summary>
        /// the destination attribute on the TypeName
        /// </summary>
        private String _TypeAttributeName;

        /// <summary>
        /// The real new name of the attribute
        /// </summary>
        private String _AttributeName;

        /// <summary>
        /// The Type of the edge
        /// </summary>
        private IEdgeType _EdgeType;

        #endregion

        #region constructor

        public BackwardEdgeNode()
        {
            
        }

        #endregion

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            
            #region Extract type and attribute

            //if (parseNode.ChildNodes.Count != 4)
            //    throw new Exception("This is not a [Type].[Attribute] definition: " + parseNode.ChildNodes[0].ToString());

            _TypeName = parseNode.ChildNodes[0].Token.ValueString;
            _TypeAttributeName = parseNode.ChildNodes[2].Token.ValueString;

            #endregion

            _EdgeType = new EdgeTypeSetOfReferences();
            _AttributeName = parseNode.ChildNodes[3].Token.ValueString;

            BackwardEdgeDefinition = new BackwardEdgeDefinition(_AttributeName, _TypeName, _TypeAttributeName, _EdgeType);

        }

    }

}
