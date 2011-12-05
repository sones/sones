using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Index;

namespace sones.Plugins.Index.LuceneIdx
{
    public class LuceneKeyListEnumerator : IEnumerator<string>
    {
        private IndexReader _reader;
        private int _count;
        private int _pos = -1;
        
        public LuceneKeyListEnumerator(IndexReader myReader)
        {
            _reader = myReader;
            _count = myReader.NumDocs();
        }

        public string Current
        {
            get
            {
                return _reader.Document(_pos).Get(LuceneIndex.FieldNames[LuceneIndex.Fields.TEXT]);
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

    public class LuceneKeyListPredicateEnumerator : IEnumerator<string>
    {
        private IndexReader _reader;
        private int _count;
        private int _pos = -1;
        private Predicate<LuceneEntry> _predicate;

        public LuceneKeyListPredicateEnumerator(IndexReader myReader, Predicate<LuceneEntry> myPredicate)
        {
            _reader = myReader;
            _count = myReader.NumDocs();
            _predicate = myPredicate;
        }

        public string Current
        {
            get
            {
                return _reader.Document(_pos).Get(LuceneIndex.FieldNames[LuceneIndex.Fields.TEXT]);
            }
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            bool bEnd = false;

            do
            {
                _pos++;

                if (_pos >= _count)
                {
                    bEnd = true;
                }
                else
                {
                    if (_predicate(new LuceneEntry(_reader.Document(_pos))))
                    {
                        return true;
                    }
                }
            } while (!bEnd);

            return false;
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
    
    public class LuceneKeyList : IEnumerable<string>
    {
        private IndexReader _reader;
        private Predicate<LuceneEntry> _predicate;

        public LuceneKeyList(IndexReader myReader)
        {
            _reader = myReader;
            _predicate = null;
        }

        public LuceneKeyList(IndexReader myReader, Predicate<LuceneEntry> myPredicate)
        {
            _reader = myReader;
            _predicate = myPredicate;
        }

        public IEnumerator<string> GetEnumerator()
        {
            if (_predicate == null)
            {
                return new LuceneKeyListEnumerator(_reader);
            }
            else
            {
                return new LuceneKeyListPredicateEnumerator(_reader, _predicate);
            }
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
