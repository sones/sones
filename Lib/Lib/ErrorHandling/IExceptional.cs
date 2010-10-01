/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
* 
*/

/*
 * IExceptional/IExceptional<TValue>
 * (c) Achim Friedland, 2010
 * 
 * <summary>
 *  Added Exceptional&lt;TValue/gt;, which may encode any value of type TValue and an exception
 *  that might have been thrown while determining the actual value of TValue.
 *  For more information on this idea please watch the following MSDN Channel 9 video:
 *  http://channel9.msdn.com/shows/Going+Deep/E2E-Erik-Meijer-and-Burton-Smith-Concurrency-Parallelism-and-Programming/
 * </summary>
 */

#region Usings

using System;
using System.Text;
using System.Collections.Generic;

#endregion

namespace sones.Lib.ErrorHandling
{

    /// <summary>
    /// The normal/non-generic IExceptional interface holding a list of IWarnings and IErrors
    /// </summary>
    public interface IExceptional
    {

        // Properties
        IEnumerable<IWarning> IWarnings { get; }
        IEnumerable<IError>   IErrors   { get; }


        // Push(IWarning(s)/IError(s)/IExceptional)
        Exceptional PushIWarning     (IWarning myWarning);
        Exceptional PushIWarnings    (IEnumerable<IWarning> myWarnings);

        Exceptional PushIError       (IError myError);
        Exceptional PushIErrors      (IEnumerable<IError> myIErrors);

        Exceptional PushIExceptional (IExceptional myIExceptional);


        // ...ToString()
        String GetIWarningsAsString();
        String GetIErrorsAsString();
        String ToString();

    }

    /// <summary>
    /// The generic IExceptional interface holding a list of IWarnings,
    /// IErrors and an encapsulated value.
    /// </summary>
    /// <typeparam name="TValue">The type of the encapsulated value</typeparam>
    public interface IExceptional<TValue> : IExceptional
    {

        // The encapsulated value
        TValue Value { get; set; }


        // PushT(IWarning(s)/IError(s)/IExceptional)
        Exceptional<TValue> PushIWarningT     (IWarning myWarning);
        Exceptional<TValue> PushIWarningsT    (IEnumerable<IWarning> myWarnings);

        Exceptional<TValue> PushIErrorT       (IError myError);
        Exceptional<TValue> PushIErrorsT      (IEnumerable<IError> myIErrors);

        Exceptional<TValue> PushIExceptionalT (IExceptional myIExceptional);


        // Additional Helpers
        Boolean Equals(Object myObject);
        Int32   GetHashCode();

    }

}
