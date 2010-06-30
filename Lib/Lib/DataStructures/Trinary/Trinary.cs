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


/* PandoraFS - Trinary
 * Achim Friedland, 2008 - 2009
 * 
 * Lead programmer:
 *      Achim Friedland
 * 
 * */

#region Usings

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace sones.Lib.DataStructures
{

    //
    //public enum Trinary
    //{
    //    FALSE,
    //    TRUE,
    //    DELETED
    //}


    public sealed class Trinary : IComparable, IComparable<Trinary>, IComparable<Boolean>, IEquatable<Trinary>, IEquatable<Boolean>
    {

        #region Data

        public readonly String Name;
        public readonly Byte Value;

        #endregion

        #region Init default values

        public static readonly Trinary FALSE   = new Trinary(0, "FALSE");
        public static readonly Trinary TRUE    = new Trinary(1, "TRUE");
        public static readonly Trinary DELETED = new Trinary(2, "DELETED");

        #endregion


        #region (private) Constructor

        private Trinary(Byte myValue, String myName)
        {
            this.Name   = myName;
            this.Value  = myValue;
        }

        #endregion



        #region Implicit conversation to/from Boolean

        public static implicit operator Trinary(Boolean myBoolean)
        {

            if (myBoolean)
                return Trinary.TRUE;

            return Trinary.FALSE;

        }

        public static implicit operator Boolean(Trinary myTrinary)
        {

            if (myTrinary == Trinary.FALSE)
                return false;

            if (myTrinary == Trinary.DELETED)
                return false;

            //if (myTrinary == Trinary.TRUE)
            return true;

        }

        #endregion

        #region Implicit conversation to/from String

        public static implicit operator Trinary(String myString)
        {

            if (myString.Equals(Trinary.TRUE.Name))
                return Trinary.TRUE;

            if (myString.Equals(Trinary.FALSE.Name))
                return Trinary.FALSE;

            return Trinary.DELETED;

        }

        public static implicit operator String(Trinary myTrinary)
        {
            return myTrinary.Name;
        }

        #endregion


        #region Operator overloading

        #region Operator == (myTrinary1, myTrinary2)

        public static Boolean operator ==(Trinary myTrinary1, Trinary myTrinary2)
        {

            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(myTrinary1, myTrinary2))
                return true;

            // If one is null, but not both, return false.
            if (((Object)myTrinary1 == null) || ((Object)myTrinary2 == null))
                return false;

            return myTrinary1.Equals(myTrinary2);

        }

        #endregion

        #region Operator != (myTrinary1, myTrinary2)

        public static Boolean operator !=(Trinary myTrinary1, Trinary myTrinary2)
        {
            return !(myTrinary1 == myTrinary2);
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
            var _Trinary = myObject as Trinary;
            if ((Object) _Trinary == null)
                throw new ArgumentException("myObject is not of type Trinary!");

            return CompareTo(_Trinary);

        }

        #endregion

        #region IComparable<Trinary> Members

        public Int32 CompareTo(Trinary myTrinary)
        {

            if (this.Equals(myTrinary))
                return 0;

            return 1;

        }

        #endregion

        #region IComparable<Boolean> Members

        public Int32 CompareTo(Boolean myBoolean)
        {

            if (Value == 0 && myBoolean == false)
                return 0;

            if (Value == 2 && myBoolean == false)
                return 0;
            
            if (Value == 1 && myBoolean == true)
                return 0;

            return 1;

        }

        #endregion


        #region Equals(myObject)

        public override Boolean Equals(Object myObject)
        {

            // Check if myObject is null
            if (myObject == null)
                return false;

            // If parameter cannot be cast to Point return false.
            Trinary _Trinary = myObject as Trinary;
            if ((Object)_Trinary == null)
                return false;

            return Equals(_Trinary);

        }

        #endregion

        #region Equals(myTrinary)

        public Boolean Equals(Trinary myTrinary)
        {

            // If parameter is null return false:
            if ((Object)myTrinary == null)
                return false;

            // Check if the inner fields have the same values
            if (Value != myTrinary.Value)
                return false;

            if (Name != myTrinary.Name)
                return false;

            return true;

        }

        #endregion

        #region Equals(myBoolean)

        public Boolean Equals(Boolean myBoolean)
        {
            return Equals((Trinary)myBoolean);
        }

        #endregion


        #region GetHashCode()

        public override Int32 GetHashCode()
        {
            return Value.GetHashCode() ^ Name.GetHashCode();
        }

        #endregion

        #region ToString

        public override String ToString()
        {
            return Name;
        }

        #endregion


    }

}
