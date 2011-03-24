using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.Structure.Nodes;
using Irony.Parsing;
using sones.GraphQL.Result;

namespace sones.GraphQL.StatementNodes
{
    /// <summary>
    /// The abstract class for all statements in GraphDB.
    /// </summary>
    public abstract class AStatement : AStructureNode
    {

        #region General Command Infos

        public abstract String StatementName { get; }
        public abstract TypesOfStatements TypeOfStatement { get; }

        #endregion

        #region abstract Methods

        #region abstract Execute

        public abstract QueryResult Execute();

        #endregion

        #endregion
    }
}
