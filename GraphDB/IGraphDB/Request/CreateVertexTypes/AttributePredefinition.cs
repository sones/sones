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
        public String AttributeType { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public String AttributeName { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public String Comment { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="myAttributeName"></param>
        protected AttributePredefinition(String myAttributeName)
        {
            AttributeName = myAttributeName;
        }
    }
}
