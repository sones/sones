using System;

namespace sones.Plugins.SonesGQL.Function.ErrorHandling
{
    /// <summary>
    /// This class represents an unknown function exception
    /// </summary>
    public sealed class UnknownFunctionException : ASonesQLFunctionException
    {
        /// <summary>
        /// The exception that has been thrown
        /// </summary>
        public Exception ThrownException { get; private set; }
                
        #region constructor

        /// <summary>
        /// Creates a new UnknownFunctionException exception
        /// </summary>
        /// <param name="e"></param>
        public UnknownFunctionException(Exception e)
        {
            ThrownException = e;
            _msg = "An unknown function error has occurred.";
        }

        #endregion
    }
}
