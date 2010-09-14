/*
 * IObjectLocation
 * (c) Achim Friedland, 2008 - 2010
 */

#region Usings

using System;
using sones.GraphFS.DataStructures;

#endregion

namespace sones.GraphFS.Objects
{

    /// <summary>
    /// The interface for ObjectLocations
    /// </summary>

    public interface IObjectLocation
    {
        ObjectLocation  ObjectLocation  { get; }
        ObjectLocation  ObjectPath      { get; }
        String          ObjectName      { get; }
    }

}
