/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
*
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using sones.Lib.DataStructures.Timestamp;
using sones.Lib.DataStructures;


namespace sones.Lib.Caches
{

    public abstract class ACacheEntry
    {



        #region Constructors

        #region StreamCacheEntry(myExpirationType, mySlidingExpirationTimeSpan, myAbsoluteExpirationTimeSpan)

        /// <summary>
        /// The basic constructor of the CachedAGraphObject
        /// </summary>
        public ACacheEntry(ExpirationTypes myExpirationType, TimeSpan mySlidingExpirationTimeSpan, TimeSpan myAbsoluteExpirationTimeSpan)
        {
            
            IsPinned                  = false;
            IsVolatile                = false;

            this.ExpirationType              = myExpirationType;
            this.SlidingExpirationTimeSpan   = mySlidingExpirationTimeSpan;
            this.AbsoluteExpirationTimeSpan  = myAbsoluteExpirationTimeSpan;

            _Created = TimestampNonce.Now;

            _ReadCount  = 0;
            _WriteCount = 0;

            _WriteDependencies = new HashSet<string>();

        }

        #endregion

//        #region StreamCacheEntry(myCacheEntry)

//        public ACacheEntry(StreamCacheEntry myCacheEntry)
//        {

//            ExpirationType              = myCacheEntry.ExpirationType;
//            SlidingExpirationTimeSpan   = myCacheEntry.SlidingExpirationTimeSpan;
//            AbsoluteExpirationTimeSpan  = myCacheEntry.AbsoluteExpirationTimeSpan;
////            ObjectLocation              = myCacheEntry.ObjectLocation;
//            _Created                    = TimestampNonce.Now;

//            _ReadCount                  = 0;
//            _WriteCount                 = 0;

//        }

//        #endregion

        #endregion



        #region Object Characteristics

        #region ObjectLocation

        public String ObjectLocation { get; set; }

        #endregion


        #region IsDirty

        /// <summary>
        /// This boolean signals if the status or content of the associated GraphObject
        /// was changed since it was moved to the cache and therefore needs to be written
        /// to disc again
        /// </summary>
        public abstract Boolean IsDirty
        {
            get;
        }

        #endregion

        #region IsPinned

        /// <summary>
        /// this boolean marks if the object is pinned into the cache or not - that means that the
        /// object, even when it's successfully written or timed off will stay in the cache
        /// </summary>
        public Boolean IsPinned;

        #endregion

        #region IsVolatile

        /// <summary>
        /// this boolean marks that the object is volatile and does not need to be written back to disc.
        /// This marker is commonly used to pull/push temporary objects around
        /// </summary>
        public Boolean IsVolatile;

        #endregion


        #region Created

        protected DateTime _Created;

        public DateTime Created
        {
            get
            {
                return _Created;
            }
        }

        #endregion

        #region ExpiresAt

        public Int64 ExpiresAt
        {
            get;
            set;
        }

        #endregion

        #region ReadCount

        protected Int64 _ReadCount;

        public Int64 ReadCount
        {
            get {
                return _ReadCount;
            }
            //set { _ReadCount = value; }
        }

        #endregion

        #region WriteCount

        protected Int64 _WriteCount;

        public Int64 WriteCount
        {
            get { return _WriteCount; }
            //set { _WriteCount = value; }
        }

        #endregion

        public abstract List<Byte[]> PreallocationTickets
        {
            get;
        }

        #endregion



        #region SlidingExpirationTimeSpan

        protected TimeSpan _SlidingExpirationTimeSpan;

        /// <summary>
        /// Renew the CacheExpire DateTime about this TimeSpan when the entry is accessed or stored
        /// </summary>
        public TimeSpan SlidingExpirationTimeSpan
        {
            get
            {
                return _SlidingExpirationTimeSpan;
            }
            set
            {
                _SlidingExpirationTimeSpan = value;
            }
        }

        #endregion

        #region AbsoluteExpirationTimeSpan

        protected TimeSpan _AbsoluteExpirationTimeSpan;

        /// <summary>
        /// When a new entry is added to the cache this entry will expire about this Timespan later.
        /// </summary>
        public TimeSpan AbsoluteExpirationTimeSpan
        {

            get
            {
                return _AbsoluteExpirationTimeSpan;
            }

            set
            {
                _AbsoluteExpirationTimeSpan = value;
            }

        }

        #endregion

        #region ExpirationType

        protected ExpirationTypes _ExpirationType;

        /// <summary>
        /// Choose an ExpirationType (Sliding or Absolut)
        /// </summary>
        public ExpirationTypes ExpirationType
        {

            get
            {
                return _ExpirationType;
            }
            
            set
            {
                _ExpirationType = value;
            }

        }

        #endregion

        #region Write dependencies

        /// <summary>
        /// This dictionary contains a location as key and a list of locations as value. The key location can be flushed after all value locations are flushed.
        /// </summary>
        public HashSet<String> WriteDependencies
        {
            get { return _WriteDependencies; }
            set { _WriteDependencies = value; }
        }
        private HashSet<String> _WriteDependencies;

        #endregion 


    }

}
