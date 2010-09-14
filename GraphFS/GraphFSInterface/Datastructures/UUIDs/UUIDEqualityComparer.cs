/*
 * ObjectUUIDEqualityComparer
 * (c) Achim Friedland, 2010
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
