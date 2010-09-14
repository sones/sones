/*
 * IndexDefinition
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace sones.GraphDB.Managers.Structures
{
    public class IndexDefinition
    {

        #region Properties

        public String IndexName
        {
            get;
            private set;
        }

        public String Edition
        {
            get;
            private set;
        }

        public String IndexType
        {
            get;
            private set;
        }

        public List<IndexAttributeDefinition> IndexAttributeDefinitions
        {
            get;
            private set;
        }

        #endregion

        public IndexDefinition(string myIndexName, string myEdition, string myIndexType, List<IndexAttributeDefinition> myIndexAttributeDefinitions)
        {
            IndexName = myIndexName;
            Edition = myEdition;
            IndexType = myIndexType;
            IndexAttributeDefinitions = myIndexAttributeDefinitions;
        }


    }
}
