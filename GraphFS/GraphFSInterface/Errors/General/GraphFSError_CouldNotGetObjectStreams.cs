/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
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
*/


/*
 * GraphFSError_CouldNotGetObjectStreams
 * Achim Friedland, 2010
 */

#region Usings

using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using sones.Lib.ErrorHandling;
using sones.Lib.DataStructures;
using sones.GraphFS.DataStructures;

#endregion

namespace sones.GraphFS.Errors
{

    /// <summary>
    /// Could not get object streams!
    /// </summary>
    public class GraphFSError_CouldNotGetObjectStreams : GraphFSError
    {

        #region Properties

        public ObjectLocation ObjectLocation    { get; private set; }

        #endregion

        #region Constructor

        #region GraphFSError_CouldNotGetObjectStreams(myObjectLocation)

        public GraphFSError_CouldNotGetObjectStreams(ObjectLocation myObjectLocation)
        {
            ObjectLocation  = myObjectLocation;
            Message         = String.Format("Could not get object streams at location '{0}'!", ObjectLocation);
        }

        #endregion

        #endregion

    }

}
