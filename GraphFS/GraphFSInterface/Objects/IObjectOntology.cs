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
 * IObjectOntology
 * (c) Achim Friedland, 2008 - 2010
 */

#region Usings

using System;
using System.Collections.Generic;

using sones.GraphFS.DataStructures;
using sones.Lib.DataStructures;

#endregion

namespace sones.GraphFS.Objects
{

    /// <summary>
    /// This interface defines the object ontology used within the GraphFS.
    /// </summary>

    public interface IObjectOntology : IObjectHeader, IObjectLocation
    {
        
        // ObjectStreams
        String                                  ObjectStream            { get; set; }
        IDictionary<String, ObjectStream>       ObjectStreams           { get; }

        // ObjectEditions
        String                                  ObjectEdition           { get; set; }
        IDictionary<String, ObjectEdition>      ObjectEditions          { get; }

        // ObjectRevisions
        UInt64                                  MinNumberOfRevisions    { get; set; }
        UInt64                                  NumberOfRevisions       { get; }
        UInt64                                  MaxNumberOfRevisions    { get; set; }
        IDictionary<RevisionID, ObjectRevision> ObjectRevisions         { get; }
        RevisionID                              ObjectRevision          { get; set; }

        // ObjectCopies
        UInt64                                  MinNumberOfCopies       { get; set; }
        UInt64                                  NumberOfCopies          { get; }
        UInt64                                  MaxNumberOfCopies       { get; set; }
        IEnumerable<ObjectDatastream>           ObjectCopies            { get; }

        UInt64                                  ObjectSize              { get; }
        UInt64                                  ObjectSizeOnDisc        { get; }

        void CloneObjectOntology(AObjectOntology myAObjectOntology);

    }

}
