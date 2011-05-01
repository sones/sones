using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;

namespace sones.GraphDB.Manager
{
    public interface IManager
    {
        void Initialize(IMetaManager myMetaManager);

        void Load(TransactionToken myTransaction, SecurityToken mySecurity);
    }
}
