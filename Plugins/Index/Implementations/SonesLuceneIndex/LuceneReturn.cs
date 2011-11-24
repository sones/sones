using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net;
using Lucene.Net.Search;
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.Search.Vectorhighlight;

namespace sones.Plugins.Index.LuceneIdx
{
    public class LuceneReturnEnumerator : IEnumerator<LuceneEntry>
    {
        IndexSearcher _IndexSearcher;
        TopDocs _docs;
        int _pos = -1;
        int _doccount = 0;
        FastVectorHighlighter _highlighter;
        Analyzer _analyzer;
        Query _query;

        public LuceneReturnEnumerator(TopDocs myDocuments, IndexSearcher myIndexSearcher, FastVectorHighlighter myHighlighter, Analyzer myAnalyzer, Query myQuery)
        {
            _IndexSearcher = myIndexSearcher;
            _docs = myDocuments;
            _doccount = _docs.ScoreDocs.Count();
            _highlighter = myHighlighter;
            _analyzer = myAnalyzer;
            _query = myQuery;
        }

        public LuceneEntry Current
        {
            get
            {
                if (_pos < 0) MoveNext();

                var docnum = _docs.ScoreDocs[_pos].doc;
                var entry = new LuceneEntry(_IndexSearcher.Doc(docnum), _docs.ScoreDocs[_pos].score);
                
                FieldQuery fieldquery = _highlighter.GetFieldQuery(_query);
                entry.Highlights = _highlighter.GetBestFragments(fieldquery, _IndexSearcher.GetIndexReader(), docnum, LuceneIndex.FieldNames[LuceneIndex.Fields.TEXT], 100, 3);

                return entry;
            }
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            _pos++;

            if (_pos >= _doccount)
            {
                return false;
            }

            return true;
        }

        public void Reset()
        {
            _pos = 0;
        }

        object System.Collections.IEnumerator.Current
        {
            get { return this.Current; }
        }
    }
    

    public class LuceneReturn : IEnumerable<LuceneEntry>
    {
        private IndexSearcher _IndexSearcher;
        private TopScoreDocCollector _Collector;
        private bool bOpen = false;
        TopDocs _docs;
        FastVectorHighlighter _highlighter;
        Analyzer _analyzer;
        Query _query;

        public LuceneReturn(TopScoreDocCollector myCollector, IndexSearcher myIndexSearcher, Query myQuery, Analyzer myAnalyzer)
        {
            _Collector = myCollector;
            _IndexSearcher = myIndexSearcher;
            _docs = _Collector.TopDocs();
            bOpen = true;
            
            _highlighter = new FastVectorHighlighter(true, true, new SimpleFragListBuilder(), new SimpleFragmentsBuilder());
            _analyzer = myAnalyzer;
            _query = myQuery;
        }

        public IEnumerator<LuceneEntry> GetEnumerator()
        {
            if (!bOpen)
            {
                throw new InvalidOperationException("This LuceneReturn Enumerator has already been closed!");
            }

            return new LuceneReturnEnumerator(_docs, _IndexSearcher, _highlighter, _analyzer, _query);
        }

        public void Close()
        {
            _IndexSearcher.Close();
            bOpen = false;
        }

        public int TotalHits
        {
            get {
                return _Collector.GetTotalHits();
            }
        }

        public float MaxScore
        {
            get
            {
                return _docs.GetMaxScore();
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
