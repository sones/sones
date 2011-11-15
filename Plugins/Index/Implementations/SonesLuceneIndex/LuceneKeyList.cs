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
        private TermEnum _terms;

        public LuceneKeyListEnumerator(IndexReader myReader)
        {
            _terms = myReader.Terms();
        }

        public string Current
        {
            get
            {
                return _terms.Term().Text();
            }
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            bool ret = false;
            do
            {
                ret = _terms.Next();

                if ((ret == true) && (_terms.Term().Field() == LuceneIndex.FieldNames[LuceneIndex.Fields.TEXT]))
                {
                    break;
                }
            } while (ret);

            return ret;
        }

        public void Reset()
        {
            _terms.Close();
            _terms = _reader.Terms();
        }

        object System.Collections.IEnumerator.Current
        {
            get { return this.Current; }
        }
    }

    public class LuceneKeyList : IEnumerable<string>
    {
        private IndexReader _reader;

        public LuceneKeyList(IndexReader myReader)
        {
            _reader = myReader;
        }

        public IEnumerator<string> GetEnumerator()
        {
            return new LuceneKeyListEnumerator(_reader);
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
