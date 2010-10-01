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
