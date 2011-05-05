using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Expression;

namespace sones.GraphDB.Request
{
    public sealed class RequestDelete : IRequest
    {
        #region Data

        public readonly RequestGetVertices ToBeDeletedVertices;
        public readonly HashSet<String> ToBeDeletedAttributes;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new delete request that is able to delete attributes from certain vertices or 
        /// the vertices themself
        /// </summary>
        /// <param name="myToBeDeletedVertices">The vertices that should be deleted/changed</param>
        public RequestDelete(RequestGetVertices myToBeDeletedVertices)
        {
            ToBeDeletedAttributes = new HashSet<string>();
            ToBeDeletedVertices = myToBeDeletedVertices;
        }

        #endregion

        #region IRequest Members

        public GraphDBAccessMode AccessMode
        {
            get { return GraphDBAccessMode.TypeChange; }
        }

        #endregion

        #region fluent methods

        /// <summary>
        /// Adds an attribute that should be deleted
        /// </summary>
        /// <param name="myAttributeName">The name of the attribute that should be deleted</param>
        /// <returns>The request itself</returns>
        public RequestDelete AddAttribute(String myAttributeName)
        {
            if (!String.IsNullOrWhiteSpace(myAttributeName))
            {
                ToBeDeletedAttributes.Add(myAttributeName);                
            }

            return this;
        }

        /// <summary>
        /// Adds attributes that should be deleted
        /// </summary>
        /// <param name="myAttributeNames">The names of the attributes that should be deleted</param>
        /// <returns>The request itself</returns>
        public RequestDelete AddAttributes(IEnumerable<String> myAttributeNames)
        {
            foreach (var aToBeDeletedAttribute in myAttributeNames)
            {
                this.AddAttribute(aToBeDeletedAttribute);
            }

            return this;
        }

        #endregion
    }
}
