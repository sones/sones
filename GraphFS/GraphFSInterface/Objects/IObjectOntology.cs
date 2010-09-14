/*
 * IFSObjectOntology
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
    /// This interface defines the object ontology used within a IGraphFS.
    /// </summary>

    public interface IFSObjectOntology : IFSObjectHeader, IObjectLocation
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
        IDictionary<ObjectRevisionID, ObjectRevision> ObjectRevisions   { get; }
        ObjectRevisionID                              ObjectRevisionID  { get; set; }

        // ObjectCopies
        UInt64                                  MinNumberOfCopies       { get; set; }
        UInt64                                  NumberOfCopies          { get; }
        UInt64                                  MaxNumberOfCopies       { get; set; }
        IEnumerable<ObjectDatastream>           ObjectCopies            { get; }

        UInt64                                  ObjectSize              { get; }
        UInt64                                  ObjectSizeOnDisc        { get; }

        void CloneObjectOntology(AFSObjectOntology myAObjectOntology);

    }

}
