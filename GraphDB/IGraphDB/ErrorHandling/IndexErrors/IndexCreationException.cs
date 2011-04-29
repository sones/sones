using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Request.CreateVertexTypes;

namespace sones.GraphDB.ErrorHandling
{
    public sealed class IndexCreationException : AGraphDBIndexException
    {
        public IndexPredefinition IndexPredef { get; private set; }
        public String Info { get; private set; }

        /// <summary>
        /// Creates a new InvalidIndexAttributeException exception
        /// </summary>
        /// <param name="myInvalidIndexAttribute">The name of the invalid vertex type</param>
        /// <param name="myInfo"></param>
        public IndexCreationException(IndexPredefinition myIndexPredef, String myInfo)
            : base()
        {
            Info = myInfo;
            IndexPredef = myIndexPredef;

            _msg = String.Format("Could Not Create Index {0}  type {1} on type {2}.\n\n{3}.", IndexPredef.Name, IndexPredef.TypeName, IndexPredef.VertexTypeName, Info);
        }
    }
}
