using System;
using System.Collections.Generic;
using System.Text;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// Droping a vertex attribute is not allowed, because of remaining references from other attributes
    /// </summary>
    public sealed class DropOfVertexAttributeNotAllowedException : AGraphDBVertexAttributeException
    {
        #region data

        public String VertexAttributeName { get; private set; }
        public String VertexTypeName { get; private set; }
        public Dictionary<String, String> ConflictingAttributes { get; private set; }

        #endregion

        #region constructor        

        /// <summary>
        /// Creates a new DropOfVertexAttributeNotAllowedException exception
        /// </summary>
        /// <param name="myVertexTypeName">The name of the vertex type</param>
        /// <param name="myVertexAttributeName">The name of the vertex attribute </param>
        /// <param name="myConflictingAttributes">A dictionary of the conflicting attributes (TypeAttributeName, GraphDBType)   </param>
        public DropOfVertexAttributeNotAllowedException(String myVertexTypeName, String myVertexAttributeName, Dictionary<String, String> myConflictingAttributes)
        {
            ConflictingAttributes = myConflictingAttributes;
            VertexTypeName = myVertexTypeName;
            VertexAttributeName = myVertexAttributeName;
        }

        #endregion

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var aConflictingAttribute in ConflictingAttributes)
            {
                sb.Append(String.Format("{0} ({1}),", aConflictingAttribute.Key, aConflictingAttribute.Value));
            }

            sb.Remove(sb.Length - 1, 1);

            return String.Format("It is not possible to drop {0} of vertex type {1} because there are remaining references from the following attributes: {2}" + Environment.NewLine + "Please remove them in previous.", VertexAttributeName, VertexTypeName, sb);

        }
        
    }
}
