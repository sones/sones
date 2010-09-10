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


#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Reflection;

#endregion

namespace sones.Lib
{

    /// <summary>
    /// Usage:
    ///  dynamic wrapper = DynamicUnitTestHelper.FromPrivateConstructor(typeof(SomeKnownClass).Assembly, "ClassWithPrivateConstructor");
    ///  dynamic wrapper = new DynamicUnitTestHelper(_IGraphFSSession);
    ///  wrapper.somePrivateField    = "Field Val";
    ///  wrapper.SomePrivateProperty = "Property Val";
    ///  wrapper.DumpValues();
    /// </summary>  
    public class DynamicUnitTestHelper : DynamicObject
    {

        #region Data

        /// <summary>  
        /// The object we are going to wrap  
        /// </summary>  
        Object _WrappedObject;

        /// <summary>  
        /// Specify the flags for accessing members  
        /// </summary>  
        static BindingFlags _BindingFlags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;

        #endregion

        #region Constructor(s)

        public DynamicUnitTestHelper(Object myWrappedObject)
        {
            _WrappedObject = myWrappedObject;
        }

        #endregion

        #region Create(myAssembly, myType, params myArguments)

        /// <summary>  
        /// Create an instance via the constructor matching the args   
        /// </summary>  
        public static dynamic FromPrivateConstructor(Assembly myAssembly, String myType, params Object[] myConstructorArguments)
        {

            var _AllTypesWithinAssembly = myAssembly.GetTypes();
            var _Type  = _AllTypesWithinAssembly.First(item => item.Name == myType);

            var _Types = from _Argument in myConstructorArguments select _Argument.GetType();

            // Gets the constructor matching the specified set of args  
            var _Constructor = _Type.GetConstructor(_BindingFlags, null, _Types.ToArray(), null);

            if (_Constructor != null)
            {
                var _Instance = _Constructor.Invoke(myConstructorArguments);
                return new DynamicUnitTestHelper(_Instance);
            }

            return null;
        
        }

        #endregion


        #region TryInvokeMember(myBinder, myArguments, out myResult)

        /// <summary>  
        /// Try invoking a private method  
        /// </summary>  
        public override Boolean TryInvokeMember(InvokeMemberBinder myBinder, Object[] myArguments, out Object myResult)
        {

            var _Types = from _Argument in myArguments select _Argument.GetType();

            var _Method = _WrappedObject.GetType().GetMethod
                (myBinder.Name, _BindingFlags, null, _Types.ToArray(), null);

            if (_Method == null)
            {
                myResult = null;
                return false;
                //return base.TryInvokeMember(myBinder, myArguments, out myResult);
            }

            else
            {
                myResult = _Method.Invoke(_WrappedObject, myArguments);
                return true;
            }

        }

        #endregion

        #region TryGetMember(myBinder, out myResult)

        /// <summary>  
        /// Tries to get a private property or private field with the given name  
        /// </summary>  
        public override Boolean TryGetMember(GetMemberBinder myBinder, out Object myResult)
        {

            // Try getting a property of that name
            var _Property = _WrappedObject.GetType().GetProperty(myBinder.Name, _BindingFlags);

            if (_Property == null)
            {

                // Try getting a field of that name
                var _Field = _WrappedObject.GetType().GetField(myBinder.Name, _BindingFlags);
                if (_Field != null)
                {
                    myResult = _Field.GetValue(_WrappedObject);
                    return true;
                }
                
                else
                {
                    myResult = null;
                    return false;
                    //return base.TryGetMember(myBinder, out myResult);
                }

            }

            else
            {
                myResult = _Property.GetValue(_WrappedObject, null);
                return true;
            }

        }

        #endregion

        #region TrySetMember(myBinder, myValue)

        /// <summary>  
        /// Tries to set a private property or private field with the given name  
        /// </summary>  
        public override Boolean TrySetMember(SetMemberBinder myBinder, Object myValue)
        {

            // Try getting a property of that name
            var _Property = _WrappedObject.GetType().GetProperty(myBinder.Name, _BindingFlags);

            if (_Property == null)
            {
                // Try getting a field of that name
                var _Field = _WrappedObject.GetType().GetField(myBinder.Name, _BindingFlags);
                if (_Field != null)
                {
                    _Field.SetValue(_WrappedObject, myValue);
                    return true;
                }
                else
                    return false;
                    //return base.TrySetMember(myBinder, myValue);
            }

            else
            {
                _Property.SetValue(_WrappedObject, myValue, null);
                return true;
            }

        }

        #endregion

    }

}
