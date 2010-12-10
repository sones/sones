using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Functions;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.Lib.ErrorHandling;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.TypeManagement;

namespace sones.GraphDB.Functions
{
    public class ResolveEdgeFunction : ABaseFunction
    {

        public ResolveEdgeFunction()
        {
            this.Parameters.Add(new ParameterValue("ResolvingEdge", new DBTypeAttribute(), false));
        }


        public override string FunctionName
        {
            get { return "RESOLVE"; }
        }

        public override string GetDescribeOutput()
        {
            return "resolves the edge using the edge defined by the 'ResolvingEdge' on the target vertex type. All properties will be resolved.";
        }

        public override bool ValidateWorkingBase(IObject myWorkingBase, sones.GraphDB.TypeManagement.DBTypeManager myTypeManager)
        {
            return (myWorkingBase is DBTypeAttribute);
        }

        //public override IObject GetReturnType(IObject myWorkingBase, DBTypeManager myTypeManager)
        //{
        //    return base.GetReturnType(myWorkingBase, myTypeManager);
        //}
        
        /// <summary>
        /// Calls a webservice to get the current currency conversion ratio of the two given currencies and returns the calling value times the ratio.
        /// This method contains the entire logic of the function.
        /// </summary>
        /// <param name="dbContext">The current DBContext.</param>
        /// <param name="myParams">The parameters for the function. Must be two strings, that contains the currency codes. <see cref="CurrencyConverterFunction"/></param>
        /// <returns>The currency converted value</returns>
        public override Exceptional<FuncParameter> ExecFunc(DBContext dbContext, params FuncParameter[] myParams)
        {

            var resolvingEdge = (myParams[0].Value as DBTypeAttribute).GetValue();
            var source = (CallingObject as IReferenceEdge);

            var returningEdge = new EdgeTypeSetOfReferences();

            foreach(var dbStream in source.GetAllEdgeDestinations(dbContext.DBObjectCache))
            {

                if (dbStream.Failed())
                {
                    return new Exceptional<FuncParameter>(dbStream);
                }

                var hyperEdge = dbStream.Value.GetAttribute(resolvingEdge, resolvingEdge.GetDBType(dbContext.DBTypeManager), dbContext);
                if (hyperEdge.Value == null)
                {
                    return new Exceptional<FuncParameter>(new FuncParameter(null));
                }
                if (hyperEdge.Failed())
                {
                    return new Exceptional<FuncParameter>(hyperEdge);
                }

                returningEdge.AddRange((hyperEdge.Value as IReferenceEdge).GetAllReferenceIDs(), resolvingEdge.DBTypeUUID);

            }

            return new Exceptional<FuncParameter>(new FuncParameter(returningEdge, resolvingEdge));

        }

    }
}
