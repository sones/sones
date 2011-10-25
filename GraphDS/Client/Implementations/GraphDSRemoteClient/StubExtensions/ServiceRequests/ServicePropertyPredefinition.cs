using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDS.GraphDSRemoteClient.sonesGraphDSRemoteAPI
{
    public partial class ServicePropertyPredefinition
    {
        internal ServicePropertyPredefinition(PropertyPredefinition myPropertyPredefinition) : base(myPropertyPredefinition)
        {
            this.IsIndexed = myPropertyPredefinition.IsIndexed;
            this.IsMandatory = myPropertyPredefinition.IsMandatory;
            this.IsUnique = myPropertyPredefinition.IsUnique;
            this.DefaultValue = myPropertyPredefinition.DefaultValue;
            this.Multiplicity = ConvertHelper.ToServicePropertyMultiplicity(myPropertyPredefinition.Multiplicity);
        }
    }
}
