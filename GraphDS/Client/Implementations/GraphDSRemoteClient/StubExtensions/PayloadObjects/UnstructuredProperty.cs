using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDS.GraphDSRemoteClient.sonesGraphDSRemoteAPI
{
    public partial class UnstructuredProperty
    {
        internal UnstructuredProperty(String myPropertyName, object myPropertyValue)
        {
            this.PropertyName = myPropertyName;
            this.PropertyValue = myPropertyValue;
        }
    }
}
