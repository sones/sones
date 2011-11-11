using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Search;

namespace sones.Plugins.Index.LuceneIdx
{
    public class LuceneReturnEnumerator : IEnumerator<LuceneEntry>
    {
        IndexSearcher _IndexSearcher;
        TopDocs _docs;
        int _pos = -1;
        int _doccount = 0;

        public LuceneReturnEnumerator(TopDocs myDocuments, IndexSearcher myIndexSearcher)
        {
            _IndexSearcher = myIndexSearcher;
            _docs = myDocuments;
            _doccount = _docs.scoreDocs.Count();
        }

        public LuceneEntry Current
        {
            get
            {
                if (_pos < 0) MoveNext();

                var docnum = _docs.scoreDocs.ElementAt(_pos).doc;
                return new LuceneEntry(_IndexSearcher.Doc(docnum));
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

        public LuceneReturn(TopScoreDocCollector myCollector, IndexSearcher myIndexSearcher)
        {
            _Collector = myCollector;
            _IndexSearcher = myIndexSearcher;
            _docs = _Collector.TopDocs();
            bOpen = true;
        }

        public IEnumerator<LuceneEntry> GetEnumerator()
        {
            if (!bOpen)
            {
                throw new InvalidOperationException("This LuceneReturn Enumerator has already been closed!");
            }

            return new LuceneReturnEnumerator(_docs, _IndexSearcher);
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

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
