/* GraphFS - FSSessionSettings
 * (c) Stefan Licht, 2009
 *  
 * This class stores all file system specific settings for one session
 * 
 * Lead programmer:
 *      Stefan Licht
 * 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphFS
{

    /// <summary>
    /// Stores all file system specific settings for one session
    /// </summary>
    public class FSSessionSettings
    {

        public enum ValidSettings : byte
        {
            AutoCommitAfterNumberOfPendingTransactions,
            PreallocateObjects
        }


        #region AutoCommitAfterNumberOfPendingTransactions

        Int32 _AutoCommitAfterNumberOfPendingTransactions = 300;

        /// <summary>
        /// AutoCommit current transaction after numberOf pending transactions reaches this value
        /// Default: <value>30</value>
        /// </summary>
        public Int32 AutoCommitAfterNumberOfPendingTransactions
        {

            get
            {
                return _AutoCommitAfterNumberOfPendingTransactions;
            }

            set
            {
                _AutoCommitAfterNumberOfPendingTransactions = value;
            }

        }

        #endregion

        #region PreallocateObjects

        Boolean _PreallocateObjects = true;

        /// <summary>
        /// If true, each changes on an object will result in a preallocate of this object. This will again result in a serialize/deserialize
        /// Keep in mind, that no changes will be flushed to the disk during this time!
        /// Default: <value>true</value>
        /// </summary>
        public Boolean PreallocateObjects
        {

            get
            {
                return _PreallocateObjects;
            }

            set
            {
                _PreallocateObjects = value;
            }

        }

        #endregion

        #region UseRevisionsForParentDirectories

        /// <summary>
        /// If set to False - the parent directory will NOT be cloned. All changes (adding streams) will be done on the same revision
        /// </summary>
        public Boolean UseRevisionsForParentDirectories
        {
            get { return _UseRevisionsForParentDirectories; }
            set { _UseRevisionsForParentDirectories = value; }
        }
        private Boolean _UseRevisionsForParentDirectories = true;

        #endregion

        #region ReflushAllocationMap

        /// <summary>
        /// If set to False - the parent directory will NOT be cloned. All changes (adding streams) will be done on the same revision
        /// </summary>
        public Boolean ReflushAllocationMap
        {
            get { return _ReflushAllocationMap; }
            set { _ReflushAllocationMap = value; }
        }
        private Boolean _ReflushAllocationMap = true;

        #endregion

        #region CheckChildDirectoryEntriesOnDeleteRevision

        /// <summary>
        /// If set to False - the parent directory will NOT be cloned. All changes (adding streams) will be done on the same revision
        /// </summary>
        public Boolean CheckChildDirectoryEntriesOnDeleteRevision
        {
            get { return _CheckChildDirectoryEntriesOnDeleteRevision; }
            set { _CheckChildDirectoryEntriesOnDeleteRevision = value; }
        }
        private Boolean _CheckChildDirectoryEntriesOnDeleteRevision = true;

        #endregion

    }

}
