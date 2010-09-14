/*
 * DBSessionInfo
 * (c) sones GmbH, 2009 - 2010
 */

#region Usings

using System;
using sones.GraphFS.Session;

#endregion

namespace sones.GraphDB.Session
{

    public class DBSessionInfo : ISessionInfo
    {

        #region Properties

        #region SessionUUID

        /// <summary>
        /// The current SessionUUID
        /// </summary>
        public SessionUUID SessionUUID  { get; private set; }

        #endregion

        #region Username

        /// <summary>
        /// The current user
        /// </summary>
        public String Username          { get; private set; }

        #endregion

        #region ThrowExceptions

        /// <summary>
        /// Methods must or must not throw exceptions
        /// </summary>
        public Boolean ThrowExceptions { get; set; }

        #endregion        

        #endregion

        #region Constructors

        public DBSessionInfo(String myUsername)
        {
            Username        = myUsername;
            SessionUUID     = SessionUUID.NewUUID;
            ThrowExceptions = true;
        }

        #endregion

    }

}
