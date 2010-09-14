
#region Usings

using System;
using System.Text;

#endregion

namespace sones.GraphFS.Transactions
{

    /// <summary>
    /// Handles the event for a disposed transaction.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void TransactionDisposedHandler(Object sender, TransactionDisposedEventArgs args);

}
