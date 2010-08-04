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
 * ObjectLocation
 * Achim Friedland, 2010
 */

#region Usings

using System;
using System.Text;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Diagnostics;

using sones.Lib;
using sones.Lib.Serializer;


using sones.Lib.XML;
using sones.Lib.NewFastSerializer;
using sones.Lib.DataStructures;

#endregion

namespace sones.GraphFS.DataStructures
{

    /// <summary>
    /// The ObjectLocation is consists of the ObjectPath and -Name
    /// </summary>

    //[Serializable]
    
    public class ObjectLocation : IComparable, IComparable<ObjectLocation>, IComparable<String>, IEquatable<ObjectLocation>, IEquatable<String>, IDisposable
    {


        #region Data

        [NonSerialized]
        protected String _ObjectLocation;

        protected List<String> _PathElements;

        #endregion

        #region Properties

        #region Path

        protected String _Path;

        /// <summary>
        /// Stores the path of this AFSObject.
        /// </summary>
        public ObjectLocation Path
        {
            get
            {
                return new ObjectLocation(_Path);
            }
        }

        #endregion

        #region Name

        /// <summary>
        /// Stores the name of this AFSObject.
        /// </summary>
        public String Name { get; protected set; }

        #endregion

        #region Length

        /// <summary>
        /// Returns the length of the ObjectLocation.
        /// </summary>
        public Int32 Length
        {
            get
            {
                return _ObjectLocation.Length;
            }
        }

        #endregion

        #region ULongLength

        /// <summary>
        /// Returns the length of the ObjectLocation.
        /// </summary>
        public UInt64 ULongLength
        {
            get
            {
                return (UInt64) _ObjectLocation.Length;
            }
        }

        #endregion

        #region OK

        private static readonly ObjectLocation _Root = new ObjectLocation();

        /// <summary>
        /// This returns the root of a file system object location
        /// </summary>
        public static ObjectLocation Root
        {
            get
            {
                return _Root;
            }
        }

        #endregion

        #endregion

        #region Constructors

        #region ObjectLocation()

        /// <summary>
        /// This will create an empty ObjectLocation
        /// </summary>
        public ObjectLocation()
            : this(FSPathConstants.PathDelimiter)
        {
        }

        #endregion

        #region ObjectLocation(myObjectLocation)

