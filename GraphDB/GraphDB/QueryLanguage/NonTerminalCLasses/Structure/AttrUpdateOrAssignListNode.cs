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


/* <id name="sones GraphDB – AttrUpdateOrAssignListNode Node" />
 * <copyright file="AttrUpdateOrAssignListNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <developer>Stefan Licht</developer>
 * <summary>This node is requested in case of an AttrUpdateOrAssignListNode Node.</summary>
 */

using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using sones.Lib.Frameworks.Irony.Scripting.Runtime;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.Lib.Frameworks.Irony.Scripting.Ast;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.Exceptions;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Errors;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.Managers.Structures;

namespace sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure
{
    /// <summary>
    /// 
    /// </summary>
    public class AttrUpdateOrAssignListNode : AStructureNode, IAstNodeInit
    {
     
        #region Properties

        public HashSet<AAttributeAssignOrUpdateOrRemove> ListOfUpdate { get; private set; }

        #endregion

        #region constructor

        public AttrUpdateOrAssignListNode()
        {
            ListOfUpdate = new HashSet<AAttributeAssignOrUpdateOrRemove>();
        }

        #endregion

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            foreach (ParseTreeNode aChild in parseNode.ChildNodes)
            {
                if (aChild.AstNode is AttributeAssignNode)
                {
                    
                    #region attribute assign

                    AttributeAssignNode aAttributeAssignNode = (AttributeAssignNode)aChild.AstNode;
                    ListOfUpdate.Add((aChild.AstNode as AttributeAssignNode).AttributeValue);
                  
                    #endregion

                }
                else
                {
                    if ((aChild.AstNode is AddToListAttrUpdateNode) || (aChild.AstNode is RemoveFromListAttrUpdateNode))
                    {   

                        #region list update

                        if (aChild.AstNode is AddToListAttrUpdateNode)
                        {
                            ListOfUpdate.Add((aChild.AstNode as AddToListAttrUpdateNode).AttributeUpdateList);

                        }
                        #endregion
                                                
                        if (aChild.AstNode is RemoveFromListAttrUpdateNode)
                        {
                            ListOfUpdate.Add((aChild.AstNode as RemoveFromListAttrUpdateNode).AttributeRemoveList);
                     
                        }
                    }
                    else
                    {
                        if (aChild.AstNode is AttrRemoveNode)
                        {

                            #region remove attribute

                            ListOfUpdate.Add((aChild.AstNode as AttrRemoveNode).AttributeRemove);

                            #endregion

                        }
                        else
                        {
                            throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true), "Invalid task node \"" + aChild.AstNode.GetType().Name + "\" in update statement"));
                        }
                    }
                }
            }
        }

        #region IAstNodeInit Members

        public void Init(CompilerContext context, ParseTreeNode parseNode)
        {
            GetContent(context, parseNode);
        }

        #endregion

    }//class
}//namespace
