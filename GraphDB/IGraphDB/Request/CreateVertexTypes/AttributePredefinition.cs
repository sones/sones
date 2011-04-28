using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Request
{
    public abstract class AttributePredefinition
    {
        /// <summary>
        /// 
        /// </summary>
        public String AttributeType { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public String AttributeName { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public String Comment { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="myAttributeName"></param>
        protected AttributePredefinition(String myAttributeName)
        {
            AttributeName = myAttributeName;
        }

        public virtual AttributePredefinition SetAttributeType(String myAttributeType)
        {
            AttributeType = myAttributeType;

            return this;
        }

        public AttributePredefinition SetComment(String myComment)
        {
            Comment = myComment;

            return this;
        }

    }
}
