/*
 * DescribeIndexDefinition
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using sones.GraphDB.Structures.EdgeTypes;
using sones.Lib.ErrorHandling;
using sones.GraphDB.Errors;
using sones.GraphDB.Functions;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Indices;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Result;
using sones.GraphDB.NewAPI;

#endregion

namespace sones.GraphDB.Managers.Structures.Describe
{
    /// <summary>
    /// Describes an index
    /// </summary>
    public class DescribeIndexDefinition : ADescribeDefinition
    {
        #region Data

        private String _IndexEdition;
        private String _TypeName;
        private String _IndexName;

        #endregion

        #region Ctor

        public DescribeIndexDefinition() { }

        public DescribeIndexDefinition(String myTypeName, string myIndexName, String myIndexEdition)
        {
            _TypeName = myTypeName;
            _IndexName = myIndexName;
            _IndexEdition = myIndexEdition;
        }

        #endregion

        #region ADescribeDefinition

        /// <summary>
        /// <seealso cref=" ADescribeDefinition"/>
        /// </summary>        
        public override Exceptional<IEnumerable<Vertex>> GetResult(DBContext myDBContext)
        {

            if (!String.IsNullOrEmpty(_TypeName))
            {

                #region Specific index

                var type = myDBContext.DBTypeManager.GetTypeByName(_TypeName);
                if (type == null)
                {
                    return new Exceptional<IEnumerable<Vertex>>(new Error_TypeDoesNotExist(_TypeName));
                }

                if (String.IsNullOrEmpty(_IndexEdition))
                {
                    _IndexEdition = DBConstants.DEFAULTINDEX;
                }

                var attrIndex = type.GetAttributeIndex(_IndexName, _IndexEdition);

                if (attrIndex.Failed())
                {
                    return new Exceptional<IEnumerable<Vertex>>(attrIndex);
                }

                if (attrIndex != null)
                {
                    return new Exceptional<IEnumerable<Vertex>>(new List<Vertex>(){(GenerateOutput(attrIndex.Value, _IndexName))});
                }
                else
                {
                    return new Exceptional<IEnumerable<Vertex>>(new Error_IndexDoesNotExist(_IndexName, _IndexEdition));
                }

                #endregion

            }
            else
            {

                #region All indices

                var resultingReadouts = new List<Vertex>();

                foreach (var type in myDBContext.DBTypeManager.GetAllTypes(false))
                {
                    if (type.IsUserDefined)
                    {
                        foreach (var index in type.GetAllAttributeIndices())
                        {
                            resultingReadouts.Add(GenerateOutput(index, index.IndexName));
                        }
                    }
                }

                return new Exceptional<IEnumerable<Vertex>>(resultingReadouts);

                #endregion

            }
        }
        
        #endregion

        #region Output

        /// <summary>
        /// Generate an output for an index
        /// </summary>
        /// <param name="myIndex">The index</param>
        /// <param name="myName">The index name</param>
        /// <returns>List of readouts which contain the index information</returns>
        private Vertex GenerateOutput(AAttributeIndex myIndex, String myName)
        {

            var _Index = new Dictionary<String, Object>();

            _Index.Add("Name",                   myName);
            _Index.Add("Edition",                myIndex.IndexEdition);
            _Index.Add("IndexType",              myIndex.IndexType);
            _Index.Add("IsUuidIndex",            myIndex.IsUUIDIndex);
            _Index.Add("IsUniqueAttributeIndex", myIndex.IsUniqueAttributeIndex);

            return new Vertex(_Index);
            
        }

        #endregion

    }
}
