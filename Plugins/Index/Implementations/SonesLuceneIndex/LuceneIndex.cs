using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Store;

namespace sones.Plugins.Index.LuceneIdx
{
    class LuceneIndex
    {
        private String _IndexId;
        private Lucene.Net.Index.IndexWriter _IndexWriter;

        #region Constructor

        /// <summary>
        /// Index constructor, in-memory version
        /// </summary>
		/// 
		/// <param name="myLocation">The Solr URL (i.e. "http://localhost:8983/solr").</param>
		/// 
		/// <exception cref="System.ArgumentNullException">
		///		myLocation is NULL.
		/// </exception>
        public LuceneIndex(String myIndexId)
        {
            _IndexId = myIndexId;

            RAMDirectory idx = new RAMDirectory();

            Lucene.Net.Analysis.Analyzer analyzer = new
            Lucene.Net.Analysis.Standard.StandardAnalyzer();

            _IndexWriter = new Lucene.Net.Index.IndexWriter(idx, analyzer, true); 
        }

        /// <summary>
        /// Index constructor, persistent version with path
        /// </summary>
        /// 
        /// <param name="myLocation">The Solr URL (i.e. "http://localhost:8983/solr").</param>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///		myLocation is NULL.
        /// </exception>
        public LuceneIndex(String myIndexId, String myPath)
        {
            _IndexId = myIndexId;

            Lucene.Net.Store.Directory dir = Lucene.Net.Store.FSDirectory.GetDirectory(myPath, true);

            Lucene.Net.Analysis.Analyzer analyzer = new
            Lucene.Net.Analysis.Standard.StandardAnalyzer();

            _IndexWriter = new Lucene.Net.Index.IndexWriter(myPath, analyzer, true);
        }

        #endregion

        /// <summary>
        /// Adds the specified entry to the index.
        /// </summary>
        /// 
        /// <param name="myEntry">The entry to be added.</param>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///		myEntry is NULL.
        /// </exception>
        void AddEntry(LuceneEntry myEntry)
        {
            Lucene.Net.Documents.Document doc = new Lucene.Net.Documents.Document();

            Lucene.Net.Documents.Field id =
              new Lucene.Net.Documents.Field("id",
              myEntry.Id,
              Lucene.Net.Documents.Field.Store.YES,
              Lucene.Net.Documents.Field.Index.ANALYZED,
              Lucene.Net.Documents.Field.TermVector.YES);

            doc.Add(id);

            Lucene.Net.Documents.Field indexId =
              new Lucene.Net.Documents.Field("indexId",
              myEntry.IndexId,
              Lucene.Net.Documents.Field.Store.YES,
              Lucene.Net.Documents.Field.Index.ANALYZED,
              Lucene.Net.Documents.Field.TermVector.YES);

            doc.Add(indexId);

            Lucene.Net.Documents.NumericField vertexId =
              new Lucene.Net.Documents.NumericField("vertexId",
              Lucene.Net.Documents.Field.Store.YES,
              false);

            vertexId.SetLongValue(myEntry.VertexId);

            doc.Add(vertexId);

            if (myEntry.PropertyId != null)
            {

                Lucene.Net.Documents.NumericField propertyId =
                  new Lucene.Net.Documents.NumericField("propertyId",
                  Lucene.Net.Documents.Field.Store.YES,
                  false);
             
                propertyId.SetLongValue((long)myEntry.PropertyId);

                doc.Add(propertyId);
            }

            Lucene.Net.Documents.Field text =
              new Lucene.Net.Documents.Field("text",
              myEntry.Text,
              Lucene.Net.Documents.Field.Store.YES,
              Lucene.Net.Documents.Field.Index.ANALYZED,
              Lucene.Net.Documents.Field.TermVector.YES);

            doc.Add(text);

            _IndexWriter.AddDocument(doc);
        }

