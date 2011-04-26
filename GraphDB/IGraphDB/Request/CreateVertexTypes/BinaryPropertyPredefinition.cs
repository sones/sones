using System;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// The definition for binary properties.
    /// </summary>
    public sealed class BinaryPropertyPredefinition
    {
        #region Data

        /// <summary>
        /// The name of the property
        /// </summary>
        public readonly String PropertyName;


        /// <summary>
        /// The comment for this binary property.
        /// </summary>
        public string Comment { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new PropertyPredefinition
        /// </summary>
        /// <param name="myPropertyName">The name of the property</param>
        /// <param name="myComment">The comment for this binary property.</param>
        public BinaryPropertyPredefinition(String myPropertyName, String myComment)
        {
            PropertyName = myPropertyName;
            Comment = myComment;
        }

        #endregion


    }
}