using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Index;

namespace sones.Plugins.Index.LuceneIdx
{
    public class LuceneValueListEnumerator : IEnumerator<long>
    {
        private TermEnum _terms;
        private IndexReader _reader;
        private int _count;
        private int _pos = -1;

        public LuceneValueListEnumerator(IndexReader myReader)
        {
            _terms = myReader.Terms();
            _reader = myReader;
            _count = myReader.NumDocs();
        }

        public long Current
        {
            get
            {
                return Convert.ToInt64(_reader.Document(_pos).Get(LuceneIndex.FieldNames[LuceneIndex.Fields.VERTEX_ID]));
            }
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            _pos++;

            if (_pos >= _count)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void Reset()
        {
            _pos = -1;
        }

        object System.Collections.IEnumerator.Current
        {
            get { return this.Current; }
        }
    }

    public class LuceneValueList : IEnumerable<long>
    {
        private IndexReader _reader;

        public LuceneValueList(IndexReader myReader)
        {
            _reader = myReader;
        }

        public IEnumerator<long> GetEnumerator()
        {
            return new LuceneValueListEnumerator(_reader);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Close()
        {
            _reader.Close();
        }
    }
}