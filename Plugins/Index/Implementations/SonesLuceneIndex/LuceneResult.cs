using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Plugins.Index.Fulltext;
using sones.Plugins.Index.LuceneIdx;

namespace sones.Plugins.Index.LuceneIdx
{
    public class LuceneResult : ISonesFulltextResult
    {
        private LuceneResultEntryList _entries;
        private double? _MaxScore = null;
        
        public LuceneResult(LuceneReturn result)
        {
            _entries = new LuceneResultEntryList(result);
            _MaxScore = result.MaxScore;
        }

        public ICloseableEnumerable<ISonesFulltextResultEntry> Entries
        {
            get { return _entries; }
        }

        public IDictionary<string, object> AdditionalParameters
        {
            get { throw new NotImplementedException(); }
        }

        public Double? MaxScore
        {
            get { return _MaxScore; }
        }
    }

    public class LuceneResultEntry : ISonesFulltextResultEntry
    {
        private long _VertexID;
        private IDictionary<Int64, string> _Highlights = null;
        private double? _Score = null;

        public LuceneResultEntry(LuceneEntry entry)
        {
            _VertexID = entry.VertexId;
            _Highlights = new Dictionary<Int64, string>();
            _Score = entry.Score;
        }

        public void AddHighlight(string highlight)
        {
            _Highlights.Add(0, highlight); // TODO Set correct Property ID
        }

        public long VertexID
        {
            get { return _VertexID; }
        }

        public IDictionary<long, string> Highlights
        {
            get { return _Highlights; }
        }

        public IDictionary<string, object> AdditionalParameters
        {
            get { throw new NotImplementedException(); }
        }

        public Double? Score
        {
            get { return _Score; }
        }
    }

    public class LuceneResultEntryListEnumerator : IEnumerator<ISonesFulltextResultEntry>
    {
        IEnumerator<LuceneEntry> _LuceneReturnEnumerator;

        public LuceneResultEntryListEnumerator(LuceneReturn myLuceneReturn)
        {
            _LuceneReturnEnumerator = myLuceneReturn.GetEnumerator();
        }

        public ISonesFulltextResultEntry Current
        {
            get {
                var curluceneentry = _LuceneReturnEnumerator.Current;
                var curresultentry = new LuceneResultEntry(curluceneentry);
                foreach (var highlight in curluceneentry.Highlights)
                {
                    curresultentry.AddHighlight(highlight);
                }
                return curresultentry;
            }
        }

        public void Dispose()
        {
            _LuceneReturnEnumerator.Dispose();
        }

        object System.Collections.IEnumerator.Current
        {
            get { return Current; }
        }

        public bool MoveNext()
        {
            return _LuceneReturnEnumerator.MoveNext();
        }

        public void Reset()
        {
            _LuceneReturnEnumerator.Reset();
        }
    }

    public class LuceneResultEntryList : ICloseableEnumerable<ISonesFulltextResultEntry>
    {
        LuceneReturn _LuceneReturn;

        public LuceneResultEntryList(LuceneReturn myLuceneReturn)
        {
            _LuceneReturn = myLuceneReturn;
        }

        public void Close()
        {
            _LuceneReturn.Close();
        }

        public IEnumerator<ISonesFulltextResultEntry> GetEnumerator()
        {
            return new LuceneResultEntryListEnumerator(_LuceneReturn);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Dispose()
        {
            Close();
        }
    }
}

