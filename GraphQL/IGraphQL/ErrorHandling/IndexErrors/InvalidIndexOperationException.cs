using System;
using sones.Library.ErrorHandling;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// The index operation is invalid
    /// </summary>
    public sealed class InvalidIndexOperationException : AGraphQLIndexException
    {
        #region data

        public String IndexName { get; private set; }
        public Object IndexKey { get; private set; }
        public Object Operand { get; private set; }
        public String OperationName { get; private set; }

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new InvalidIndexOperationException exception
        /// </summary>
        /// <param name="myIndexName">The name of the index</param>
        /// <param name="myOperationName">The name of the operation</param>
        public InvalidIndexOperationException(String myIndexName, String myOperationName)
        {
            IndexName = myIndexName;
            OperationName = myOperationName;
            IndexKey = null;
            Operand = null;
        }

        /// <summary>
        /// Creates a new InvalidIndexOperationException exception
        /// </summary>
        /// <param name="myIndexName">The name of the index</param>
        public InvalidIndexOperationException(String myIndexName)
        {
            IndexName = myIndexName;
            OperationName = null;
            IndexKey = null;
            Operand = null;
        }

        /// <summary>
        /// Creates a new InvalidIndexOperationException exception
        /// </summary>
        /// <param name="myIndexName">The name of the index</param>
        /// <param name="myIndexKey">The index key</param>
        /// <param name="myOperand">The current operand</param>
        public InvalidIndexOperationException(String myIndexName, Object myIndexKey, Object myOperand)
        {
            IndexName = myIndexName;
            IndexKey = myIndexKey;
            OperationName = null;
            Operand = myOperand;
        }

        #endregion

        public override string ToString()
        {
            if (IndexKey == null)
            {
                if (OperationName == null)
                {
                    return String.Format("A invalid index operation on \"{0}\" occurred.", IndexName);
                }
                else
                {
                    return String.Format("A invalid index operation ({0}) on \"{1}\" occurred.", OperationName, IndexName);
                }
            }
            else
            {
                return String.Format("A invalid index operation on \"{0}\" occurred (IndexKey: \"{1}\", Operand: \"{2}\").", IndexName, IndexKey, Operand);
            }
        }

    }
}
