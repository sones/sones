using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.ErrorHandling;

namespace sones.GraphQL.GQL.ErrorHandling
{
    public sealed class GQLGrammarSetExtandableMemberException : AGraphQLException
    {
        #region data
        
        public String Info { get; private set; }
        public Type ExtandableMemberType { get; private set; }

        #endregion

        #region constructor
        
        /// <summary>
        /// Creates a new GQLGrammarSetExtandableMemberException exception
        /// </summary>
        /// <param name="myMember">The extandable member which occurs the error</param>
        /// <param name="myInfo">Exception info</param>
        public GQLGrammarSetExtandableMemberException(Type myMemberType, String myInfo)
        {
            Info = myInfo;
            ExtandableMemberType = myMemberType;

            _msg = String.Format("Error during setting the extandable member of type: {0} in GQL grammar, plugin not found? \n {1}", myMemberType.ToString(), Info);
        }

        #endregion
    }
}
