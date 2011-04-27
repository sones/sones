using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.Result;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphQL.GQL.Structure.Nodes.Expressions;

namespace sones.GraphQL.GQL.Manager.Select
{
    public class SelectManager
    {
        public QueryResult ExecuteSelect(IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken, SelectDefinition selectDefinition)
        {
            throw new NotImplementedException();
        }
    }
}
