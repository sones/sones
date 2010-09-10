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
