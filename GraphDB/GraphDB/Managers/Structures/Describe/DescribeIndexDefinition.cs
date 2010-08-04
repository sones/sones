/*
 * DescribeIndexDefinition
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.Structures.EdgeTypes;
using sones.Lib.ErrorHandling;
using sones.GraphDB.Errors;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Functions;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Indices;
using sones.GraphDB.Exceptions;

#endregion

namespace sones.GraphDB.Managers.Structures.Describe
{
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

        public override Exceptional<List<SelectionResultSet>> GetResult(DBContext myDBContext)
        {

            var result = new List<SelectionResultSet>();

            if (!String.IsNullOrEmpty(_TypeName))
            {

                #region Specific index

                var type = myDBContext.DBTypeManager.GetTypeByName(_TypeName);
                if (type == null)
                {
                    return new Exceptional<List<SelectionResultSet>>(new Error_TypeDoesNotExist(_TypeName));
                }

                if (String.IsNullOrEmpty(_IndexEdition))
                {
                    _IndexEdition = DBConstants.DEFAULTINDEX;
                }
                var attrIndex = type.GetAttributeIndex(_IndexName, _IndexEdition);

                if (attrIndex != null)
                {
                    result.Add(new SelectionResultSet(GenerateOutput(attrIndex, _IndexName)));
                }
                else
                {
                    return new Exceptional<List<SelectionResultSet>>(new Error_IndexDoesNotExist(_IndexName, _IndexEdition));
                }

                #endregion

            }
            else
            {

                #region All indices

                foreach (var type in myDBContext.DBTypeManager.GetAllTypes(false))
                {
                    if (type.IsUserDefined)
                    {
                        foreach (var index in type.GetAllAttributeIndices())
                        {
                            result.Add(new SelectionResultSet(GenerateOutput(index, index.IndexName)));

                        }
                    }
                }

                #endregion

            }

            return new Exceptional<List<SelectionResultSet>>(result);

        }
        
        #endregion

        #region Output

        /// <summary>
        /// generate an output for an index
        /// </summary>
        /// <param name="myIndex">the index</param>
        /// <param name="myName">the index name</param>
        /// <returns>list of readouts which contain the index information</returns>
        private IEnumerable<DBObjectReadout> GenerateOutput(AAttributeIndex myIndex, String myName)
        {

            var _Index = new Dictionary<String, Object>();

            _Index.Add("Name", myName);
            _Index.Add("Edition", myIndex.IndexEdition);
            _Index.Add("IndexType", myIndex.IndexType);
            _Index.Add("IsUuidIndex", myIndex is UUIDIndex);
            _Index.Add("IsUniqueAttributeIndex", myIndex.IsUniqueAttributeIndex);

            return new List<DBObjectReadout>() { new DBObjectReadout(_Index) };

        }

        #endregion

    }
}
