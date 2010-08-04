/*
 * IndexAttributeDefinition
 * (c) Stefan Licht, 2009 - 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace sones.GraphDB.Managers.Structures
{
    public class IndexAttributeDefinition
    {

        #region Properties

        public String IndexAttribute { get; private set; }
        public String OrderDirection { get; private set; }
        public String IndexType { get; private set; }

        #endregion

        #region Ctor

        public IndexAttributeDefinition(string myIndexAttribute, string myIndexType, string myOrderDirection)
        {
            IndexAttribute = myIndexAttribute;
            IndexType = myIndexType;
            OrderDirection = myOrderDirection;
        }

        #endregion

    }
}