        /// <summary>
        /// This will create an ObjectLocation based on the information
        /// given within the array of strings.
        /// </summary>
        public ObjectLocation(params String[] myObjectLocation)
        {

            var _PathElements = new List<String>();

            foreach (var a in myObjectLocation)
                foreach (var b in a.Split(new String[] { FSPathConstants.PathDelimiter }, StringSplitOptions.RemoveEmptyEntries))
                    _PathElements.Add(b);

            //var __ObjectLocation = "";

            //foreach (var c in _PathElements)
            //{
            //    __ObjectLocation += FSPathConstants.PathDelimiter;
            //    __ObjectLocation += c;
            //}

            var __ObjectLocation = _PathElements.ToArray();

            if (__ObjectLocation.Length == 0)
                __ObjectLocation = new String[] { FSPathConstants.PathDelimiter };

            // Note: Do not check for additional PathDelimiters within the String array
            // Six might be the right break event!
            //if (__ObjectLocation.Length > 6)
            //{

            //    var _NewObjectLocation = new StringBuilder(__ObjectLocation[0]);

            //    _NewObjectLocation.Append(FSPathConstants.PathDelimiter);

            //    for (int i = 1; i < __ObjectLocation.Length - 1; i++)
            //        _NewObjectLocation.Append(__ObjectLocation[i]).Append(FSPathConstants.PathDelimiter);

            //    _ObjectPath     = _NewObjectLocation.ToString();
            //    Name     = __ObjectLocation[__ObjectLocation.Length - 1];
            //    _ObjectLocation = _ObjectPath + FSPathConstants.PathDelimiter + Name;

            //}

            //else
            if (__ObjectLocation.Length > 1)
            {

                String _NewObjectName = "";
                _ObjectLocation = FSPathConstants.PathDelimiter;

                for (int i = 0; i < __ObjectLocation.Length; i++)
                {

                    _NewObjectName = __ObjectLocation[i];

                    while (_NewObjectName.StartsWith(FSPathConstants.PathDelimiter))
                        _NewObjectName = _NewObjectName.Remove(0, FSPathConstants.PathDelimiter.Length);

                    while (_NewObjectName.EndsWith(FSPathConstants.PathDelimiter))
                        _NewObjectName = _NewObjectName.Substring(0, _NewObjectName.Length - FSPathConstants.PathDelimiter.Length);

                    if (_NewObjectName != "")
                        _ObjectLocation = String.Concat(_ObjectLocation, _NewObjectName, FSPathConstants.PathDelimiter);

                }

                if (_ObjectLocation.StartsWith("/..") || _ObjectLocation.StartsWith("/."))
                {
                    _ObjectLocation = _ObjectLocation.Substring(1, _ObjectLocation.Length - FSPathConstants.PathDelimiter.Length - 1);
                }

                else
                    _ObjectLocation = _ObjectLocation.Substring(0, _ObjectLocation.Length - FSPathConstants.PathDelimiter.Length);

            }

            else
            {

                //HACK: For "/dir/file" within first parameter
                if (__ObjectLocation[0].Length > FSPathConstants.PathDelimiter.Length && __ObjectLocation[0].Contains(FSPathConstants.PathDelimiter))
                {
                    var _tmp = new ObjectLocation(__ObjectLocation[0].Split(new String[] { FSPathConstants.PathDelimiter }, StringSplitOptions.RemoveEmptyEntries));
                    _ObjectLocation = _tmp._ObjectLocation;
                    Name            = _tmp.Name;
                    _Path           = _tmp.Path;
                    return;
                }


                //Name = __ObjectLocation[0] == null ? __ObjectLocation[0] : "";
                Name = __ObjectLocation[0];

                while (Name.StartsWith(FSPathConstants.PathDelimiter))
                    Name = Name.Remove(0, FSPathConstants.PathDelimiter.Length);

                while (Name.EndsWith(FSPathConstants.PathDelimiter))
                    Name = Name.Substring(0, Name.Length - FSPathConstants.PathDelimiter.Length);

                if (Name == "..")
                {
                    _ObjectLocation = Name;
                    _Path           = "";
                }

                else
                {
                    _ObjectLocation = FSPathConstants.PathDelimiter + Name;
                    _Path           = FSPathConstants.PathDelimiter;
                }

                return;

            }

            var _Slash = _ObjectLocation.LastIndexOf(FSPathConstants.PathDelimiter);
            _Path = _ObjectLocation.Substring(0, _Slash);//new ObjectLocation(_ObjectLocation.Substring(0, _Slash));//
            Name  = _ObjectLocation.Substring(_Slash + FSPathConstants.PathDelimiter.Length);

            if (Name.Contains("/"))
            {
                var b = "";
            }

        }

        #endregion

        #endregion


        #region Object specific methods

        #region Combine(myObjectPath, myObjectName)

        private String Combine(String myObjectPath, String myObjectName)
        {

            var PathSeperatorRegExpr = new Regex(String.Concat("(", FSPathConstants.PathDelimiter, ")+"));

            if (myObjectPath == null) myObjectPath = "";
            if (myObjectName == null) myObjectName = "";

            var Combined = String.Concat(myObjectPath, FSPathConstants.PathDelimiter, myObjectName);

            if (Combined.Equals(""))
                Combined = FSPathConstants.PathDelimiter;

            if (!Combined.StartsWith(FSPathConstants.PathDelimiter))
                Combined = String.Concat(FSPathConstants.PathDelimiter, Combined);

            Combined = PathSeperatorRegExpr.Replace(Combined, FSPathConstants.PathDelimiter);

            while (Combined.Length > FSPathConstants.PathDelimiter.Length && Combined.EndsWith(FSPathConstants.PathDelimiter))
                Combined = Combined.Substring(0, Combined.Length - FSPathConstants.PathDelimiter.Length);

            if (Combined.EndsWith("."))
                Combined.Remove(Combined.Length - 1);

            return Combined;

        }

        #endregion

        #region StartsWith(myString)

        public Boolean StartsWith(String myString)
        {
            return _ObjectLocation.StartsWith(myString);
        }

        #endregion

        #region EndsWith(myString)

        public Boolean EndsWith(String myString)
        {
            return _ObjectLocation.EndsWith(myString);
        }

        #endregion

        #region Contains(myString)

        public Boolean Contains(String myString)
        {
            return _ObjectLocation.Contains(myString);
        }

        #endregion

        #region Substring(myStartIndex)

        public String Substring(Int32 myStartIndex)
        {
            return _ObjectLocation.Substring(myStartIndex);
        }

        #endregion

        #region Substring(myStartIndex, myLength)

        public String Substring(Int32 myStartIndex, Int32 myLength)
        {
            return _ObjectLocation.Substring(myStartIndex, myLength);
        }

        #endregion

        #region Remove(myStartIndex)

        public String Remove(Int32 myStartIndex)
        {
            return _ObjectLocation.Remove(myStartIndex);
        }

