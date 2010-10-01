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
using System.Text;
using System.Linq;
using System.Collections.Generic;
using sones.Lib.Caches;


namespace sones.GraphFS.Caches
{

    public class ObjectCacheSettings : CacheSettings
    {


        #region FlushtimerPeriod

        private TimeSpan _FlushtimerPeriod = TimeSpan.FromDays(10);

        /// <summary>
        /// The period for the cache autoflush. Default is 30sec.
        /// </summary>
        public TimeSpan FlushtimerPeriod
        {

            get
            {
                return _FlushtimerPeriod;
            }

            set
            {
                _FlushtimerPeriod = value;
            }

        }

        #endregion

        #region MaxWriteDepth

        private Int32 _MaxWriteDepth = 2;

        /// <summary>
        /// The maximum write depth of depending items. Default is 0.
        /// </summary>
        public Int32 MaxWriteDepth
        {

            get
            {
                return _MaxWriteDepth;
            }

            set
            {
                _MaxWriteDepth = value;
            }

        }

        #endregion


        #region Constructors

        public ObjectCacheSettings()
            : base()
        { }

        public ObjectCacheSettings(Byte[] mySerializedData)
            : base(mySerializedData)
        { }

        #endregion


        #region Clone()

        public new ObjectCacheSettings Clone()
        {

            var _ObjectCacheSettings = base.Clone<ObjectCacheSettings>();
            _ObjectCacheSettings.FlushtimerPeriod   = FlushtimerPeriod;
            _ObjectCacheSettings.MaxWriteDepth      = MaxWriteDepth;

            return _ObjectCacheSettings;

        }

        #endregion


        #region ToString()

        public override String ToString()
        {
            return ToString("");
        }

        #endregion

        #region ToString(mySeperator);

        public new String ToString(String mySeperator)
        {

            var _StringBuilder = new StringBuilder(base.ToString(mySeperator));

            _StringBuilder.AppendLine("FlushtimerPeriod: ".PadRight(33) + mySeperator + _FlushtimerPeriod);
            _StringBuilder.AppendLine("MaxWriteDepth: ".PadRight(33) + mySeperator + _MaxWriteDepth);

            return _StringBuilder.ToString();

        }

        #endregion

    }
}
