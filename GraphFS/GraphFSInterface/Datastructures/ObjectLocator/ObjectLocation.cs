/*
 * ObjectLocation
 * (c) Achim Friedland, 2009 - 2010
 */

#region Usings

using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using sones.Lib.DataStructures;

#endregion

namespace sones.GraphFS.DataStructures
{

    /// <summary>
    /// The ObjectLocation locates the AFSObject within a file system hierarchy.
    /// It is the combination of the AFSObject path and its name.
    /// </summary>

    public class ObjectLocation : IComparable, IComparable<ObjectLocation>, IEquatable<ObjectLocation>, IDisposable
    {

        #region Data

        private String       _ObjectLocation;
        private List<String> _PathElements;

        #endregion

        #region Properties

        #region Path

        /// <summary>
        /// Stores the path of this AFSObject.
        /// </summary>
        public ObjectLocation Path
        {
            get
            {

                var _Count = _PathElements.Count();

                if (_Count > 0)
                    return new ObjectLocation(_PathElements.Take(_Count - 1));

                return this;

            }
        }

        #endregion

        #region Name

        /// <summary>
        /// Stores the name of this AFSObject.
        /// </summary>
        public String Name
        {
            get
            {

                var _Count = _PathElements.Count();

                if (_Count > 0)
                    return _PathElements[_Count - 1];

                return "";

            }
        }

        #endregion

        #region Length

        /// <summary>
        /// Returns the length of the ObjectLocation as Int32.
        /// </summary>
        public Int32 Length
        {
            get
            {
                return _ObjectLocation.Length;
            }
        }

        #endregion

        #region LongLength

        /// <summary>
        /// Returns the length of the ObjectLocation as Int64.
        /// </summary>
        public Int64 LongLength
        {
            get
            {
                return (Int64) _ObjectLocation.Length;
            }
        }

        #endregion

        #region ULongLength

        /// <summary>
        /// Returns the length of the ObjectLocation as UInt64.
        /// </summary>
        public UInt64 ULongLength
        {
            get
            {
                return (UInt64) _ObjectLocation.Length;
            }
        }

        #endregion

        #region IsRoot

        public Boolean IsRoot
        {
            get
            {
                
                if (_PathElements.Count == 0)
                    return true;
                
                return false;

            }
        }

        #endregion

        #region PathElements

        public IEnumerable<String> PathElements
        {
            get
            {
                return _PathElements;
            }
        }

        #endregion

        #region (static) Root

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
        /// This will create the root ObjectLocation.
        /// You may also use the static ObjectLocation.Root.
        /// </summary>
        public ObjectLocation()
        {
            _PathElements = new List<String>();
            PathElements2ObjectLocation();
        }

        #endregion

        #region ObjectLocation(params myPathElements)

        /// <summary>
        /// Transforms the given array of path elements into an valid ObjectLocation.
        /// </summary>
        /// <param name="myPathElements">An array of valid path elements (null and empty entries will be removed!)</param>
        public ObjectLocation(params String[] myPathElements)
        {
            _PathElements = myPathElements.Where(s => !String.IsNullOrEmpty(s)).ToList();
            PathElements2ObjectLocation();
        }

        #endregion

        #region ObjectLocation(myPathElements)

        /// <summary>
        /// Transforms the given enumeration of path elements into an valid ObjectLocation.
        /// </summary>
        /// <param name="myPathElements">An enumeration of valid path elements (null and empty entries will be removed!)</param>
        public ObjectLocation(IEnumerable<String> myPathElements)
        {
            _PathElements = myPathElements.Where(s => s != null && s != "").ToList();
            PathElements2ObjectLocation();
        }

        #endregion

        #region ObjectLocation(myObjectLocation, params myPathElements)

        /// <summary>
        /// Adds the given array of path elements to the given ObjectLocation.
        /// </summary>
        /// <param name="myObjectLocation">A valid ObjectLocation</param>
        /// <param name="myPathElements">An array of valid path elements (null and empty entries will be removed!)</param>
        public ObjectLocation(ObjectLocation myObjectLocation, params String[] myPathElements)
        {

            _PathElements = new List<String>(myObjectLocation.PathElements);
            _PathElements.AddRange(myPathElements.Where(s => s != null && s != ""));
            
            PathElements2ObjectLocation();

        }

