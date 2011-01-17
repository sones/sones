using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.Internal.Token;
using sones.GraphDB.Error;

namespace sones.GraphDB.Result
{
    /// <summary>
    /// The result header gives some generak information concerning the result
    /// </summary>
    public sealed class OutputHeader
    {
        #region Data

        public readonly RequestToken RequestToken;

        /// <summary>
        /// The conclusion of the result
        /// </summary>
        public ConclusionEnum Conclusion { get { return _errors.Count() == 0 ? ConclusionEnum.Success : ConclusionEnum.Fail; } }

        /// <summary>
        /// The errors of the result
        /// </summary>
        public List<IError> Errors { get { return _errors.ToList(); } }
        private IEnumerable<IError> _errors = new List<IError>();

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new ResultHeader
        /// </summary>
        /// <param name="myErrors"></param>
        public OutputHeader(RequestToken myRequestToken, IEnumerable<IError> myErrors = null)
        {
            RequestToken = myRequestToken;
            _errors = myErrors;
        }

        #endregion

        #region methods

        /// <summary>
        /// Returns a string which cummulates all the errors
        /// </summary>
        /// <returns>A string with cummulated errors</returns>
        public string GetErrorsAsString()
        {
            StringBuilder sb = new StringBuilder();

            _errors.ToList().ForEach(anIError => sb.AppendLine(anIError.ToString() + Environment.NewLine));

            return sb.ToString();
        }

        #endregion
    }
}