        #endregion

        #region Remove(myStartIndex, myCount)

        public String Remove(Int32 myStartIndex, Int32 myCount)
        {
            return _ObjectLocation.Remove(myStartIndex, myCount);
        }

        #endregion

        #region Split(mySeperator, myStringSplitOptions)

        public String[] Split(String[] mySeperator, StringSplitOptions myStringSplitOptions)
        {
            return _ObjectLocation.Split(mySeperator, myStringSplitOptions);
        }

        #endregion

        #region Split(mySeperator, myCount, myStringSplitOptions)

        public String[] Split(String[] mySeperator, Int32 myCount, StringSplitOptions myStringSplitOptions)
        {
            return _ObjectLocation.Split(mySeperator, myCount, myStringSplitOptions);
        }

        #endregion

        #region IndexOf(myValue)

        public Int32 IndexOf(String myValue)
        {
            return _ObjectLocation.IndexOf(myValue);
        }

        #endregion

        #endregion


        #region Implicit conversation to/from String

        //public static implicit operator ObjectLocation(String myString)
        //{
        //    return new ObjectLocation(myString);
        //}

        public static implicit operator String(ObjectLocation myObjectLocation)
        {
            return myObjectLocation.ToString();
        }

        #endregion


        #region Operator overloading

        #region Operator == (myObjectLocation1, myObjectLocation2)

        public static Boolean operator == (ObjectLocation myObjectLocation1, ObjectLocation myObjectLocation2)
        {

            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(myObjectLocation1, myObjectLocation2))
                return true;

            // If one is null, but not both, return false.
            if (((Object)myObjectLocation1 == null) || ((Object)myObjectLocation2 == null))
                return false;

            return myObjectLocation1.Equals(myObjectLocation2);

        }

        #endregion

        #region Operator != (myObjectLocation1, myObjectLocation2)

        public static Boolean operator != (ObjectLocation myObjectLocation1, ObjectLocation myObjectLocation2)
        {
            return !(myObjectLocation1 == myObjectLocation2);
        }

        #endregion

        #region Operator + (myObjectLocation1, myObjectLocation2)

        public static ObjectLocation operator +(ObjectLocation myObjectLocation1, String myObjectLocation2)
        {
            return new ObjectLocation(myObjectLocation1._ObjectLocation, myObjectLocation2);
        }

        #endregion

        #endregion


        #region IComparable Members

        public Int32 CompareTo(Object myObject)
        {

            // Check if myObject is null
            if (myObject == null)
                throw new ArgumentNullException("myObject must not be null!");

            // Check if myObject can be casted to an UUID object
            var _ObjectLocation = myObject as ObjectLocation;
            if ((Object) _ObjectLocation == null)
                throw new ArgumentException("myObject is not of type ObjectLocation!");

            return CompareTo(_ObjectLocation);

        }

        #endregion

        #region IComparable<ObjectLocation> Members

        public Int32 CompareTo(ObjectLocation myObjectLocation)
        {
            return myObjectLocation.CompareTo(_ObjectLocation);
        }

        #endregion

        #region IComparable<String> Members

        public Int32 CompareTo(String myString)
        {
            return myString.CompareTo(_ObjectLocation);
        }

        #endregion


        #region IEquatable Members

        public override Boolean Equals(Object myObject)
        {

            // Check if myObject is null
            if (myObject == null)
                return false;

            // If parameter cannot be cast to Point return false.
            var _ObjectLocation = myObject as ObjectLocation;
            if ((Object) _ObjectLocation == null)
                return false;

            return Equals(_ObjectLocation);

        }

        #endregion

        #region IEquatable<ObjectLocation> Members

        public Boolean Equals(ObjectLocation myObjectLocation)
        {

            // If parameter is null return false:
            if ((Object) myObjectLocation == null)
                return false;

            // Check if the inner fields have the same values
            if (_ObjectLocation != myObjectLocation)
                return false;

            return true;

        }

        #endregion

        #region IEquatable<String> Members

        public Boolean Equals(String myString)
        {

            // If parameter is null return false:
            if ((Object) myString == null)
                return false;

            // Check if the inner fields have the same values
            if (_ObjectLocation != myString)
                return false;

            return true;

        }

        #endregion


        #region GetHashCode()

        public override int GetHashCode()
        {
            return _ObjectLocation.GetHashCode();
        }

        #endregion

        #region ToString()

        public override String ToString()
        {
            return _ObjectLocation;
        }

        #endregion


        #region IDisposable Members

        public void Dispose()
        {
            // Do nothing!
        }

        #endregion

    }

}

