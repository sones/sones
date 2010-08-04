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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.Lib.DataStructures;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Managers.Structures;
using sones.Lib.ErrorHandling;

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure
{

    public class OrderByNode : AStructureNode, IAstNodeInit
    {

        public OrderByDefinition OrderByDefinition { get; private set; }

        private SortDirection _OrderDirection;
        private List<OrderByAttributeDefinition> _OrderByAttributeList;
       

        public OrderByNode() { }

        private void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            if (parseNode.HasChildNodes())
            {
                if (parseNode.ChildNodes[3] != null && parseNode.ChildNodes[3].HasChildNodes() && parseNode.ChildNodes[3].ChildNodes[0].Term.Name.ToUpper() == "DESC")
                    _OrderDirection = SortDirection.Desc;
                else
                    _OrderDirection = SortDirection.Asc;

                _OrderByAttributeList = new List<OrderByAttributeDefinition>();

                foreach (ParseTreeNode treeNode in parseNode.ChildNodes[2].ChildNodes)
                {
                    if (treeNode.AstNode != null && treeNode.AstNode is IDNode)
                    {
                        Exceptional validateResult = ((IDNode)treeNode.AstNode).IDChainDefinition.Validate(context.IContext as DBContext, false);
                        if (validateResult.Failed)
                        {
                            throw new GraphDBException(validateResult.Errors);
                        }
                        _OrderByAttributeList.Add(new OrderByAttributeDefinition(((IDNode)treeNode.AstNode).IDChainDefinition.Edges, ((IDNode)treeNode.AstNode).IDChainDefinition.LastAttribute.Name));
                    }
                    else
                    {
                        _OrderByAttributeList.Add(new OrderByAttributeDefinition(null, treeNode.Token.ValueString));
                    }
                }

                OrderByDefinition = new OrderByDefinition(_OrderDirection, _OrderByAttributeList);
            }
        }

        #region IAstNodeInit Members

        public void Init(CompilerContext context, ParseTreeNode parseNode)
        {
            GetContent(context, parseNode);
        }

        #endregion
    }

    public struct OrderByAttributeDefinition
    {
        List<EdgeKey> _edges;

        /// <summary>
        /// List of EdgeKeys
        /// </summary>
        public List<EdgeKey> Edges
        {
            get { return _edges; }
        }


        String _asOrderByString;

        /// <summary>
        /// in case of an as, this would be the as-string.
        /// if there has been no as-option, the name of the last attribute of the IDNode is used
        /// </summary>
        public String AsOrderByString
        {
            get { return _asOrderByString; }
        }

        public OrderByAttributeDefinition(List<EdgeKey> myEdges, String myAsOrderByString)
        {
            _edges = myEdges;
            _asOrderByString = myAsOrderByString;
        }
    }
}
