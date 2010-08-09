/* <id name="GraphDB – SetRefNode node" />
 * <copyright file="SetRefNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This node is requested in case of SetRefNode statement.</summary>
 */

#region Usings

using System;

using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.GraphQL.StructureNodes;
using sones.GraphDB.TypeManagement.BasicTypes;

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    /// <summary>
    /// This node is requested in case of an SetRefNode statement.
    /// </summary>
    public class SetRefNode : AStructureNode, IAstNodeInit
    {

        public SetRefDefinition SetRefDefinition { get; private set; }

        #region Data

        private Boolean _IsREFUUID = false;

        #endregion

        #region constructor

        public SetRefNode()
        {
            
        }

        #endregion

        private void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            var grammar = GetGraphQLGrammar(context);
            if (parseNode.ChildNodes[0].Term == grammar.S_REFUUID || parseNode.ChildNodes[0].Term == grammar.S_REFERENCEUUID)
            {
                _IsREFUUID = true;
            }

            var tupleNode = parseNode.ChildNodes[1].AstNode as TupleNode;

            if (tupleNode == null)
            {
                throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
            }

            ADBBaseObject[] parameters = null;
            if (parseNode.ChildNodes[2].AstNode is ParametersNode)
            {
                parameters = (parseNode.ChildNodes[2].AstNode as ParametersNode).ParameterValues.ToArray();
            }

            SetRefDefinition = new SetRefDefinition(tupleNode.TupleDefinition, _IsREFUUID, parameters);
        
        }

        #region IAstNodeInit Members

        public void Init(CompilerContext context, ParseTreeNode parseNode)
        {
            GetContent(context, parseNode);
        }

        #endregion

    }

}
