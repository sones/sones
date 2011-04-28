using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.ErrorHandling;
using sones.Library.ErrorHandling;

namespace sones.GraphQL.GQL.ErrorHandling
{
    public sealed class AggregateOrFunctionDoesNotExistException : AGraphQLException
    {
        #region data
        
        public String Info { get; private set; }
        public String AggrOrFuncName { get; private set; }
        public ASonesException Exception { get; private set; }
        public Type AggrOrFuncType { get; private set; }

        #endregion

        #region constructor
        
        /// <summary>
        /// Creates a new AggregateOrFunctionDoesNotExistException exception
        /// </summary>
        public AggregateOrFunctionDoesNotExistException(Type myAggrOrFuncType, String myAggrOrFuncName, String myInfo, ASonesException myException = null)
        {
            Info = myInfo;
            Exception = myException;
            AggrOrFuncType = myAggrOrFuncType;
            AggrOrFuncName = myAggrOrFuncName;
            
            if (Exception != null)
            {
                if (Exception.Message != null && !Exception.Message.Equals(""))
                    _msg = String.Format("Error during loading the aggregate plugin of type: [{0}] name: [{1}]\n\nInner Exception: {2}\n\n{3}", myAggrOrFuncType.ToString(), myAggrOrFuncName, Exception.Message, myInfo);
                else
                    _msg = String.Format("Error during loading the aggregate plugin of type: [{0}] name: [{1}]\n\n{2}", myAggrOrFuncType.ToString(), myAggrOrFuncName, myInfo);
            }
        }

        #endregion
    }
}
