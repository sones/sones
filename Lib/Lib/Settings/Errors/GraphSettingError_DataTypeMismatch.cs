using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Lib.Settings.Errors
{

    /// <summary>
    /// The type of the value does not match the type definition
    /// </summary>
    public class GraphSettingError_DataTypeMismatch : GraphSettingError
    {
        private   Type _ExpectedType;
        private   Type _ActualType;

        public GraphSettingError_DataTypeMismatch(Type myExpectedType, Type myActualType)
        {
            this._ExpectedType = myExpectedType;
            this._ActualType = myActualType;
        }
    }
}
