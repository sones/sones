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
 * GraphFSError_NoObjectRevisionsFound
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
    /// No object revisions found!
    /// </summary>
    public class GraphFSError_NoObjectRevisionsFound : GraphFSError
    {

        #region Properties

        public ObjectLocation ObjectLocation { get; private set; }
        public String         ObjectStream   { get; private set; }
        public String         ObjectEdition  { get; private set; }

        #endregion

        #region Constructor

        #region GraphFSError_NoObjectRevisionsFound(myObjectLocation, myObjectStream, myObjectEdition)

        public GraphFSError_NoObjectRevisionsFound(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition)
        {
            ObjectLocation  = myObjectLocation;
            ObjectStream    = myObjectStream;
            ObjectEdition   = myObjectEdition;
            Message         = String.Format("No object revisions found at location '{1}{0}{2}{0}{3}'!", FSPathConstants.PathDelimiter, ObjectLocation, ObjectStream, ObjectEdition);
        }

        #endregion

        #endregion

    }

}
