/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.Commons.VertexStore.Definitions;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace sones.Plugins.SonesGQL.XMLBulkImport
{
    /// <summary>
    /// Contains the data of one incoming edge.
    /// </summary>
    internal struct IncomingEdge
    {
        /// <summary>
        /// The size of the data.
        /// </summary>
        public static readonly int Size = 40;

        /// <summary>
        /// The id of the source vertex type.
        /// </summary>
        public long SourceVertexTypeID;

        /// <summary>
        /// The id of the source vertex.
        /// </summary>
        public long SourceVertexID;

        /// <summary>
        /// The id of the target vertex type.
        /// </summary>
        public long TargetVertexTypeID;

        /// <summary>
        /// The id of the target vertex.
        /// </summary>
        public long TargetVertexID;

        /// <summary>
        /// The property id of the corresponding edge.
        /// </summary>
        public long PropertyID;

    }

    /// <summary>
    /// A comparer for incoming edges and enumerators of incoming edges.
    /// </summary>
    internal class IncomingEdgeComparer: IComparer<IncomingEdge>, IComparer<IEnumerator<IncomingEdge>>
    {
        /// <summary>
        /// An instance of the class.
        /// </summary>
        public static IncomingEdgeComparer Instance = new IncomingEdgeComparer();

        #region IComparer<IncomingEdge> Members

        public int Compare(IncomingEdge x, IncomingEdge y)
        {
            if (x.TargetVertexTypeID == y.TargetVertexTypeID)
                return x.TargetVertexID.CompareTo(y.TargetVertexID);

            return x.TargetVertexTypeID.CompareTo(y.TargetVertexTypeID);
        }

        #endregion

        #region IComparer<IEnumerator<IncomingEdge>> Members

        public int Compare(IEnumerator<IncomingEdge> x, IEnumerator<IncomingEdge> y)
        {
            return Compare(x.Current, y.Current);
           
        }

        #endregion
    }

    /// <summary>
    /// The states the IncomingEdgeSorter can engage.
    /// </summary>
    internal enum IncomingEdgeSorterState
    {
        /// <summary>
        /// The IncomingEdgeSorter is in write mode. That means, more incoming edges can be stored.
        /// </summary>
        Writing,

        /// <summary>
        /// The IncomingEdgeSorter is in read mode. That means, no more incoming edges can be stored.
        /// </summary>
        Reading,

        /// <summary>
        /// The IncomingEdgeSorter is closed.
        /// </summary>
        Closed
    }

    /// <summary>
    /// A class that can sort a huge amount of incoming edges, with a fixed amount of main memory and a fixed amount of files to be open.
    /// </summary>
    /// <remarks>
    /// This class is not thread-safe. Concurrent calls to Add and GetSorted might end unexpected behaviour.
    /// </remarks>
    internal class IncomingEdgeSorter: IDisposable
    {

        #region Import Parameter

        public const string ConfigSortLocations = "Locations";
        public const string ConfigMaxMemory     = "Memory";
        public const string ConfigMaxOpenFiles  = "Files";

        #endregion

        private const long DefaultMaxMemoryConsumption = 1000000000; //1 GB
        private const uint DefaultMaxOpenFiles = 200;
        private const IEnumerable<String> DefaultLocations = null;

        /// <summary>
        /// Stores the current files that stores needed information.
        /// </summary>
        private List<string> _chunkFiles = new List<String>();


        /// <summary>
        /// Stores the reference to the asynchron chunk storer.
        /// </summary>
        private CancellationTokenSource _cancel = new CancellationTokenSource();

        private IDictionary<Location, Task> _storeTasks;

        private IDictionary<Location, Task> _sortTasks;

        private IDictionary<Location, IncomingEdge[]> _chunks;

        private IncomingEdge[] _current;

        public IEnumerable<String> Locations
        {
            get
            {
                return _chunks.Select(_=>_.Key.LocationPath);
            }
        }

        private int ChunkCount 
        { 
            get 
            {
                return _chunks.Count;
            } 
        }

        private int ChunkLength
        {
            get
            {
                return _current.GetLength(0);
            }
        }

        public IncomingEdgeSorter(Dictionary<String, String> myUserConfiguration)
        {
            if (myUserConfiguration != null)
            {
                var maxMemoryConsumption = (myUserConfiguration.ContainsKey(ConfigMaxMemory))
                    ? ReadMaxMemory(myUserConfiguration[ConfigMaxMemory])
                    : DefaultMaxMemoryConsumption;
                var maxOpenFiles = (myUserConfiguration.ContainsKey(ConfigMaxOpenFiles))
                    ? ReadMaxOpenFiles(myUserConfiguration[ConfigMaxOpenFiles])
                    : DefaultMaxOpenFiles;
                var locations = (myUserConfiguration.ContainsKey(ConfigSortLocations))
                    ? ReadLocations(myUserConfiguration[ConfigSortLocations])
                    : DefaultLocations;

                Initialize(maxMemoryConsumption, maxOpenFiles, locations);
            }
            else
            {
                Initialize(DefaultMaxMemoryConsumption, DefaultMaxOpenFiles, DefaultLocations);
            }
        }

        private IEnumerable<string> ReadLocations(string p)
        {
            return p.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        }

        private uint ReadMaxOpenFiles(string p)
        {
            return uint.Parse(p);
        }

        private long ReadMaxMemory(string p)
        {
            p = p.Trim();
            var numberString = new string(p.TakeWhile(_ => Char.IsDigit(_) || Char.IsPunctuation(_)).ToArray());
            var unit = p.Remove(0, numberString.Length).Trim();

            var number = double.Parse(numberString);

            switch (unit)
            {
                case "":
                case "B":
                    return (long)number;
                case "KB":
                case "kB":
                    return (long)(number * 1000);
                case "KiB":
                    return (long)(number * 1024);
                case "MB":
                    return (long)(number * 1000000);
                case "MiB":
                    return (long)(number * 1048576);
                case "GB":
                    return (long)(number * 1000000000);
                case "GiB":
                    return (long)(number * 1073741824);
                case "TB":
                    return (long)(number * 1000000000000);
                case "TiB":
                    return (long)(number * 1099511627776);
                case "PB":
                    return (long)(number * 1000000000000000);
                case "PiB":
                    return (long)(number * 1125899906842624);

                default:
                    throw new ArgumentException("Can not read value for max memory.");
            }
            
        }

        /// <summary>
        /// Creates a new instance of IncomingEdgeSorter.
        /// </summary>
        /// <param name="myMaxMemoryConsumption">A soft limit that handles the memory consumption of this instance.</param>
        /// <param name="myMaxOpenFiles">A hard limit of files this instance will keep open.</param>
        public IncomingEdgeSorter(long myMaxMemoryConsumption = DefaultMaxMemoryConsumption, uint myMaxOpenFiles = DefaultMaxOpenFiles, IEnumerable<String> myLocations = DefaultLocations)
        {
            Initialize(myMaxMemoryConsumption, myMaxOpenFiles, myLocations);
        }

        private void Initialize(long myMaxMemoryConsumption, uint myMaxOpenFiles, IEnumerable<String> myLocations)
        {
            #region sanity checks

            //240 MB
            if (myMaxMemoryConsumption < 251658240)
                throw new ArgumentOutOfRangeException("myMaxMemoryConsumption", "At least 240 MB (251658240) are needed.");

            if (myMaxOpenFiles < 3)
                throw new ArgumentOutOfRangeException("myMaxOpenFiles", "At least 3 open files are needed.");

            #endregion

            #region fill properties

            State = IncomingEdgeSorterState.Writing;

            Count = 0L;

            MaxMemoryConsumption = myMaxMemoryConsumption;

            MaxOpenFiles = myMaxOpenFiles;

            #endregion

            #region calculate the chunk size and create them

            var locations = (myLocations == null)
                ? new String[] { Path.GetTempPath() }
                : myLocations;

            int chunkCount = locations.Count() + 1;

            int chunkLength = (int)Math.Min(
                (long)(2147480000 / IncomingEdge.Size),                     //about 2GB (max data size in .NET) divided by the size of edges
                MaxMemoryConsumption / (IncomingEdge.Size * (chunkCount))); 

            _current = new IncomingEdge[chunkLength];
            _chunks = locations.ToDictionary(_ => new Location(_), _ => new IncomingEdge[chunkLength]);

            _storeTasks = new Dictionary<Location, Task>(chunkCount);
            _sortTasks = new Dictionary<Location, Task>(ChunkCount);
            #endregion

        }

        /// <summary>
        /// The memory this instance can use to sort the edges.
        /// </summary>
        /// <remarks>
        /// This is not the amount of memory this instance uses over all. It is the size this instances reserves for each chunk, and for the final merge.
        /// </remarks>
        public long MaxMemoryConsumption { get; private set; }

        /// <summary>
        /// The maximum of files that instance will keep open.
        /// </summary>
        public uint MaxOpenFiles { get; private set; }

        /// <summary>
        /// The count of items currently stored.
        /// </summary>
        public long Count { get; private set; }

        /// <summary>
        /// The current state of the sorter.
        /// </summary>
        public IncomingEdgeSorterState State { get; private set; }

        /// <summary>
        /// Adds an incoming edge.
        /// </summary>
        /// <param name="mySource">A vertex information of the source.</param>
        /// <param name="myTarget">A vertex information of the target.</param>
        /// <param name="myPropertyID">The property id of the corresponding edge.</param>
        public void Add(VertexInformation mySource, VertexInformation myTarget, long myPropertyID)
        {
            Add(mySource.VertexTypeID, mySource.VertexID, myTarget.VertexTypeID, myTarget.VertexID, myPropertyID);
        } 

        /// <summary>
        /// Adds an incoming edge.
        /// </summary>
        /// <param name="mySourceVertexTypeID">The type id of the source vertex.</param>
        /// <param name="mySourceVertexID">The id of the source vertex.</param>
        /// <param name="myTargetVertexTypeID">The type id of the target vertex.</param>
        /// <param name="myTargetVertexID">The id of the target vertex.</param>
        /// <param name="myPropertyID">The property id of the corresponding edge.</param>
        public void Add(long mySourceVertexTypeID, long mySourceVertexID, long myTargetVertexTypeID, long myTargetVertexID, long myPropertyID)
        {
            if (State != IncomingEdgeSorterState.Writing)
                throw new InvalidOperationException("Object is not in 'Writing' state.");

            var currentSlot = (int)(Count % ChunkLength);
            _current[currentSlot].SourceVertexTypeID = mySourceVertexTypeID;
            _current[currentSlot].SourceVertexID = mySourceVertexID;
            _current[currentSlot].TargetVertexTypeID = myTargetVertexTypeID;
            _current[currentSlot].TargetVertexID = myTargetVertexID;
            _current[currentSlot].PropertyID= myPropertyID;

            Count++;

            if ((Count) % _current.LongLength == 0)
            {
                SaveChunk();
            }
        }

        /// <summary>
        /// Adds an incoming edge.
        /// </summary>
        /// <param name="myEdge">The incoming edge.</param>
        public void Add(IncomingEdge myEdge)
        {

        }


        private void SaveChunk()
        {

            //here we wait until a location is free
            var location = GetFreeLocation();
            var filename = Path.Combine(location.LocationPath, Path.GetRandomFileName());

            #region Swap

            var help = _current;
            _current = _chunks[location];
            _chunks[location] = help;

            #endregion

            //start a task to sort the current (now swapped) chunk
            Task sortTask = Task.Factory.StartNew(() => SortChunk(_chunks[location]));
            _sortTasks[location] = sortTask;

            if (_chunkFiles.Count + 1 == MaxOpenFiles)
            {
                //If no more open files are available, the present ones are merged to one.

                //now we store the merge of all previous chunks
                //var storeTask = sortTask.ContinueWith(_ => _locations[location].Store(ReadFromSortedChunks()));

                //storeTask.ContinueWith(_ => RemoveFiles(_chunkFiles.ToArray()));



            }
            else
            {
                _storeTasks[location] = sortTask.ContinueWith(_ => location.Store(_chunks[location], filename));
            }

            Console.WriteLine("Send: {0}", filename);


            _chunkFiles.Add(filename);
        }

        private void RemoveFiles(IEnumerable<string> myFilesToDelete)
        {
            foreach (var file in myFilesToDelete)
                File.Delete(file);
        }

        private Location GetFreeLocation()
        {
            if (_chunks.Count > _storeTasks.Count)
            {
                //there is a location that was not used yet
                return _chunks.Keys.Except(_storeTasks.Keys).First();
            }
            else
            {
                //all locations were at least one time used
                //wait for a completed task
                var completedTask = Task.WaitAny(_storeTasks.Values.ToArray());
                return _storeTasks.Keys.ElementAt(completedTask);
            }
        }

        private void SortChunk(IncomingEdge[] myChunk)
        {
            Array.Sort<IncomingEdge>(myChunk, IncomingEdgeComparer.Instance);
        }


        public IEnumerable<IncomingEdge> GetSorted()
        {
            if (State == IncomingEdgeSorterState.Closed)
                throw new InvalidOperationException("Sorter is closed.");

            if (State == IncomingEdgeSorterState.Writing)
            {
                State = IncomingEdgeSorterState.Reading;
            }

            var pos = (Count - 1);
            var posChunk = (int)((pos / ChunkLength) % ChunkCount);
            var posSlot = (int)(pos % ChunkLength);



            //sort the chunk of the last inserted edge.
            Array.Sort<IncomingEdge>(_current, 0, posSlot + 1, IncomingEdgeComparer.Instance);



            //we read the chunks from memory
            var inMemoryChunks = _chunks //take all chunks
                .Where(_=>_sortTasks.ContainsKey(_.Key)) //filter all unused
                .Select(_=>_.Value) //take the values
                .Union(Enumerable.Repeat(_current.Take(posSlot + 1), 1)); //union them with the current one

            //we wait for the sort task to finish
            Task.WaitAll(_sortTasks.Values.ToArray());

            //We cancel the store process of the current locations
            foreach (var location in _storeTasks.Keys)
            {
                location.Cancel();
            }

            //now we wait until all store tasks are canceled.
            Task.WaitAll(_storeTasks.Values.ToArray());

            var filesToRead = _chunkFiles.Except(_storeTasks.Keys.Select(_ => _.CurrentFile));

            Console.WriteLine(string.Join(", ", _chunkFiles));

            return ReadFromSortedChunks(inMemoryChunks, filesToRead);
        }

        private IEnumerable<IncomingEdge> ReadFromSortedChunks(IEnumerable<IEnumerable<IncomingEdge>> myInMemoryChunks, IEnumerable<String> myFiles)
        {

            //the list of all enumerables to merge (in memory and on disk)
            List<IEnumerable<IncomingEdge>> toMerge = myInMemoryChunks.ToList();

            toMerge.AddRange(myFiles.Select(_ => new ChunkReader(_, false)));

            return toMerge.Merge();
        }


        #region IDisposable Members

        public void Dispose()
        {
            State = IncomingEdgeSorterState.Closed;
            _cancel.Cancel();

            foreach (var file in _chunkFiles)
            {
                File.Delete(file);
            }
        }

        #endregion
    }
}
