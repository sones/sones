using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Expression;

namespace GraphDSRemoteClient.sonesGraphDSRemoteAPI
{
    public partial class ServiceSingleLiteralExpression
    {
        internal ServiceSingleLiteralExpression(SingleLiteralExpression myExpression)
        {
            this.Constant = myExpression.Constant;
        }
    }
}
