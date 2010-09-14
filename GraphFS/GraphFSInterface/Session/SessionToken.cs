/*
 * GraphFS - SessionToken
 * (c) Achim Friedland, 2009 - 2010
 */

#region Usings

#endregion

namespace sones.GraphFS.Session
{

    public class SessionToken
    {

        #region Properties

        public ISessionInfo    SessionInfo     { get; set; }
        public SessionSettings SessionSettings { get; set; }
        //public ATransaction    Transaction     { get; set; }

        #endregion

        #region Constructor(s)

        #region SessionToken(mySessionInfo)

        public SessionToken(ISessionInfo mySessionInfo)
        {
            SessionInfo     = mySessionInfo;
            SessionSettings = new SessionSettings();
          //  Transaction     = null;
        }

        #endregion

        #endregion

    }

}
