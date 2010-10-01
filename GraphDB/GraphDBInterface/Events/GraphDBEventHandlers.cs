/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
* 
*/

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
