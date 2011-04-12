using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Library.Settings.ErrorHandling
{
    /// <summary>
    /// The type of the value does not match the type definition
    /// </summary>
    public sealed class DataTypeMismatchException : ASettingsException
    {
        #region Data

        /// <summary>
        /// The type that is expected by the setting
        /// </summary>
        public readonly Type ReferenceType;

        /// <summary>
        /// The type of the actual setting
        /// </summary>
        public readonly Type ActualType;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new DataTypeMismatchException
        /// </summary>
        /// <param name="myReferenceType">The type that is expected by the setting</param>
        /// <param name="myActualType">The type of the actual setting</param>
        public DataTypeMismatchException(Type myReferenceType, Type myActualType)
        {
            ReferenceType = myReferenceType;
            ActualType = myActualType;
        }

        #endregion

        public override string ToString()
        {
            return String.Format("The type of the value ({0}) does not match the type definition ({1})", ActualType.Name, ReferenceType.Name);
        }
    }
}