        /// <summary>
        /// Deletes an entry that matches the specified LuceneEntry.
        /// </summary>
        /// 
        /// <param name="myEntry">The entry to be deleted.</param>
        /// 
        /// <returns>
        /// 0 (zero) if the operation succeeded; otherwise a value other than 0 (zero).
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///		myEntry is NULL.
        /// </exception>
        Int32 DeleteEntry(LuceneEntry myEntry)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes all entries that match the specified Lucene query.
        /// </summary>
        /// 
        /// <param name="myLuceneQuery">The Lucene query.</param>
        /// <param name="select">A predicate which takes a LuceneEntry and returns whether a LuceneEntry should be taken into account when deleting entries.
        ///						 If this parameter is NULL, no Lucene entry is ignored.</param>
        /// 
        /// <returns>
        /// 0 (zero) if the operation succeeded; otherwise a value other than 0 (zero).
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///		myLuceneQuery is NULL.
        /// </exception>
        Int32 DeleteEntry(String myLuceneQuery, Predicate<LuceneEntry> select = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks whether the Lucene index has entries matching the specified Lucene query.
        /// </summary>
        /// 
        /// <param name="myQuery">The query string.</param>
        /// <param name="select">A predicate which takes a LuceneEntry and returns whether a LuceneEntry should be taken into account when looking for Lucene entries.
        ///						 If this parameter is NULL, no Lucene entry is ignored.</param>
        /// 
        /// <returns>
        /// true, if there are matching entries; otherwise false.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///		myQuery is NULL.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        ///		myQuery is an empty string or contains only whitespace.
        /// </exception>
        Boolean HasEntry(String myQuery, Predicate<LuceneEntry> select = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets all entries matching the specified Lucene query and (optionally) inner Lucene query.
        /// </summary>
        /// 
        /// <param name="myQuery">The query string.</param>
        /// <param name="myInnerQuery">The optional inner query string (to prefilter entries).</param>
        /// 
        /// <returns>
        /// A collection containing all matching Lucene entries; or an empty collection if no entries are matching the query string.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///		myQuery is NULL.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        ///		myQuery is an empty string or contains only whitespace.
        /// </exception>
        IEnumerable<LuceneEntry> GetEntries(String myQuery, String myInnerQuery = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets all entries matching the specified Lucene query and inner field query.
        /// </summary>
        /// 
        /// <param name="myQuery">The query string.</param>
        /// <param name="myInnerQuery">The inner field query string (to prefilter entries).</param>
        /// <param name="myInnerField">Name of the field to use for inner field query.</param>
        /// 
        /// <returns>
        /// A collection containing all matching Lucene entries; or an empty collection if no entries are matching the query string.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///		myQuery is NULL.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        ///		myQuery is an empty string or contains only whitespace.
        /// </exception>
        IEnumerable<LuceneEntry> GetEntriesInnerByField(String myQuery, String myInnerQuery, String myInnerField)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets all Lucene index keys (an index key is represented by LuceneEntry.Text).
        /// </summary>
        /// 
        /// <param name="select">A predicate which takes a LuceneEntry and returns whether a LuceneEntry should be taken into account when looking for keys.
        ///						 If this parameter is NULL, no Lucene entry is ignored.</param>
        /// 
        /// <returns>
        /// A collection with all keys; or an empty list if no entries are within the index
        /// </returns>
        IEnumerable<String> GetKeys(Predicate<LuceneEntry> select = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets all distinct values (values are the vertex IDs).
        /// </summary>
        /// 
        /// <param name="select">A predicate which takes a LuceneEntry and returns whether a LuceneEntry should be taken into account when looking for vertex ids.
        ///						 If this parameter is NULL, no Lucene entry is ignored.</param>
        /// 
        /// <returns>
        /// A collection containing a single set of Int64 values, representing the distinct vertex ids within the Lucene index;
        /// or a collection containing an empty set, if no entries are within the index.
        /// </returns>
        /// 
        /// <dev_doc>
        /// TODO: the return value should be a simple IEnumerable(Of long)
        /// </dev_doc>
        IEnumerable<ISet<long>> GetValues(Predicate<LuceneEntry> select = null)
        {
            throw new NotImplementedException();
        }
    }
}
