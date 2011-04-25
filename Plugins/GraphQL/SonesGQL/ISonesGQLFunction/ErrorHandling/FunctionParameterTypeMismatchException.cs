using System;

namespace sones.Plugins.SonesGQL.Function.ErrorHandling
{
    public sealed class FunctionParameterTypeMismatchException : ASonesQLFunctionException
    {
        
        #region data        

        public Type ExpectedType { get; private set; }
        public Type CurrentType { get; private set; }

        #endregion

        #region constructor

        public FunctionParameterTypeMismatchException(Type myExpectedType, Type myCurrentType)
        {
            ExpectedType = myExpectedType;
            CurrentType = myCurrentType;

            _msg = String.Format("Function parameter type mismatch! Expected type \"{0}\" dos not match \"{1}\"!", ExpectedType, CurrentType);
        }
        
        #endregion
       
    }
}
