using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_OperatorDoesNotExist : GraphDBOperatorError
    {
        public String Operator { get; private set; }

        public Error_OperatorDoesNotExist(String myOperator)
        {
            Operator = myOperator;
        }

        public override string ToString()
        {
            return String.Format("The operator {0} does not exist.", Operator);
        }
    }
}
