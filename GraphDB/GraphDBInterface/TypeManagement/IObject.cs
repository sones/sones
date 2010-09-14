
#region Usings

using System;
using sones.Lib.NewFastSerializer;
using sones.Lib.Serializer;

#endregion

namespace sones.GraphDB.TypeManagement
{

    /// <summary>
    /// Each Object which go threw the WebService need to derive from AObject.
    /// The main transforming is currently done by GraphDatabaseHost.TransformSelectionListForCustomer.
    /// </summary>
    public interface IObject : IComparable, IFastSerialize, IFastSerializationTypeSurrogate
    {
    }

}
