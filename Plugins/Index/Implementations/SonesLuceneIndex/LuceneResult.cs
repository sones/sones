using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Plugins.Index.Fulltext;
using sones.Plugins.Index.LuceneIdx;

namespace sones.Plugins.Index.LuceneIdx
{
    class LuceneResult : ISonesFulltextResult
    {
        private List<LuceneResultEntry> _entries;
        private double? _MaxScore = null;
        
        public LuceneResult(LuceneReturn result)
        {
            _entries = new List<LuceneResultEntry>();

            foreach (var entry in result)
            {
                var newentry = new LuceneResultEntry(entry);
                foreach (var highlight in entry.Highlights)
                {
                    newentry.AddHighlight(highlight);
                }
                _entries.Add(newentry);
            }

            _MaxScore = result.MaxScore;
        }

        public IEnumerable<ISonesFulltextResultEntry> Entries
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

    class LuceneResultEntry : ISonesFulltextResultEntry
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
}

