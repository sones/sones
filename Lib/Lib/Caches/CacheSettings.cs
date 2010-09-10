/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
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
* 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.Serializer;
using System.Runtime.Serialization;
using sones.Lib.NewFastSerializer;

namespace sones.Libraries.Caches
{

    public class CacheSettings : IFastSerialize
    {

        /// <summary>
        /// Renew the CacheExpire DateTime about this TimeSpan when the entry is accessed or stored.
        /// Standard: 1min
        /// </summary>
        public TimeSpan SlidingExpirationTimeSpan
        {
            get { return _SlidingExpirationTimeSpan; }
            set
            {
                if (value.Ticks <= 0)
                    throw new Exception("SlidingExpirationTimeSpan <= 0 is not allowed!");
                _SlidingExpirationTimeSpan = value;
            }
        }
        private TimeSpan _SlidingExpirationTimeSpan = TimeSpan.FromMinutes(1);

        /// <summary>
        /// When a new entry is added to the cache this entry will expire about this Timespan later.
        /// </summary>
        public TimeSpan AbsoluteExpirationTimeSpan { get; set; }

        /// <summary>
        /// Choose an ExpirationType (Sliding or Absolut)
        /// Standard: Sliding
        /// </summary>
        public ExpirationTypes ExpirationType { get { return _ExpirationType; } set { _ExpirationType = value; } }
        private ExpirationTypes _ExpirationType = ExpirationTypes.Sliding;

        /// <summary>
        /// Force cleanup the cache from expired objects if the containing objects reaches X percent of MaxNumberOfCachedPandoraObjects or MaxNumberOfCachedObjectLocators - default: 0.8
        /// </summary>
        public Double ForceCleanPercentage
        {
            get { return _ForceCleanPercentage; }
            set { _ForceCleanPercentage = value; }
        }
        private Double _ForceCleanPercentage = 0.8; //0.8;

        /// <summary>
        /// Renew a SlidingExpiration item only if the new expiration time would be higher than the current plus the tolerance.
        /// Standard: 0.01 == 1%
        /// (100% and more means, the item will be renewed only if it is already expired!)
        /// </summary>
        public Double SlidingExpirationTolerance
        {
            get { return _SlidingExpirationTolerance; }
            set { _SlidingExpirationTolerance = value; }
        }
        private Double _SlidingExpirationTolerance = 0.01; //0.01;
        

        /// <summary>
        /// Maximum Number of cached ObjectLocators (_ObjectLocatorCache) - default: 10000000
        /// </summary>
        public UInt32 MaxNumberOfCachedItems
        {
            get { return _MaxNumberOfCachedItems; }
            set { _MaxNumberOfCachedItems = value; }
        }
        private UInt32 _MaxNumberOfCachedItems = 1000 * 1000 * 1000;

        /// <summary>
        /// Maximum amount of used memory (bytes) - default: 64GB
        /// This is for the process! Not for single threads!!
        /// </summary>
        public UInt64 MaxAmountOfMemory
        {
            get { return _MaxAmountOfMemory; }
            set { _MaxAmountOfMemory = value; }
        }
        private UInt64 _MaxAmountOfMemory = 1024L * 1024L * 1024L * 64UL; // B * KB * MB * GB

        /// <summary>
        /// Delay before the first invocation of clean cache callback - default: 30sec
        /// </summary>
        public TimeSpan TimerDueTime
        {
            get { return _TimerDueTime; }
            set { _TimerDueTime = value; }
        }
        private TimeSpan _TimerDueTime = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Time interval between invocations of clean cache callback - default: 1min
        /// </summary>
        public TimeSpan TimerPeriod
        {
            get { return _TimerPeriod; }
            set { _TimerPeriod = value; }
        }
        private TimeSpan _TimerPeriod = TimeSpan.FromMinutes(10);

        private Boolean _AllowNotExistingDependOnMeItems;
        /// <summary>
        /// Do not throw an exception if the depend-on item does not exist
        /// </summary>
        public Boolean AllowNotExistingDependOnMeItems
        {
            get { return _AllowNotExistingDependOnMeItems; }
            set { _AllowNotExistingDependOnMeItems = value; }
        }

        /// <summary>
        /// Set the depth of the depending item checks.
        /// 0: there are no depending items
        /// 1: only the direct depending items will be checked
        /// </summary>
        private UInt32 _DependingItemsDepth = 1;
        public UInt32 DependingItemsDepth
        {
            get { return _DependingItemsDepth; }
            set { _DependingItemsDepth = value; }
        }


        public CacheSettings(Byte[] mySerializedData)
        {
            Deserialize(mySerializedData);
        }

