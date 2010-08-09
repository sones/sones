/* 
 * GraphFS - GraphFSEventHandlers
 * (c) Achim Friedland, 2010
 */

#region Usings

using System;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Objects;

#endregion

namespace sones.GraphFS.Events
{

    public class GraphFSEventHandlers
    {
        public delegate void OnLoadEventHandler    (ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, ObjectRevisionID myRevisionID);
        public delegate void OnLoadedEventHandler  (ObjectLocator  myObjectLocator,  AFSObject myAFSObject);
        public delegate void OnSaveEventHandler    (ObjectLocation myObjectLocation, AFSObject myAFSObject);
        public delegate void OnSavedEventHandler   (ObjectLocator  myObjectLocator,  AFSObject myAFSObject, ObjectRevisionID myOldRevisionID);
        public delegate void OnRemoveEventHandler  (ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, ObjectRevisionID myRevisionID);
        public delegate void OnRemovedEventHandler (ObjectLocator  myObjectLocator,  String myObjectStream, String myObjectEdition, ObjectRevisionID myRevisionID);
    }

}
