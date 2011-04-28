using System;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// The definition for binary properties.
    /// </summary>
    public sealed class BinaryPropertyPredefinition: AttributePredefinition
    {
        public const string TypeName = "Stream";

        #region Constructor

        /// <summary>
        /// Creates a new BinaryPropertyPredefinition
        /// </summary>
        public BinaryPropertyPredefinition(String myPropertyName)
            : base(myPropertyName)
        {
            AttributeType = TypeName;
        }

        #endregion

        public BinaryPropertyPredefinition SetComment(String myComment)
        {
            Comment = myComment;

            return this;
        }
    }
}