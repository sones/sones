/* <id name="GraphDB – AttrUpdateOrAssignListNode Node" />
 * <copyright file="AttrUpdateOrAssignListNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <developer>Stefan Licht</developer>
 * <summary>This node is requested in case of an AttrUpdateOrAssignListNode Node.</summary>
 */

#region Usings

using System.Collections.Generic;

using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Managers.Structures;

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    /// <summary>
    /// Warnings: Warning_ObsoleteGQL
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
                            base.ParsingResult.PushIExceptional((aChild.AstNode as AddToListAttrUpdateNode).ParsingResult);
                        }
                        #endregion
                                                
                        if (aChild.AstNode is RemoveFromListAttrUpdateNode)
                        {
                            ListOfUpdate.Add((aChild.AstNode as RemoveFromListAttrUpdateNode).AttributeRemoveList);
                            base.ParsingResult.PushIExceptional((aChild.AstNode as RemoveFromListAttrUpdateNode).ParsingResult);
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

    }

}
