using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Expression;

namespace GraphDSRemoteClient.sonesGraphDSRemoteAPI
{
    public partial class ServiceRangeLiteralExpression
    {
        internal ServiceRangeLiteralExpression(RangeLiteralExpression myExpression)
        {
            this.IncludeBorders = myExpression.IncludeBorders;
            this.Lower = myExpression.Lower;
            this.Upper = myExpression.Upper;
        }
    }
}
