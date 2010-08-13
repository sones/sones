/* <id name="GraphDB – Exceptions" />
 * <copyright file="GraphExceptions.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Daniel Kirstenpfad</developer>
 * <summary>This class carries the definitions of exceptions.</summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using sones.Lib;
using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphDB.Exceptions
{

    #region Generic Standard GraphDB Exception

    public class GraphDBException : ApplicationException
    {
        public IEnumerable<IError> GraphDBErrors { get; set; }

        public GraphDBException(IError myGraphDBError)
            : base(myGraphDBError.ToString())
        {
            GraphDBErrors = new List<IError>() { myGraphDBError };
        }

        public GraphDBException(IError myGraphDBError, Exception innerException)
            : base(myGraphDBError.ToString(), innerException)
        {
            GraphDBErrors = new List<IError>() { myGraphDBError };
        }

        public GraphDBException(IEnumerable<IError> myGraphDBErrors)
            : base(myGraphDBErrors.ToAggregatedString().ToString())
        {
            GraphDBErrors = myGraphDBErrors;
        }

    }

    #endregion

}
