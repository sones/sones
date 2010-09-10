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
