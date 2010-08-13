/* 
 * GraphDB - DBVertexEventHandlers
 * (c) Achim Friedland, 2010
 */

#region Usings

using System;

#endregion

namespace sones.GraphDB.Events
{

    public class DBVertexEventHandlers
    {

        // Attribute(Property+Edge)Changed...
        public delegate void OnUpdateEventHandler       ();
        public delegate void OnUpdatedEventHandler      ();
        public delegate void OnUpdatedAsyncEventHandler ();

        // EdgeChanged...
        public delegate void OnLinkEventHandler         ();
        public delegate void OnLinkedEventHandler       ();
        public delegate void OnLinkedAsyncEventHandler  ();

        // DBVertex deletion...
        public delegate void OnDeleteEventHandler       ();
        public delegate void OnDeletedEventHandler      ();
        public delegate void OnDeletedAsyncEventHandler ();

    }

}
