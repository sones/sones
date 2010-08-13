
#region Usings

using sones.Lib.Serializer;
using sones.Lib.NewFastSerializer;

#endregion

namespace sones.GraphDB.TypeManagement
{

    /// <summary>
    /// Each Object which go threw the WebService need to derive from AObject.
    /// The main transforming is currently done by GraphDatabaseHost.TransformSelectionListForCustomer.
    /// </summary>
    public interface IObject : IFastSerialize, IFastSerializationTypeSurrogate
    {
    }

}
