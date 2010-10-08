/* <id name="DBTransactionManager" />
 * <copyright file="DBTransactionManager.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 */

using System;
using System.Collections.Generic;
using sones.GraphDB.Transactions;
using sones.GraphFS.DataStructures;
using sones.Lib.Caches;

namespace sones.GraphDB.Managers
{

    /// <summary>
    /// This class handles the transactions for stateless connections like rest.
    /// For a transactionUUID you can get the corresponding DBTransaction
    /// </summary>
    public class DBTransactionManager : ASimpleCache<TransactionUUID, DBTransaction>
    {

        public DBTransactionManager()
            : base("DBTransactionManager", new CacheSettings(), new Dictionary<TransactionUUID, SimpleCacheItem<TransactionUUID, DBTransaction>>())
        {
            _GlobalCacheSettings.ExpirationType = ExpirationTypes.Sliding;
            _GlobalCacheSettings.SlidingExpirationTimeSpan = TimeSpan.FromHours(1.0);
            _GlobalCacheSettings.MaxNumberOfCachedItems = 100;

            //base.OnItemRemoved += new EventHandler<ItemRemovedEventArgs<DBTransaction>>(DBTransactionManager_OnItemRemoved);
        }

        void DBTransactionManager_OnItemRemoved(object sender, ItemRemovedEventArgs<DBTransaction> e)
        {
            /// if the item was removed because of a timeout, it is still running and it should be rollbacked
            if (e.Item.State == GraphFS.Transactions.TransactionState.Running)
            {
                e.Item.Rollback();
            }
        }

        public void Set(DBTransaction dbTransaction)
        {
            if (dbTransaction.HasNestedTransaction)
                base.Add(dbTransaction.UUID, dbTransaction, dbTransaction.GetNestedTransaction().UUID);
            else
                base.Add(dbTransaction.UUID, dbTransaction);
        }

        public new DBTransaction Get(TransactionUUID transactionUUID)
        {
            return base.Get(transactionUUID);
        }

        public new Boolean Remove(TransactionUUID transactionUUID)
        {
            return base.Remove(transactionUUID, true, true);
        }

    }

}
