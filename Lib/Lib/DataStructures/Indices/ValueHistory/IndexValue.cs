/*
 * IndexValue
 * (c) Achim Friedland, 2009 - 2010
 */

#region Usings

using System;

#endregion

namespace sones.Lib.DataStructures.Indices
{

    /// <summary>
    /// This datastructure implements a value and serialized value
    /// to be used within index datastructures
    /// </summary>
    /// <typeparam name="TValue">The type of the stored value</typeparam>
    public class IndexValue<TValue>
    {

        #region Properties

        public  TValue    Value             { get; set; }
        public  Byte[]    SerializedValue   { get; set; }

        #endregion

        #region Constructors

        #region IndexValue()

        /// <summary>
        /// Creates a new IndexValue using default values
        /// </summary>
        public IndexValue()
        {
            Value               = default(TValue);
            SerializedValue     = null;
        }

        #endregion

        #region IndexValue(myValue)

        /// <summary>
        /// Creates a new IndexValue, setting the internal value to the content of myValue
        /// </summary>
        /// <param name="myValue"></param>
        public IndexValue(TValue myValue)
        {
            Value               = myValue;
            SerializedValue     = null;
        }

        #endregion

        #region IndexValue(myValue, mySerializedValue)

        /// <summary>
        /// Creates a new IndexValue, setting the internal value to the content of myValue
        /// and setting the interal SerializedValue to the content of mySerializedValue
        /// </summary>
        /// <param name="myValue"></param>
        /// <param name="mySerializedValue"></param>
        public IndexValue(TValue myValue, Byte[] mySerializedValue)
        {
            Value               = myValue;
            SerializedValue     = mySerializedValue;
        }

        #endregion

        #endregion

        #region GetHashCode()

        public override int GetHashCode()
        {

            if (Value != null && SerializedValue != null)
                return Value.GetHashCode() ^ SerializedValue.GetHashCode();

            if (Value != null)
                return Value.GetHashCode();

            if (SerializedValue != null)
                return SerializedValue.GetHashCode();

            return 0;

        }

        #endregion

        #region Equals()

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (this.GetType() != obj.GetType())
            {
                return false;
            }
            return ((IndexValue<TValue>)obj).Value.Equals(this.Value);
        }

        #endregion

    }

}
