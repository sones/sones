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
 * ExceptionalExtensions
 * Achim Friedland, 2009-2010
 */

#region Usings

using System;
using System.Linq;

#endregion

namespace sones.Lib.ErrorHandling
{

    public static class ExceptionalExtensions
    {

        #region When failed...

        public static Exceptional WhenFailed(this Exceptional myExceptional, Func<Exceptional, Exceptional> myFunc)
        {

            if (myExceptional == null || myExceptional.Failed)
                if (myFunc != null)
                    return myFunc(myExceptional);

            return myExceptional;

        }

        public static Exceptional<T> WhenFailed<T>(this Exceptional<T> myExceptional, Func<Exceptional<T>, Exceptional<T>> myFunc)
        {

            if (myExceptional == null || myExceptional.Failed || myExceptional.Value == null)
                if (myFunc != null)
                    return myFunc(myExceptional);

            return myExceptional;

        }

        public static Exceptional FailedAction(this Exceptional myExceptional, Action<Exceptional> myAction)
        {

            if (myExceptional == null || myExceptional.Failed)
                if (myAction != null)
                    myAction(myExceptional);

            return myExceptional;

        }

        public static Exceptional<T> FailedAction<T>(this Exceptional<T> myExceptional, Action<Exceptional<T>> myAction)
        {

            if (myExceptional == null || myExceptional.Failed || myExceptional.Value == null)
                if (myAction != null)
                    myAction(myExceptional);

            return myExceptional;

        }

        #endregion

        #region When succeded...

        public static Exceptional WhenSucceded(this Exceptional myExceptional, Func<Exceptional, Exceptional> myFunc)
        {

            if (myExceptional != null && myExceptional.Success)
                if (myFunc != null)
                    return myFunc(myExceptional);

            return myExceptional;

        }

        public static Exceptional<T> WhenSucceded<T>(this Exceptional<T> myExceptional, Func<Exceptional<T>, Exceptional<T>> myFunc)
        {

            if (myExceptional != null && myExceptional.Success && myExceptional.Value != null)
                if (myFunc != null)
                    return myFunc(myExceptional);

            return myExceptional;

        }

        public static Exceptional SuccessAction(this Exceptional myExceptional, Action<Exceptional> myAction)
        {

            if (myExceptional != null && myExceptional.Success)
                if (myAction != null)
                    myAction(myExceptional);

            return myExceptional;

        }

        public static Exceptional<T> SuccessAction<T>(this Exceptional<T> myExceptional, Action<Exceptional<T>> myAction)
        {

            if (myExceptional != null && myExceptional.Success && myExceptional.Value != null)
                if (myAction != null)
                    myAction(myExceptional);

            return myExceptional;

        }

        #endregion


        #region Convert an Exceptional<TIn> to an Exceptional<TOut>

        public static Exceptional<TOut> Convert<TOut>(this Exceptional myExceptional)
        {

            if (myExceptional != null)
                return new Exceptional<TOut>(myExceptional);

            return new Exceptional<TOut>(new ExceptionalConversionError(typeof(TOut).ToString()));

        }

        public static Exceptional<TOut> ConvertWithFunc<TIn, TOut>(this Exceptional<TIn> myExceptional, Func<TIn, TOut> myFunc)
        {

            if (myExceptional != null)
            {

                // Create Exceptional<TOut> and copy IErrors and IWarnings
                var _Exceptional = new Exceptional<TOut>(myExceptional);

                // If Success => Generate new Exceptional.Value!
                if (myExceptional != null && !myExceptional.Failed && myExceptional.Value != null)
                    _Exceptional.Value = myFunc(myExceptional.Value);

                return _Exceptional;

            }

            return new Exceptional<TOut>(new ExceptionalConversionError(typeof(TIn).ToString(), typeof(TOut).ToString()));

        }

        #endregion


        public static T ReturnValue<T>(this Exceptional<T> myExceptional)
        {

            // If Success => return value!
            if (myExceptional != null && myExceptional.Success && myExceptional.Value != null)
                return myExceptional.Value;

            return default(T);

        }

        /// <summary>
        /// This will always return the value. <paramref name="myAction"/> can be used to break execution via exception.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="myExceptional"></param>
        /// <param name="myAction"></param>
        /// <returns></returns>
        public static T ReturnValue<T>(this Exceptional<T> myExceptional, Action<Exceptional> myAction)
        {

            if (myAction != null)
            {
                myAction(myExceptional);
            }

            return myExceptional.Value;

        }

        public static T ReturnValue<T>(this Exceptional<T> myExceptional, T valueIfFailed)
        {

            // If Success => return value!
            if (myExceptional != null && myExceptional.Success && myExceptional.Value != null)
                return myExceptional.Value;

            return valueIfFailed;

        }

