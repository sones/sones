
#region Usings

using sones.GraphDB.Managers.Structures;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphDB.Functions
{

    /// <summary>
    /// Return true if an DBObject contains this attribute.
    /// </summary>
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

        public override bool ValidateWorkingBase(IObject workingBase, DBTypeManager typeManager)
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
            if (CallingObject != null)
                return new Exceptional<FuncParameter>(new FuncParameter(new DBBoolean(true)));
            else
                return new Exceptional<FuncParameter>(new FuncParameter(new DBBoolean(false)));
        }

        #endregion

    }

}
