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
 * ObjectUUIDEqualityComparer
 * Achim Friedland, 2010
 */

#region Usings

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

#endregion

namespace sones.GraphFS.DataStructures
{

    /// <summary>
    /// Helper class for comparing ObjectUUIDs
    /// </summary>
    public class ObjectUUIDEqualityComparer : IEqualityComparer<ObjectUUID>
    {

        public bool Equals(ObjectUUID myObjectUUID1, ObjectUUID myObjectUUID2)
        {

            if (myObjectUUID1.Equals(myObjectUUID2))
                return true;

            else
                return false;

        }

        public int GetHashCode(ObjectUUID obj)
        {
            return base.GetHashCode();
        }

    }

}
