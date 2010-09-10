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

/* <id name="PandoraDB – BulkTypeListMember Node" />
 * <copyright file="BulkTypeListMemberNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This node is requested in case of an BulkTypeListMember node.</summary>
 */

#region usings

using System;
using System.Linq;
using System.Collections.Generic;
using sones.GraphDB.Exceptions;
using sones.GraphDB.TypeManagement;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;
using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure
{
    /// <summary>
    /// 
    /// </summary>
    public class BulkTypeListMemberNode : AStructureNode
    {

        #region Data

        private String  _TypeName = ""; //the name of the type that should be created
        private String _Extends = ""; //the name of the type that should be extended
        private String _Comment = ""; //the name of the type that should be extended
        private Boolean _IsAbstract = false;
        private Dictionary<TypeAttribute, String> _Attributes = new Dictionary<TypeAttribute, String>(); //the dictionayry of attribute definitions
        private List<BackwardEdgeNode> _BackwardEdgeInformation;
        private List<Exceptional<IndexOptOnCreateTypeMemberNode>> _Indices;

        #endregion

        #region constructor

        public BulkTypeListMemberNode()
        {
        }

        #endregion

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            try
            {
                #region get Abstract

                if (parseNode.ChildNodes[0].HasChildNodes())
                    _IsAbstract = true;

                #endregion

                var bulkTypeNode = (BulkTypeNode)parseNode.ChildNodes[1].AstNode;

                #region get Name

                _TypeName = bulkTypeNode.TypeName;

                #endregion

                #region get Extends

                _Extends = bulkTypeNode.Extends;

                #endregion

                #region get myAttributes

                _Attributes = bulkTypeNode.Attributes;

                #endregion

                #region get BackwardEdges

                _BackwardEdgeInformation = bulkTypeNode.BackwardEdges;

                #endregion

                #region Get Optional Indices

                _Indices = bulkTypeNode.Indices;

                #endregion

                #region get Comment

                _Comment = bulkTypeNode.Comment;

                #endregion
            }
            catch (GraphDBException e)
            {
                throw new GraphDBException(e.GraphDBErrors);
            }

        }

        #region Accessessors

        public String TypeName { get { return _TypeName; } }
        public String Extends { get { return _Extends; } }
        public String Comment { get { return _Comment; } }
        public Boolean IsAbstract { get { return _IsAbstract; } }
        public Dictionary<TypeAttribute, String> Attributes { get { return _Attributes; } }
        public List<BackwardEdgeNode> BackwardEdges { get { return _BackwardEdgeInformation; } }
        public List<Exceptional<IndexOptOnCreateTypeMemberNode>> Indices { get { return _Indices; } }

        #endregion
    }//class
}//namespace
