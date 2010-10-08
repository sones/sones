/*
 * GraphDBError_CouldNotWriteSettings
 * (c) Achim Friedland, 2010
 */

#region Usings

using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using sones.Lib.ErrorHandling;
using sones.Lib.DataStructures;
using sones.GraphDB.Errors;
using sones.GraphFS.DataStructures;

#endregion

namespace sones.GraphDB.Errors
{

    /// <summary>
    /// Database settings could not been writen on disc!
    /// </summary>
    public class GraphDBError_CouldNotWriteSettings : GraphDBSettingError
    {

        #region Properties

        public ObjectLocation ObjectLocation { get; private set; }

        #endregion

        #region Constructor

        #region GraphDBError_CouldNotWriteSettings(myObjectLocation)

        public GraphDBError_CouldNotWriteSettings(ObjectLocation myObjectLocation)
        {
            ObjectLocation  = myObjectLocation;
        }

        #endregion

        #endregion

        #region ToString()

        public override String ToString()
        {
            return String.Format("Database settings '{0}' could not been writen on disc!", ObjectLocation);
        }

        #endregion

    }

}
