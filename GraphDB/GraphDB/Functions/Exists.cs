#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Managers.Structures;
using sones.Lib.ErrorHandling;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphDB.TypeManagement;

#endregion

namespace sones.GraphDB.Functions
{
    public class Exists : ABaseFunction
    {

        #region constructors

        public Exists()
        { }

        #endregion


        #region ABaseFunction

        public override string FunctionName
        {
            get { return "EXISTS"; }
        }

        public override string GetDescribeOutput()
        {
            return "Return true if an DBObject contains this attribute.";
        }

        public override bool ValidateWorkingBase(TypeAttribute workingBase, DBTypeManager typeManager)
        {
            if (workingBase != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override Exceptional<FuncParameter> ExecFunc(DBContext dbContext, params FuncParameter[] myParams)
        {
            if(CallingObject != null)
                return new Exceptional<FuncParameter>(new FuncParameter(new DBBoolean(true)));
            else
                return new Exceptional<FuncParameter>(new FuncParameter(new DBBoolean(false)));

        }

        #endregion
    }
}
