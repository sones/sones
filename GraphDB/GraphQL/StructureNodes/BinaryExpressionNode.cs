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

/* <id name="GraphDB – BinaryExpressionNode" />
 * <copyright file="BinaryExpressionNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <developer>Daniel Kirstenpfad</developer>
 * <developer>Stefan Licht</developer>
 * <summary>This node is requested in case of expression statement.</summary>
 */

#region Usings

using System;

using sones.GraphDB.Managers.Structures;

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    public class BinaryExpressionNode : AStructureNode
    {

        #region Data

        private String _OperatorSymbol;
        private AExpressionDefinition _left = null;
        private AExpressionDefinition _right = null;
        private String OriginalString = String.Empty;
        
        #endregion

        #region Properties

        public BinaryExpressionDefinition BinaryExpressionDefinition { get; private set; }

        #endregion

        #region constructor

        public BinaryExpressionNode()
        {

        }

        #endregion

        #region public methods

        /// <summary>
        /// Handles the Binary Expression node
        /// </summary>
        /// <param name="context"></param>
        /// <param name="parseNode"></param>
        /// <param name="typeManager"></param>
        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            #region set type of binary expression
            
            _OperatorSymbol = parseNode.ChildNodes[1].Term.Name;
            _left = GetExpressionDefinition(parseNode.ChildNodes[0]);
            _right = GetExpressionDefinition(parseNode.ChildNodes[2]);

            #endregion

            BinaryExpressionDefinition = new BinaryExpressionDefinition(_OperatorSymbol, _left, _right);

            OriginalString += _left.ToString() + " ";
            OriginalString += _OperatorSymbol + " ";
            OriginalString += _right.ToString();
            
        }

        #endregion

    }

}