        #endregion

        #region ObjectLocation(myObjectLocation, myPathElements)

        /// <summary>
        /// Adds the given enumeration of path elements to the given ObjectLocation.
        /// </summary>
        /// <param name="myObjectLocation">A valid ObjectLocation</param>
        /// <param name="myPathElements">An enumeration of valid path elements (null and empty entries will be removed!)</param>
        public ObjectLocation(ObjectLocation myObjectLocation, IEnumerable<String> myPathElements)
        {

            _PathElements = new List<String>(myObjectLocation.PathElements);
            _PathElements.AddRange(myPathElements.Where(s => s != null && s != ""));

            PathElements2ObjectLocation();

        }

        #endregion

        #region ObjectLocation(myObjectLocation1, myObjectLocation2, params myObjectLocations)

        /// <summary>
        /// Transforms the given ObjectLocations into a single valid ObjectLocation.
        /// Note: myObjectLocation1, myObjectLocation2 just to work around Mono's
        ///       strage optional parameters behavior!
        /// </summary>
        /// <param name="myObjectLocation1">A valid ObjectLocation</param>
        /// <param name="myObjectLocation2">A valid ObjectLocation</param>
        /// <param name="myObjectLocations">Some valid ObjectLocations</param>
        public ObjectLocation(ObjectLocation myObjectLocation1, ObjectLocation myObjectLocation2, params ObjectLocation[] myObjectLocations)
        {

            _PathElements = new List<String>(myObjectLocation1.PathElements);
            _PathElements.AddRange(myObjectLocation2.PathElements);

            foreach (var _ObjectLocation in myObjectLocations)
                _PathElements.AddRange(_ObjectLocation.PathElements);

            PathElements2ObjectLocation();

        }

        #endregion

        #region ObjectLocation(myObjectLocations)

        /// <summary>
        /// Transforms the given enumeration of ObjectLocations into a single valid ObjectLocation.
        /// </summary>
        /// <param name="myObjectLocations">Some valid ObjectLocations</param>
        public ObjectLocation(IEnumerable<ObjectLocation> myObjectLocations)
        {

            _PathElements = new List<String>();

            foreach (var _ObjectLocation in myObjectLocations)
                _PathElements.AddRange(_ObjectLocation.PathElements);

            PathElements2ObjectLocation();

        }

        #endregion

        #endregion


        #region Clone()

        /// <summary>
        /// Returns a clone of this ObjectLocation.
        /// </summary>
        public ObjectLocation Clone()
        {
            return new ObjectLocation(_PathElements.ToArray());
        }

        #endregion

        #region (static) Clone(myObjectLocation)

        /// <summary>
        /// Returns a clone of myObjectLocation.
        /// </summary>
        public static ObjectLocation Clone(ObjectLocation myObjectLocation)
        {
            return new ObjectLocation(myObjectLocation.PathElements.ToArray());
        }

        #endregion

        #region (private) PathElements2ObjectLocation()

        /// <summary>
        /// Transforms the private list of PathElements into a valid ObjectLocation.
        /// NOTE: The PathElements should not have any null or empty entries.
        /// </summary>
        private void PathElements2ObjectLocation()
        {

            var _Count = _PathElements.Count();

            if (_Count > 0)
            {

                // Aggreagte PathElements to _ObjectLocation
                _ObjectLocation = _PathElements.Aggregate((result, next) => result + FSPathConstants.PathDelimiter + next);

                // Do not add an additional "/" in fromt of e.g. "../directory" and not in front of an already leading "/"
                if (!_ObjectLocation.StartsWith(".") && !_ObjectLocation.StartsWith(FSPathConstants.PathDelimiter))
                    _ObjectLocation = FSPathConstants.PathDelimiter + _ObjectLocation;

            }

            else
                _ObjectLocation = FSPathConstants.PathDelimiter;

        }

