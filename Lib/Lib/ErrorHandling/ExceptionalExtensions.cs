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
 * ExceptionalExtensions
 * (c) Achim Friedland, 2009 - 2010
 */

#region Usings

using System;
using System.Linq;

#endregion

namespace sones.Lib.ErrorHandling
{

    public static class ExceptionalExtensions
    {


        #region Succeess(myExceptional)

        /// <summary>
        /// Is true, if there were no errors and warnings
        /// </summary>
        public static Boolean Success(this Exceptional myExceptional)
        {
            return !myExceptional.Failed() && (!myExceptional.IWarnings.Any());
        }

        #endregion

        #region Failed(myExceptional)

        /// <summary>
        /// Is true, if there were at least one error
        /// </summary>
        public static Boolean Failed(this Exceptional myExceptional)
        {

            if (myExceptional == null || myExceptional.IErrors.Any())
                return true;

            return false;

        }

        #endregion


        #region IsValid<T>(this myExceptional)

        /// <summary>
        /// Is true, if the Exceptional&lt;T&gt; and the value of T are not
        /// null and there had been no errors.
        /// </summary>
        public static Boolean IsValid<T>(this Exceptional<T> myExceptional)
        {

            if (myExceptional != null && !myExceptional.IErrors.Any() && myExceptional.Value != null)
                return true;

            return false;

        }

        #endregion

        #region IsInvalid<T>(this myExceptional)

        /// <summary>
        /// Is true, if the Exceptional&lt;T&gt; or the value of T are null
        /// or there had been at least one error.
        /// </summary>
        public static Boolean IsInvalid<T>(this Exceptional<T> myExceptional)
        {

            if (myExceptional == null || myExceptional.IErrors.Any() || myExceptional.Value == null)
                return true;

            return false;

        }

        #endregion


        #region When failed...

        public static Exceptional WhenFailed(this Exceptional myExceptional, Func<Exceptional, Exceptional> myFunc)
        {

            if (myExceptional.Failed() && myFunc != null)
                return myFunc(myExceptional);

            return myExceptional;

        }

        public static Exceptional<T> WhenFailed<T>(this Exceptional<T> myExceptional, Func<Exceptional<T>, Exceptional<T>> myFunc)
        {

            if (myExceptional.IsInvalid() && myFunc != null)
                return myFunc(myExceptional);

            return myExceptional;

        }

        public static Exceptional FailedAction(this Exceptional myExceptional, Action<Exceptional> myAction)
        {

            if (myExceptional.Failed() && myAction != null)
                myAction(myExceptional);

            return myExceptional;

        }

        public static Exceptional<T> FailedAction<T>(this Exceptional<T> myExceptional, Action<Exceptional<T>> myAction)
        {

            if (myExceptional.IsInvalid() && myAction != null)
                myAction(myExceptional);

            return myExceptional;

        }

        #endregion

        #region When succeded...

        public static Exceptional WhenSucceded(this Exceptional myExceptional, Func<Exceptional, Exceptional> myFunc)
        {

            if (myExceptional.Success() && myFunc != null)
                return myFunc(myExceptional);

            return myExceptional;

        }

        public static Exceptional<T> WhenSucceded<T>(this Exceptional<T> myExceptional, Func<Exceptional<T>, Exceptional<T>> myFunc)
        {

            if (myExceptional.IsValid() && myFunc != null)
                return myFunc(myExceptional);

            return myExceptional;

        }

        public static Exceptional SuccessAction(this Exceptional myExceptional, Action<Exceptional> myAction)
        {

            if (myExceptional.Success() && myAction != null)
                myAction(myExceptional);

            return myExceptional;

        }

        public static Exceptional<T> SuccessAction<T>(this Exceptional<T> myExceptional, Action<Exceptional<T>> myAction)
        {

            if (myExceptional.IsValid() && myAction != null)
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
                if (myExceptional.Success() && myExceptional.Value != null)
                    _Exceptional.Value = myFunc(myExceptional.Value);

                return _Exceptional;

            }

            return new Exceptional<TOut>(new ExceptionalConversionError(typeof(TIn).ToString(), typeof(TOut).ToString()));

        }

        #endregion


        #region ValueOrDefault<T>(this myExceptional)

        /// <summary>
        /// Will return the value of the Exceptional&lt;T&gt; or default(T)
        /// if there had been at least one error.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="myExceptional"></param>
        public static T ValueOrDefault<T>(this Exceptional<T> myExceptional)
        {

            // If Success => return value!
            if (myExceptional.IsValid())
                return myExceptional.Value;

            return default(T);

        }

        #endregion

        #region ValueOrAlternative<T>(this myExceptional, myAlternative)

        /// <summary>
        /// Will return the value of the Exceptional&lt;T&gt; or the given
        /// alternative if there had been at least one error.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="myExceptional"></param>
        /// <param name="myAlternative"></param>
        public static T ValueOrAlternative<T>(this Exceptional<T> myExceptional, T myAlternative)
        {

            // If Success => return value!
            if (myExceptional.IsValid())
                return myExceptional.Value;

            return myAlternative;

        }

