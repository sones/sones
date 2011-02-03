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
        /// The hashcode of the property type
        /// </summary>
        readonly private int _typeHashCode;

        /// <summary>
        /// The name of the type (AssemblyQualifiedName)
        /// </summary>
        readonly public String TypeName;

        /// <summary>
        /// The name of the property
        /// </summary>
        readonly public String PropertyName;

        /// <summary>
        /// Should there be an index on the property?
        /// </summary>
        readonly public Boolean IsIndexed;

        /// <summary>
        /// Should this property be unique?
        /// </summary>
        readonly public Boolean IsUnique;

        /// <summary>
        /// Should this property be mandatory?
        /// </summary>
        readonly public Boolean IsMandatory;

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
        public PropertyDefinition(String myPropertyName, Type myPropertyType, Boolean myIsIndexed = false, Boolean myIsUnique = false, Boolean myIsMandatory = false)
        {
            _typeHashCode = myPropertyType.GetHashCode();
            TypeName = myPropertyType.AssemblyQualifiedName;
            PropertyName = myPropertyName;
            IsIndexed = myIsIndexed;
            IsUnique = myIsUnique;
            IsMandatory = myIsMandatory;
        }

        #endregion
    }
}
