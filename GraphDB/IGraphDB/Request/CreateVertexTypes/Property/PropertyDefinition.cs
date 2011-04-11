using System;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// The definition for vertex properties
    /// </summary>
    public sealed class PropertyDefinition
    {
        #region Data

        /// <summary>
        /// Should there be an index on the property?
        /// </summary>
        public readonly Boolean IsIndexed;

        /// <summary>
        /// Should this property be mandatory?
        /// </summary>
        public readonly Boolean IsMandatory;

        /// <summary>
        /// Should this property be unique?
        /// </summary>
        public readonly Boolean IsUnique;

        /// <summary>
        /// The name of the property
        /// </summary>
        public readonly String PropertyName;

        /// <summary>
        /// The name of the type (AssemblyQualifiedName)
        /// </summary>
        public readonly String TypeName;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new PropertyDefinition
        /// </summary>
        /// <param name="myPropertyName">The name of the property</param>
        /// <param name="myPropertyType">The type of the property</param>
        /// <param name="myIsIndexed">Should there be an index on the property?</param>
        /// <param name="myIsUnique">Should this property be unique?</param>
        /// <param name="myIsMandatory">Should this property be mandatory?</param>
        public PropertyDefinition(String myPropertyName, Type myPropertyType, Boolean myIsIndexed = false,
                                  Boolean myIsUnique = false, Boolean myIsMandatory = false)
        {
            TypeName = myPropertyType.AssemblyQualifiedName;
            PropertyName = myPropertyName;
            IsIndexed = myIsIndexed;
            IsUnique = myIsUnique;
            IsMandatory = myIsMandatory;
        }

        #endregion
    }
}