        public CacheSettings()
        {
            AbsoluteExpirationTimeSpan = TimeSpan.FromMinutes(30);
            SlidingExpirationTimeSpan = TimeSpan.FromMinutes(15);
            ExpirationType = ExpirationTypes.Sliding;
        }

        public override string ToString()
        {
            return ToString("");
        }

        public string ToString(String mySeperator)
        {
            StringBuilder retVal = new StringBuilder();

            retVal.AppendLine("AllowNotExistingDependOnMeItems: ".PadRight(33) + mySeperator + _AllowNotExistingDependOnMeItems);
            retVal.AppendLine("DependingItemsDepth: ".PadRight(33) + mySeperator + _DependingItemsDepth);
            retVal.AppendLine("ForceCleanPercentage: ".PadRight(33) + mySeperator + _ForceCleanPercentage);
            retVal.AppendLine("MaxAmountOfMemory: ".PadRight(33) + mySeperator + _MaxAmountOfMemory);
            retVal.AppendLine("SlidingExpirationTimeSpan: ".PadRight(33) + mySeperator + _SlidingExpirationTimeSpan);
            retVal.AppendLine("SlidingExpirationTolerance: ".PadRight(33) + mySeperator + _SlidingExpirationTolerance);
            retVal.AppendLine("TimerDueTime: ".PadRight(33) + mySeperator + _TimerDueTime);
            retVal.AppendLine("TimerPeriod: ".PadRight(33) + mySeperator + _TimerPeriod);

            return retVal.ToString();
        }

        public CacheSettings Clone()
        {
            return Clone<CacheSettings>();
        }

        protected T Clone<T>()
            where T : CacheSettings, new()

        {
            T retVal = new T();
            retVal.AbsoluteExpirationTimeSpan = AbsoluteExpirationTimeSpan;
            retVal.SlidingExpirationTimeSpan = SlidingExpirationTimeSpan;
            retVal.ExpirationType = ExpirationType;
            retVal._ForceCleanPercentage = ForceCleanPercentage;
            retVal._MaxAmountOfMemory = MaxAmountOfMemory;
            retVal._MaxNumberOfCachedItems = MaxNumberOfCachedItems;
            retVal._TimerDueTime = TimerDueTime;
            retVal._TimerPeriod = TimerPeriod;
            retVal._AllowNotExistingDependOnMeItems = AllowNotExistingDependOnMeItems;
            retVal._DependingItemsDepth = DependingItemsDepth;

            return retVal;
        }

        #region IFastSerialize Members

        private Byte[] _SerializedObject;

        public bool isDirty
        {
            get;
            set;
        }

        public DateTime ModificationTime
        {
            get { throw new NotImplementedException(); }
        }

        public byte[] Serialize()
        {
            try
            {

                #region Init SerializationWriter

                SerializationWriter writer = new SerializationWriter();

                #endregion

                #region Write

                writer.WriteObject(AbsoluteExpirationTimeSpan);
                writer.WriteObject(AllowNotExistingDependOnMeItems);
                writer.WriteObject(ExpirationType);
                writer.WriteObject(ForceCleanPercentage);
                writer.WriteObject(MaxAmountOfMemory);
                writer.WriteObject(MaxNumberOfCachedItems);
                writer.WriteObject(SlidingExpirationTimeSpan);
                writer.WriteObject(TimerDueTime);
                writer.WriteObject(TimerPeriod);

                #endregion

                _SerializedObject = writer.ToArray();

            }

            catch (SerializationException e)
            {
                throw e;
            }

            return _SerializedObject;

        }

        public void Deserialize(byte[] mySerializedData)
        {
            try
            {

                #region Init reader

                SerializationReader reader = new SerializationReader(mySerializedData);

                #endregion

                #region Read

                AbsoluteExpirationTimeSpan = (TimeSpan)reader.ReadObject();
                AllowNotExistingDependOnMeItems = (Boolean)reader.ReadObject();
                ExpirationType = (ExpirationTypes)reader.ReadObject();
                ForceCleanPercentage = (Double)reader.ReadObject();
                MaxAmountOfMemory = (UInt64)reader.ReadObject();
                MaxNumberOfCachedItems = (UInt32)reader.ReadObject();
                SlidingExpirationTimeSpan = (TimeSpan)reader.ReadObject();
                TimerDueTime = (TimeSpan)reader.ReadObject();
                TimerPeriod = (TimeSpan)reader.ReadObject();

                #endregion

            }

            catch (Exception e)
            {
                throw new Exception("The CacheSettings could not be deserialized!\n\n" + e);
            }

            _SerializedObject = mySerializedData;

        }

        #endregion



        #region IFastSerialize Members


        public void Serialize(ref SerializationWriter mySerializationWriter)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(ref SerializationReader mySerializationReader)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
