using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Lucene.Net;
using Lucene.Net.Store;
using Lucene.Net.Search;
using Lucene.Net.Index;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.QueryParsers;
using Lucene.Net.Util;
using Lucene.Net.Documents;
using System.Collections;

namespace sones.Plugins.Index.LuceneIdx
{
    public class LuceneIndex
    {
        private String _IndexId;
        private Lucene.Net.Store.Directory _IndexDirectory;
        private Analyzer _Analyzer;

        public enum Fields { ID, INDEX_ID, VERTEX_ID, PROPERTY_ID, TEXT };
        internal static Dictionary<Fields, String> FieldNames = new Dictionary<Fields, string>()
        { 
            { Fields.ID, "id" },
            { Fields.INDEX_ID, "indexId" },
            { Fields.VERTEX_ID, "vertexId" },
            { Fields.PROPERTY_ID, "propertyId" },
            { Fields.TEXT, "Text" }
        };

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

            _IndexDirectory = new RAMDirectory();

            _Analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);
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

            _IndexDirectory = new SimpleFSDirectory(new DirectoryInfo(myPath));
            
            _Analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);
        }

        #endregion

        private IndexWriter GetIndexWriter()
        {
            IndexWriter _IndexWriter;
            try
            {
                _IndexWriter = new IndexWriter(_IndexDirectory, _Analyzer, false, IndexWriter.MaxFieldLength.UNLIMITED);
            }
            catch (IOException)
            {
                _IndexWriter = new IndexWriter(_IndexDirectory, _Analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED);
            }
            return _IndexWriter;
        }
                
        private void CloseIndexWriter(IndexWriter myWriter, bool bOptimize=false)
        {
            if (bOptimize) myWriter.Optimize();
            myWriter.Close();
            myWriter.Dispose();
        }

        private IndexReader GetIndexReader(bool bReadOnly = true)
        {
            return IndexReader.Open(_IndexDirectory, bReadOnly);
        }

        private void CloseIndexReader(IndexReader myReader)
        {
            myReader.Close();
            myReader.Dispose();
        }

        /// <summary>
        /// Optimizes the index for fastest search.
        /// </summary>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///		myEntry is NULL.
        /// </exception>
        public void Optimize()
        {
            var writer = GetIndexWriter();
            CloseIndexWriter(writer, true);
        }

        /// <summary>
        /// Adds the specified entry to the index.
        /// </summary>
        /// 
        /// <param name="myEntry">The entry to be added.</param>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///		myEntry is NULL.
        /// </exception>
        public void AddEntry(LuceneEntry myEntry)
        {
            if (myEntry == null)
            {
                throw new InvalidOperationException("Parameter myEntry cannot be null!");
            }
            
            Document doc = new Document();

            Field id =
              new Field(FieldNames[Fields.ID],
              myEntry.Id,
              Field.Store.YES,
              Field.Index.ANALYZED,
              Field.TermVector.NO);

            doc.Add(id);

            Field indexId =
              new Field(FieldNames[Fields.INDEX_ID],
              myEntry.IndexId,
              Field.Store.YES,
              Field.Index.ANALYZED,
              Field.TermVector.NO);

            doc.Add(indexId);

            Field vertexId =
              new Field(FieldNames[Fields.VERTEX_ID],
              myEntry.VertexId.ToString(),
              Field.Store.YES,
              Field.Index.ANALYZED,
              Field.TermVector.NO);

            doc.Add(vertexId);

            if (myEntry.PropertyId != null)
            {
                Field propertyId =
                    new Field(FieldNames[Fields.PROPERTY_ID],
                    myEntry.PropertyId.ToString(),
                    Field.Store.YES,
                    Field.Index.ANALYZED,
                    Field.TermVector.NO);

                doc.Add(propertyId);
            }

            Field text =
              new Field(FieldNames[Fields.TEXT],
              myEntry.Text,
              Field.Store.YES,
              Field.Index.ANALYZED,
              Field.TermVector.WITH_POSITIONS_OFFSETS);

            doc.Add(text);

            var writer = GetIndexWriter();
            writer.AddDocument(doc);
            CloseIndexWriter(writer);
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
        public int DeleteEntry(LuceneEntry myEntry)
        {
            if (myEntry == null)
            {
                throw new InvalidOperationException("myEntry parameter cannot be null!");
            }

            Term delterm = new Term(LuceneIndex.FieldNames[LuceneIndex.Fields.ID], myEntry.Id);

            var reader = GetIndexReader(false);
            int count = reader.DeleteDocuments(delterm);
            CloseIndexReader(reader);

            return count;
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
        public int DeleteEntry(String myLuceneQuery, Predicate<LuceneEntry> select = null)
        {
            int count = 0;

            if (myLuceneQuery == null)
            {
                throw new InvalidOperationException("myLuceneQuery parameter cannot be null!");
            }

            var reader = GetIndexReader(false);

            LuceneReturn ret = null;

            ret = GetEntries(1, myLuceneQuery);
            if (ret.TotalHits > 1)
            {
                ret.Close();
                ret = GetEntries(ret.TotalHits, myLuceneQuery);
            }

            foreach (var entry in ret)
            {
                if ((select == null) || select(entry))
                {
                    if (entry.DocNum != null)
                    {
                        int docnum = (int)entry.DocNum;
                        reader.DeleteDocument(docnum);
                        count++;
                    }
                }
            }
            ret.Close();

            CloseIndexReader(reader);
            return count;
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
        public Boolean HasEntry(String myQuery, Predicate<LuceneEntry> select = null)
        {
            if (myQuery == null)
            {
                throw new InvalidOperationException("myQuery cannot be null!");
            }

            LuceneReturn ret = null;
            if (select != null)
            {
                ret = GetEntries(1, myQuery);
                ret.Close();
                if (ret.TotalHits > 0)
                {
                    ret = GetEntries(ret.TotalHits, myQuery);

                    foreach (var entry in ret)
                    {
                        if (select(entry))
                        {
                            ret.Close();
                            return true;
                        }
                    }
                }
                ret.Close();
                return false;
            }
            else
            {
                ret = GetEntries(1, myQuery);
                ret.Close();
                if (ret.TotalHits > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets all entries matching the specified Lucene query and (optionally) inner Lucene query.
        /// </summary>
        /// 
        /// <remarks>
        /// You must call Close method of returned LuceneReturn class if no more usage is desired!
        /// Furthermore check if return of method GetTotalHits of returned LuceneReturn class may
        /// be bigger than given myMaxResultsCount --> means you are missing results not returned!
        /// </remarks>
        /// 
        /// <param name="myMaxResultsCount">The count of maximum results to return (to limit the complexity).</param>
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
        public LuceneReturn GetEntries(int myMaxResultsCount, String myQuery, String myInnerQuery = null)
        {
            if (myMaxResultsCount <= 0)
            {
                throw new InvalidOperationException("myMaxResultsCount cannot be <= 0 !");
            }

            if (myQuery == null)
            {
                throw new InvalidOperationException("myQuery cannot be null!");
            }

            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);
            var queryparser = new QueryParser(Lucene.Net.Util.Version.LUCENE_29, FieldNames[Fields.TEXT], analyzer);
            Query outerquery = null;
            Query innerquery = null;
            Query query = null;
            try
            {
                outerquery = queryparser.Parse(myQuery);
                if (myInnerQuery != null)
                {
                    innerquery = queryparser.Parse(myInnerQuery);
                    BooleanQuery boolquery = new BooleanQuery();
                    boolquery.Add(outerquery, BooleanClause.Occur.MUST);
                    boolquery.Add(innerquery, BooleanClause.Occur.MUST);
                    query = (Query)boolquery;
                }
                else
                {
                    query = outerquery;
                }
            }
            catch (ParseException)
            {
                return null;
            }

            var _IndexSearcher = new IndexSearcher(_IndexDirectory, true);
            var _Collector = TopScoreDocCollector.create(myMaxResultsCount, true);

            _IndexSearcher.Search(query, _Collector);
            
            return new LuceneReturn(_Collector, _IndexSearcher, query, analyzer);
        }

        /// <summary>
        /// Gets all entries matching the specified Lucene query and inner field query.
        /// </summary>
        /// 
        /// <param name="myMaxResultsCount">The count of maximum results to return (to limit the complexity).</param>
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
        public LuceneReturn GetEntriesInnerByField(int myMaxResultsCount, String myQuery, String myInnerQuery, Fields myInnerField)
        {
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);
            var outerqueryparser = new QueryParser(Lucene.Net.Util.Version.LUCENE_29, FieldNames[Fields.TEXT], analyzer);
            var innerqueryparser = new QueryParser(Lucene.Net.Util.Version.LUCENE_29, FieldNames[myInnerField], analyzer);
            Query outerquery = null;
            Query innerquery = null;
            Query query = null;
            try
            {
                outerquery = outerqueryparser.Parse(myQuery);
                innerquery = innerqueryparser.Parse(myInnerQuery);
                BooleanQuery boolquery = new BooleanQuery();
                boolquery.Add(outerquery, BooleanClause.Occur.MUST);
                boolquery.Add(innerquery, BooleanClause.Occur.MUST);
                query = (Query)boolquery;
            }
            catch (ParseException)
            {
                return null;
            }

            var _IndexSearcher = new IndexSearcher(_IndexDirectory, true);
            var _Collector = TopScoreDocCollector.create(myMaxResultsCount, true);

            _IndexSearcher.Search(query, _Collector);

            return new LuceneReturn(_Collector, _IndexSearcher, query, analyzer);
        }

        /// <summary>
        /// Gets all Lucene index keys (an index key is represented by LuceneEntry.Text).
        /// </summary>
        /// 
        /// <remarks>
        /// If enumeration of returned LuceneKeyList is finished, please call "Close" method
        /// of LuceneKeyList to close all open handles.
        /// </remarks>
        /// 
        /// <returns>
        /// A collection with all keys; or an empty list if no entries are within the index
        /// </returns>
        public LuceneKeyList GetKeys(Predicate<LuceneEntry> select = null)
        {
            var searcher = new IndexSearcher(_IndexDirectory, true);
            var reader = searcher.GetIndexReader();

            if (select == null)
            {
                var ret = new LuceneKeyList(reader);
                return ret;
            }
            else
            {
                var ret = new LuceneKeyList(reader, select);
                return ret;
            }
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
        public LuceneValueList GetValues(Predicate<LuceneEntry> select = null)
        {
            var searcher = new IndexSearcher(_IndexDirectory, true);
            var reader = searcher.GetIndexReader();

            if (select == null)
            {
                var ret = new LuceneValueList(reader);
                return ret;
            }
            else
            {
                var ret = new LuceneValueList(reader, select);
                return ret;
            }
        }

        /// <summary>
        /// Empties the whole lucene index.
        /// </summary>        
        public void Empty()
        {
            var writer = GetIndexWriter();
            writer.DeleteAll();
            CloseIndexWriter(writer);
        }
    }
}
