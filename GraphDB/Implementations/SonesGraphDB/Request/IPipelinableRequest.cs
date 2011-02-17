using System;
using sones.GraphDB.Manager;
using sones.Security;
using sones.Transaction;

namespace sones.GraphDB.Request
{
    public interface IPipelinableRequest
    {
        Guid ID { get; }

        IRequest Request { get; }

        SecurityToken SecurityToken { get; }

        TransactionToken TransactionToken { get; }

        Boolean Validate(MetaManager myMetaManager);

        void Execute(MetaManager myMetaManager);
    }
}
