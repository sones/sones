using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Indices;
using sones.Lib;

namespace sones.GraphDB.Errors
{
    public class Error_IndexAlreadyExistWithSameEditionAndAttribute : GraphDBIndexError
    {
        public String ExistingIndexName { get; private set; }
        public String IndexEdition { get; private set; }
        public IndexKeyDefinition IndexKeyDefinition { get; private set; }
        private String _IndexAttributes;

        public Error_IndexAlreadyExistWithSameEditionAndAttribute(DBContext myDBContext, String myExistingIndexName , IndexKeyDefinition myIndexKeyDefinition, String myIndexEdition)
        {
            IndexEdition = myIndexEdition;
            IndexKeyDefinition = myIndexKeyDefinition;
            ExistingIndexName = myExistingIndexName;

            try
            {
                if (myDBContext != null)
                {
                    _IndexAttributes = IndexKeyDefinition.IndexKeyAttributeUUIDs.ToAggregatedString(a => a.ToString());
                }
                else
                {
                    _IndexAttributes = IndexKeyDefinition.IndexKeyAttributeUUIDs.ToAggregatedString(a => a.ToString());
                }
            }
            catch { }

        }

        public override string ToString()
        {
            return String.Format("There is already an index \"{0}\" with the same edition \"{1}\" and attribute(s) \"{2}\"!", ExistingIndexName, IndexEdition, _IndexAttributes);
        }
    }
}
