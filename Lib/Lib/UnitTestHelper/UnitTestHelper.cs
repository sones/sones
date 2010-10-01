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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using sones.Lib.ErrorHandling;

namespace sones.Lib
{

    public class UnitTestHelper
    {

        #region GetPrivateField

        [Obsolete("Use GetPrivateField(String myField, Object myObjectInstance)")]
        public static object GetPrivateField(Type myType, String myField, Object myObjectInstance)
        {
            return GetPrivateField(myField, myObjectInstance);
        }
        
        public static object GetPrivateField(String myField, Object myObjectInstance)
        {

            var _BindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var FieldInfo = myObjectInstance.GetType().GetField(myField, _BindingFlags);

            if (FieldInfo != null)
                return FieldInfo.GetValue(myObjectInstance);

            throw new ArgumentException(myField + " could not be found in " + myObjectInstance.GetType());

        }

        public static T GetPrivateField<T>(String myField, Object myObjectInstance)
        {

            return (T)GetPrivateField(myField, myObjectInstance);

        }

        public static object GetPrivateFieldFromBaseType(String myField, Object myObjectInstance)
        {

            var _BindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var FieldInfo = myObjectInstance.GetType().BaseType.GetField(myField, _BindingFlags);

            if (FieldInfo != null)
                return FieldInfo.GetValue(myObjectInstance);

            return null;

        }

        public static T GetPrivateFieldFromBaseType<T>(String myField, Object myObjectInstance)
        {
            return (T)GetPrivateFieldFromBaseType(myField, myObjectInstance);
        }

        #endregion

        public static void SetPrivateField(String myField, Object myObjectInstance, Object myValue)
        {

            var _BindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var FieldInfo = myObjectInstance.GetType().GetField(myField, _BindingFlags);

            if (FieldInfo == null && myObjectInstance.GetType().BaseType != null)
                FieldInfo = myObjectInstance.GetType().BaseType.GetField(myField, _BindingFlags);

            if (FieldInfo == null && myObjectInstance.GetType().BaseType != null && myObjectInstance.GetType().BaseType.BaseType != null)
                FieldInfo = myObjectInstance.GetType().BaseType.BaseType.BaseType.GetField(myField, _BindingFlags);

            if (FieldInfo != null)
                FieldInfo.SetValue(myObjectInstance, myValue);
            else
                throw new ArgumentException(myField + " could not be found in " + myObjectInstance.GetType());

        }

        public static void SetPrivateProperty(String myProperty, Object myObjectInstance, Object myValue)
        {

//            var _BindingFlags = BindingFlags. .Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var _PropertyInfo = myObjectInstance.GetType().GetProperty(myProperty);

            _PropertyInfo.SetValue(myObjectInstance, myValue, new Object[0]);

            //if (_PropertyInfo == null && myObjectInstance.GetType().BaseType != null)
            //    _PropertyInfo = myObjectInstance.GetType().BaseType.GetField(myProperty);

            //if (_PropertyInfo == null && myObjectInstance.GetType().BaseType != null && myObjectInstance.GetType().BaseType.BaseType != null)
            //    _PropertyInfo = myObjectInstance.GetType().BaseType.BaseType.BaseType.GetField(myProperty);

            //if (_PropertyInfo != null)
            //    _PropertyInfo.SetValue(myObjectInstance, myValue);
            //else
            //    throw new ArgumentException(myProperty + " could not be found in " + myObjectInstance.GetType());

        }

        #region InvokePrivateMethod

        public static object InvokePrivateMethod(String myMethod, Object myObjectInstance, params Object[] myParameters)
        {

            var _BindingFlags   = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var _MethodInfo     = myObjectInstance.GetType().GetMethod(myMethod, _BindingFlags);

            if (_MethodInfo != null)
                return _MethodInfo.Invoke(myObjectInstance, myParameters);

            throw new ArgumentException(myMethod + " could not be found in " + myObjectInstance.GetType());

        }

        public static T InvokePrivateMethod_Generic<T>(String myMethod, Object myObjectInstance, params Object[] myParameters)
        {

            var _BindingFlags       = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var _MethodInfo         = myObjectInstance.GetType().GetMethod(myMethod, _BindingFlags);
            var _GenericMethodInfo  = _MethodInfo.MakeGenericMethod(typeof(T));

            if (_GenericMethodInfo != null)
                return (T) _GenericMethodInfo.Invoke(myObjectInstance, myParameters);

            throw new ArgumentException(myMethod + " could not be found in " + myObjectInstance.GetType());

        }

        public static object InvokePrivateMethodFromBaseType(String myMethod, Object myObjectInstance, params Object[] myParameters)
        {

            var _BindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var MethodInfo = myObjectInstance.GetType().BaseType.GetMethod(myMethod, _BindingFlags);

            if (MethodInfo != null)
                return MethodInfo.Invoke(myObjectInstance, myParameters);

            return null;

        }

        #endregion

        public static Boolean IronyContainsError(List<IError> myErrorList, IError myGraphDBError)
        {
            return myErrorList.Any(err => err.GetType() == myGraphDBError.GetType());
        }
    }
}
