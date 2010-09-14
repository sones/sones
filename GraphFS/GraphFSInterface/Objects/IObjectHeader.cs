/*
 * IObjectHeader
 * (c) Achim Friedland, 2008 - 2010
 */

#region Usings

using System;

using sones;
using sones.GraphFS.Session;
using sones.GraphFS.DataStructures;
using sones.Lib.DataStructures.UUID;
using sones.GraphFS.Session;
using sones.Lib.DataStructures.WeakReference;

#endregion

namespace sones.GraphFS.Objects
{

    public interface IFSObjectHeader
    {

        Boolean         isNew                       { get; set; }
        ObjectUUID      ObjectUUID                  { get; }
        UInt16          StructureVersion            { get; }

        WeakReference<IGraphFSSession> IGraphFSSessionReference { get; set; }
        WeakReference<IGraphFS>        IGraphFSReference           { get; set; }
        INode           INodeReference              { get; }
        ObjectLocator   ObjectLocatorReference      { get; set; }

        Byte[]          EncryptionParameters        { get; set; }
        Byte[]          IntegrityCheckValue         { get; }

        DateTime        ModificationTime            { get; }

        UInt64          EstimatedSize               { get; set; }
        UInt64          ReservedSize                { get; set; }
        Byte[]          SerializedAGraphStructure { get; set; }

        SessionToken    SessionToken                { get; set; }

        Boolean         isDirty                     { get; set; }

    }

}
