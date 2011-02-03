using System;

namespace sones.PropertyHyperGraph
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
        /// The id of the property
        /// </summary>
        readonly public UInt64 ID;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new PropertyID
        /// </summary>
        /// <param name="myID">The ID of the property</param>
        /// <param name="myPropertyType">The type of the property</param>
        public PropertyID(UInt64 myID, Type myPropertyType)
        {
            TypeName = myPropertyType.AssemblyQualifiedName;
            ID = myID;
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

            return (this.TypeName == myVertex.TypeName) && (this.ID == myVertex.ID);
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
            return TypeName.GetHashCode() ^ ID.GetHashCode();
        }

        #endregion
    }
}
