/*
 * GraphDBError_SettingsNotFound
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
    /// Database setting not found!
    /// </summary>
    public class GraphDBError_SettingsNotFound : GraphDBSettingError
    {

        #region Properties

        public ObjectLocation ObjectLocation { get; private set; }

        #endregion

        #region Constructor

        #region GraphDBError_SettingsNotFound(myObjectLocation)

        public GraphDBError_SettingsNotFound(ObjectLocation myObjectLocation)
        {
            ObjectLocation  = myObjectLocation;
        }

        #endregion

        #endregion

        #region ToString()

        public override String ToString()
        {
            return String.Format("Database setting '{0}' not found!", ObjectLocation);
        }

        #endregion

    }

}
