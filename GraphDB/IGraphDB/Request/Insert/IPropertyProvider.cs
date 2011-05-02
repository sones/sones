using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Request;

namespace sones.GraphDB.Request.Insert
{
    public interface IPropertyProvider: IUnknownProvider
    {
        IDictionary<String, IComparable> StructuredProperties { get; }

        IDictionary<String, Object> UnstructuredProperties { get; }

        IDictionary<String, Object> UnknownProperties { get; }

        IPropertyProvider AddStructuredProperty(String myPropertyName, IComparable myProperty);

        IPropertyProvider AddUnstructuredProperty(String myPropertyName, Object myProperty);

        IPropertyProvider AddUnknownProperty(String myPropertyName, Object myProperty);
        
    }
}
