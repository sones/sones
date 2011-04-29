using System;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// The definition for vertex properties
    /// </summary>
    public sealed class PropertyPredefinition: AttributePredefinition
    {
        #region Data

        /// <summary>
        /// Should there be an index on the property?
        /// </summary>
        public Boolean IsIndexed { get; private set; }

        /// <summary>
        /// Should this property be mandatory?
        /// </summary>
        public Boolean IsMandatory { get; private set; }

        /// <summary>
        /// Should this property be unique?
        /// </summary>
        public Boolean IsUnique { get; private set; }

        /// <summary>
        /// The default value for this property.
        /// </summary>
        public String DefaultValue { get; private set; }

        /// <summary>
        /// The multiplicity of this property.
        /// </summary>
        public PropertyMultiplicity Multiplicity { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new PropertyPredefinition.
        /// </summary>
        /// <param name="myPropertyName">The name of the property</param>
        public PropertyPredefinition(String myPropertyName)
            : base(myPropertyName)
        {
            IsIndexed = false;
            IsMandatory = false;
            IsUnique = false;
            Multiplicity = PropertyMultiplicity.Single;
        }

        #endregion

        public PropertyPredefinition SetAsIndexed()
        {
            IsIndexed = true;

            return this;
        }

        public PropertyPredefinition SetAsMandatory()
        {
            IsMandatory = true;

            return this;
        }

        public PropertyPredefinition SetAsUnique()
        {
            IsUnique = true;

            return this;
        }

        public PropertyPredefinition SetMultiplicityToList()
        {
            Multiplicity = PropertyMultiplicity.List;

            return this;
        }

        public PropertyPredefinition SetMultiplicityToSet()
        {
            Multiplicity = PropertyMultiplicity.Set;

            return this;
        }


        public PropertyPredefinition SetDefaultValue(string myDefaultValue)
        {
            DefaultValue = myDefaultValue;
            return this;
        }

        public PropertyPredefinition SetComment(String myComment)
        {
            Comment = myComment;

            return this;
        }

        public PropertyPredefinition SetAttributeType(String myTypeName)
        {
            if (myTypeName != null)
                AttributeType = myTypeName;

            return this;
        }


    }
}