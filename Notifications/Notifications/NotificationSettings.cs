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

using sones.Notifications.Messages;

namespace sones.Notifications
{

    public class NotificationSettings
    {

        #region Properties

        #region StartDispatcher

        private Boolean _StartDispatcher = false;

        /// <summary>
        /// Start the default Notification Dispatcher on initialization
        /// Default: true
        /// </summary>
        public Boolean StartDispatcher
        {

            get
            {
                return _StartDispatcher;
            }

            set
            {
                _StartDispatcher = value;
            }

        }

        #endregion

        #region StartBrigde

        private Boolean _StartBrigde = false;

        /// <summary>
        /// Start the NotificationBridge on initialization
        /// Default: false
        /// </summary>
        public Boolean StartBrigde
        {

            get
            {
                return _StartBrigde;
            }

            set
            {
                _StartBrigde = value;
            }

        }

        #endregion

        #region BridgePort

        private Int32 _BridgePort = 5555;

        /// <summary>
        /// Define the NotificationBridge-SenderPort
        /// Default: 5555
        /// </summary>
        public Int32 BridgePort
        {
            
            get
            {
                return _BridgePort;
            }

            set
            {
                _BridgePort = value;
            }

        }

        #endregion

        #region BridgeMulticastTTL

        private Int32 _BridgeMulticastTTL = 2;

        /// <summary>
        /// Define the UDP TimeToLive 
        /// 0 - LAN
        /// 1 - Single Router Hop
        /// 2 - Two Router Hops... 
        /// </summary>
        public Int32 BridgeMulticastTTL
        {

            get
            {
                return _BridgeMulticastTTL;
            }

            set
            {
                _BridgeMulticastTTL = value;
            }

        }

        #endregion

        #endregion

        #region Constructors

        public NotificationSettings()
        {
            foreach (Byte _ByteValue in Enum.GetValues(typeof(NotificationPriority)))
                _NumberOfSentPrioritiesThresholds.Add(_ByteValue, Math.Max((DefaultNumberOfSentPrioritiesThreshold - (Int32)_ByteValue * 2), 1));
        }

        #endregion


        #region NumberOfSentPrioritiesThreshold

        public const Int32 DefaultNumberOfSentPrioritiesThreshold = 10;

        /// <summary>
        /// Holds the threshold for each priority. The threshold of the highest priority should be much higher than the lower priorities
        /// </summary>
        private Dictionary<Byte, Int32> _NumberOfSentPrioritiesThresholds = new Dictionary<Byte, Int32>();

        public void ChangeNumberOfSentPrioritiesThreshold(NotificationPriority myPriority, Int32 myThreshold)
        {
            if (!_NumberOfSentPrioritiesThresholds.ContainsKey((Byte)myPriority))
                throw new ArgumentOutOfRangeException("myPriority");

            _NumberOfSentPrioritiesThresholds[(Byte)myPriority] = myThreshold;
        }

        public Int32 GetNumberOfSentPrioritiesThreshold(NotificationPriority myPriority)
        {
            if (!_NumberOfSentPrioritiesThresholds.ContainsKey((Byte)myPriority))
                return DefaultNumberOfSentPrioritiesThreshold;

            return _NumberOfSentPrioritiesThresholds[(Byte)myPriority];
        }

        #endregion

    }

}