        #endregion


        #region TransformValue<TIn, TOut>(this myExceptional, myTransformation)

        /// <summary>
        /// Will return the transformed value of the Exceptional&lt;T&gt; or
        /// default(T) if there had been at least one error.
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="myExceptional"></param>
        /// <param name="myTransformation"></param>
        /// <returns></returns>
        public static TOut TransformValue<TIn, TOut>(this Exceptional<TIn> myExceptional, Func<TIn, TOut> myTransformation)
        {

            // If Success => Transform value!
            if (myExceptional.IsValid())
                return myTransformation(myExceptional.Value);

            return default(TOut);

        }

        #endregion

        #region TransformValue<TIn, TOut>(this myExceptional, myTransformation, myAlternative)

        /// <summary>
        /// Will return the transformed value of the Exceptional&lt;T&gt; or
        /// the given alternative if there had been at least one error.
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="myExceptional"></param>
        /// <param name="myTransformation"></param>
        /// <param name="myAlternative"></param>
        /// <returns></returns>
        public static TOut TransformValue<TIn, TOut>(this Exceptional<TIn> myExceptional, Func<TIn, TOut> myTransformation, TOut myAlternative)
        {

            // If Success => Transform value!
            if (myExceptional.IsValid())
                return myTransformation(myExceptional.Value);

            return myAlternative;

        }

        #endregion


        #region CheckArgumentsNotNull - with error message!

        public static Exceptional NotNullMsg(this Exceptional myExceptional, String myParameter, params Object[] myObjects)
        {

            if (myObjects.Any(o => o == null))
                myExceptional.PushIError(new ArgumentNullError(myParameter));

            return myExceptional;

        }

        public static Exceptional<T> NotNullMsg<T>(this Exceptional<T> myExceptional, String myParameter, params Object[] myObjects)
        {

            if (myObjects.Any(o => o == null))
                myExceptional.PushIError(new ArgumentNullError(myParameter));

            return myExceptional;

        }

        public static Exceptional ArgumentsNotNullMsg(String myParameter, params Object[] myObjects)
        {
            return new Exceptional().NotNullMsg(myParameter, myObjects);
        }

        public static Exceptional<T> ArgumentsNotNullMsg<T>(String myParameter, params Object[] myObjects)
        {
            return new Exceptional<T>().NotNullMsg<T>(myParameter, myObjects);
        }

        #endregion

        #region CheckArgumentsNotNull - without error message!

        public static Exceptional NotNull(this Exceptional myExceptional, params Object[] myObjects)
        {

            if (myObjects.Any(o => o == null))
                myExceptional.PushIError(new ArgumentNullError());

            return myExceptional;

        }

        public static Exceptional<T> NotNull<T>(this Exceptional<T> myExceptional, params Object[] myObjects)
        {

            if (myObjects.Any(o => o == null))
                myExceptional.PushIError(new ArgumentNullError());

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
                myExceptional.PushIError(new ArgumentNullError("String"));

            return myExceptional;

        }

        public static Exceptional<T> NotNullOrEmpty<T>(this Exceptional<T> myExceptional, params String[] myStrings)
        {

            if (myStrings.Any(o => o == null || o == ""))
                myExceptional.PushIError(new ArgumentNullError("String"));

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

        public static Exceptional NotNullOrEmptyMsg(this Exceptional myExceptional, String myParameter, params String[] myStrings)
        {

            if (myParameter.IsNullOrEmpty())
                myParameter = "String";

            if (myStrings.Any(o => o == null || o == ""))
                myExceptional.PushIError(new ArgumentNullOrEmptyError(myParameter));

            return myExceptional;

        }

        public static Exceptional<T> NotNullOrEmptyMsg<T>(this Exceptional<T> myExceptional, String myParameter, params String[] myStrings)
        {

            if (myParameter.IsNullOrEmpty())
                myParameter = "String";

            if (myStrings.Any(o => o == null || o == ""))
                myExceptional.PushIError(new ArgumentNullOrEmptyError(myParameter));

            return myExceptional;

        }

        public static Exceptional StringsNotNullOrEmptyMsg(String myParameter, params String[] myStrings)
        {
            return new Exceptional().NotNullOrEmptyMsg(myParameter, myStrings);
        }

        public static Exceptional<T> StringsNotNullOrEmptyMsg<T>(String myParameter, params String[] myStrings)
        {
            return new Exceptional<T>().NotNullOrEmptyMsg<T>(myParameter, myStrings);
        }

        #endregion


        ///// <summary>
        ///// This will always return the value. <paramref name="myAction"/> can be used to break execution via exception.
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="myExceptional"></param>
        ///// <param name="myAction"></param>
        ///// <returns></returns>
        //public static T ReturnValue<T>(this Exceptional<T> myExceptional, Action<Exceptional> myAction)
        //{

        //    if (myAction != null)
        //        myAction(myExceptional);

        //    return myExceptional.Value;

        //}

    }

}

