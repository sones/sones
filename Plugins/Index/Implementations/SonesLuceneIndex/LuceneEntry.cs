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
			set;
		}
 
		/// <summary>
		/// Gets or sets the text stored within the index.
		/// </summary>
		public string Text
		{
			get;
			set;
		}	
 
		/// <summary>
		/// Gets or sets the id of the entry within the index.
		/// </summary>
		public string Id
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the index id.
		/// </summary>
		public string IndexId
		{
			get;
			set;
		}

        /// <summary>
        /// Gets or sets the property id.
        /// </summary>
        public long? PropertyId
        {
            get;
            set;
        }

		#endregion

        /// <summary>
        /// Gets or sets the score. (Set only by Solr.Net)
        /// </summary>
        public double? Score 
        { 
            get; 
            set; 
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
        public LuceneEntry(String myIndexId, long myVertexId, String myText, long? myPropertyId = null)
        {
			this.IndexId = myIndexId;
            this.VertexId = myVertexId;
			this.Text = myText;
            this.PropertyId = myPropertyId;

            if (myPropertyId != null)
            {
                this.Id = string.Format("{0}#{1}#{2}", this.IndexId ?? "", this.VertexId, this.PropertyId);
            }
            else
            {
                this.Id = string.Format("{0}#{1}", this.IndexId ?? "", this.VertexId);
            }
        }

        public LuceneEntry(Document myLuceneDocument)
        {
            this.IndexId = myLuceneDocument.GetField("indexId").StringValue();
            this.VertexId = Convert.ToInt64(myLuceneDocument.GetField("vertexId").StringValue());
            this.Text = myLuceneDocument.GetField("text").StringValue();

            var propertyIdField = myLuceneDocument.GetField("propertyId");
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
