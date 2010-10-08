/*
 * GraphFS - FSSessionInfo
 * (c) Achim Friedland, 2009 - 2010
 */

using System;
using System.Text;
using System.Collections.Generic;
using sones.GraphFS.Session;

namespace sones.GraphFS.Session
{
    /// <summary>
    /// Stores file system specific session informations
    /// </summary>
    public class FSSessionInfo : ISessionInfo
    {

        #region SessionUUID

        private SessionUUID _SessionUUID;

        /// <summary>
        /// The current SessionUUID
        /// </summary>
        public SessionUUID SessionUUID
        {
            get
            {
                return _SessionUUID;
            }
        }

        #endregion

        #region Username

        private String _Username;

        /// <summary>
        /// The current user
        /// </summary>
        public String Username
        {
            get
            {
                return _Username;
            }
        }        

        #endregion

        #region ThrowExceptions

        Boolean _ThrowExceptions = true;

        /// <summary>
        /// Methods must or must not throw exceptions
        /// </summary>
        public Boolean ThrowExceptions
        {

            get
            {
                return _ThrowExceptions;
            }

            set
            {
                _ThrowExceptions = value;
            }

        }

        #endregion


        #region FSSettings

        private FSSessionSettings _FSSettings;

        /// <summary>
        /// The current file system specific settings
        /// </summary>
        public FSSessionSettings FSSettings
        {
            get
            {
                return _FSSettings;
            }
        }

        #endregion



        /// <summary>
        /// Create a new instance with the default FSSessionSettings
        /// </summary>
        /// <param name="myUsername"></param>
        public FSSessionInfo(String myUsername)
        {
            _Username       = myUsername;
            _FSSettings     = new FSSessionSettings();
            _SessionUUID    = new SessionUUID();
        }


    }

}
