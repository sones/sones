/* 
 * GraphFS - IDirectoryEventHandlers
 * (c) Achim Friedland, 2010
 */

#region Usings

using System;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Objects;
using sones.GraphFS.Transactions;

#endregion

namespace sones.GraphFS.Events
{

    public class IDirectoryEventHandlers
    {

        //DirectoryEntryCreated,
        //DirectoryEntryChanged,
        //DirectoryEntryRemoved,

        // ObjectStream Events
        public delegate void OnObjectStreamCreateEventHandler         (ObjectLocation myObjectLocation, String myObjectStream);
        public delegate void OnObjectStreamCreatedEventHandler        (ObjectLocation myObjectLocation, String myObjectStream);
        public delegate void OnObjectStreamCreatedAsyncEventHandler   (ObjectLocation myObjectLocation, String myObjectStream);

        public delegate void OnObjectStreamRemoveEventHandler         (ObjectLocation myObjectLocation, String myObjectStream);
        public delegate void OnObjectStreamRemovedEventHandler        (ObjectLocation myObjectLocation, String myObjectStream);
        public delegate void OnObjectStreamRemovedAsyncEventHandler   (ObjectLocation myObjectLocation, String myObjectStream);


        //InlineDataCreated,
        //InlineDataChanged,
        //InlineDataRemoved,

        //SymlinkCreated,
        //SymlinkChanged,
        //SymlinkRemoved,

    }

}
