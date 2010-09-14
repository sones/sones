
#region Usings

using System;

#endregion

namespace sones.Lib.ErrorHandling
{

    /// <summary>
    /// This class carries information of errors.
    /// </summary>

    public class ExceptionalConversionError : GeneralError
    {
        String info = String.Empty;


        public ExceptionalConversionError()
        {
            info = String.Format("Converting Exceptional failed!");
        }

        public ExceptionalConversionError(String myDestinationType)
        {
            info = String.Format("Converting Exceptional to Exceptional<{1}> failed!", myDestinationType);
        }

        public ExceptionalConversionError(String mySourceType, String myDestinationType)
        {
            info = String.Format("Converting Exceptional<{0}> to Exceptional<{1}> failed!", mySourceType, myDestinationType);
        }

        public override string ToString()
        {
            return info;
        }

    }

}