using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphDSRemoteClient.sonesGraphDSRemoteAPI
{
    public partial class StructuredProperty
    {
        internal StructuredProperty(String myPropertyName, object myPropertyValue)
        {
            this.PropertyName = myPropertyName;
            this.PropertyValue = myPropertyValue;
        }
    }
}