        #endregion

        #region (static) ParseString(myString)

        /// <summary>
        /// Parses a String into an ObjectLocation by splitting it using FSPathConstants.PathDelimiter. 
        /// </summary>
        /// <param name="myPath">An ObjectLocation as String</param>
        /// <returns>A valid ObjectLocation</returns>
        public static ObjectLocation ParseString(String myPath)
        {
            return new ObjectLocation(myPath.Split(new String[] { FSPathConstants.PathDelimiter }, StringSplitOptions.RemoveEmptyEntries));
        }

        #endregion

        #region (static) SimplifyObjectLocation(myPath)

        /// <summary>
        /// Simplifies an object location by removing relative path fragments
        /// like /directoryname1/../directoryname2 -> /directoryname2
        /// </summary>
        /// <param name="myPath">the current path</param>
        /// <returns>a simplified current path</returns>
        public static String SimplifyObjectLocation(String myPath)
        {

            String   _newPath = "";
            String[] _SplittedPathElements = myPath.Split(new String[] { FSPathConstants.PathDelimiter }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 1; i < _SplittedPathElements.Length; i++)
            {

                if (_SplittedPathElements[i].Equals(FSConstants.DotDotLink))
                {
                    _SplittedPathElements[i]     = "";
                    _SplittedPathElements[i - 1] = "";
                }

                else if (_SplittedPathElements[i].Equals(FSPathConstants.PathDelimiter))
                {
                    _SplittedPathElements[i]     = "";
                }

            }

            foreach (var _Path in _SplittedPathElements)
                if (!_Path.Equals("")) _newPath = String.Concat(_newPath, FSPathConstants.PathDelimiter, _Path);

            if (_newPath.Equals(""))
                _newPath = FSPathConstants.PathDelimiter;

            return _newPath;

        }

        #endregion


        #region String-like usage of the ObjectLocation class

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


        #region Operator overloading

        #region Operator == (myObjectLocation1, myObjectLocation2)

        /// <summary>
        /// Compares two ObjectLocations.
        /// </summary>
        /// <param name="myObjectLocation1">A valid ObjectLocation</param>
        /// <param name="myObjectLocation2">A valid ObjectLocation</param>
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

        /// <summary>
        /// Compares two ObjectLocations.
        /// </summary>
        /// <param name="myObjectLocation1">A valid ObjectLocation</param>
        /// <param name="myObjectLocation2">A valid ObjectLocation</param>
        public static Boolean operator !=(ObjectLocation myObjectLocation1, ObjectLocation myObjectLocation2)
        {
            return !(myObjectLocation1 == myObjectLocation2);
        }

        #endregion

        #region Operator +  (myObjectLocation1, myObjectLocation2)

        /// <summary>
        /// Transforms the given ObjectLocations into a single valid ObjectLocation.
        /// </summary>
        /// <param name="myObjectLocation1">A valid ObjectLocation</param>
        /// <param name="myObjectLocation2">A valid ObjectLocation</param>
        /// <returns>A valid ObjectLocation</returns>
        public static ObjectLocation operator + (ObjectLocation myObjectLocation1, String myObjectLocation2)
        {
            return new ObjectLocation(myObjectLocation1, myObjectLocation2);
        }

        #endregion

        #region Operator +  (myObjectLocation,  myStringArray)

        /// <summary>
        /// Transforms the given ObjectLocation and array of strings into a single valid ObjectLocation.
        /// </summary>
        /// <param name="myObjectLocation">A valid ObjectLocation</param>
        /// <param name="myStringArray">An array of strings</param>
        /// <returns>A valid ObjectLocation</returns>
        public static ObjectLocation operator + (ObjectLocation myObjectLocation, String[] myStringArray)
        {
            return new ObjectLocation(myObjectLocation, myStringArray);
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
            if (_ObjectLocation.ToString() != myObjectLocation.ToString())
                return false;

            return true;

        }

        #endregion
        

        #region GetHashCode()

        public override Int32 GetHashCode()
        {
            return _ObjectLocation.ToString().GetHashCode();
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

