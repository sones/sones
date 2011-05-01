using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.Commons.Transaction;
using sones.Library.Commons.Security;

namespace sones.GraphDB.Manager
{
    public interface IManagerOf<T>: IManager
    {
        /// <summary>
        /// Returns an instance, that can be used to check if the entire method of the manager <typeparam name="T"/>can be executed correctly.
        /// </summary>
        T CheckManager { get; }

        /// <summary>
        /// Returns an instance, that can be used to execute the entire method of the manager <typeparam name="T"/>.
        /// </summary>
        T ExecuteManager { get; }

        /// <summary>
        /// Returns an instance, that can be used to undo the execution of an entire method invoked on the ExecuteManager.
        /// </summary>
        T UndoManager { get; }


    }
}
