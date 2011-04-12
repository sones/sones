using sones.GraphDB.Expression;
using System;
using System.Collections.Generic;
using sones.Library.Settings;

namespace sones.GraphDB.Manager
{
    /// <summary>
    /// A simple optimizer which doesn't optimize anything
    /// </summary>
    public sealed class BasicLogicExpressionOptimizer : ILogicExpressionOptimizer
    {
         #region constructor

        /// <summary>
        /// Creates a new BasicLogicExpressionOptimizer 
        /// BEWARE!!! This constructor is necessary for plugin-functionality.
        /// </summary>
        public BasicLogicExpressionOptimizer()
        {

        }

        #endregion

        #region ILogicExpressionOptimizer Members

        public IExpression OptimizeExpression(IExpression myExpression)
        {
            //return the expression itself and do not optimize anything
            return myExpression;
        }

        #endregion

        #region IPluginable Members

        public String PluginName
        {
            get { return "BasicLogicExpressionOptimizer"; }
        }

        public Dictionary<String, Type> SetableParameters
        {
            get
            {
                return new Dictionary<string, Type>();
            }
        }

        public void InitializePlugin(Dictionary<String, Object> myParameters, GraphApplicationSettings mySettings)
        {

        }

        #endregion
    }
}