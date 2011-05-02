using System;

namespace sones.GraphQL.Structure.Helper.Enums
{
    /// <summary>
    /// describe the type of selection * or # or - or @
    /// </summary>
    [Flags]
    public enum TypesOfSelect
    {
        /// <summary>
        /// attribute selection
        /// </summary>
        None = 0,

        /// <summary>
        /// select all attributes and undefined attributes
        /// </summary>
        Asterisk = 1,
    }
}
