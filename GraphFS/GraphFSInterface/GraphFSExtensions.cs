/*
 * GraphFSExtensions
 * (c) Achim Friedland, 2008 - 2010
 */

#region Usings

using System;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Session;
using sones.Lib.DataStructures;
using sones.Lib.DataStructures.WeakReference;
using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphFS
{

    public static class GraphFSExtensions
    {

        //HACK: Remove me!
        public static Exceptional RemoveObjectIfExists(this IGraphFSSession myIGraphFSSession, ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition = null, ObjectRevisionID myRevisionID = null)
        {

            var objExists = myIGraphFSSession.ObjectStreamExists(myObjectLocation, myObjectStream);

            if (objExists.Failed())
            {
                return new Exceptional(objExists);
            }

            if (objExists.Value == Trinary.TRUE)
            {
                var _RemoveObjectExceptional = myIGraphFSSession.RemoveFSObject(myObjectLocation, myObjectStream, myObjectEdition, myRevisionID);
                return _RemoveObjectExceptional;
            }

            else
                return Exceptional.OK;

        }


        #region IsPersistent(this myIGraphFS)

        public static Boolean IsPersistent(this WeakReference<IGraphFS> myIGraphFS)
        {

            if (myIGraphFS == null)             return false;
            if (!myIGraphFS.IsAlive)            return false;
            if (!myIGraphFS.Value.IsPersistent) return false;
            if (!myIGraphFS.Value.IsMounted)    return false;

            return true;

        }

        #endregion

        #region IsPersistent(this myIGraphFSSession)

        public static Boolean IsPersistent(this WeakReference<IGraphFSSession> myIGraphFSSession)
        {

            if (myIGraphFSSession == null)              return false;
            if (!myIGraphFSSession.IsAlive)             return false;
            if (!myIGraphFSSession.Value.IsPersistent)  return false;
            if (!myIGraphFSSession.Value.IsMounted)     return false;

            return true;

        }

        #endregion


        #region ToShortString(this myObjectRevisionID)

        /// <summary>
        /// Returns a formated short string representation of this revision
        /// </summary>
        /// <returns>A formated short string representation of this revision</returns>
        public static String ToShortString(this ObjectRevisionID myObjectRevisionID)
        {
            
            if (myObjectRevisionID != null)
                return String.Format("{0:yyyyddMM.HHmmss.fffffff}", new DateTime((Int64) myObjectRevisionID.Timestamp));

            return "<null>";

        }

        #endregion

    }

}
