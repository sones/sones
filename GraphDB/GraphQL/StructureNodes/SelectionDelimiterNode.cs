/* <id name="PandoraDB – aggregate node" />
 * <copyright file="AggregateNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This node is requested in case of an aggregate statement.</summary>
 */

#region Usings

using System;

using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Structures.Enums;

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    /// <summary>
    /// This node is requested in case of an aggregate statement.
    /// </summary>
    public class SelectionDelimiterNode : AStructureNode
    {

        #region Data

        private KindOfDelimiter _KindOfDelimiter;

        #endregion

        #region constructor

        public SelectionDelimiterNode()
        {
            
        }

        #endregion

        public void GetContent(CompilerContext context, ParseTreeNode parseNode, KindOfDelimiter myKindOfDelimiter)
        {
            _KindOfDelimiter = myKindOfDelimiter;
        }

        public KindOfDelimiter GetKindOfDelimiter()
        {
            return _KindOfDelimiter;
        }

        public String GetDelimiterString()
        {
            switch (_KindOfDelimiter)
            {
                case KindOfDelimiter.Dot:
                    return DBConstants.EdgeTraversalDelimiterSymbol;

                case KindOfDelimiter.EdgeInformationDelimiter:
                    return DBConstants.EdgeInformationDelimiterSymbol;

                default:
                    throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
            }
        }

    }

}
