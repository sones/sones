/* 
 * GraphDB - GraphDBEventHandlers
 * (c) Achim Friedland, 2010
 */

#region Usings

using System;

#endregion

namespace sones.GraphDB.Events
{

    public class GraphDBEventHandlers
    {
        public delegate void OnCreateVertexEventHandler  ();
        public delegate void OnVertexCreatedEventHandler ();
        public delegate void OnCreateEdgeEventHandler    ();
        public delegate void OnEdgeCreatedEventHandler   ();
        public delegate void OnInsertEventHandler        ();
        public delegate void OnInsertedEventHandler      ();
        public delegate void OnUpdateEventHandler        ();
        public delegate void OnUpdatedEventHandler       ();
        public delegate void OnLinkEventHandler          ();
        public delegate void OnLinkedEventHandler        ();
        public delegate void OnDeleteEventHandler        ();
        public delegate void OnDeletedEventHandler       ();
        public delegate void OnDropVertexEventHandler    ();
        public delegate void OnVertexDroppedEventHandler ();
        public delegate void OnDropEdgeEventHandler      ();
        public delegate void OnEdgeDroppedEventHandler   ();
    }

}
