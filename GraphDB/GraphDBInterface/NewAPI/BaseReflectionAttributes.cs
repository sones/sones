/*
 * sones GraphDS API - ReflectionAttributes
 * (c) Achim 'ahzf' Friedland, 2009 - 2010
 */

#region Usings

using System;
using sones.GraphDB.Structures;

#endregion

namespace sones.GraphDB.NewAPI
{

    #region HideFromDatabase

    /// <summary>
    /// Attributes may be hidden from the database and thus
    /// not be created or synched with the database.
    /// </summary>
    public class HideFromDatabase : Attribute
    {
    }

    #endregion

}
