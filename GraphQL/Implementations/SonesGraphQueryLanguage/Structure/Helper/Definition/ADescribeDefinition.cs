using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using sones.Library.PropertyHyperGraph;
using sones.GraphQL.GQL.Manager.Plugin;
using sones.GraphQL.Result;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;

namespace sones.GraphQL.GQL.Structure.Helper.Definition
{
    /// <summary>
    /// The abstract base class for all describe commands
    /// </summary>
    public abstract class ADescribeDefinition
    {
        /// <summary>
        /// Return the result of a describe command
        /// </summary>
        /// <param name="myDBContext">The db context</param>
        /// <returns>An exceptional that contains an enumerable of vertices</returns>
        public abstract IEnumerable<IVertexView> GetResult(ParsingContext myContext, 
                                                            GQLPluginManager myPluginManager, 
                                                            IGraphDB myGraphDB, 
                                                            SecurityToken mySecurityToken,  
                                                            TransactionToken myTransactionToken);
    }
}
