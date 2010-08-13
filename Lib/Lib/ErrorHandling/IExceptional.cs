/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
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
*/


/*
 * IExceptional/IExceptional<TValue>
 * Achim Friedland, 2010
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
    /// The normal/non-generic IExceptional interface holding a list of IErrors
    /// </summary>
    public interface IExceptional
    {

        // Properties
        //Boolean             Success { get; }
        //Boolean             Failed  { get; }
        IEnumerable<IError> Errors  { get; }

        // Methods
        Exceptional Push(IError myError);
        Exceptional Push(IWarning myWarning);

        // Helpers
        String  ToString();

    }

    /// <summary>
    /// The generic IExceptional interface holding a list of IErrors and a value
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public interface IExceptional<TValue> : IExceptional
    {

        // The inner value
        TValue Value { get; set; }

        // Methods
        Exceptional<TValue> PushT(IError myError);
        Exceptional<TValue> PushT(IWarning myWarning);

        // Additional Helpers
        Boolean Equals(Object myObject);
        Int32   GetHashCode();

    }

}
