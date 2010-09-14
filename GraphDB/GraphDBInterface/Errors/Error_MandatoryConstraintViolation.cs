using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_MandatoryConstraintViolation : GraphDBError
    {
        public String MandatoryConstraint { get; private set; }

        public Error_MandatoryConstraintViolation(String myMandatoryConstraint)
        {
            MandatoryConstraint = myMandatoryConstraint;
        }

        public override string ToString()
        {
            return String.Format("The mandatory constraint \"{0}\" was violated!", MandatoryConstraint);
        }
    }
}
