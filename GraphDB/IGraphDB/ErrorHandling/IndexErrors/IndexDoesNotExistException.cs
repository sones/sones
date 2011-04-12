using System;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// The index does not exists
    /// </summary>
    public sealed class IndexDoesNotExistException : AGraphDBIndexException
    {
        #region data        

        public String IndexName { get; private set; }
        public String IndexEdition { get; private set; }

        #endregion

        #region constructor        

        /// <summary>
        /// Create a new IndexDoesNotExistException exception
        /// </summary>
        /// <param name="myIndexName"></param>
        /// <param name="myIndexEdition"></param>
        public IndexDoesNotExistException(String myIndexName, String myIndexEdition)
        {
            IndexName = myIndexName;
            IndexEdition = myIndexEdition;
        }

        #endregion

        public override string ToString()
        {
            if (!String.IsNullOrEmpty(IndexName) && !String.IsNullOrEmpty(IndexEdition))
                return String.Format("The index \"{0}\" with edition \"{1}\" does not exist!", IndexName, IndexEdition);
            if (!String.IsNullOrEmpty(IndexName))
            {
                return String.Format("The index \"{0}\" does not exist!", IndexName);
            }

            return String.Format("The indexedition \"{0}\" does not exist!", IndexEdition);
        }        
    }
}
