/*
 * ExpressionGraphDefinition
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.QueryLanguage.ExpressionGraph;

#endregion

namespace sones.GraphDB.Managers.Structures
{
    class ExpressionGraphDefinition : ATermDefinition
    {
        public IExpressionGraph ExpressionGraph { get; private set; }

        public ExpressionGraphDefinition(IExpressionGraph myExpressionGraph)
        {
            ExpressionGraph = myExpressionGraph;
        }

    }
}
