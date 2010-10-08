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


        // Vertices
        public delegate void OnCreateVertexEventHandler         ();
        public delegate void OnVertexCreatedEventHandler        ();
        public delegate void OnVertexCreatedAsyncEventHandler   ();

        public delegate void OnAlterVertexEventHandler          ();
        public delegate void OnVertexAlternatedEventHandler     ();
        public delegate void OnVertexAlternatedAsyncEventHandler();

        public delegate void OnDropVertexEventHandler           ();
        public delegate void OnVertexDroppedEventHandler        ();
        public delegate void OnVertexDroppedAsyncEventHandler   ();


        // Edges
        public delegate void OnCreateEdgeEventHandler           ();
        public delegate void OnEdgeCreatedEventHandler          ();
        public delegate void OnEdgeCreatedAsyncEventHandler     ();

        public delegate void OnAlterEdgeEventHandler            ();
        public delegate void OnEdgeAlternatedEventHandler       ();
        public delegate void OnEdgeAlternatedAsyncEventHandler  ();

        public delegate void OnDropEdgeEventHandler             ();
        public delegate void OnEdgeDroppedEventHandler          ();
        public delegate void OnEdgeDroppedAsyncEventHandler     ();


        // (Base-)Types
        public delegate void OnCreateTypeEventHandler           ();
        public delegate void OnTypeCreatedEventHandler          ();
        public delegate void OnTypeCreatedAsyncEventHandler     ();

        public delegate void OnDropTypeEventHandler             ();
        public delegate void OnTypeDroppedEventHandler          ();
        public delegate void OnTypeDroppedAsyncEventHandler     ();


        // Data
        public delegate void OnInsertEventHandler               ();
        public delegate void OnInsertedEventHandler             ();
        public delegate void OnInsertedAsyncEventHandler        ();

        public delegate void OnUpdateEventHandler               ();
        public delegate void OnUpdatedEventHandler              ();
        public delegate void OnUpdatedAsyncEventHandler         ();

        public delegate void OnLinkEventHandler                 ();
        public delegate void OnLinkedEventHandler               ();
        public delegate void OnLinkedAsyncEventHandler          ();

        public delegate void OnDeleteEventHandler               ();
        public delegate void OnDeletedEventHandler              ();
        public delegate void OnDeletedAsyncEventHandler         ();


        // Transactions


    }

}
