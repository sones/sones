using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.ErrorHandling;

namespace sones.GraphQL.GQL.ErrorHandling
{
    public sealed class ImportFormatDoesNotExistsException : AGraphQLException
    {
        #region data

        public String ImportFormat{ get; private set; }

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new ImportFormatDoesNotExistsException exception
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, this parameter can be NULL.</param>
        public ImportFormatDoesNotExistsException(String myImportFormat, Exception myInnerException = null)
            : base(myInnerException)
        {
            ImportFormat = myImportFormat;

            if (InnerException != null)
            {
                if (InnerException.Message != null && !InnerException.Message.Equals(""))
                    _msg = String.Format("Error during loading the aggregate plugin of type: [{0}]\n\nInner Exception: {1}", ImportFormat, InnerException.Message);
                else
                    _msg = String.Format("Error during loading the aggregate plugin of type: [{0}]", ImportFormat);
            }
        }

        #endregion
    }
}
