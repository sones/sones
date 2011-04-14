using System;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// An aggregate is not valid on an attribute
    /// </summary>
    public sealed class AggregateIsNotValidOnThisAttributeException : ASonesQLAggregateException
    {
        public String Info { get; private set; }

        /// <summary>
        /// Creates a new AggregateIsNotValidOnThisAttributeException exception
        /// </summary>
        /// <param name="myInfo"></param>
        public AggregateIsNotValidOnThisAttributeException(String myInfo)
        {
            Info = myInfo;
            _msg = Info;
        }
          
    }
}
