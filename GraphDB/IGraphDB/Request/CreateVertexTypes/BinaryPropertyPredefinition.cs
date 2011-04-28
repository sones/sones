using System;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// The definition for binary properties.
    /// </summary>
    public sealed class BinaryPropertyPredefinition: AttributePredefinition
    {
        #region Constructor

        /// <summary>
        /// Creates a new BinaryPropertyPredefinition
        /// </summary>
        public BinaryPropertyPredefinition(String myPropertyName)
            : base(myPropertyName)
        {
            base.SetAttributeType("Stream");
        }

        /// <remarks>This method will be ignored.</remarks>
        public override AttributePredefinition SetAttributeType(string myAttributeType)
        {
            return this;
        }

        #endregion


    }
}