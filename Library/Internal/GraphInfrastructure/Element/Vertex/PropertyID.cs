using System;

namespace sones.GraphInfrastructure.Element
{
    /// <summary>
    /// The definition for vertex properties
    /// </summary>
    public sealed class PropertyID
    {
        #region Data

        /// <summary>
        /// The name of the type (AssemblyQualifiedName)
        /// </summary>
        readonly public String TypeName;

        /// <summary>
        /// The name of the property
        /// </summary>
        readonly public String PropertyName;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new PropertyID
        /// </summary>
        /// <param name="myPropertyName">The name of the property</param>
        /// <param name="myPropertyType">The type of the property</param>
        public PropertyID(String myPropertyName, Type myPropertyType)
        {
            TypeName = myPropertyType.AssemblyQualifiedName;
            PropertyName = myPropertyName;
        }

        #endregion

        #region Equals Overrides

        public override Boolean Equals(System.Object obj)
        {
            if (obj == null)
            {
                return false;
            }

            PropertyID v = obj as PropertyID;

            if (v != null)
            {
                return Equals(v);
            }
            else
            {
                return false;
            }
        }

        public Boolean Equals(PropertyID myVertex)
        {
            if ((object)myVertex == null)
            {
                return false;
            }

            return (this.TypeName == myVertex.TypeName) && (this.PropertyName == myVertex.PropertyName);
        }

        public static Boolean operator ==(PropertyID aVertex, PropertyID bVertex)
        {
            if (Object.ReferenceEquals(aVertex, bVertex))
            {
                return true;
            }

            if (((object)aVertex == null) || ((object)bVertex == null))
            {
                return false;
            }

            return aVertex.Equals(bVertex);
        }

        public static Boolean operator !=(PropertyID aVertex, PropertyID bVertex)
        {
            return !(aVertex == bVertex);
        }

        public override int GetHashCode()
        {
            return TypeName.GetHashCode() ^ PropertyName.GetHashCode();
        }

        #endregion
    }
}
