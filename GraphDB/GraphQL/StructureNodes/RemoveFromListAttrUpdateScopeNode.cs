/* <id name="GraphDB – RemoveFromListAttrUpdateScopeNode Node" />
 * <copyright file="RemoveFromListAttrUpdateScopeNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Dirk Bludau</developer> 
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{
    /// <summary>
    /// contains the scope of a list update statement
    /// </summary>
    public class RemoveFromListAttrUpdateScopeNode : RemoveFromListAttrUpdateNode
    {

        #region propertys

        public TupleDefinition TupleDefinition { get; private set; }
        
        #endregion

        #region constructor

        public RemoveFromListAttrUpdateScopeNode()
        { }

        #endregion

        /// <summary>
        /// Get the scope of an remove list update
        /// </summary>
        /// <param name="context">Irony compiler context</param>
        /// <param name="parseNode">The parse node that contains the information</param>
        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            if (parseNode.ChildNodes[1].AstNode is CollectionOfDBObjectsNode)
            { 
                var collection = parseNode.ChildNodes[1].AstNode as CollectionOfDBObjectsNode;

                if (collection.CollectionDefinition.CollectionType != CollectionType.SetOfUUIDs)
                {
                    throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                }

                TupleDefinition = collection.CollectionDefinition.TupleDefinition;
            }

            if (parseNode.ChildNodes[1].AstNode is TupleNode)
            {
                TupleDefinition = ((TupleNode)parseNode.ChildNodes[1].AstNode).TupleDefinition;
            }
        }

    }
}
