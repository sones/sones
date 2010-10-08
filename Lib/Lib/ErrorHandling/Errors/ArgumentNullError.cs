/*
 * ArgumentNullError
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
    public class ArgumentNullError : GeneralError
    {

        #region Properties

        public String Parameter { get; private set; }

        #endregion

        #region Constructor(s)

        #region ArgumentNullError()

        public ArgumentNullError()
        {
            Parameter = default(String);
            Message   = String.Format("Parameter must not be null!");
        }

        #endregion

        #region ArgumentNullError(myParameter)

        public ArgumentNullError(String myParameter)
        {
            Parameter = myParameter;
            Message = String.Format("Parameter '{0}' must not be null!", myParameter);
        }

        #endregion

        #endregion

    }

}