        public static TOut TransformValue<TIn, TOut>(this Exceptional<TIn> myExceptional, Func<TIn, TOut> myFunc)
        {

            // If Success => Transform value!
            if (myExceptional != null && myExceptional.Success && myExceptional.Value != null)
                return myFunc(myExceptional.Value);

            return default(TOut);

        }


        #region CheckArgumentsNotNull - with error message!

        public static Exceptional NotNullMsg(this Exceptional myExceptional, String myMessage, params Object[] myObjects)
        {

            if (myMessage.IsNullOrEmpty())
                myMessage = "Argument is null!";

            if (myObjects.Any(o => o == null))
                myExceptional.Push(new ExceptionalError(myMessage));

            return myExceptional;

        }

        public static Exceptional<T> NotNullMsg<T>(this Exceptional<T> myExceptional, String myMessage, params Object[] myObjects)
        {

            if (myMessage.IsNullOrEmpty())
                myMessage = "Argument is null!";

            if (myObjects.Any(o => o == null))
                myExceptional.Push(new ExceptionalError(myMessage));

            return myExceptional;

        }

        public static Exceptional ArgumentsNotNullMsg(String myMessage, params Object[] myObjects)
        {
            return new Exceptional().NotNullMsg(myMessage, myObjects);
        }

        public static Exceptional<T> ArgumentsNotNullMsg<T>(String myMessage, params Object[] myObjects)
        {
            return new Exceptional<T>().NotNullMsg<T>(myMessage, myObjects);
        }

        #endregion

        #region CheckArgumentsNotNull - without error message!

        public static Exceptional NotNull(this Exceptional myExceptional, params Object[] myObjects)
        {

            if (myObjects.Any(o => o == null))
                myExceptional.Push(new ExceptionalError("Argument is null!"));

            return myExceptional;

        }

        public static Exceptional<T> NotNull<T>(this Exceptional<T> myExceptional, params Object[] myObjects)
        {

            if (myObjects.Any(o => o == null))
                myExceptional.Push(new ExceptionalError("Argument is null!"));

            return myExceptional;

        }

        public static Exceptional ArgumentsNotNull(params Object[] myObjects)
        {
            return new Exceptional().NotNullMsg(null, myObjects);
        }

        public static Exceptional<T> ArgumentsNotNull<T>(params Object[] myObjects)
        {
            return new Exceptional<T>().NotNullMsg<T>(null, myObjects);
        }

        #endregion


        #region CheckStringNotNullOrEmpty - without error message

        public static Exceptional NotNullOrEmpty(this Exceptional myExceptional, params String[] myStrings)
        {

            if (myStrings.Any(o => o == null || o == ""))
                myExceptional.Push(new ExceptionalError("String is null or empty!"));

            return myExceptional;

        }

        public static Exceptional<T> NotNullOrEmpty<T>(this Exceptional<T> myExceptional, params String[] myStrings)
        {

            if (myStrings.Any(o => o == null || o == ""))
                myExceptional.Push(new ExceptionalError("String is null or empty!"));

            return myExceptional;
        
        }

        public static Exceptional StringsNotNullOrEmpty(params String[] myStrings)
        {
            return new Exceptional().NotNullOrEmptyMsg(null, myStrings);
        }

        public static Exceptional<T> StringsNotNullOrEmpty<T>(params String[] myStrings)
        {
            return new Exceptional<T>().NotNullOrEmptyMsg<T>(null, myStrings);
        }

        #endregion

        #region CheckStringNotNullOrEmpty - with error message

        public static Exceptional NotNullOrEmptyMsg(this Exceptional myExceptional, String myMessage, params String[] myStrings)
        {

            if (myMessage.IsNullOrEmpty())
                myMessage = "String is null or empty!";

            if (myStrings.Any(o => o == null || o == ""))
                myExceptional.Push(new ExceptionalError(myMessage));

            return myExceptional;

        }

        public static Exceptional<T> NotNullOrEmptyMsg<T>(this Exceptional<T> myExceptional, String myMessage, params String[] myStrings)
        {

            if (myMessage.IsNullOrEmpty())
                myMessage = "String is null or empty!";

            if (myStrings.Any(o => o == null || o == ""))
                myExceptional.Push(new ExceptionalError(myMessage));

            return myExceptional;

        }

        public static Exceptional StringsNotNullOrEmptyMsg(String myMessage, params String[] myStrings)
        {
            return new Exceptional().NotNullOrEmptyMsg(myMessage, myStrings);
        }

        public static Exceptional<T> StringsNotNullOrEmptyMsg<T>(String myMessage, params String[] myStrings)
        {
            return new Exceptional<T>().NotNullOrEmptyMsg<T>(myMessage, myStrings);
        }

        #endregion

    }

}

