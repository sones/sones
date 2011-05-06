using System;
using Irony.Ast;
using Irony.Parsing;
using System.Collections.Generic;
using sones.GraphQL.GQL.Structure.Helper.Definition.Update;
using sones.GraphQL.ErrorHandling;

namespace sones.GraphQL.Structure.Nodes.DML
{
    /// <summary>
    /// This node is requested in case of an AttrUpdateOrAssignListNode Node.
    /// </summary>
    public sealed class AttributeUpdateOrAssignListNode : AStructureNode, IAstNodeInit
    {
        #region Properties

        public HashSet<AAttributeAssignOrUpdateOrRemove> ListOfUpdate { get; private set; }

        #endregion

        #region constructor

        public AttributeUpdateOrAssignListNode()
        {
            ListOfUpdate = new HashSet<AAttributeAssignOrUpdateOrRemove>();
        }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
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
                        if (aChild.AstNode is AttributeRemoveNode)
                        {

                            #region remove attribute

                            ListOfUpdate.Add((aChild.AstNode as AttributeRemoveNode).AttributeRemove);

                            #endregion

                        }
                        else
                        {
                            throw new NotImplementedQLException("Invalid task node \"" + aChild.AstNode.GetType().Name + "\" in update statement");
                        }
                    }
                }
            }
        }

        #endregion
    }
}
