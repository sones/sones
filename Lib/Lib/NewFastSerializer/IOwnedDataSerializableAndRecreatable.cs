using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Lib.NewFastSerializer
{

    /// <summary>
    /// Allows a class to specify that it can be recreated during deserialization using a default constructor
    /// and then calling DeserializeOwnedData()
    /// </summary>
    public interface IOwnedDataSerializableAndRecreatable : IOwnedDataSerializable { }

}
