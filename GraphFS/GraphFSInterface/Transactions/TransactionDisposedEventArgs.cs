
#region Usings

using System;
using System.Text;
using sones.GraphFS.Session;
using sones.GraphFS.Session;

#endregion

namespace sones.GraphFS.Transactions
{

    public class TransactionDisposedEventArgs
    {

        private SessionToken _SessionToken;

        public SessionToken SessionToken
        {
            get { return _SessionToken; }
            set { _SessionToken = value; }
        }

        private FSTransaction _Transaction;

        public FSTransaction Transaction
        {
            get { return _Transaction; }
            set { _Transaction = value; }
        }

        public TransactionDisposedEventArgs(FSTransaction myTransaction, SessionToken mySessionToken)
        {
            _Transaction = myTransaction;
            _SessionToken = mySessionToken;
        }

    }
}
