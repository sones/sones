using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.ErrorHandling.IndexErrors
{
    public sealed class RebuildIndicesFaildException : AGraphDBIndexException
    {
        public String Info { get; private set; }
        public IEnumerable<String> TypeNames { get; private set; }

        /// <summary>
        /// Creates a new IndexTypeDoesNotExistException exception
        /// </summary>
        /// <param name="myIndexTypeName"></param>
        public RebuildIndicesFaildException(IEnumerable<String> myTypes, String myInfo)
        {
            Info = myInfo;
            TypeNames = myTypes;

            StringBuilder temp = new StringBuilder();

            temp.AppendLine(String.Format("Failed to rebuild following indices:"));

            foreach (var name in TypeNames)
            {
                temp.AppendLine(String.Format("Rebuild index \"{0}\" failed!", name));
            }

            temp.AppendLine(Info);

            _msg = temp.ToString();
        }
    }
}
