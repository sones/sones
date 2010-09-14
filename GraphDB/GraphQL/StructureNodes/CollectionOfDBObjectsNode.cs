/* <id name="GraphDB – CollectionOfDBObjectsNode node" />
 * <copyright file="CollectionOfDBObjectsNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <developer>Stefan Licht</developer>
 * <summary>This node is requested in case of CollectionOfDBObjectsNode statement (SETOF or LISTOF).</summary>
 */

#region Usings

using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Managers.Structures;
using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    /// <summary>
    /// This node is requested in case of CollectionOfDBObjectsNode statement (SETOF or LISTOF).
    /// </summary>
    public class CollectionOfDBObjectsNode : AStructureNode, IAstNodeInit
    {

        public CollectionDefinition CollectionDefinition { get; private set; }

        #region constructor

        public CollectionOfDBObjectsNode()
        {
            
        }

        #endregion

        private void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            var multiplyString = parseNode.ChildNodes[0].Token.Text.ToUpper();
            CollectionType collectionType;

            switch (multiplyString)
            {
                case DBConstants.LISTOF:

                    collectionType = CollectionType.List;

                    break;

                case DBConstants.SETOF:

                    collectionType = CollectionType.Set;

                    break;

                case DBConstants.SETOFUUIDS:

                    collectionType = CollectionType.SetOfUUIDs;

                    break;
                default:

                    throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
            }

            var tupleNode = (TupleNode)parseNode.ChildNodes[1].AstNode;

            if (tupleNode == null)
            {
                CollectionDefinition = new CollectionDefinition(collectionType, new TupleDefinition());
            }

            else
            {
                CollectionDefinition = new CollectionDefinition(collectionType, tupleNode.TupleDefinition);
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
