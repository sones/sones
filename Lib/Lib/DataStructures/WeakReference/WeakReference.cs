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

/* WeakReference<TValue>
 * (c) Achim Friedland, 2009
 * 
 * Lead programmer:
 *      Achim Friedland
 */

#region Usings

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

#endregion

namespace sones.Lib.DataStructures.WeakReference
{

    /// <summary>
    /// This class will maintain a type-save weak reference to the given object.
    /// </summary>
    /// <typeparam name="TValue">The type of a weakly referenced object.</typeparam>
    public class WeakReference<TValue> : IEquatable<WeakReference<TValue>>, IDisposable where TValue : class
    {

        #region Data

        protected GCHandle _GCHandle;
        protected Int32    _HashCode;

        #endregion

        #region Properties

        #region IsAlive

        public virtual Boolean IsAlive
        {            
            get
            {
                return (_GCHandle.Target != null);
            }
        }

        #endregion

        #region Value

        public virtual TValue Value
        {
            get
            {
                Object _Object = _GCHandle.Target;

                if ((_Object == null) || (!(_Object is TValue)))
                    return null;

                return (TValue) _Object;

            }
        }

        #endregion

        #endregion


        #region Constructors

        #region WeakReference(myValue)

        /// <summary>
        /// Initializes a weak reference to the given object.
        /// </summary>
        /// <param name="myValue">the weakly referenced object.</param>
        public WeakReference(TValue myValue)
        {

            if (myValue == null)
                throw new ArgumentNullException("target");

            _HashCode = myValue.GetHashCode();
            InitializeHandle(myValue, GCHandleType.Weak);

        }

        #endregion

        #region WeakReference(myValue, myGCHandleType)

        /// <summary>
        /// Initializes a (weak) reference to the given object.
        /// The kind of the reference can be controlled by the myGCHandleType parameter.
        /// </summary>
        /// <param name="myValue">the (weakly) referenced object.</param>
        public WeakReference(TValue myValue, GCHandleType myGCHandleType)
        {

            if (myValue == null)
                throw new ArgumentNullException("target");

            _HashCode = myValue.GetHashCode();
            InitializeHandle(myValue, myGCHandleType);

        }

        #endregion

        #region InitializeHandle(myValue, myGCHandleType)

        protected virtual void InitializeHandle(TValue myValue, GCHandleType myGCHandleType)
        {
            _GCHandle = GCHandle.Alloc(myValue, myGCHandleType);
        }

        #endregion

        #endregion

        #region Destructor()

        #region ~WeakReference()

        ~WeakReference()
        {
            Dispose();
        }

        #endregion

        #region Dispose()

        public void Dispose()
        {
            _GCHandle.Free();
            GC.SuppressFinalize(this);
        }

        #endregion

        #endregion


        #region Equals(myObject)

        public override Boolean Equals(Object myObject)
        {

            if (myObject is WeakReference<TValue>)
                return Equals((WeakReference<TValue>) myObject);

            return false;

        }

        #endregion

        #region Equals(WeakReference<TValue> myWeakReference)

        /// <summary>
        /// Returns true is both references are equal based on their reference
        /// </summary>
        /// <param name="myWeakReference"></param>
        /// <returns></returns>
        public Boolean Equals(WeakReference<TValue> myWeakReference)
        {
            return ReferenceEquals(myWeakReference.Value, this.Value);
        }

        #endregion

        #region Implicit conversation to/from TValue

        public static implicit operator WeakReference<TValue>(TValue myValue)
        {
            return new WeakReference<TValue>(myValue);
        }

        public static implicit operator TValue(WeakReference<TValue> myWeakReference)
        {
            return myWeakReference.Value;
        }

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Returns the stored HashCode of the value
        /// </summary>
        /// <returns></returns>
        public override Int32 GetHashCode()
        {
            return _HashCode;
        }

        #endregion


    }

}