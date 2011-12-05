using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net;
using Lucene.Net.Documents;

namespace sones.Plugins.Index.LuceneIdx
{
    public class LuceneEntry
    {
        
		#region Properties
		 
		/// <summary>
		/// Gets or sets the vertex id.
		/// </summary>
		public long VertexId
		{
			get;
			private set;
		}
 
		/// <summary>
		/// Gets or sets the text stored within the index.
		/// </summary>
		public string Text
		{
			get;
			private set;
		}	
 
		/// <summary>
		/// Gets or sets the id of the entry within the index.
		/// </summary>
		public string Id
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets or sets the index id.
		/// </summary>
		public string IndexId
		{
			get;
			private set;
		}

        /// <summary>
        /// Gets or sets the property id.
        /// </summary>
        public long? PropertyId
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the highlights.
        /// </summary>
        public string[] Highlights
        {
            get;
            set;
        }

		#endregion

        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        public double? Score 
        { 
            get; 
            private set; 
        }

        /// <summary>
        /// Gets or sets the document number.
        /// </summary>
        public int? DocNum
        {
            get;
            private set;
        } 

		#region Constructors

		/// <summary>
		/// The default constructor
		/// </summary>
		public LuceneEntry()
		{
 
		}

		/// <summary>
		/// Initializes a new instance of the SolrEntry class using the specified values.
		/// </summary>
		/// <param name="myIndexId">The index id.</param>
		/// <param name="myVertexId">The vertex id.</param>
		/// <param name="myText">The text.</param>
        public LuceneEntry(String myIndexId, long myVertexId, String myText, long? myPropertyId = null, float? myScore = null, int? myDocNum = null)
        {
			this.IndexId = myIndexId;
            this.VertexId = myVertexId;
			this.Text = myText;
            this.PropertyId = myPropertyId;
            this.Score = myScore;
            this.DocNum = myDocNum;

            if (myPropertyId != null)
            {
                this.Id = string.Format("{0}_{1}_{2}", this.IndexId ?? "", this.VertexId, this.PropertyId);
            }
            else
            {
                this.Id = string.Format("{0}_{1}", this.IndexId ?? "", this.VertexId);
            }
        }

        internal LuceneEntry(Document myLuceneDocument, float? myScore = null, int? myDocNum = null)
        {
            this.Id = myLuceneDocument.GetField(LuceneIndex.FieldNames[LuceneIndex.Fields.ID]).StringValue();
            this.IndexId = myLuceneDocument.GetField(LuceneIndex.FieldNames[LuceneIndex.Fields.INDEX_ID]).StringValue();
            this.VertexId = Convert.ToInt64(myLuceneDocument.GetField(LuceneIndex.FieldNames[LuceneIndex.Fields.VERTEX_ID]).StringValue());
            this.Text = myLuceneDocument.GetField(LuceneIndex.FieldNames[LuceneIndex.Fields.TEXT]).StringValue();
            this.Score = myScore;
            this.DocNum = myDocNum;
            
            var propertyIdField = myLuceneDocument.GetField(LuceneIndex.FieldNames[LuceneIndex.Fields.PROPERTY_ID]);
            if (propertyIdField != null)
            {
                this.PropertyId = Convert.ToInt64(propertyIdField.StringValue());
            }
        }

		#endregion

		#region Methods

		/// <summary>
		/// Returns a string that represents the current SolrEntry.
		/// </summary>
		/// 
		/// <returns>A string representing the current SolrEntry.</returns>
		public override string ToString()
		{
			return string.Format("{0}{1}:{2},{3}{4}" , '{', this.IndexId, this.VertexId, this.Text ?? "", '}');
		}

		#endregion
    }
}
