using System;
using sones.GraphDB.Indices;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.BasicTypes;


namespace sones.GraphDB.Errors
{
    public class Error_IndexKeyCreationError : GraphDBIndexError
    {
        public AttributeUUID AttributeUUID { get; private set; }
        public ADBBaseObject IndexKeyPayload { get; private set; }
        public IndexKeyDefinition IndexDefinition { get; private set; }

        public Error_IndexKeyCreationError(AttributeUUID myAttributeUUID, ADBBaseObject myIndexKeyPayload, IndexKeyDefinition myIndexDefinition)
        {
            AttributeUUID = myAttributeUUID;
            IndexKeyPayload = myIndexKeyPayload;
            IndexDefinition = myIndexDefinition;
        }

        public override string ToString()
        {
            return String.Format("Error while creating an IndexKey.");
        }
    }
}
