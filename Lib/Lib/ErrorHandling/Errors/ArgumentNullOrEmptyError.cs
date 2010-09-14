/*
 * ArgumentNullOrEmptyError
 * (c) Achim Friedland, 2010
 */

#region Usings

using System;

#endregion

namespace sones.Lib.ErrorHandling
{

    /// <summary>
    /// Like a ArgumentNullException
    /// </summary>
    public class ArgumentNullOrEmptyError : GeneralError
    {

        #region Properties

        public String Parameter { get; private set; }

        #endregion

        #region Constructor(s)

        #region ArgumentNullOrEmptyError()

        public ArgumentNullOrEmptyError()
        {
            Parameter = default(String);
            Message   = String.Format("Parameter must not be null or empty!");
        }

        #endregion

        #region ArgumentNullOrEmptyError(myParameter)

        public ArgumentNullOrEmptyError(String myParameter)
        {
            Parameter = myParameter;
            Message = String.Format("Parameter '{0}' must not be null or empty!", myParameter);
        }

        #endregion

        #endregion

    }

}
