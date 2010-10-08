/*
 * sones GraphDB - Attributes for reflection
 * (c) Achim 'ahzf' Friedland, 2009 - 2010
 */

#region Usings

using System;

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

    #region NoAutoCreation

    /// <summary>
    /// Attributes may not be created automatically, but it
    /// is assumed that these attributes are valid attributes
    /// after manual creation.
    /// </summary>
    public class NoAutoCreation : Attribute
    {
    }

    #endregion

}
