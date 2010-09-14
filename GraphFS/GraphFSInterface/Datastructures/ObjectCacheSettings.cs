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
