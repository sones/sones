/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
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
*/


/* <id name="sones GraphDB – BackwardEdgeNode" />
 * <copyright file="BackwardEdgeNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>One single BackwardEdge definition node.</summary>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.Structures.EdgeTypes;

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure
{
    public class BackwardEdgeNode : AStructureNode
    {
        #region Data

        /// <summary>
        /// The destination type of the backwardedge
        /// </summary>
        public String TypeName
        {
            get { return _TypeName; }
        }
        private String _TypeName;

        /// <summary>
        /// the destination attribute on the TypeName
        /// </summary>
        public String TypeAttributeName
        {
            get { return _TypeAttributeName; }
        }
        private String _TypeAttributeName;

        /// <summary>
        /// The real new name of the attribute
        /// </summary>
        public String AttributeName
        {
            get { return _AttributeName; }
        }
        private String _AttributeName;

        /// <summary>
        /// The Type of the edge, currently EdgeTypeList or EdgeTypeWeightedList
        /// </summary>
        public AEdgeType EdgeType
        {
            get { return _EdgeType; }
        }
        private AEdgeType _EdgeType;

        /// <summary>
        /// The parameters of the EdgeType defined in the statement, for EdgeTypeWeightedList this is the datatype, default weight etc,
        /// </summary>
        public Object[] EdgeTypeParams
        {
            get
            {
                if (_EdgeTypeParamsNode == null)
                    return new Object[0];
                return _EdgeTypeParamsNode.Parameters;
            }
        }
        private EdgeTypeParamsNode _EdgeTypeParamsNode;

        #endregion

        #region constructor

        public BackwardEdgeNode()
        {
            
        }

        #endregion

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            var dbContext = context.IContext as DBContext;
            var typeManager = dbContext.DBTypeManager;
            
            #region Extract type and attribute

            //if (parseNode.ChildNodes.Count != 4)
            //    throw new Exception("This is not a [Type].[Attribute] definition: " + parseNode.ChildNodes[0].ToString());

            _TypeName = parseNode.ChildNodes[0].Token.ValueString;
            _TypeAttributeName = parseNode.ChildNodes[2].Token.ValueString;

            #endregion

            #region Get the AttributeName or specials

            if (parseNode.ChildNodes.Count == 4) // a simple list definition
            {
                _EdgeType = new EdgeTypeSetOfReferences();
                _AttributeName = parseNode.ChildNodes[3].Token.ValueString;
            }
            else
            {
                if (!dbContext.DBPluginManager.HasEdgeType(parseNode.ChildNodes[4].Token.ValueString))
                    throw new Exception("type not found");

                _EdgeType = dbContext.DBPluginManager.GetEdgeType(parseNode.ChildNodes[4].Token.ValueString);
                if (!(_EdgeType is EdgeTypeWeightedList))
                {
                    throw new NotImplementedException(parseNode.ChildNodes[4].Token.ValueString);
                }

                _EdgeTypeParamsNode = parseNode.ChildNodes[5].AstNode as EdgeTypeParamsNode;
                _EdgeType.ApplyParams(_EdgeTypeParamsNode.Parameters);
                _AttributeName = parseNode.ChildNodes[7].Token.ValueString;
            }

            #endregion
        }

    }
}
