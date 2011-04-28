using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISonesGQLFunction.Structure;
using sones.GraphDB.TypeSystem;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.PropertyHyperGraph;
using sones.GraphDB.ErrorHandling.Type;
using sones.Library.VersionedPluginManager;

namespace sones.Plugins.SonesGQL.Functions
{
    public sealed class TopFunc : ABaseFunction
    {
        #region constructor

        public TopFunc()
        {
            Parameters.Add(new ParameterValue("NumOfEntries", new UInt64()));
        }

        #endregion

        public override string GetDescribeOutput()
        {
            return "Will return the top elements of an edge.";
        }

        public override bool ValidateWorkingBase(Type myWorkingBase, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            if ((myWorkingBase is IAttributeDefinition) && (myWorkingBase as IAttributeDefinition).Kind == AttributeType.OutgoingEdge)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override FuncParameter ExecFunc(IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken, params FuncParameter[] myParams)
        {
            long propertyID = 0;

            if (CallingObject is IHyperEdge)
            {
                var hyperEdge = CallingObject as IHyperEdge;

                var topVertices = hyperEdge.InvokeHyperEdgeFunc<IEnumerable<IVertex>>(singleEdges =>
                    {
                        return singleEdges
                            .OrderByDescending(edge => edge.GetProperty(propertyID))
                            .Select(aOrderedEdge => aOrderedEdge.GetTargetVertex());
                    });

                return new FuncParameter(topVertices);
            }

            throw new InvalidTypeException(CallingObject.GetType().ToString(), "IHyperEdge");
        }

        public override string PluginName
        {
            get { return"sones.top"; }
        }

        public override Dictionary<string, Type> SetableParameters
        {
            get { return new Dictionary<string,Type>(); }
        }

        public override IPluginable InitializePlugin(Dictionary<string, object> myParameters = null)
        {
            return new TopFunc();
        }
    }
}
