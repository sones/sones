using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.ObjectManagement;

namespace sones.GraphDB.Errors
{
    public class Error_InvalidIndexOperation : GraphDBIndexError
    {
        public String IndexName { get; private set; }
        public Object IndexKey { get; private set; }
        public Object Operand { get; private set; }
        public String OperationName { get; private set; }

        public Error_InvalidIndexOperation(String myIndexName, String myOperationName)
        {
            IndexName = myIndexName;
            OperationName = myOperationName;
            IndexKey = null;
            Operand = null;
        }

        public Error_InvalidIndexOperation(String myIndexName)
        {
            IndexName = myIndexName;
            OperationName = null;
            IndexKey = null;
            Operand = null;
        }

        public Error_InvalidIndexOperation(String myIndexName, Object myIndexKey, Object myOperand)
        {
            IndexName = myIndexName;
            IndexKey = myIndexKey;
            OperationName = null;
            Operand = myOperand;
        }

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
